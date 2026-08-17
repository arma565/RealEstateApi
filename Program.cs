using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RealEstate.Data;
using RealEstate.Entities.Users;
using RealEstate.Entities.Users.Authentications;
using RealEstate.Enums.Users.Authentications;
using RealEstate.Repositories.Images.Documents;
using RealEstate.Repositories.Images.Properties;
using RealEstate.Repositories.Images.Supports;
using RealEstate.Repositories.Images.Users;
using RealEstate.Repositories.Owners;
using RealEstate.Repositories.Properties;
using RealEstate.Repositories.Properties.Addresses;
using RealEstate.Repositories.Properties.Addresses.Maps;
using RealEstate.Repositories.Properties.Documents;
using RealEstate.Repositories.Properties.Features;
using RealEstate.Repositories.Properties.Leases;
using RealEstate.Repositories.Properties.Leases.Payments;
using RealEstate.Repositories.Supports;
using RealEstate.Repositories.Tenants;
using RealEstate.Repositories.Users;
using RealEstate.Services.Images;
using RealEstate.Services.Images.Properties;
using RealEstate.Services.Persons;
using RealEstate.Services.Properties;
using RealEstate.Services.Properties.Addresses;
using RealEstate.Services.Properties.Addresses.Maps;
using RealEstate.Services.Properties.Documents;
using RealEstate.Services.Properties.Features;
using RealEstate.Services.Properties.Leases;
using RealEstate.Services.Properties.Leases.Payments;
using RealEstate.Services.Supports;
using RealEstate.Services.Users;
using RealEstate.Services.Users.Authentication;
using RealEstate.Validations;
using System.Text;
using System.Text.Json.Serialization;

namespace RealEstate;

#pragma warning disable CA1515
public partial class Program
{
    private static async Task Main(string[] args)
    {
        //Allows Emulator of android studio to contact the backend API
        var MyAllowSpecificOrigins = "_estatePolicy";
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                MyAllowSpecificOrigins,
                policy =>
                {
                    policy
                        .WithOrigins("http://0.0.0.0:5068")
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                }
            );
        });


        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        //Database services
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("PropertyConnection"))
        );
        builder
            .Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        //Repositories
        builder.Services.AddScoped<PropertyDeedImageRepository>();
        builder.Services.AddScoped<PropertyImageRepository>();
        builder.Services.AddScoped<SupportImageRepository>();
        builder.Services.AddScoped<ApplicationUserImageRepository>();
        builder.Services.AddScoped<OwnerRepository>();
        builder.Services.AddScoped<TenantRepository>();
        builder.Services.AddScoped<LocationRepository>();
        builder.Services.AddScoped<AddressRepository>();
        builder.Services.AddScoped<PropertyDeedRepository>();
        builder.Services.AddScoped<FeatureRepository>();
        builder.Services.AddScoped<PaymentRepository>();
        builder.Services.AddScoped<LeaseRepository>();
        builder.Services.AddScoped<PropertyRepository>();
        builder.Services.AddScoped<SupportRepository>();
        builder.Services.AddScoped<UserRepository>();

        //Services
        builder.Services.AddScoped<ImageProccess>();
        builder.Services.AddScoped<PropertyImageService>();
        builder.Services.AddScoped<OwnerService>();
        builder.Services.AddScoped<TenantService>();
        builder.Services.AddScoped<LocationService>();
        builder.Services.AddScoped<AddressService>();
        builder.Services.AddScoped<PropertyDeedService>();
        builder.Services.AddScoped<FeatureService>();
        builder.Services.AddScoped<PaymentService>();
        builder.Services.AddScoped<LeaseService>();
        builder.Services.AddScoped<PropertyService>();
        builder.Services.AddScoped<SupportService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<EmailValidationService>();
        builder.Services.AddScoped<PasswordValidationService>();

        //Authentication and Authorization Services
        builder.Services.AddScoped<TokenService>();
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt!.Issuer,
                    ValidAudience = jwt!.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret))
                };
            });

        builder.Services.AddAuthorizationBuilder()
        .AddPolicy("ManagerOnly", policy => policy.RequireRole(Roles.Manager))
        .AddPolicy("AdminOrManager", policy => policy.RequireRole(Roles.Admin , Roles.Manager))
        .AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT token",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            options.AddSecurityRequirement(document =>
            {

                return new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                };
            });

            options.CustomSchemaIds(type => type.FullName);
        });

        var app = builder.Build();

        //Seed roles and create admin
        using (var scope = app.Services.CreateScope())
        {
            await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider).ConfigureAwait(false);
            await IdentitySeeder.CreateManager(scope.ServiceProvider).ConfigureAwait(false);
        };

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //Middlewares
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseStaticFiles();
        app.UseCors(MyAllowSpecificOrigins);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync().ConfigureAwait(false);
    }
}