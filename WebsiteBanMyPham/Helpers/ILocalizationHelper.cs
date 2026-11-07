using Microsoft.Extensions.Localization;
using WebsiteBanMyPham;

namespace WebsiteBanMyPham.Helpers
{
    public interface ILocalizationHelper
    {
        string FormatCurrency(decimal? amount);
        string FormatDateTime(DateTime? dateTime);
        string GetOrderStatusLabel(string status);
        string GetPaymentStatusLabel(string status);
        string GetRefundStatusLabel(string status);
        string TenDanhMuc(int maDM, string tenGoc);
        string MaDonHang(int maDH);
        string GetPaymentMethodLabel(string tenPT);
        string GetReviewStatusLabel(string? trangThai);
        string GetComplaintStatusLabel(string? trangThai);
        string FormatCompactCurrency(decimal amount);
        string FormatShortMonth(int month, int year);
        string FormatMonthName(int month);
    }
}
