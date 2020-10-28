using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Utility;

namespace Rocky.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            IEnumerable<Product> prodList = GetProductCart();
            return View(prodList);
        }

        private IEnumerable<Product> GetProductCart()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            var shoppingCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if (shoppingCart != null && shoppingCart.Count() > 0)
            {
                // Session exists
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            List<int> productInCart = shoppingCartList.Select(p => p.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Products.Where(p => productInCart.Contains(p.Id));
            return prodList;
        }

        public IActionResult DeleteProduct(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            var shoppingCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if (shoppingCart != null && shoppingCart.Count() > 0)
            {
                // Session exists
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
                shoppingCartList.Remove(shoppingCartList.Find(item => item.ProductId == id));
                // Set Session
                HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            }
            IEnumerable<Product> prodList = GetProductCart();
            return View(prodList);

        }
    }
}
