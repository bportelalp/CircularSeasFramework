using System.Collections.Generic;
using System.Threading.Tasks;
using CircularSeas.DB;
using CircularSeas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CircularSeas.Cloud.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagementController : Controller
    {
        private readonly DbService dbService;

        public ManagementController(DbService dbService)
        {
            this.dbService = dbService;
        }

        [HttpGet("materials")]
        public async Task<IActionResult> GetMaterials()
        {
            try
            {
                var result = await dbService.GetMaterials(true, System.Guid.Empty, false);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                
                return null;
                throw;
            }
        }

        // GET: ManagementController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ManagementController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ManagementController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagementController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ManagementController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ManagementController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ManagementController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ManagementController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
