using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;

namespace Rocky.Controllers
{
    public class AppTypeController : Controller
    {
        private ApplicationDbContext _db;
        public AppTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<AppType> appTypes = _db.AppTypes;
            return View(appTypes);
        }
        public IActionResult Create()
        {           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppType obj)
        {
            if (ModelState.IsValid)
            {
                _db.AppTypes.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);          
        }
        // Get- Edit
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var appType = _db.AppTypes.Find(id);
            if (appType == null)
            {
                return NotFound();
            }
            return View(appType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppType obj)
        {
            if (ModelState.IsValid)
            {
                _db.AppTypes.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        // Get- Delete
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var appType = _db.AppTypes.Find(id);
            if (appType == null)
            {
                return NotFound();
            }
            return View(appType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(AppType obj)
        {        
            _db.AppTypes.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
