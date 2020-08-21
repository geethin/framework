using AutoMapper;
using Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Services.AutoMapper;
using System.Security.Claims;
using System.Text;

namespace App.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            // 
            // services.AddRepositories();
            services.AddAutoMapper(typeof(MapperProfile));
            services.AddHttpContextAccessor();

            #region 接口相关内容:jwt/swagger/授权/cors
            // jwt
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                //cfg.RequireHttpsMetadata = true;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt")["Sign"])),
                    //ValidIssuer = Configuration.GetSection("Jwt")["Issuer"],
                    //ValidAudience = Configuration.GetSection("Jwt")["Audience"],
                    ValidateAudience = false,
                    //ValidateIssuer = true,
                    ValidateLifetime = false,
                    RequireExpirationTime = false,
                    //ValidateIssuerSigningKey = true
                };
            });
            // 验证
            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Name));
            });

            // services.AddScoped(typeof(JwtService)); 
           // cors配置 
            services.AddCors(options =>
            {
                options.AddPolicy("default", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
            // swagger 设置
            services.AddOpenApiDocument(doc =>
            {
                doc.PostProcess = post =>
                {
                    post.Info.Version = "v1";
                    post.Info.Title = "Web Api";
                    post.Info.Description = "Api 接口列表";
                    post.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Niltor",
                        Email = string.Empty,
                        Url = "https://github.com/geethin/"
                    };
                };
                doc.DocumentName = "app";
            });
            #endregion

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("default");
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
