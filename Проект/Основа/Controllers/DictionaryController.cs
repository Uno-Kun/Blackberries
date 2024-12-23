using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blackberries.Services;
using System.Web;

namespace Blackberries.Controllers
{
    public class DictionaryController : Controller
    {
        [Authorize]
        [HttpPost]
        public ActionResult CityDistrict()
        {
            var result = DictionaryService.GetAllCityDistrict()
                .Select(x => new
                {
                    id = x.Id,
                    name = HttpUtility.HtmlEncode($"{x.City} - {x.District}"),
                }).ToArray();

            return new JsonResult(result);
        }
    }
}
