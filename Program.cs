using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RealEstate.Data;
using RealEstate.Data.Authentication;
using RealEstate.Entities.Images.Documents;
using RealEstate.Entities.Images.Properties;
using RealEstate.Entities.Images.Supports;
using RealEstate.Entities.Images.Users;
using RealEstate.Entities.Persons.Owners;
using RealEstate.Entities.Persons.Tenants;
using RealEstate.Entities.Properties.Addresses;
using RealEstate.Entities.Properties.Addresses.Map;
using RealEstate.Entities.Properties.Documents;
using RealEstate.Entities.Properties.Features;
using RealEstate.Entities.Properties.Leases;
using RealEstate.Entities.Properties.Leases.Payments;
using RealEstate.Entities.Supports;
using RealEstate.Entities.Users;
using RealEstate.Entities.Users.Authentications;
using RealEstate.Enums.Users.Authentications;
using RealEstate.Repositories.Images.Documents;
using RealEstate.Repositories.Images.Properties;
using RealEstate.Repositories.Images.Supports;
using RealEstate.Repositories.Images.Users;
using RealEstate.Repositories.Persons;
using RealEstate.Repositories.Properties;
using RealEstate.Repositories.Properties.Addresses;
using RealEstate.Repositories.Properties.Addresses.Maps;
using RealEstate.Repositories.Properties.Documents;
using RealEstate.Repositories.Properties.Features;
using RealEstate.Repositories.Properties.Leases;
using RealEstate.Repositories.Properties.Leases.Payments;
using RealEstate.Repositories.Supports;
using RealEstate.Repositories.Users;
using RealEstate.Repositories.Users.Authentications;
using RealEstate.Services.Images;
using RealEstate.Services.Images.Documents;
using RealEstate.Services.Images.Properties;
using RealEstate.Services.Images.Supports;
using RealEstate.Services.Images.Users;
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
using RealEstate.Services.Users.Authentications;
using RealEstate.Validations;
using System.Net;
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
        builder.Services.AddScoped<PropertyDeedImageRepository<PropertyDeedImage>>();
        builder.Services.AddScoped<PropertyImageRepository<PropertyImage>>();
        builder.Services.AddScoped<SupportImageRepository<SupportImage>>();
        builder.Services.AddScoped<ApplicationUserImageRepository<ApplicationUserImage>>();
        builder.Services.AddScoped<OwnerRepository<Owner>>();
        builder.Services.AddScoped<TenantRepository<Tenant>>();
        builder.Services.AddScoped<LocationRepository<PropertyLocation>>();
        builder.Services.AddScoped<AddressRepository<PropertyAddress>>();
        builder.Services.AddScoped<PropertyDeedRepository<PropertyDeed>>();
        builder.Services.AddScoped<FeatureRepository<PropertyFeature>>();
        builder.Services.AddScoped<PaymentRepository<Payment>>();
        builder.Services.AddScoped<LeaseRepository<Lease>>();
        builder.Services.AddScoped<PropertyRepository<Property>>();
        builder.Services.AddScoped<SupportRepository<RealEstateSupport>>();
        builder.Services.AddScoped<TokenRepository>();
        builder.Services.AddScoped<UserRepository>();

        //Services
        builder.Services.AddScoped<PropertyDeedImageService>();
        builder.Services.AddScoped<PropertyImageService>();
        builder.Services.AddScoped<SupportImageService>();
        builder.Services.AddScoped<ApplicationUserImageService>();
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
        builder.Services.AddScoped<TokenService>();
        builder.Services.AddScoped<UserService>();

        //Other services
        builder.Services.AddScoped<ImageProccess>();
        builder.Services.AddScoped<EmailValidationService>();
        builder.Services.AddScoped<PasswordValidationService>();

        //Authentication and Authorization
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
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
        .AddPolicy("ManagerOnly", policy => policy.RequireRole(Roles.Manager.ToString()))
        .AddPolicy("AdminOrManager", policy => policy.RequireRole(Roles.Admin.ToString(), Roles.Manager.ToString()))
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
        }
        ;

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