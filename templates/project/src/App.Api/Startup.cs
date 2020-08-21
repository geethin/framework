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

            // ��ʹ��GT.CLI���ɲִ��������ȡ������ע��
            // services.AddRepositories();
            services.AddAutoMapper(typeof(MapperProfile));
            services.AddHttpContextAccessor();

            #region ��ӽӿ�jwt��Ȩ��swagger��cors�ȷ���
            // �����֤
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
                    // ���� jwt ��֤����ʹ��identityServer
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
            // �����Ȩ
            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Name));
            });

            // services.AddScoped(typeof(JwtService)); 
           // cors����
            services.AddCors(options =>
            {
                options.AddPolicy("default", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
            // swagger ����
            services.AddOpenApiDocument(doc =>
            {
                doc.PostProcess = post =>
                {
                    post.Info.Version = "v1";
                    post.Info.Title = "��վ�ӿ�";
                    post.Info.Description = "��վ�ӿ�";
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
