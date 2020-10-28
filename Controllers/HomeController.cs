using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Products = _db.Products.Include(p => p.Category).Include(p => p.AppType),
                Categories = _db.Categories
            };
            return View(homeVM);
        }
        public IActionResult Details(int id)
        {
            var shoppingCartList = GetShoppingCartList();
            DetailsVM detailsVM = new DetailsVM
            {
                Product = _db.Products.Include(p => p.Category).Include(P => P.AppType)
                .Where(p => p.Id == id).FirstOrDefault(),
                ExistsInCart = false
            };
            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id)
                {
                    detailsVM.ExistsInCart = true;
                }
            }
            return View(detailsVM);
        }

        private List<ShoppingCart> GetShoppingCartList()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            var sessionCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if (sessionCart != null && sessionCart.Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            return shoppingCartList;
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            var shoppingCartList = GetShoppingCartList();
            shoppingCartList.Add(new ShoppingCart
            {
                ProductId = id
            });
            // Set session
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {          
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult RemoveFromCart(int id)
        {
            var shoppingCartList = GetShoppingCartList();
            var removedItem = shoppingCartList.Find(i => i.ProductId == id);
            if (removedItem != null)
            {
                shoppingCartList.Remove(removedItem);
            }
            // Set session
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}
