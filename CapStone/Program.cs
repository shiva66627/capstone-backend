using CapStone.Configuration;
using CapStone.Data;

using CapStone.Repositories;
using CapStone.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CapStone
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================
            // Database
            // =========================
            builder.Services.AddDbContext<CapstoneDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // =========================
            // JWT Configuration
            // =========================
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("Jwt"));

            var jwtSettings = builder.Configuration
                .GetSection("Jwt")
                .Get<JwtSettings>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };
                });

            builder.Services.AddAuthorization();

            // =========================
            // AutoMapper
            // =========================

            // =========================
            // Repository Dependency Injection
            // =========================
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IInsuranceProductRepository, InsuranceProductRepository>();
            builder.Services.AddScoped<IPolicyPlanRepository, PolicyPlanRepository>();
            builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
            builder.Services.AddScoped<IPremiumPaymentRepository, PremiumPaymentRepository>();
            builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
            builder.Services.AddScoped<IClaimDocumentRepository, ClaimDocumentRepository>();
            builder.Services.AddScoped<IClaimHistoryRepository, ClaimHistoryRepository>();

            // =========================
            // Service Dependency Injection
            // =========================
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IInsuranceProductService, InsuranceProductService>();
            builder.Services.AddScoped<IPolicyPlanService, PolicyPlanService>();
            builder.Services.AddScoped<IPolicyService, PolicyService>();
            builder.Services.AddScoped<IPremiumPaymentService, PremiumPaymentService>();
            builder.Services.AddScoped<IClaimService, ClaimService>();
            builder.Services.AddScoped<IClaimDocumentService, ClaimDocumentService>();
            builder.Services.AddScoped<IClaimHistoryService, ClaimHistoryService>();

            // =========================
            // Controllers
            // =========================
            builder.Services.AddControllers();

            // =========================
            // Swagger
            // =========================
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CapStone Insurance API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT Token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // =========================
            // Configure HTTP Pipeline
            // =========================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}