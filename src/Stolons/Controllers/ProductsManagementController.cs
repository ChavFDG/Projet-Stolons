using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Hosting;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Stolons.ViewModels.ProductsManagement;
using Microsoft.AspNet.Authorization;
using System.Drawing;

namespace Stolons.Controllers
{
    public class ProductsManagementController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public ProductsManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;    
        }

        // GET: ProductsManagement
        [Authorize(Roles = Configurations.UserType_Producer)]
        public async Task<IActionResult> Index()
        {
            var appUser = await GetCurrentUserAsync();
            var products = _context.Products.Include(m => m.Familly).Include(m=>m.Familly.Type).Where(x => x.Producer.Email == appUser.Email).ToList();
            return View(products);
        }

	[Authorize(Roles = Configurations.UserType_Producer)]
	[HttpGet, ActionName("ProducerProducts"), Route("api/producerProducts")]
	public string JsonProducerProducts()
        {
	        var appUser = GetCurrentUserSync();
	        List<ProductViewModel> vmProducts = new List<ProductViewModel>();
	        var products = _context.Products.Include(m => m.Familly).Include(m=>m.Familly.Type).Where(x => x.Producer.Email == appUser.Email).ToList();
	        foreach (var product in products)
	        {
		        int orderedQty = 0;
                List<BillEntry> billEntries = new List<BillEntry>();
                foreach(var validateWeekBasket in _context.ValidatedWeekBaskets.Include(x => x.Products))
                {
                    validateWeekBasket.Products.Where(x => x.ProductId == product.Id).ToList().ForEach(x=> orderedQty +=x.Quantity);
                }
		        vmProducts.Add(new ProductViewModel(product, orderedQty));
	        }
	        return JsonConvert.SerializeObject(vmProducts, Formatting.Indented, new JsonSerializerSettings() {
		        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
		    });
	    }

        // GET: ProductsManagement/Details/5
        [Authorize(Roles = Configurations.UserType_Producer)]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = _context.Products.Single(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: ProductsManagement/Create
        [Authorize(Roles = Configurations.UserType_Producer)]
        public IActionResult Manage(Guid? id)
        {
            Product product = id == null ? new Product() : _context.Products.Include(x=>x.Familly).First(x => x.Id == id);
            return View(new ProductEditionViewModel(product, _context,id == null));

        }

        // POST: ProductsManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Configurations.UserType_Producer)]
        public async Task<IActionResult> Manage(ProductEditionViewModel vmProduct)
        {
            if (ModelState.IsValid)
            {
                //Set Labels
                vmProduct.Product.SetLabels(vmProduct.SelectedLabels);
                //Set Product familly (si �a retourne null c'est que la famille selectionn�e n'existe pas, alors on est dans la merde)
                vmProduct.Product.Familly = _context.ProductFamillys.FirstOrDefault(x => x.FamillyName == vmProduct.FamillyName);
                //Set Producer (si �a retourne null, c'est que c'est pas un producteur qui est logger, alors on est dans la merde)
                var appUser = await GetCurrentUserAsync();
                vmProduct.Product.Producer = _context.Producers.FirstOrDefault(x => x.Email == appUser.Email);
                //On s'occupe des images du produit
                int cpt = 0;
                foreach (IFormFile uploadFile in new List<IFormFile>() { vmProduct.UploadFile1, vmProduct.UploadFile2, vmProduct.UploadFile3 })
                {
                    if (uploadFile != null)
                    {
                        //Image uploading
                        string fileName = await Configurations.UploadAndResizeImageFile(_environment, uploadFile, Configurations.ProductsStockagePath);
                        if(!vmProduct.IsNew && vmProduct.Product.Pictures.Count > cpt)
                        {
                            //Replace
                            vmProduct.Product.Pictures[cpt] = fileName;
                        }
                        else
                        {
                            //Add
                            vmProduct.Product.Pictures.Add(fileName);
                        }
                    }
                    cpt++;
                }
                if(vmProduct.IsNew)
                {
                    vmProduct.Product.Id = Guid.NewGuid();
                    _context.Products.Add(vmProduct.Product);
                }
                else
                {
                    _context.Products.Update(vmProduct.Product);
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            vmProduct.RefreshTypes(_context);
            return View(vmProduct);
        }
        
        // GET: ProductsManagement/Delete/5
        [ActionName("Delete")]
        [Authorize(Roles = Configurations.UserType_Producer)]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = _context.Products.Single(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: ProductsManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Configurations.UserType_Producer)]
        public IActionResult DeleteConfirmed(Guid id)
        {
            Product product = _context.Products.Single(m => m.Id == id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = Configurations.UserType_Producer)]
        public IActionResult Enable(Guid? id)
        {
            _context.Products.First(x => x.Id == id).State = Product.ProductState.Enabled;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        [Authorize(Roles = Configurations.UserType_Producer)]
        public IActionResult EnableAllStockProduct()
        {
            foreach(var product in _context.Products.Where(x => x.State == Product.ProductState.Stock))
            {
                product.State = Product.ProductState.Enabled;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = Configurations.UserType_Producer)]
        public IActionResult DisableAllProduct()
        {
            foreach (var product in _context.Products.Where(x => x.State != Product.ProductState.Disabled))
            {
                product.State = Product.ProductState.Disabled;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = Configurations.UserType_Producer)]
        public IActionResult Disable(Guid? id)
        {
            _context.Products.First(x => x.Id == id).State = Product.ProductState.Disabled;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = Configurations.UserType_Producer)]
        [HttpPost, ActionName("ChangeStock")]
        public IActionResult ChangeStock(Guid id, float newStock)
        {
            _context.Products.First(x => x.Id == id).WeekStock = newStock;
            _context.Products.First(x => x.Id == id).RemainingStock = newStock;
            _context.SaveChanges();
	    return Ok();
        }

	[Authorize(Roles = Configurations.UserType_Producer)]
        [HttpPost, ActionName("ChangeCurrentStock")]
        public IActionResult ChangeCurrentStock(Guid id, float newStock)
        {
            _context.Products.First(x => x.Id == id).RemainingStock = newStock;
            _context.SaveChanges();
            return Ok();
        }

	[Authorize(Roles = Configurations.UserType_Producer)]
	private ApplicationUser GetCurrentUserSync()
        {
            return _userManager.FindByIdAsync(HttpContext.User.GetUserId()).GetAwaiter().GetResult();
        }

        [Authorize(Roles = Configurations.UserType_Producer)]
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
