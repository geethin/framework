using System.Security.Claims;
using System.Text;
using AutoMapper;
using Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Services.AutoMapper;

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

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextPool<ContextBase>(option =>
                {
                    option.UseSqlServer(connectionString, sql => { sql.MigrationsAssembly("Data.Context"); });
                });

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
                    // 如果不如果jwt，可注释掉，你可能会使用IdentityServer
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt")["Sign"])),
                    ValidIssuer = Configuration.GetSection("Jwt")["Issuer"],
                    ValidAudience = Configuration.GetSection("Jwt")["Audience"],
                    ValidateIssuer = true,
                    ValidateLifetime = false,
                    RequireExpirationTime = false,
                    ValidateAudience = false,
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
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
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
                // 生产环境需要新的配置
                app.UseCors("default");
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
