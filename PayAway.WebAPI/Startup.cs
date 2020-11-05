using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PayAway.WebAPI
{
    public class Startup
    {
        //Contructor
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            // routes and endpoints are discovered automatically
            services.AddMvc(c =>
            {
                c.Conventions.Add(new ApiExplorerGroupPerVersionConvention()); // decorate Controllers to distinguish SwaggerDoc (v1, v2, etc.)
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0", new OpenApiInfo 
                        { 
                            Title = "PayAway.WebAPI", 
                            Version = "v0",
                            Description = @"This version of the controller has stubbed out responses to enable front-end development to begin. 
                            <table>
                                <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Version</th>
                                    <th>Changes</th>
                                </tr>
                                </thead>
                                <tbody>
                                <tr>
                                    <td>2020/11/5</td>
                                    <td>v0.10</td>
                                    <td>Changed UpdateCustomer method to take newCustomer object
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/5</td>
                                    <td>v0.6</td>
                                    <td>Added MakeMerchantActive method.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/4</td>
                                    <td>v0.5</td>
                                    <td>Changed UpdateMerchant method to take newMerchant object.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/4</td>
                                    <td>v0.4</td>
                                    <td>reset method now takes a boolean parameter
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/3</td>
                                    <td>v0.3</td>
                                    <td>removed url, IsActive from add merchant request object
                                        corrected path for adding a new merchant was /merchant now /merchants
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/3</td>
                                    <td>v0.21</td>
                                    <td>Internal Code Cleanup
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/2</td>
                                    <td>v0.2</td>
                                    <td>Finalized changes for demo controller. All methods return stubbed data.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/10/28</td>
                                    <td>v0.1</td>
                                    <td>Added Demo controller to get stubbed results
                                    </td>
                                </tr>
                                </tbody>
                            <table>
                            ",

                    TermsOfService = new Uri("https://example.com/terms"),
                            Contact = new OpenApiContact
                            {
                                Name = "Gabriel Levit",
                                Email = @"gabriel.levit@fisglobal.com",
                                Url = new Uri("https://twitter.com/demo"),
                            },
                            License = new OpenApiLicense
                            {
                                Name = "Use under LICX",
                                Url = new Uri("https://example.com/license"),
                            }
                    }
                );

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PayAway.WebAPI",
                    Version = "v1",
                    Description = @"This is a functional implemention of v0. 
                            <table>
                                <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Version</th>
                                    <th>Changes</th>
                                </tr>
                                </thead>
                                <tbody>
                                <tr>
                                    <td>2020/11/5</td>
                                    <td>v1.10</td>
                                    <td>Changed UpdateCustomer method to take newCustomer object
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/5</td>
                                    <td>v1.06</td>
                                    <td>Added MakeMerchantActive method.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/4</td>
                                    <td>v1.05</td>
                                    <td>Changed UpdateMerchant method to take newMerchant object.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/4</td>
                                    <td>v1.04</td>
                                    <td>Implemented AddNewCustomer, UpdateCustomer and DeleteCustomer methods. Finished Implementation of ResetDB method.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/4</td>
                                    <td>v1.03</td>
                                    <td>Started implementation of ResetDB method, Implemented UpdateMerchant and DeleteMerchant methods
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/3</td>
                                    <td>v1.02</td>
                                    <td>Added insert Merchant Method
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/3</td>
                                    <td>v1.01</td>
                                    <td>Initial Implementation
                                    </td>
                                </tr>
                                </tbody>
                            <table>
                            ",

                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Gabriel Levit",
                        Email = @"gabriel.levit@fisglobal.com",
                        Url = new Uri("https://twitter.com/demo"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                }
);

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint in version 3.0 of the specification (OpenAPI Specification)
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.) specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v0/swagger.json", "PayAway.WebAPI v0");
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PayAway.WebAPI v1");

                    // serve the Swagger UI at the app's root (http://localhost:<port>/) otherwise it is at https://localhost:44331/swagger/index.html
                    // note: if you chg this you likely also want to edit launchsettings.json with sets the debug startup up (and remove the swagger from the startup url attribute
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            //  adds route matching to the middleware pipeline. This middleware looks at the set of endpoints defined in the app, and selects the best match based on the request.
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)); // allow any origin

            app.UseAuthorization();

            // adds endpoint execution to the middleware pipeline. It runs the delegate associated with the selected endpoint.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
        {
            /// <summary>
            /// Called to apply the convention to the <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.
            /// </summary>
            /// <param name="controller">The <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.</param>
            public void Apply(ControllerModel controller)
            {
                var controllerNamespace = controller.ControllerType.Namespace; // e.g. "Controllers.v1"
                var apiVersion = controllerNamespace?.Split('.').Last().ToLower();

                controller.ApiExplorer.GroupName = apiVersion;
            }
        }
    }
}
