using System.Globalization;
using ClosedXML.Excel;
using Microsoft.Extensions.Localization;
using WebsiteBanMyPham.Helpers;

namespace WebsiteBanMyPham.Services
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly IThongKeService _thongKeService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILocalizationHelper _locHelper;

        private static readonly XLColor HeaderBackgroundColor = XLColor.FromArgb(248, 221, 229); // Hồng pastel #F8DDE5
        private static readonly XLColor TotalBackgroundColor = XLColor.FromArgb(243, 244, 246);  // Xám nhạt #F3F4F6

        public ExcelExportService(
            IThongKeService thongKeService,
            IStringLocalizer<SharedResource> localizer,
            ILocalizationHelper locHelper)
        {
            _thongKeService = thongKeService;
            _localizer = localizer;
            _locHelper = locHelper;
        }

        private static bool IsEnglish => CultureInfo.CurrentUICulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase);

        private static string CurrencyNumberFormat => IsEnglish ? "#,##0 \"VND\"" : "#,##0 \"đ\"";
        private static string DateTimeFormatString => IsEnglish ? "MM/dd/yyyy hh:mm AM/PM" : "dd/MM/yyyy HH:mm";

        /// <summary>
        /// Ghi chuỗi vào ô một cách an toàn chống CSV/Formula Injection.
        /// Không ghép thêm dấu nháy đơn vào dữ liệu, mà bật cell.Style.IncludeQuotePrefix = true nếu chuỗi bắt đầu bằng =, +, -, @.
        /// </summary>
        private static void SetSafeString(IXLCell cell, string? value)
        {
            var text = value ?? string.Empty;
            cell.SetValue(text);
            if (!string.IsNullOrEmpty(text) && (text[0] == '=' || text[0] == '+' || text[0] == '-' || text[0] == '@'))
            {
                cell.Style.IncludeQuotePrefix = true;
            }
        }

        private static void ApplyHeaderStyle(IXLRow headerRow, int columnCount)
        {
            for (int col = 1; col <= columnCount; col++)
            {
                var cell = headerRow.Cell(col);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = HeaderBackgroundColor;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromArgb(229, 231, 235);
            }
            headerRow.Height = 24;
        }

        public async Task<byte[]> XuatDoanhThuTheoThangAsync(int nam)
        {
            var data = await _thongKeService.LayDoanhThu12ThangAsync(nam);

            using var workbook = new XLWorkbook();
            var wsName = IsEnglish ? $"Revenue {nam}" : $"Doanh thu {nam}";
            var worksheet = workbook.Worksheets.Add(wsName);

            // Tiêu đề cột
            string[] headers =
            {
                _localizer["Admin_Excel_Col_Month"].Value,
                _localizer["Admin_Excel_Col_CompletedOrders"].Value,
                _localizer["Admin_Excel_Col_Revenue"].Value
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).SetValue(headers[i]);
            }
            ApplyHeaderStyle(worksheet.Row(1), headers.Length);

            // Dữ liệu 12 tháng
            int currentRow = 2;
            int totalOrders = 0;
            decimal totalRevenue = 0m;

            foreach (var item in data)
            {
                var monthText = _locHelper.FormatShortMonth(item.Thang, item.Nam);
                SetSafeString(worksheet.Cell(currentRow, 1), monthText);
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var cellOrders = worksheet.Cell(currentRow, 2);
                cellOrders.SetValue(item.SoDonHoanThanh);
                cellOrders.Style.NumberFormat.Format = "#,##0";
                cellOrders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                var cellRevenue = worksheet.Cell(currentRow, 3);
                cellRevenue.SetValue(item.DoanhThu);
                cellRevenue.Style.NumberFormat.Format = CurrencyNumberFormat;
                cellRevenue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, col).Style.Border.OutsideBorderColor = XLColor.FromArgb(243, 244, 246);
                }

                totalOrders += item.SoDonHoanThanh;
                totalRevenue += item.DoanhThu;
                currentRow++;
            }

            // Dòng tổng cộng
            var totalRow = worksheet.Row(currentRow);
            totalRow.Style.Font.Bold = true;
            totalRow.Style.Fill.BackgroundColor = TotalBackgroundColor;

            SetSafeString(worksheet.Cell(currentRow, 1), _localizer["Admin_Excel_Col_Total"].Value);
            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var totalOrdersCell = worksheet.Cell(currentRow, 2);
            totalOrdersCell.SetValue(totalOrders);
            totalOrdersCell.Style.NumberFormat.Format = "#,##0";
            totalOrdersCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            var totalRevenueCell = worksheet.Cell(currentRow, 3);
            totalRevenueCell.SetValue(totalRevenue);
            totalRevenueCell.Style.NumberFormat.Format = CurrencyNumberFormat;
            totalRevenueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(currentRow, col).Style.Border.OutsideBorderColor = XLColor.FromArgb(209, 213, 219);
            }

            worksheet.Columns().AdjustToContents();
            worksheet.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> XuatDoanhThuChiTietThangAsync(int nam, int thang)
        {
            var data = await _thongKeService.LayDonHoanThanhTheoThangAsync(nam, thang);

            using var workbook = new XLWorkbook();
            var wsName = IsEnglish ? $"Revenue {nam}-{thang:D2}" : $"DoanhThu {thang}-{nam}";
            var worksheet = workbook.Worksheets.Add(wsName);

            // Dòng 1: Tiêu đề báo cáo ghi rõ kỳ báo cáo
            var reportTitle = IsEnglish
                ? $"Revenue {_locHelper.FormatMonthName(thang)} {nam}"
                : $"Doanh thu tháng {thang}/{nam}";

            worksheet.Cell(1, 1).SetValue(reportTitle);
            worksheet.Range(1, 1, 1, 8).Merge();
            var titleRow = worksheet.Row(1);
            titleRow.Height = 28;
            var titleCell = worksheet.Cell(1, 1);
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 13;
            titleCell.Style.Fill.BackgroundColor = HeaderBackgroundColor;
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            titleCell.Style.Border.OutsideBorderColor = XLColor.FromArgb(229, 231, 235);

            // Dòng 2: Tiêu đề các cột
            string[] headers =
            {
                _localizer["Admin_Excel_Col_OrderCode"].Value,
                _localizer["Admin_Excel_Col_Customer"].Value,
                _localizer["Admin_Excel_Col_DeliveredDate"].Value,
                _localizer["Admin_Excel_Col_Subtotal"].Value,
                _localizer["Admin_Excel_Col_PromoCode"].Value,
                _localizer["Admin_Excel_Col_Discount"].Value,
                _localizer["Admin_Excel_Col_TotalPayment"].Value,
                _localizer["Admin_Excel_Col_PaymentMethod"].Value
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(2, i + 1).SetValue(headers[i]);
            }
            ApplyHeaderStyle(worksheet.Row(2), headers.Length);

            // Dữ liệu chi tiết các đơn hoàn thành
            int currentRow = 3;
            decimal totalTongTienHang = 0m;
            decimal totalTienGiam = 0m;
            decimal totalTongThanhToan = 0m;

            foreach (var item in data)
            {
                // Cột 1: Mã đơn
                SetSafeString(worksheet.Cell(currentRow, 1), item.MaDonHangText);
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Cột 2: Khách hàng
                SetSafeString(worksheet.Cell(currentRow, 2), item.TenKhachHang);

                // Cột 3: Ngày giao thành công
                var cellDate = worksheet.Cell(currentRow, 3);
                cellDate.SetValue(item.NgayGiaoThanhCong);
                cellDate.Style.DateFormat.Format = DateTimeFormatString;
                cellDate.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Cột 4: Tiền hàng
                var cellSubtotal = worksheet.Cell(currentRow, 4);
                cellSubtotal.SetValue(item.TongTienHang);
                cellSubtotal.Style.NumberFormat.Format = CurrencyNumberFormat;
                cellSubtotal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Cột 5: Mã khuyến mãi
                SetSafeString(worksheet.Cell(currentRow, 5), item.CodeKM);
                worksheet.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Cột 6: Tiền giảm
                var cellDiscount = worksheet.Cell(currentRow, 6);
                cellDiscount.SetValue(item.TienGiam);
                cellDiscount.Style.NumberFormat.Format = CurrencyNumberFormat;
                cellDiscount.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Cột 7: Tổng thanh toán
                var cellTotal = worksheet.Cell(currentRow, 7);
                cellTotal.SetValue(item.TongThanhToan);
                cellTotal.Style.NumberFormat.Format = CurrencyNumberFormat;
                cellTotal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Cột 8: Phương thức thanh toán
                SetSafeString(worksheet.Cell(currentRow, 8), item.TenPhuongThuc);
                worksheet.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, col).Style.Border.OutsideBorderColor = XLColor.FromArgb(243, 244, 246);
                }

                totalTongTienHang += item.TongTienHang;
                totalTienGiam += item.TienGiam;
                totalTongThanhToan += item.TongThanhToan;
                currentRow++;
            }

            // Dòng tổng cộng cuối (kể cả khi 0 đơn vẫn có dòng tổng)
            var totalRow = worksheet.Row(currentRow);
            totalRow.Style.Font.Bold = true;
            totalRow.Style.Fill.BackgroundColor = TotalBackgroundColor;

            // Cột 1: Chữ "Tổng cộng"
            SetSafeString(worksheet.Cell(currentRow, 1), _localizer["Admin_Excel_Col_Total"].Value);
            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Cột 2: Số đơn
            var orderUnit = _localizer["Admin_Dashboard_Unit_Order"].Value;
            SetSafeString(worksheet.Cell(currentRow, 2), $"{data.Count} {orderUnit}");
            worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Cột 4: Tổng tiền hàng
            var totalSubtotalCell = worksheet.Cell(currentRow, 4);
            totalSubtotalCell.SetValue(totalTongTienHang);
            totalSubtotalCell.Style.NumberFormat.Format = CurrencyNumberFormat;
            totalSubtotalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // Cột 6: Tổng tiền giảm
            var totalDiscountCell = worksheet.Cell(currentRow, 6);
            totalDiscountCell.SetValue(totalTienGiam);
            totalDiscountCell.Style.NumberFormat.Format = CurrencyNumberFormat;
            totalDiscountCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // Cột 7: Tổng thanh toán
            var totalPaymentCell = worksheet.Cell(currentRow, 7);
            totalPaymentCell.SetValue(totalTongThanhToan);
            totalPaymentCell.Style.NumberFormat.Format = CurrencyNumberFormat;
            totalPaymentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(currentRow, col).Style.Border.OutsideBorderColor = XLColor.FromArgb(209, 213, 219);
            }

            worksheet.Columns().AdjustToContents();
            worksheet.SheetView.FreezeRows(2);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> XuatSanPhamTonKhoAsync()
        {
            var data = await _thongKeService.LayDuLieuSanPhamTonKhoExcelAsync();

            using var workbook = new XLWorkbook();
            var wsName = IsEnglish ? "Inventory" : "TonKho";
            var worksheet = workbook.Worksheets.Add(wsName);

            string[] headers =
            {
                _localizer["Admin_Excel_Col_ProductCode"].Value,
                _localizer["Admin_Excel_Col_ProductName"].Value,
                _localizer["Admin_Excel_Col_Category"].Value,
                _localizer["Admin_Excel_Col_Brand"].Value,
                _localizer["Admin_Excel_Col_Price"].Value,
                _localizer["Admin_Excel_Col_Stock"].Value,
                _localizer["Admin_Excel_Col_Threshold"].Value,
                _localizer["Admin_Excel_Col_Status"].Value,
                _localizer["Admin_Excel_Col_Warning"].Value
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).SetValue(headers[i]);
            }
            ApplyHeaderStyle(worksheet.Row(1), headers.Length);

            int currentRow = 2;
            foreach (var item in data)
            {
                // Mã SP
                worksheet.Cell(currentRow, 1).SetValue(item.MaSP);
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Tên SP
                SetSafeString(worksheet.Cell(currentRow, 2), item.TenSP);

                // Danh mục
                SetSafeString(worksheet.Cell(currentRow, 3), item.TenDM);

                // Thương hiệu
                SetSafeString(worksheet.Cell(currentRow, 4), item.TenTH);

                // Giá
                var cellPrice = worksheet.Cell(currentRow, 5);
                cellPrice.SetValue(item.Gia);
                cellPrice.Style.NumberFormat.Format = CurrencyNumberFormat;
                cellPrice.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Tồn kho
                var cellStock = worksheet.Cell(currentRow, 6);
                cellStock.SetValue(item.SoLuongTon);
                cellStock.Style.NumberFormat.Format = "#,##0";
                cellStock.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Ngưỡng cảnh báo
                var cellThreshold = worksheet.Cell(currentRow, 7);
                cellThreshold.SetValue(item.NguongCanhBao);
                cellThreshold.Style.NumberFormat.Format = "#,##0";
                cellThreshold.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Trạng thái
                var statusText = item.TrangThaiKinhDoanh
                    ? _localizer["Admin_Excel_Status_Active"].Value
                    : _localizer["Admin_Excel_Status_Inactive"].Value;
                SetSafeString(worksheet.Cell(currentRow, 8), statusText);
                worksheet.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Cảnh báo
                SetSafeString(worksheet.Cell(currentRow, 9), item.CanhBao);
                worksheet.Cell(currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (item.LaSapHet)
                {
                    worksheet.Cell(currentRow, 9).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 9).Style.Font.Bold = true;
                }

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, col).Style.Border.OutsideBorderColor = XLColor.FromArgb(243, 244, 246);
                }

                currentRow++;
            }

            worksheet.Columns().AdjustToContents();
            worksheet.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> XuatDanhSachDonHangAsync(
            string? trangThai, DateTime? tuNgay, DateTime? denNgay, string? keyword)
        {
            var data = await _thongKeService.LayDuLieuDonHangExcelAsync(trangThai, tuNgay, denNgay, keyword);

            using var workbook = new XLWorkbook();
            var wsName = IsEnglish ? "Orders" : "DonHang";
            var worksheet = workbook.Worksheets.Add(wsName);

            string[] headers =
            {
                _localizer["Admin_Excel_Col_OrderCode"].Value,
                _localizer["Admin_Excel_Col_Customer"].Value,
                _localizer["Admin_Excel_Col_Recipient"].Value,
                _localizer["Admin_Excel_Col_Phone"].Value,
                _localizer["Admin_Excel_Col_OrderDate"].Value,
                _localizer["Admin_Excel_Col_OrderStatus"].Value,
                _localizer["Admin_Excel_Col_PaymentMethod"].Value,
                _localizer["Admin_Excel_Col_PaymentStatus"].Value,
                _localizer["Admin_Excel_Col_TotalPayment"].Value,
                _localizer["Admin_Excel_Col_WaybillDate"].Value,
                _localizer["Admin_Excel_Col_DeliveredDate"].Value
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).SetValue(headers[i]);
            }
            ApplyHeaderStyle(worksheet.Row(1), headers.Length);

            int currentRow = 2;
            foreach (var item in data)
            {
                // Mã đơn hàng text (DH000001)
                SetSafeString(worksheet.Cell(currentRow, 1), item.MaDonHangText);
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Tên khách hàng
                SetSafeString(worksheet.Cell(currentRow, 2), item.TenKhachHang);

                // Tên người nhận
                SetSafeString(worksheet.Cell(currentRow, 3), item.TenNguoiNhan);

                // SĐT người nhận
                SetSafeString(worksheet.Cell(currentRow, 4), item.SdtNguoiNhan);

                // Ngày đặt
                var cellOrderDate = worksheet.Cell(currentRow, 5);
                cellOrderDate.SetValue(item.NgayDat);
                cellOrderDate.Style.DateFormat.Format = DateTimeFormatString;
                cellOrderDate.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Trạng thái đơn
                SetSafeString(worksheet.Cell(currentRow, 6), item.TrangThaiDon);
                worksheet.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Phương thức thanh toán
                SetSafeString(worksheet.Cell(currentRow, 7), item.TenPhuongThuc);
                worksheet.Cell(currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Trạng thái thanh toán
                SetSafeString(worksheet.Cell(currentRow, 8), item.TrangThaiThanhToan);
                worksheet.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Tổng thanh toán
                var cellTotal = worksheet.Cell(currentRow, 9);
                cellTotal.SetValue(item.TongThanhToan);
                cellTotal.Style.NumberFormat.Format = CurrencyNumberFormat;
                cellTotal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Ngày vận đơn
                var cellWaybillDate = worksheet.Cell(currentRow, 10);
                if (item.NgayVanDon.HasValue)
                {
                    cellWaybillDate.SetValue(item.NgayVanDon.Value);
                    cellWaybillDate.Style.DateFormat.Format = DateTimeFormatString;
                }
                else
                {
                    cellWaybillDate.SetValue("-");
                }
                cellWaybillDate.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Ngày giao thành công
                var cellDeliveredDate = worksheet.Cell(currentRow, 11);
                if (item.NgayGiaoThanhCong.HasValue)
                {
                    cellDeliveredDate.SetValue(item.NgayGiaoThanhCong.Value);
                    cellDeliveredDate.Style.DateFormat.Format = DateTimeFormatString;
                }
                else
                {
                    cellDeliveredDate.SetValue("-");
                }
                cellDeliveredDate.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, col).Style.Border.OutsideBorderColor = XLColor.FromArgb(243, 244, 246);
                }

                currentRow++;
            }

            worksheet.Columns().AdjustToContents();
            worksheet.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}

