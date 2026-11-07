using AngleSharp.Html.Dom;
using Ganss.Xss;

namespace WebsiteBanMyPham.Services
{
    public class HtmlSanitizerService : IHtmlSanitizerService
    {
        private const int MaxHtmlLength = 20000;
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizerService()
        {
            _sanitizer = new HtmlSanitizer();

            // 1. Chỉ cho phép các thẻ được quy định tại AGENTS 2.9
            _sanitizer.AllowedTags.Clear();
            var allowedTags = new[] { "p", "br", "strong", "em", "u", "h2", "h3", "ul", "ol", "li", "blockquote", "a" };
            foreach (var tag in allowedTags)
            {
                _sanitizer.AllowedTags.Add(tag);
            }

            // 2. Thuộc tính: chỉ cho phép 'href' (Điều chỉnh 7)
            _sanitizer.AllowedAttributes.Clear();
            _sanitizer.AllowedAttributes.Add("href");

            // 3. Giao thức URL: chỉ cho phép http, https, mailto (Điều chỉnh 7)
            _sanitizer.AllowedSchemes.Clear();
            _sanitizer.AllowedSchemes.Add("http");
            _sanitizer.AllowedSchemes.Add("https");
            _sanitizer.AllowedSchemes.Add("mailto");

            // 4. Xóa toàn bộ thuộc tính CSS và quy tắc @rule (Điều chỉnh 7)
            _sanitizer.AllowedCssProperties.Clear();
            _sanitizer.AllowedAtRules.Clear();

            // 5. Hook PostProcessNode: tự động gắn rel="noopener noreferrer" cho thẻ <a>
            _sanitizer.PostProcessNode += (sender, args) =>
            {
                if (args.Node is IHtmlAnchorElement anchor)
                {
                    anchor.SetAttribute("rel", "noopener noreferrer");
                }
            };
        }

        public string? Sanitize(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return null;
            }

            // Điều chỉnh 1: Chuẩn hóa ký tự \u00A0 và thực thể &nbsp; thành dấu cách thường trước khi làm sạch
            var normalized = html.Replace('\u00A0', ' ').Replace("&nbsp;", " ");

            var sanitized = _sanitizer.Sanitize(normalized)?.Trim();

            if (string.IsNullOrWhiteSpace(sanitized))
            {
                return null;
            }

            // Loại bỏ các đoạn rỗng phổ biến mà trình soạn thảo sinh ra
            if (sanitized == "<p><br></p>" || sanitized == "<p></p>" || sanitized == "<br>")
            {
                return null;
            }

            return sanitized;
        }

        public bool KiemTraDoDaiHopLe(string? html)
        {
            if (string.IsNullOrEmpty(html)) return true;
            return html.Length <= MaxHtmlLength;
        }
    }
}

