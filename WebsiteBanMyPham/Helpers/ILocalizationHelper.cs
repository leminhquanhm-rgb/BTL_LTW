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
    }
}
