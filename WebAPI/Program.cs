
using DataAccess.EFCore.Data;
using DataAccess.EFCore.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Domain.Utilities;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Text;
using System.Text.Json.Serialization;
using WebAPI.Services;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register ApplicationContext 
            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName));
            });

            //Identity
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            // Register Generic Repository & UnitOfWork services
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            // JWT configuration
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
                    ValidAudience = builder.Configuration["Jwt:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
                };
            });

            // Stripe Configuration
            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

            // Configure JSON Serialization Settings
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // Optionally, ignore null values
                //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            // Register the S3 Service
            builder.Services.AddTransient<S3Service>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //swagger docs configuration
            builder.Services.AddSwaggerGen(swagger =>
            {

                swagger.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "",
                    Description = ""
                });

                // To enable Authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token. Example: 'Bearer [YourToken]'"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Add CORS Policy
            builder.Services.AddCors(CorsOptions =>
            {
                CorsOptions.AddPolicy("mypolicy", CorsPolicyBuilder =>
                {
                    CorsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.UseCors("mypolicy");

            app.UseDeveloperExceptionPage();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:SecretKey").Get<string>();

            app.UseCors("mypolicy");

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
