using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blackberries.Services;
using Blackberries.ViewModels;

namespace Blackberries.Controllers
{
    public class AdminController : Controller
    {
        [Authorize]
        public ActionResult DeleteCityDistrict(long cityDistrictId)
        {
            AdminService.DeleteCityDistrict(cityDistrictId, HttpContext);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult CreateOrUpdateCityDistrict(CityDistrictViewModel model)
        {
            if (model.Id == null || model.Id == 0)
            {
                AdminService.CreateCityDistrict(model, HttpContext);
            }
            else
            {
                AdminService.UpdateCityDistrict(model, HttpContext);
            }


            return RedirectToAction("Index", "Home");
        }
    }
}
