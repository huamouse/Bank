using System;
using System.Text;
using System.Text.Json;
using Bank.Domains.Payment;
using Bank.EFCore;
using Bank.EFCore.Repositories;
using CPTech.Middleware;
using CPTech.ModelBinding;
using CPTech.Models;
using Icbc.Services;
using Icbc.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Bank
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AnyOrigin", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            IConfiguration jwtBearerConfig = configuration.GetSection("Authentication:JwtBearer");
            services.Configure<JwtBearer>(jwtBearerConfig);
            services.Configure<IcbcOptions>(configuration.GetSection("Payment:ICBC"));

            services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtBearerConfig["SecurityKey"])),

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    ValidIssuer = jwtBearerConfig["Issuer"],

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = jwtBearerConfig["Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            //services.AddDbContext<SqlDbContext>(option => option.UseLoggerFactory(loggerFactory).UseSqlServer(configuration.GetConnectionString("SqlServer")));
            services.AddDbContext<SqlDbContext>(option => option.UseMySql(configuration.GetConnectionString("MySql")));

            //services.AddScoped<IUserRepository, UserRepository>();
            services.AddHttpClient();

            services.AddTransient(typeof(IPaymentService), typeof(IcbcService));
            services.AddScoped(typeof(IPaymentRepository), typeof(PaymentRepository));

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new TupleModelBinderProvider());

                //options.Filters.Add(typeof(SampleAsyncActionFilter));

                Console.WriteLine("Default output formatters:");
                foreach (IOutputFormatter formatter in options.OutputFormatters)
                {
                    var mediaFormatter = formatter as OutputFormatter;

                    if (mediaFormatter == null)
                    {
                        Console.WriteLine($"  {formatter.GetType().Name}");
                    }
                    else // OutputFormatter class has SupportedMediaType
                    {
                        Console.WriteLine("  {0}, Media types: {1}",
                            arg0: mediaFormatter.GetType().Name,
                            arg1: string.Join(", ", mediaFormatter.SupportedMediaTypes));
                    }
                }
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseException();

            app.UseRouting();
            app.UseCors("AnyOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
