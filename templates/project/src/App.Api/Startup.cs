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

            // 当使用GT.CLI生成仓储服务后，请取消下行注释
            // services.AddRepositories();
            services.AddAutoMapper(typeof(MapperProfile));
            services.AddHttpContextAccessor();

            #region 添加接口jwt授权、swagger、cors等服务
            // 添加验证
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
                    // 配置 jwt 验证，或使用identityServer
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
            // 添加授权
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
            // swagger 配置
            services.AddOpenApiDocument(doc =>
            {
                doc.PostProcess = post =>
                {
                    post.Info.Version = "v1";
                    post.Info.Title = "网站接口";
                    post.Info.Description = "网站接口";
                    post.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Niltor",
                        Email = string.Empty,
                        Url = "https://github.com/geethin/"
                    };
                };
                doc.DocumentName = "web";
                doc.ApiGroupNames = new[] { "Web" };
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
