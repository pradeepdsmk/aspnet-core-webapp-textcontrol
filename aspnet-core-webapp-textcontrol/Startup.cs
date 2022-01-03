using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace aspnet_core_webapp_textcontrol
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
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "ExportPDF",
                    pattern: "{controller=TX}/{action=ExportPDF}"
                );

                endpoints.MapControllerRoute(
                    name: "ReportingMerge",
                    pattern: "{controller=TX}/{action=ReportingMerge}"
                );

                //endpoints.MapPost("/api/TX/ExportPDF", async context => {

                //    // read request body into document variable
                //    string? document;

                //    // Leave the body open so the next middleware can read it.
                //    // context.Request.EnableBuffering();

                //    using (var reader = new StreamReader(
                //        context.Request.Body,
                //        encoding: Encoding.UTF8,
                //        detectEncodingFromByteOrderMarks: false,
                //        bufferSize: 1024,
                //        leaveOpen: true))
                //    {
                //        var requestBodyStr = await reader.ReadToEndAsync();
                //        dynamic requestBody = JsonConvert.DeserializeObject<dynamic>(requestBodyStr);
                //        document = requestBody.document;

                //        // Reset the request body stream position so the next middleware can read it
                //        context.Request.Body.Position = 0;
                //    }


                //    // context.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);

                //    byte[] bPDF;

                //    // create temporary ServerTextControl
                //    using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl())
                //    {
                //        tx.Create();

                //        // load the document
                //        tx.Load(Convert.FromBase64String(document), TXTextControl.BinaryStreamType.InternalUnicodeFormat);

                //        TXTextControl.SaveSettings saveSettings = new TXTextControl.SaveSettings()
                //        {
                //            CreatorApplication = "TX Text Control Sample Application",
                //        };

                //        // save the document as PDF
                //        tx.Save(out bPDF, TXTextControl.BinaryStreamType.AdobePDF, saveSettings);
                //    }

                //    // return as Base64 encoded string
                //    var responseBody = Convert.ToBase64String(bPDF);

                //    await context.Response.WriteAsync("Hello World!!");
                //});
            });



            // enable local 'App_Data' folder to access local documents
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(env.ContentRootPath, "App_Data"));

            // serve static linked files (JavaScript and CSS for the editor)
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                   System.IO.Path.Combine(System.IO.Path.GetDirectoryName(
                       System.Reflection.Assembly.GetEntryAssembly().Location),
                       "TXTextControl.Web")),
                RequestPath = "/TXTextControl.Web"
            });

            // enable Web Sockets
            app.UseWebSockets();

            // attach the Text Control WebSocketHandler middleware
            app.UseMiddleware<TXTextControl.Web.WebSocketMiddleware>();
        }
    }
}
