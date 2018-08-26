using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class UsersController : Controller
    {
        private ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string option = null, string search = null)
        {
            List<ApplicationUser> users = null;

            //var users = _db.Users.ToList();
            if (search == null)
            {
                users = _db.Users.ToList();
            }
            else
            {
                if (option == "email")
                {
                    users = _db.Users.Where(u => u.Email.Contains(search)).ToList();
                }
                if (option == "firstname")
                {
                    users = _db.Users.Where(u => u.FirstName.Contains(search)).ToList();
                }
                if (option == "lastname")
                {
                    users = _db.Users.Where(u => u.LastName.Contains(search)).ToList();
                }
            }
            return View(users);
        }

        public IActionResult Edit(string id)
        {
            var applicationUser = _db.Users.SingleOrDefault(u => u.Id == id);
            return View(applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", user);
            }
            else
            {
                var userinDB = await _db.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
                if (userinDB == null)
                {
                    return NotFound();
                }
                else
                {
                    userinDB.FirstName = user.FirstName;
                    userinDB.LastName = user.LastName;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public IActionResult Details(string id)
        {
            var applicationUserModel = new ApplicationUserModel();
            var applicationUser = _db.Users.SingleOrDefault(u => u.Id == id);
            applicationUserModel.Id = applicationUser.Id;
            applicationUserModel.FirstName = applicationUser.FirstName;
            applicationUserModel.LastName = applicationUser.LastName;
            applicationUserModel.Email = applicationUser.Email;

            // extact user object of selected user
            var user = _db.Users.FirstOrDefault(x => x.UserName == applicationUser.UserName);
            // Check admin user role assigned
            var roles = _db.UserRoles.Where(u => u.UserId == user.Id && u.RoleId == _db.Roles.SingleOrDefault(r => r.Name == SD.AdminEndUser).Id);
            applicationUserModel.IsAdmin = roles.Count() > 0;
            return View(applicationUserModel);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var cars = _db.Cars.Where(c => c.UserId == id).ToList();

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                if (cars.Count > 0)
                {
                    foreach (var car in cars)
                    {
                        _db.Cars.Remove(car);
                    }
                }
                _db.Users.Remove(user);
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