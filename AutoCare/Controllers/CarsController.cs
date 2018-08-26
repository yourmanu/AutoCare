using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AutoCare.Controllers
{
    public class CarsController : Controller
    {

        ApplicationDbContext _db;
        public CarsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(string userId = null)
        {

            // user id will be null when customer logs in
            if (userId == null)
            {
                userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var model = new CarAndCustomerViewModel
            {

                Cars = _db.Cars.Where(u => u.UserId == userId),
                UserObj = _db.Users.FirstOrDefault(u => u.Id == userId)
            };

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Car car = _db.Cars.FirstOrDefault(c => c.Id == id);
            // user id will be null when customer logs in
            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { userId = car.UserId });
        }
        //Create Get
        public IActionResult Create(string userId)
        {
            Car car = new Car
            {
                Year = DateTime.Now.Year,
                UserId = userId
            };
            return View(car);
        }

        //Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Car car)
        {
            if (ModelState.IsValid)
            {
                _db.Cars.Add(car);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { userId = car.UserId });
            }
            else
            {
                return View(car);
            }
        }
        //Edit Get
        public IActionResult Edit(int id)
        {
            Car car = _db.Cars.FirstOrDefault(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }
        //Edit post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Car car)
        {
            
            if (car.Id != id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.Cars.Update(car);
                await _db.SaveChangesAsync();
            }
            else
            {
                return View(car);
            }

            //return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Index), new { userId = car.UserId });
        }

        //Detail Get
        public IActionResult Details(int id)
        {
            Car car = _db.Cars.FirstOrDefault(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
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