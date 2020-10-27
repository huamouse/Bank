using Bank.Domains.Payment;
using Bank.EFCore;
using Bank.ICBC.Config;
using Bank.Services;
using CPTech.Middleware;
using CPTech.ModelBinding;
using CPTech.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Text.Json;

namespace Bank
{
    public class Startup
    {
        private readonly IConfiguration configuration;

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

            //services.AddDbContext<SqlDbContext>(option => option.UseSqlServer(configuration.GetConnectionString("SqlServer")));
            services.AddDbContext<SqlDbContext>(option => option.UseMySql(configuration.GetConnectionString("MySql")));

            //services.AddScoped<IUserRepository, UserRepository>();
            services.AddHttpClient();

            services.AddTransient(typeof(IPaymentService), typeof(IcbcService));
            //Func<IServiceProvider, IPaymentService> func = provider =>
            //{
            //    Func<ChannelEnum, IPaymentService> func2 = channel =>
            //    {
            //        return provider.GetService<IcbcService>();
            //    };
            //    return func2(ChannelEnum.ICBC);
            //};
            //services.AddSingleton<IPaymentService>(func);
            //services.AddSingleton<IPaymentService>(provider =>
            //{
            //    Func<ChannelEnum, IPaymentService> func = channel =>
            //    {
            //        switch (channel)
            //        {
            //            case ChannelEnum.ICBC:
            //                return provider.GetService<IcbcService>();
            //            case ChannelEnum.ABC:
            //                break;
            //            case ChannelEnum.BOC:
            //                break;
            //            case ChannelEnum.CCB:
            //                break;
            //            default:
            //                break;
            //        }
            //    };
            //    return func;
            //});

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new TupleModelBinderProvider());
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
