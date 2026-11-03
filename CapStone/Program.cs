using CapStone.Configuration;
using CapStone.Data;
using CapStone.Middleware;
using CapStone.Repositories;
using CapStone.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace CapStone
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================
            // CORS
            // =========================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontends", policy =>
                    policy.WithOrigins(
                              "http://localhost:4200"    // Angular frontend
                          )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });

            // =========================
            // Database
            // =========================
            builder.Services.AddDbContext<CapstoneDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // =========================
            // JWT Authentication
            // =========================
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("Jwt"));

            var jwtSettings = builder.Configuration
                .GetSection("Jwt")
                .Get<JwtSettings>();

            // DefaultInboundClaimTypeMap mapping preserved
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),

                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    NameClaimType = System.Security.Claims.ClaimTypes.Name 
                };
            });

            builder.Services.AddAuthorization();
           

            // =========================
            // HTTP Context
            // =========================
            builder.Services.AddHttpContextAccessor();

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
            builder.Services.AddScoped<IEmailService, EmailService>();

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

                options.CustomSchemaIds(type => type.FullName);

                // Fix: Map DateOnly to proper 'date' format so Swagger UI renders it correctly
                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date",
                    Example = new Microsoft.OpenApi.Any.OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-dd"))
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your JWT token below. (Swagger will automatically add 'Bearer ' prefix).",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
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
            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CapStone Insurance API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontends");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
