using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class ServiceTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ServiceTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var serviceTypes = _db.ServiceTypes.FromSql("GET_SERVICETYPES").ToList();
            return View(serviceTypes);
            //return View(_db.ServiceTypes.ToList());
        }

        //get : Servicetype/create
        public IActionResult Create()
        {
            return View();
        }

        // post: servicetype/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceType serviceType)
        {
            if (ModelState.IsValid)
            {
                _db.ServiceTypes.Add(serviceType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceType);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var serviceType = await _db.ServiceTypes.SingleOrDefaultAsync(m => m.ServicetypeId == id);
            if (serviceType == null)
            {
                return NotFound();
            }
            return View(serviceType);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var serviceType = _db.ServiceTypes.SingleOrDefault(m => m.ServicetypeId == id);
            if (serviceType != null)
            {
                return View(serviceType);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceType serviceType)
        {
            if (serviceType.ServicetypeId != id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.ServiceTypes.Update(serviceType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceType);
        }

        public IActionResult Delete(int id)
        {
            var serviceType = _db.ServiceTypes.SingleOrDefault(m => m.ServicetypeId == id);
            if (serviceType != null)
            {
                return View(serviceType);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveServiceType(int id)
        {
            var serviceType = await _db.ServiceTypes.SingleOrDefaultAsync(m => m.ServicetypeId == id);
            if (serviceType != null)
            {
                _db.ServiceTypes.Remove(serviceType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
        }
    }
}