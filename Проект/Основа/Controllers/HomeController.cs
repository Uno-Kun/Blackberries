namespace Blackberries.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Blackberries.Models;
    using Blackberries.Services;
    using Blackberries.ViewModels;
    using System.Threading.Tasks;
    using System.Web;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger) 
        {
            this.logger = logger;
        }
        public ActionResult Index()
        {
            var identity = HttpContext.User.Identity;

            var model = new HomeIndexViewModel();

            if (identity == null || !identity.IsAuthenticated || !(identity is UserIdentity userIdentity))
            {
                this.ViewData["IsAuthenticated"] = false;
                this.ViewData["UserName"] = null;
                this.ViewData["Email"] = null;
                this.ViewData["Role"] = null;
            }
            else 
            {
                this.ViewData["IsAuthenticated"] = true;
                this.ViewData["UserName"] = userIdentity.DisplayName;
                this.ViewData["Email"] = userIdentity.Email;
                this.ViewData["Role"] = userIdentity.Role.ToString();

                if (userIdentity.Role == UserRole.Seller)
                {
                    var housingList = SellerService.GetHousingList(HttpContext);
                    model.SellerHousings = housingList;
                    model.SellerRequestsForViewing = SellerService.GetRequestList(HttpContext);
                }
                else if (userIdentity.Role == UserRole.Buyer)
                {
                    model.BuyerApplicableHousings = BuyerService.GetHousingList(HttpContext);
                    model.BuyerRequestsForViewing = BuyerService.GetBuyerRequestsForViewing(HttpContext);
                }
                else if (userIdentity.Role == UserRole.Admin) 
                {
                    model.AdminCityDistricts = AdminService.GetCityDistricts(HttpContext);
                }
            }

            return View(model); 
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(string userName, string password)
        {
            var authenticationResult = AuthenticationService.Authenticate(userName, password);

            if (authenticationResult.Success)
            {
                await SessionManager.SignInAsync(HttpContext, authenticationResult);

                return Redirect("Index");
            }
            else
            {
                return View("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Logout() 
        {
            await SessionManager.SignOutAsync(HttpContext);

            return Redirect("Index");
        }

        [HttpPost]
        public ActionResult RegisterSeller(SellerViewModel model)
        {
            RegistrationService.Registrate(model);
            return View("Index");
        }

        [HttpPost]
        public ActionResult RegisterBuyer(BuyerViewModel model)
        {
            RegistrationService.Registrate(model);
            return View("Index");
        }
    }
}
