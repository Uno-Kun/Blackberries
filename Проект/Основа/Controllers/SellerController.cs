using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blackberries.Services;
using Blackberries.ViewModels;

namespace Blackberries.Controllers
{
    public class SellerController : Controller
    {
        [Authorize]
        [HttpPost]
        public ActionResult GetSeller()
        {
            var model = SellerService.GetCurrentSeller(HttpContext);

            return new JsonResult(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSeller(SellerViewModel model)
        {
            SellerService.UpdateSeller(model, HttpContext);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetHousing(long housingId)
        {
            var model = SellerService.GetHousing(housingId, HttpContext);

            return new JsonResult(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateOrUpdateHousing(HousingViewModel model) 
        {
            if (model.id == null || model.id == 0)
            {
                SellerService.CreateHousing(model, HttpContext);
            }
            else 
            {
                SellerService.UpdateHousing(model, HttpContext);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult SetRequestConfirmation(long requestId, bool value)
        {
            SellerService.SetRequestConfirmation(requestId, value, HttpContext);

            return RedirectToAction("Index", "Home");
        }
    }
}
