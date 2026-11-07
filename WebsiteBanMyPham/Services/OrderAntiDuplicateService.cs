using System.Collections.Concurrent;

namespace WebsiteBanMyPham.Services
{
    public class OrderIdempotencyEntry
    {
        public string Token { get; }
        public int MaTK { get; }
        public DateTime CreatedAt { get; }
        public DateTime ExpiresAt { get; }
        public OrderStatusTokenState State { get; set; }
        public int? MaDH { get; set; }
        public object LockObj { get; } = new();

        public OrderIdempotencyEntry(string token, int maTK, DateTime expiresAt)
        {
            Token = token;
            MaTK = maTK;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            State = OrderStatusTokenState.Issued;
        }
    }

    public class OrderAntiDuplicateService : IOrderAntiDuplicateService
    {
        private readonly ConcurrentDictionary<string, OrderIdempotencyEntry> _tokens = new();
        private const int MaxActiveTokensPerUser = 20;
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(15);

        public string PhatMa(int maTK)
        {
            CleanupExpired();

            // Giới hạn tối đa 20 mã đang hiệu lực mỗi maTK (vượt thì bỏ mã cũ nhất)
            var userTokens = _tokens.Values
                .Where(e => e.MaTK == maTK && e.ExpiresAt > DateTime.UtcNow)
                .OrderBy(e => e.CreatedAt)
                .ToList();

            if (userTokens.Count >= MaxActiveTokensPerUser)
            {
                int removeCount = userTokens.Count - MaxActiveTokensPerUser + 1;
                for (int i = 0; i < removeCount && i < userTokens.Count; i++)
                {
                    _tokens.TryRemove(userTokens[i].Token, out _);
                }
            }

            var token = Guid.NewGuid().ToString("N");
            var entry = new OrderIdempotencyEntry(token, maTK, DateTime.UtcNow.Add(TokenLifetime));
            _tokens.TryAdd(token, entry);
            return token;
        }

        public ThuNhanMaResult ThuNhanMa(string? token, int maTK)
        {
            CleanupExpired();

            if (string.IsNullOrWhiteSpace(token))
            {
                return new ThuNhanMaResult(ThuNhanMaResultType.Invalid);
            }

            if (!_tokens.TryGetValue(token, out var entry))
            {
                return new ThuNhanMaResult(ThuNhanMaResultType.Invalid);
            }

            if (entry.MaTK != maTK || entry.ExpiresAt <= DateTime.UtcNow)
            {
                return new ThuNhanMaResult(ThuNhanMaResultType.Invalid);
            }

            lock (entry.LockObj)
            {
                switch (entry.State)
                {
                    case OrderStatusTokenState.Issued:
                        // Chuyển sang Processing bằng thao tác nguyên tử
                        entry.State = OrderStatusTokenState.Processing;
                        return new ThuNhanMaResult(ThuNhanMaResultType.Accepted);

                    case OrderStatusTokenState.Processing:
                        return new ThuNhanMaResult(ThuNhanMaResultType.InProgress);

                    case OrderStatusTokenState.Completed:
                        return new ThuNhanMaResult(ThuNhanMaResultType.AlreadyCompleted, entry.MaDH);

                    default:
                        return new ThuNhanMaResult(ThuNhanMaResultType.Invalid);
                }
            }
        }

        public void CapNhatKetQua(string? token, int maDH)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            if (_tokens.TryGetValue(token, out var entry))
            {
                lock (entry.LockObj)
                {
                    entry.MaDH = maDH;
                    entry.State = OrderStatusTokenState.Completed;
                }
            }
        }

        public void HuyMaKhiLoi(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;
            _tokens.TryRemove(token, out _);
        }

        private void CleanupExpired()
        {
            var now = DateTime.UtcNow;
            foreach (var kvp in _tokens)
            {
                if (kvp.Value.ExpiresAt <= now)
                {
                    _tokens.TryRemove(kvp.Key, out _);
                }
            }
        }
    }
}

