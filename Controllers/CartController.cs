using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IWebHostEnvironment _webHostsEnvironment;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ProductUserViewModel ProductUserVM { get; set; }
        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostsEnvironment = webHostEnvironment;
            _emailSender = emailSender;
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
        public async Task<IActionResult> SummaryPostAsync(ProductUserViewModel productUserViewModel)
        {
            string pathToTemplate = _webHostsEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "inquiry.html";
            var subject = "New Inquiry";
            string htmlBody = "";
            // Read template file to htmlBody
            using(var sr = System.IO.File.OpenText(pathToTemplate))
            {
                htmlBody = sr.ReadToEnd();
            }
            StringBuilder productListSB = new StringBuilder();
            foreach (var product in productUserViewModel.ProductList)
            {
                productListSB.Append($" - Name: {product.Name} <span style='font-size: 14px;'> (ID: {product.Id})</span><br/>");
            }
            string messageBody = string.Format(htmlBody,
                productUserViewModel.ApplicationUser.FullName,
                productUserViewModel.ApplicationUser.Email,
                productUserViewModel.ApplicationUser.PhoneNumber,
                productListSB.ToString());
            // Send email to user
            await _emailSender.SendEmailAsync(WC.AdminEmail, subject, messageBody);
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
