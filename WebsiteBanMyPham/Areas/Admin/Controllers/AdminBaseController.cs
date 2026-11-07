using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanMyPham.Constants;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = VaiTroConstant.QuanTriVien)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public abstract class AdminBaseController : Controller
    {
    }
}

