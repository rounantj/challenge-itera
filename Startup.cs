using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IteraCompanyGroups.Services;
using IteraCompanyGroups.Data;
using Microsoft.EntityFrameworkCore;
using IteraCompanyGroups.Seeder;
using MongoDB.Driver;
using Elasticsearch.Net;
using Nest;

namespace IteraCompanyGroups
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
             .AddNewtonsoftJson(options =>
             {
                 options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                 {
                     NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
                 };
             });

            var key = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("JwtKey"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Configuration.GetValue<string>("TitleProject"), Version = "v1" });
            });


            var connectionString = Configuration.GetValue<string>("ConnectionStrings:MySqlConnection");
            services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26))));

            // Adicione o contexto do banco de dados como um serviço
            services.AddScoped<AppDbContext>();
            // Registrar serviços
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<CostService>();
            services.AddScoped<LogService>();
            services.AddScoped<UserService>();
            services.AddScoped<TokenService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext context, CompanyService companyService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IteraCompanyGroups v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Inicialização do banco de dados
            context.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
            context.Database.Migrate();

            // Chamando o seeder
            SeederClass.Seed(context);
        }

    }
}
