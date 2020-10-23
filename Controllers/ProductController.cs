using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductController(ApplicationDbContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Products;
            foreach(var obj in objList)
            {
                // Foreign key filling
                obj.Category = _db.Categories.FirstOrDefault(u => u.Id == obj.Id);
            }
            return View(objList);
        }

        //Get - Upsert
        public IActionResult Upsert(int? id)
        {
            IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> CategoryDropDown = _db.Categories.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            ViewBag.CategoryDropDown = CategoryDropDown;
            Product product = new Product();
            if (id == null)
            {
                // this is for create
                return View(product);
            }
            else
            {
                Product obj = _db.Products.Find(id);
                if (obj == null)
                {
                    return NotFound();
                } 
                else
                {
                    return View(obj);
                }
            }
        }

        //Post - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product obj)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
             

        //Get - Delete
        public IActionResult Delete(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Products.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Post - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Products.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Products.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
