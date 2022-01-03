using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using TXTextControl.DocumentServer;

namespace aspnet_core_webapp_textcontrol.Pages
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TXController : ControllerBase
    {
        private IWebHostEnvironment Environment;

        public TXController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }


        [HttpPost]
        public string ExportPDF([FromForm]string document)
        {
            byte[] bPDF;

            // create temporary ServerTextControl
            using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl())
            {
                tx.Create();

                // load the document
                tx.Load(Convert.FromBase64String(document), TXTextControl.BinaryStreamType.InternalUnicodeFormat);

                TXTextControl.SaveSettings saveSettings = new TXTextControl.SaveSettings()
                {
                    CreatorApplication = "TX Text Control Sample Application",
                };

                // save the document as PDF
                tx.Save(out bPDF, TXTextControl.BinaryStreamType.AdobePDF, saveSettings);
            }

            // return as Base64 encoded string
            return Convert.ToBase64String(bPDF);
        }

        [HttpPost]
        public string ReportingMerge([FromBody]SimpleMergeData mergeData)
        {
            byte[] bDocument;

            // create temporary ServerTextControl
            using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl())
            {
                tx.Create();

                using (MailMerge mailMerge = new MailMerge())
                {
                    mailMerge.TextComponent = tx;

                    // var path = Server.MapPath("~/App_Data/Documents/reporting_concept.docx");
                    var path = System.IO.Path.Combine(Environment.ContentRootPath, "App_Data/invoice.docx");
                    
                    mailMerge.LoadTemplate(path, FileFormat.WordprocessingML);
                    mailMerge.MergeObject(mergeData);

                    mailMerge.SaveDocumentToMemory(out bDocument,
                        TXTextControl.BinaryStreamType.InternalUnicodeFormat,
                        new TXTextControl.SaveSettings());
                }
            }

            // return as Base64 encoded string
            return Convert.ToBase64String(bDocument);
        }
    }
}
