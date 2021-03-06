using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

using Hellang.Middleware.ProblemDetails;

using PayAway.WebAPI.BizTier;
using PayAway.WebAPI.Entities.Config;
using PayAway.WebAPI.PushNotifications;
using PayAway.WebAPI.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;

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
            // load configuration (from secrets file in dev or from enviroment variables in Azure)
            var smsServiceConfig = Configuration.GetSection("SMSConfig").Get<SMSServiceConfigBE>();
            var webUrlConfig = Configuration.GetSection("WebURLConfig").Get<WebUrlConfigurationBE>();

            // Inject configuration class into static instance
            services.AddSMSServiceConfig(smsServiceConfig ?? new SMSServiceConfigBE());
            services.AddSingleton(webUrlConfig ?? new WebUrlConfigurationBE());

            // inject DBContext in the Service IOC container
            services.AddDbContext<SQLiteDBContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString(@"SQLiteDB"));
            });

            services.AddCors();     // <== enable Cors since request will be coming from a different URL (the Web UIs)
            services.AddSignalR();
            services.AddControllers();

            services.AddProblemDetails(); // Add the required services

            services.AddControllers()
            .AddJsonOptions(options =>
            {
                // serialize enums as strings
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            // routes and endpoints are discovered automatically
            services.AddMvc(c =>
            {
                c.Conventions.Add(new ApiExplorerGroupPerVersionConvention()); // decorate Controllers to distinguish SwaggerDoc (v1, v2, etc.)
            });

            // add a health checks
            services.AddHealthChecks()
                        .AddDbContextCheck<SQLiteDBContext>()
                        .AddUrlGroup(new Uri(SMSController.TWILIO_API_URL), name: @"Twilio WebAPI");

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
                                    <td>2020/12/1</td>
                                    <td>v0.52</td>
                                    <td> Added merchantUrl to demo controller
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/24</td>
                                    <td>v0.51</td>
                                    <td>Finished stubbed out methods for customer Controller.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/24</td>
                                    <td>v0.50</td>
                                    <td>Added CustomerController, CustomerOrderMBE, and PaymentInfoMBE.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/12</td>
                                    <td>v0.4</td>
                                    <td>Added SendOrder method.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/11</td>
                                    <td>v0.3</td>
                                    <td>Added methods GetOrder, CreateOrder, and UpdateOrder.
                                    </td>
                                </tr>
                                </tbody>
                            <table>
                            ",
                            //    < tr>
                            //        <td>2020/11/11</td>
                            //        <td>v0.21</td>
                            //        <td>Get active merchant call on Merchant's Controller now includes the list of default Catalog Items
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/10</td>
                            //        <td>v0.20</td>
                            //        <td>Implemented Merchant Controller. Added GetActiveMerchants and GetOrderQueue methods.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/5</td>
                            //        <td>v0.10</td>
                            //        <td>Changed UpdateCustomer method to take newCustomer object
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/5</td>
                            //        <td>v0.06</td>
                            //        <td>Added MakeMerchantActive method.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/4</td>
                            //        <td>v0.05</td>
                            //        <td>Changed UpdateMerchant method to take newMerchant object.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/4</td>
                            //        <td>v0.04</td>
                            //        <td>reset method now takes a boolean parameter
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/3</td>
                            //        <td>v0.03</td>
                            //        <td>removed url, IsActive from add merchant request object
                            //            corrected path for adding a new merchant was /merchant now /merchants
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/3</td>
                            //        <td>v0.021</td>
                            //        <td>Internal Code Cleanup
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/2</td>
                            //        <td>v0.02</td>
                            //        <td>Finalized changes for demo controller. All methods return stubbed data.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/10/28</td>
                            //        <td>v0.01</td>
                            //        <td>Added Demo controller to get stubbed results
                            //        </td>
                            //    </tr>
                            //    </tbody>
                            //<table>
                            //",

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
                    Description = @"This WebAPI supports the three (3) UI components of the 2020 1819 Innovation Project (Demo Setup, Merchant Experience and Customer Experience) 
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
                                    <td>2020/12/3</td>
                                    <td>v1.65</td>
                                    <td> Removed OrderGuid from PaymentInfoMBE and renamed SendPaymentOrder to SubmitPaymentOrder.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/12/1</td>
                                    <td>v1.64</td>
                                    <td> Added merchantUrl to demo controller
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/12/1</td>
                                    <td>v1.63</td>
                                    <td> Fixed SendPaymentOrder and added new payment info to orders.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/24</td>
                                    <td>v1.62</td>
                                    <td> Implemented SendPaymentOrder method for customer controller.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/24</td>
                                    <td>v1.61</td>
                                    <td> Implemented GetCustomerOrder method for customer controller.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/24</td>
                                    <td>v1.60</td>
                                    <td>Implemented Sending SMS messages to demo customers.<br>
                                        GetAllMerchants method includes list of demo customers.
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/22</td>
                                    <td>v1.50</td>
                                    <td>Implemented Date and ID property renaming<br>
                                        Implemented Tech Debt item re init only properties<br>
                                        Get active merchant call on Merchant's Controller now includes the list of demo Customers
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/22</td>
                                    <td>v1.42</td>
                                    <td>Implemented logo upload and retrieval
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/21</td>
                                    <td>v1.41</td>
                                    <td>Upgraded to .NET 5 RTM and deployed to a new URL<br>
                                        Added 2 flags to Order to be used by UI<br>
                                        1st pass of implementing internal Tech Debt Items
                                    </td>
                                </tr>
                                <tr>
                                    <td>2020/11/20</td>
                                    <td>v1.31</td>
                                    <td>Implemented SendOrderPaymentLink method. 
                                    </td>
                                </tr>
                            </tbody>
                        <table>
                            ",
                            //    < tr>
                            //        <td>2020/11/19</td>
                            //        <td>v1.3</td>
                            //        <td>Implemented UpdateOrder method and changed UpdateOrder and CreateOrder method to only take in item guid for order items.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/18</td>
                            //        <td>v1.22</td>
                            //        <td>Implemented CreateOrder method.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/17</td>
                            //        <td>v1.21</td>
                            //        <td>Implemented GetOrder method and other DBContext Methods.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/14</td>
                            //        <td>v1.20</td>
                            //        <td>Significant refactoing of SQLite Entites and DB Context (PKs are now autogenerated ints, guids are autogenerated)
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/6</td>
                            //        <td>v1.13</td>
                            //        <td>Added Fix to UpdateCustomers.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/6</td>
                            //        <td>v1.12</td>
                            //        <td>Trimed Merchant Names and Customer Names when updating or adding a new merchant or customer.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/6</td>
                            //        <td>v1.11</td>
                            //        <td>Resolved common issue with Insert & Update methods that masked the underlying DB UK violation exception with a different vague exception
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/5</td>
                            //        <td>v1.10</td>
                            //        <td>Changed UpdateCustomer method to take newCustomer object
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/5</td>
                            //        <td>v1.06</td>
                            //        <td>Added MakeMerchantActive method.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/4</td>
                            //        <td>v1.05</td>
                            //        <td>Changed UpdateMerchant method to take newMerchant object.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/4</td>
                            //        <td>v1.04</td>
                            //        <td>Implemented AddNewCustomer, UpdateCustomer and DeleteCustomer methods. Finished Implementation of ResetDB method.
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/4</td>
                            //        <td>v1.03</td>
                            //        <td>Started implementation of ResetDB method, Implemented UpdateMerchant and DeleteMerchant methods
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/3</td>
                            //        <td>v1.02</td>
                            //        <td>Added insert Merchant Method
                            //        </td>
                            //    </tr>
                            //    <tr>
                            //        <td>2020/11/3</td>
                            //        <td>v1.01</td>
                            //        <td>Initial Implementation
                            //        </td>
                            //    </tr>
                            //    </tbody>
                            //<table>
                            //",

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
            app.UseProblemDetails(); // Add the middleware

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

            // enable serving static merchant logo image files
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    // do not cache logo images (so we can see changes when we upload a new one)
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "No-Cache";
                },

                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), GeneralConstants.LOGO_IMAGES_FOLDER_NAME)), RequestPath = new PathString($"/{GeneralConstants.LOGO_IMAGES_URI_FOLDER}")
            });

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true)); // allow any origin

            app.UseAuthorization();

            // adds endpoint execution to the middleware pipeline. It runs the delegate associated with the selected endpoint.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/orderUpdates");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    AllowCachingResponses = false,                  // prevent caching
                    ResponseWriter = WriteHealthCheckResponse       // customize output
                });
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

        private static Task WriteHealthCheckResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("status", result.Status.ToString());
                    writer.WriteStartObject("results");
                    foreach (var entry in result.Entries)
                    {
                        writer.WriteStartObject(entry.Key);
                        writer.WriteString("status", entry.Value.Status.ToString());
                        writer.WriteString("description", entry.Value.Description);
                        writer.WriteStartObject("data");
                        foreach (var item in entry.Value.Data)
                        {
                            writer.WritePropertyName(item.Key);
                            JsonSerializer.Serialize(
                                writer, item.Value, item.Value?.GetType() ??
                                typeof(object));
                        }
                        writer.WriteEndObject();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                var json = Encoding.UTF8.GetString(stream.ToArray());

                return context.Response.WriteAsync(json);
            }
        }
    }
}
