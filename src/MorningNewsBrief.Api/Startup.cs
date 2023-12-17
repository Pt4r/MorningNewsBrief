using AspNetCoreRateLimit;
using Hellang.Middleware.ProblemDetails;
using Microsoft.OpenApi.Models;
using MorningNewsBrief.Common.Configuration;
using System.Reflection;
using System.Text.Json.Serialization;

namespace MorningNewsBrief.Api {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            Settings = Configuration.GetSection(GeneralSettings.Name).Get<GeneralSettings>();
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }
        public GeneralSettings Settings { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers()
                    .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1",
                    new OpenApiInfo {
                        Title = $"{Settings.ApplicationName} API Documentation",
                        Version = "v1",
                        Description = $"{Settings.ApplicationDescription}",
                        Contact = new OpenApiContact {
                            Name = "Stratos Palaiologos",
                            Email = "palaiologosstr@hotmail.com"
                        }
                    });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
            services.AddDistributedCacheConfig(Configuration);
            services.AddOutputCache(options => {
                options.AddBasePolicy(x => x.With(xx => xx.HttpContext.Response.StatusCode > 200 && xx.HttpContext.Response.StatusCode < 300).NoCache());
            });
            services.AddProblemDetailsConfig(HostingEnvironment);
            //services.AddAuthenticationConfig(Settings);
            //services.AddAuthorizationConfig();
            services.AddApplicationInsightsTelemetry();
            services.AddDiConfig(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider spf) {
            if (HostingEnvironment.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseOutputCache();
            app.UseProblemDetails();
            app.UseRouting();
            app.UseAuthorization();
            app.UseIpRateLimiting();
            if (HostingEnvironment.IsDevelopment() || Settings.EnableSwagger) {
                app.UseSwaggerUI(options => {
                    options.RoutePrefix = "docs";
                    options.DocumentTitle = $"{Settings.ApplicationName} API Documentation";
                    options.SwaggerEndpoint($"/swagger/v1/swagger.json", "Morning News Brief API");
                    options.OAuth2RedirectUrl($"{Settings.Host}/docs/oauth2-redirect.html");
                    options.OAuthClientId("swagger-ui");
                    options.OAuthAppName("Swagger UI");
                });
            }
            app.UseReDoc(options => {
                options.DocumentTitle = $"{Settings.ApplicationName} API Documentation";
                options.SpecUrl = "/swagger/v1/swagger.json";
            });
            app.UseEndpoints(endpoints => {
                endpoints.MapSwagger();
                endpoints.MapControllers();
            });
        }
    }
}