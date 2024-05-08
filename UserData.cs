using Microsoft.OpenApi.Models;
using System.Reflection;

namespace keithjasper.co.uk_restfulapi
{
    /// <summary>
    /// Configures services and middleware for the application startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configures services for the application.
        /// </summary>
        /// <param name="services">The collection of services to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Configure Swagger generation
            services.AddSwaggerGen(c =>
                                   {
                                       // Define Swagger document version and metadata
                                       c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                                       // Determine XML documentation file path
                                       var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                                       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                                       // Include XML comments in Swagger documentation if the XML file exists
                                       if (System.IO.File.Exists(xmlPath))
                                       {
                                           c.IncludeXmlComments(xmlPath);
                                       }
                                       else
                                       {
                                           // Log a message if the XML file is not found
                                           Console.WriteLine(
                                               "XML file not found. Swagger documentation may not include XML comments.");
                                       }
                                   });
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Enable Swagger middleware
            app.UseSwagger();

            // Map controllers
            app.UseEndpoints(endpoints =>
                             { endpoints.MapControllers(); });
        }
    }
}
