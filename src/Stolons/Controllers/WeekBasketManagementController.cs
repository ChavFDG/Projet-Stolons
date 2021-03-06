using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Hosting;
using System.Security.Claims;
using Stolons.ViewModels.WeekBasketManagement;

namespace Stolons.Controllers
{
    public class WeekBasketManagementController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public WeekBasketManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;
        }

        // GET: Bills
        public IActionResult Index()
        {
            VmWeekBasketManagement vm = new VmWeekBasketManagement();
            vm.ConsumerBills = _context.ConsumerBills.Include(x=>x.Consumer).Where(x => x.State == BillState.Pending).OrderBy(x=>x.Consumer.Id).ToList();
            vm.ProducerBills = _context.ProducerBills.Include(x => x.Producer).Where(x => x.State != BillState.Paid).OrderBy(x => x.Producer.Id).ToList();
            return View(vm);
        }

        // GET: UpdateConsumerBill
        public IActionResult UpdateConsumerBill(string billNumber)
        {
            IBill bill = _context.ConsumerBills.Include(x => x.Consumer).First(x => x.BillNumber == billNumber);
            bill.State = BillState.Paid;
            _context.Update(bill);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: UpdateProducerBill
        public IActionResult UpdateProducerBill(string billNumber)
        {
            IBill bill = _context.ProducerBills.Include(x=>x.Producer).First(x => x.BillNumber == billNumber);
            bill.State++;
            _context.Update(bill);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
