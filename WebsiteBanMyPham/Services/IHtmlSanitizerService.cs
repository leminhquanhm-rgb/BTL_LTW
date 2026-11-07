namespace WebsiteBanMyPham.Services
{
    public interface IHtmlSanitizerService
    {
        /// <summary>
        /// Làm sạch mã HTML cho mô tả sản phẩm theo danh sách thẻ và thuộc tính cho phép (AGENTS 2.9).
        /// Chuẩn hóa &amp;nbsp; và \u00A0 thành dấu cách thường. Tự gắn rel="noopener noreferrer" cho thẻ &lt;a&gt;.
        /// Trả về null nếu HTML rỗng hoặc chỉ có thẻ trắng.
        /// </summary>
        string? Sanitize(string? html);

        /// <summary>
        /// Kiểm tra độ dài chuỗi HTML đã làm sạch có vượt quá giới hạn tối đa 20.000 ký tự hay không.
        /// </summary>
        bool KiemTraDoDaiHopLe(string? html);
    }
}

