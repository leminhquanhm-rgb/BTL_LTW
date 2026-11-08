using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace WebsiteBanMyPham.Helpers
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Tuần tự hóa đối tượng sang chuỗi JSON và lưu vào Session.
        /// </summary>
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        /// <summary>
        /// Đọc chuỗi JSON từ Session và giải tuần tự hóa về đối tượng kiểu T.
        /// Trả về default(T) nếu khóa không tồn tại hoặc dữ liệu JSON không hợp lệ.
        /// </summary>
        public static T? GetObject<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
}

