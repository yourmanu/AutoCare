using AutoCare.Data;
using AutoCare.Models.ViewModel;
using AutoCare.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ServicesController(ApplicationDbContext db)
        {
            _db = db;
        }

        //get : Services/Index
        [Authorize]
        public IActionResult Index(int carId)
        {
            var model = new CarAndServicesViewModel
            {
                CarObj = _db.Cars.FirstOrDefault(c => c.Id == carId),
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                PastServicesObj = _db.Services.Where(s => s.CarId == carId).OrderByDescending(s => s.DateAdded)
            };
            model.CarObj.ApplicationUser = _db.Users.FirstOrDefault(u => u.Id == model.CarObj.UserId);
            return View(model);
        }

        //get : Services/create
        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult Create(int carId)
        {
            var model = new CarAndServicesViewModel
            {
                CarObj = _db.Cars.FirstOrDefault(c => c.Id == carId),
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                PastServicesObj = _db.Services.Where(s => s.CarId == carId).OrderByDescending(s => s.DateAdded).Take(5)
            };
            model.CarObj.ApplicationUser = _db.Users.FirstOrDefault(u => u.Id == model.CarObj.UserId);
            return View(model);
        }

        // post: servicetype/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> Create(CarAndServicesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.NewServiceObj.CarId = model.CarObj.Id;
                model.NewServiceObj.DateAdded = DateTime.Now;
                _db.Services.Add(model.NewServiceObj);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Create), new { carId = model.CarObj.Id });
            }
            else
            {
                var newModel = new CarAndServicesViewModel
                {
                    CarObj = _db.Cars.FirstOrDefault(c => c.Id == model.CarObj.Id),
                    ServiceTypesObj = _db.ServiceTypes.ToList(),
                    PastServicesObj = _db.Services.Where(s => s.CarId == model.CarObj.Id).OrderByDescending(s => s.DateAdded).Take(5)
                };
                model.PastServicesObj = _db.Services.Where(s => s.CarId == model.CarObj.Id).OrderByDescending(s => s.DateAdded).Take(5);
                model.ServiceTypesObj = _db.ServiceTypes.ToList();
                return View(model);
            }
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _db.Services.SingleOrDefaultAsync(s => s.Id == id);
            if (service != null)
            {
                _db.Services.Remove(service);
                await _db.SaveChangesAsync();
                if (User.IsInRole(SD.AdminEndUser))
                {
                    return RedirectToAction(nameof(Create), new { carId = service.CarId });
                }
                return RedirectToAction(nameof(Index), new { carId = service.CarId });
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