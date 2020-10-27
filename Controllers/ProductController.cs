using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Products.Include(p => p.Category).Include(p => p.AppType);
            // Performance bad (Lazy loading instead)
            /*foreach(var obj in objList)
            {
                // Foreign key filling
                obj.Category = _db.Categories.FirstOrDefault(u => u.Id == obj.Id);
                obj.AppType = _db.AppTypes.FirstOrDefault(u => u.Id == obj.Id);
            }*/
            return View(objList);
        }

        //Get - Upsert
        public IActionResult Upsert(int? id)
        {
            /*IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> CategoryDropDown = _db.Categories.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });*/
            // ViewBag.CategoryDropDown = CategoryDropDown;
            /*ViewData["CategoryDropDown"] = CategoryDropDown;
            Product product = new Product();*/
            ProductVM productVM = new ProductVM
            {
                Product = new Product(),
                CategoryDropDown = _db.Categories.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                AppTypeDropDown = _db.AppTypes.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                // this is for create
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Products.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                } 
                else
                {
                    return View(productVM);
                }
            }
        }

        //Post - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (productVM.Product.Id == 0)
                {
                    // Creating
                    // File uploading C# code
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extension;
                    _db.Products.Add(productVM.Product);           
                } 
                else
                {
                    // Updating
                    var objFromDb = _db.Products.AsNoTracking().FirstOrDefault(p =>                   
                        p.Id == productVM.Product.Id
                    );
                    if (objFromDb == null)
                    {
                        return NotFound();
                    }
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        // remove the old file
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _db.Products.Update(productVM.Product);

                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM.CategoryDropDown = _db.Categories.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.AppTypeDropDown = _db.AppTypes.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(productVM.Product);
        }
            
        //Get - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Products.Include(p => p.Category).Include(p => p.AppType).Where(p => p.Id == id).FirstOrDefault();              
            if (obj == null)
            {
                return NotFound();
            }          
            return View(obj);
        }

        //Post - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int? id)
        {
            var obj = _db.Products.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            string webRootPath = _webHostEnvironment.WebRootPath;
            string deleteRootPath = webRootPath + WC.ImagePath;
            string targetFile = Path.Combine(deleteRootPath, obj.Image);
            if (System.IO.File.Exists(targetFile))
            {
                // Remove the file
                System.IO.File.Delete(targetFile);
            }
            // Remove the product from the database
            _db.Products.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
