using System;
using System.IO;
using System.Threading.Tasks;
using CircularSeas.DB;
using CircularSeas.GenPDF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CircularSeas.Cloud.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocsController : Controller
    {
        private readonly DbService dbService;
        private readonly PdfGenerator pdf;
        private readonly IWebHostEnvironment env;

        public DocsController(DbService dbService, GenPDF.PdfGenerator pdf, IWebHostEnvironment env)
        {
            this.dbService = dbService;
            this.pdf = pdf;
            this.env = env;
        }

        [HttpGet("pdf/{orderId}")]
        public async Task<IActionResult> GetPdf([FromRoute] Guid orderId)
        {
            try
            {
                var order = await dbService.GetOrder(orderId);

                var bytes = pdf.CreateSpoolPDF(order);

                return File(bytes, "application/pdf",
                    $"{order.CreationDate.ToString("yyMMdd")}_{order.Node.Name}_{order.Material.Name}.pdf");

            }
            catch (Exception ex)
            {
                throw;
                BadRequest(ex.Message);

            }
        }
    }
}
