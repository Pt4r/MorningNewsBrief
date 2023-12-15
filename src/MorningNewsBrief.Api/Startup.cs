using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MorningNewsBrief.Api.Configuration;

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
            services.AddControllers();
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
            });
            services.AddResponseCaching();
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
            app.UseResponseCaching();
            app.UseProblemDetails();
            app.UseRouting();
            app.UseAuthorization();
            if (HostingEnvironment.IsDevelopment() || Settings.EnableSwagger) {
                app.UseSwaggerUI(options => {
                    options.RoutePrefix = "docs";
                    options.DocumentTitle = $"{Settings.ApplicationName} API Documentation";
                    options.SwaggerEndpoint($"/swagger/v1/swagger.json", "Morning News Brief API");
                    options.OAuth2RedirectUrl($"{Settings.Host}/docs/oauth2-redirect.html");
                    options.OAuthClientId("swagger-ui");
                    options.OAuthAppName("Swagger UI");
                });
                app.UseReDoc(options => {
                    options.DocumentTitle = $"{Settings.ApplicationName} API Documentation";
                    options.SpecUrl = "/swagger/v1/swagger.json";
                });
            }
            app.UseEndpoints(endpoints => {
                endpoints.MapSwagger();
                endpoints.MapControllers();
            });
        }
    }
}