using Microsoft.AspNetCore.Mvc;
using Blackberries.Models;
using Blackberries.Services;
using Blackberries.ViewModels;

namespace Blackberries.Controllers
{
    public class ContactsController: Controller
    {
        private readonly ILogger<ContactsController> logger;

        public ContactsController(ILogger<ContactsController> logger)
        {
            this.logger = logger;
        }
        public ActionResult Index()
        {
            var identity = HttpContext.User.Identity;
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
            }
            return View();
        }
    }
}
