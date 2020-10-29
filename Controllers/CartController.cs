using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public ProductUserViewModel ProductUserVM { get; set; }
        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            IEnumerable<Product> prodList = GetProductCart();
            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            // Find user info from Entity Framework
            var identity = (ClaimsIdentity) User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            // var userId = User.FindFirstValue(ClaimTypes.Name);
            var products = GetProductCart();
            ProductUserVM = new ProductUserViewModel
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = products.ToList()           
            };
            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ProductUserViewModel productUserViewModel)
        {          
            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
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
            return RedirectToAction(nameof(Index));
        }
    }
}
