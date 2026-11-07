using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Services;

namespace WebsiteBanMyPham.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly IThongKeService _thongKeService;
        private readonly IExcelExportService _excelExportService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public DashboardController(
            IThongKeService thongKeService,
            IExcelExportService excelExportService,
            IStringLocalizer<SharedResource> localizer)
        {
            _thongKeService = thongKeService;
            _excelExportService = excelExportService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int? nam)
        {
            var model = await _thongKeService.LayDuLieuDashboardAsync(nam);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> XuatDoanhThuTheoThang([FromQuery] int nam, [FromQuery] int? thang)
        {
            try
            {
                if (thang.HasValue)
                {
                    if (thang.Value < 1 || thang.Value > 12)
                    {
                        TempData[ThongBaoConstant.Loi] = _localizer["Admin_Excel_Error_InvalidMonth"].Value;
                        return RedirectToAction(nameof(Index));
                    }

                    var bytes = await _excelExportService.XuatDoanhThuChiTietThangAsync(nam, thang.Value);
                    var fileName = $"DoanhThu_{nam}-{thang.Value:D2}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    return File(bytes, ExcelContentType, fileName);
                }
                else
                {
                    var bytes = await _excelExportService.XuatDoanhThuTheoThangAsync(nam);
                    var fileName = $"DoanhThu_{nam}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    return File(bytes, ExcelContentType, fileName);
                }
            }
            catch (ExcelMaxRowsExceededException)
            {
                TempData[ThongBaoConstant.Loi] = _localizer["Admin_Excel_Error_MaxRows"].Value;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> XuatSanPhamTonKho()
        {
            try
            {
                var bytes = await _excelExportService.XuatSanPhamTonKhoAsync();
                var fileName = $"TonKho_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                return File(bytes, ExcelContentType, fileName);
            }
            catch (ExcelMaxRowsExceededException)
            {
                TempData[ThongBaoConstant.Loi] = _localizer["Admin_Excel_Error_MaxRows"].Value;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> XuatDanhSachDonHang(
            [FromQuery] string? trangThai,
            [FromQuery] DateTime? tuNgay,
            [FromQuery] DateTime? denNgay,
            [FromQuery] string? keyword)
        {
            try
            {
                var bytes = await _excelExportService.XuatDanhSachDonHangAsync(trangThai, tuNgay, denNgay, keyword);
                var fileName = $"DonHang_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                return File(bytes, ExcelContentType, fileName);
            }
            catch (ExcelMaxRowsExceededException)
            {
                TempData[ThongBaoConstant.Loi] = _localizer["Admin_Excel_Error_MaxRows"].Value;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

