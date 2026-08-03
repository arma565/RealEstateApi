using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RealEstate.Data;
using RealEstate.Services.Authentication;
using RealEstate.Services.Authorization;
using RealEstate.Services.Images;
using RealEstate.Services.Models.Users;
using RealEstate.Services.Repositories.Images;
using RealEstate.Services.Repositories.Persons;
using RealEstate.Services.Repositories.Properties;
using RealEstate.Services.Repositories.Properties.Addresses;
using RealEstate.Services.Repositories.Properties.Addresses.Maps;
using RealEstate.Services.Repositories.Properties.Documents;
using RealEstate.Services.Repositories.Properties.Features;
using RealEstate.Services.Repositories.Properties.Leases;
using RealEstate.Services.Repositories.Properties.Payments;
using RealEstate.Services.Repositories.Supports;
using RealEstate.Services.Repositories.Users.AdminRepositories;
using RealEstate.Services.Repositories.Users.UserRepositories;
using RealEstate.Services.Validations;
using System.Text;

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

        //Database services
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("PropertyConnection"))
        );

        builder
            .Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        //Repositories services
        builder.Services.AddScoped<ImageRepository>();
        builder.Services.AddScoped<PersonRepository>();
        builder.Services.AddScoped<LocationRepository>();
        builder.Services.AddScoped<AddressRepository>();
        builder.Services.AddScoped<DeedRepository>();
        builder.Services.AddScoped<FeatureRepository>();
        builder.Services.AddScoped<LeaseRepository>();
        builder.Services.AddScoped<PaymentRepository>();
        builder.Services.AddScoped<PropertyRepository>();
        builder.Services.AddScoped<SupportRepository>();
        builder.Services.AddScoped<AdminRepository>();
        builder.Services.AddScoped<UserRepository>();

        //Authentication and Authorization Services
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
        .AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin))
        .AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser()); ;
        builder.Services.AddScoped<TokenService>();
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

        //Services
        builder.Services.AddScoped<ImageService>();
        builder.Services.AddScoped<EmailValidationService>();
        builder.Services.AddScoped<PasswordValidationService>();
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
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
        });


        var app = builder.Build();

        //Seed roles into the database
        using (var scope = app.Services.CreateScope())
        {
            await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider).ConfigureAwait(false);
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