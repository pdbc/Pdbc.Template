using Aertssen.Framework.Api.Common.Controllers;
using Aertssen.Framework.Api.Common.Extensions;
using Aertssen.Framework.Core.Extensions;
using Aertssen.Framework.Infra;
using Aertssen.Framework.Services.Cqrs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Aertssen.Framework.Core.Configurations;
using Microsoft.AspNetCore.OData;
using MyTemplate.Api.Extensions;
using MyTemplate.Core;
using MyTemplate.Data;
using MyTemplate.Services.Cqrs;
using Aertssen.Framework.Audit.Infra;

namespace MyTemplate.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.RegisterProducesResponseTypes();
                options.SetOutputFormatters();
                options.RegisterGlobalFilters();

                //options.Filters.Add<MyHttpResponseActionFilter>();

            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //Azure AD Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration);

            // Framework
            services.RegisterModule<AertssenFrameworkInfraModule>(Configuration);
            services.RegisterModule<AertssenFrameworkCqrsServicesModule>(Configuration);
            //services.RegisterModule<AertssenFrameworkAuditModule>(Configuration);

            services.AddAutoMapper(
                typeof(RequestToCqrsMappings),
                typeof(CqrsServiceRequestToCqrsMappings),
                //typeof(AuditRequestToCqrsMappings),
                //typeof(AuditDomainToDtoMappings),
                typeof(DomainDtoMappings)
            );

            services.RegisterClaimsPrincipal();

            // Application specific
            services.RegisterModule<MyTemplateDataModule>(Configuration);
            services.RegisterModule<MyTemplateCoreModule>(Configuration);
            services.RegisterModule<MyTemplateServicesCqrsModule>(Configuration);

            //services.AddScoped<IRequestData, RequestData>();
            services.AddSingleton<IAzureAdSettings, AzureAdSettings>();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Domain Model classes can have navigation properties leading to cycles
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                })
                .AddOData(options =>
                {
                    options.ConfigureODataOptions();
                });

            services.AddSwaggerGen(c =>
            {
                c.ConfigureSwaggerDocument(title: "IM.Scharnier API", version: "v1", description: "API for scharnier");

                c.ConfigureSwaggerDocumentationAssemblies(new[]
                                {
                    Assembly.GetExecutingAssembly(),
                    Assembly.GetAssembly(typeof(ResourcesController)),
                });

                // TODO Common somewhere - Add filters to fix enums
                //c.AddEnumsWithValuesFixFilters();

                var tenantId = Configuration.GetValue<string>("AzureAd:TenantId");
                var instanceId = Configuration.GetValue<string>("AzureAd:Instance");
                var clientId = Configuration.GetValue<string>("AzureAd:ClientId");

                c.ConfigureOauthFlowAzure(instanceId, tenantId, new Dictionary<string, string>()
                {
                    {
                        $"api://{clientId}/Frontend", "Access to the scharnier backend api"
                    }
                });

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.ConfigureODataFilters();
                //a.ConfigureSwaggerAuthentication();
            });

            services.AddApplicationInsightsTelemetry();

            // CORS
            string[] origins =
            {
                "http://localhost:4200",
                "https://localhost:4200",
                "https://localhost:5001",
                "https://localhost:44348"
            };
            services.AddCorsForTestEnviromnent(origins);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAzureAdSettings azureAdSettings)
        {
            if (env.IsDevelopment() || env.EnvironmentName.EqualsIgnoreCase("local"))
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("AllowLocalhost");
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IM.Scharnier.Api v1");

                c.OAuthClientId(azureAdSettings.ClientId);
                //c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>() { { "resource", clientId } });

                c.DefaultModelExpandDepth(2);
                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.EnableDeepLinking();
                c.DisplayOperationId();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
