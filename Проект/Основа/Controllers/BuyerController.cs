using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blackberries.Services;
using Blackberries.ViewModels;

namespace Blackberries.Controllers
{
    public class BuyerController: Controller
    {
        [Authorize]
        [HttpPost]
        public ActionResult GetBuyer() 
        {
            var buyer = BuyerService.GetCurrentBuyer(HttpContext);

            return new JsonResult(buyer);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateBuyer(BuyerViewModel model) 
        {
            BuyerService.UpdateBuyer(model, HttpContext);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetHousingRequirements()
        {
            var requirements = BuyerService.GetCurrentBuyerHousingRequirements(HttpContext);

            return new JsonResult(requirements);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateOrUpdateHousingRequirements(HousingRequirementsViewModel model)
        {
            if (model.id == null || model.id == 0)
            {
                BuyerService.CreateHousingRequirements(model, HttpContext);
            }
            else
            {
                BuyerService.UpdateHousingRequirements(model, HttpContext);
            }
            

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult CreateRequestForViewing(long housingId) 
        {
            BuyerService.CreateRequestForViewing(housingId, HttpContext);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult DeleteRequestForViewing(long requestId)
        {
            BuyerService.DeleteRequestForViewing(requestId, HttpContext);

            return RedirectToAction("Index", "Home");
        }
    }
}
