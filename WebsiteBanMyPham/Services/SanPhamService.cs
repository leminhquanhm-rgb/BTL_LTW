using Microsoft.EntityFrameworkCore;
using WebsiteBanMyPham.Constants;
using WebsiteBanMyPham.Models;
using WebsiteBanMyPham.ViewModels;

namespace WebsiteBanMyPham.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly WebsiteBanMyPhamContext _context;
        private readonly IKhuyenMaiService _khuyenMaiService;

        public SanPhamService(
            WebsiteBanMyPhamContext context,
            IKhuyenMaiService khuyenMaiService)
        {
            _context = context;
            _khuyenMaiService = khuyenMaiService;
        }

        public async Task<PaginatedList<SanPhamItemViewModel>> TimKiemAsync(SanPhamFilterViewModel filter)
        {
            filter.ChuanHoaGia();

            var query = _context.SanPham
                .AsNoTracking()
                .Where(s => s.TrangThai);

            if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
            {
                var kw = filter.TuKhoa;
                query = query.Where(s => s.TenSP.Contains(kw) || s.MaTHNavigation.TenTH.Contains(kw));
            }

            if (filter.DanhMuc.HasValue)
            {
                query = query.Where(s => s.MaDM == filter.DanhMuc.Value);
            }

            if (filter.ThuongHieu.HasValue)
            {
                query = query.Where(s => s.MaTH == filter.ThuongHieu.Value);
            }

            if (filter.GiaTu.HasValue)
            {
                query = query.Where(s => s.Gia >= filter.GiaTu.Value);
            }

            if (filter.GiaDen.HasValue)
            {
                query = query.Where(s => s.Gia <= filter.GiaDen.Value);
            }

            // Sắp xếp (mọi kiểu sắp xếp kết thúc bằng .ThenBy(s => s.MaSP) theo Điều chỉnh 4)
            switch (filter.SapXep)
            {
                case "giaTang":
                    query = query.OrderBy(s => s.Gia).ThenBy(s => s.MaSP);
                    break;
                case "giaGiam":
                    query = query.OrderByDescending(s => s.Gia).ThenBy(s => s.MaSP);
                    break;
                case "banChay":
                    query = query.OrderByDescending(s => s.ChiTietDonHang
                        .Where(ct => ct.MaDHNavigation.TrangThai == TrangThaiDonHangConstant.HoanThanh)
                        .Sum(ct => (int?)ct.SoLuong) ?? 0)
                        .ThenByDescending(s => s.NgayTao)
                        .ThenBy(s => s.MaSP);
                    break;
                case "moiNhat":
                default:
                    query = query.OrderByDescending(s => s.NgayTao).ThenBy(s => s.MaSP);
                    break;
            }

            var totalItems = await query.CountAsync();

            var pagedData = await query
                .Skip((filter.Trang - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    s.Gia,
                    s.GiaGoc,
                    s.SoLuongTon,
                    s.TrangThai,
                    s.MaDM,
                    TenDanhMuc = s.MaDMNavigation.TenDM,
                    s.MaTH,
                    TenThuongHieu = s.MaTHNavigation.TenTH,
                    AnhChinh = s.AnhSanPham.OrderByDescending(a => a.LaAnhChinh).Select(a => a.DuongDan).FirstOrDefault()
                })
                .ToListAsync();

            var productIds = pagedData.Select(p => p.MaSP).ToList();

            // Điểm sao trung bình gộp đúng 1 truy vấn SQL, làm tròn 1 chữ số trong bộ nhớ (Điều chỉnh 7)
            var rawRatings = await _context.VW_DanhGia
                .AsNoTracking()
                .Where(dg => productIds.Contains(dg.MaSP) && dg.TrangThai == "DaDuyet")
                .GroupBy(dg => dg.MaSP)
                .Select(g => new
                {
                    MaSP = g.Key,
                    DiemTB = g.Average(x => (double)x.SoSao),
                    SoDanhGia = g.Count()
                })
                .ToListAsync();

            var ratingDict = rawRatings.ToDictionary(
                r => r.MaSP,
                r => new { DiemTB = Math.Round(r.DiemTB, 1), r.SoDanhGia }
            );

            var items = pagedData.Select(p =>
            {
                ratingDict.TryGetValue(p.MaSP, out var r);
                return new SanPhamItemViewModel
                {
                    MaSP = p.MaSP,
                    TenSP = p.TenSP,
                    Gia = p.Gia,
                    GiaGoc = p.GiaGoc,
                    SoLuongTon = p.SoLuongTon,
                    TrangThai = p.TrangThai,
                    MaDM = p.MaDM,
                    TenDanhMuc = p.TenDanhMuc,
                    MaTH = p.MaTH,
                    TenThuongHieu = p.TenThuongHieu,
                    AnhChinh = p.AnhChinh,
                    DiemSaoTrungBinh = r != null ? r.DiemTB : 5.0,
                    SoDanhGia = r != null ? r.SoDanhGia : 0
                };
            }).ToList();

            return new PaginatedList<SanPhamItemViewModel>(items, totalItems, filter.Trang, filter.PageSize);
        }

        public async Task<SanPhamChiTietViewModel?> LayChiTietAsync(int maSp)
        {
            var product = await _context.SanPham
                .AsNoTracking()
                .Where(s => s.MaSP == maSp)
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    s.MoTa,
                    s.Gia,
                    s.GiaGoc,
                    s.SoLuongTon,
                    s.TrangThai,
                    s.MaDM,
                    TenDanhMuc = s.MaDMNavigation.TenDM,
                    s.MaTH,
                    TenThuongHieu = s.MaTHNavigation.TenTH,
                    Images = s.AnhSanPham.OrderByDescending(a => a.LaAnhChinh).Select(a => a.DuongDan).ToList()
                })
                .FirstOrDefaultAsync();

            if (product == null) return null;

            // 10 đánh giá DaDuyet mới nhất kèm tên khách hàng che bớt
            var reviews = await _context.DanhGia
                .AsNoTracking()
                .Where(dg => dg.MaCTDHNavigation.MaSP == maSp && dg.TrangThai == "DaDuyet")
                .OrderByDescending(dg => dg.NgayTao)
                .Take(10)
                .Select(dg => new
                {
                    dg.MaDG,
                    TenKhachHang = dg.MaCTDHNavigation.MaDHNavigation.MaTKNavigation.HoTen,
                    dg.SoSao,
                    dg.NoiDung,
                    dg.NgayTao
                })
                .ToListAsync();

            // Điểm sao trung bình sản phẩm
            var rawRating = await _context.VW_DanhGia
                .AsNoTracking()
                .Where(dg => dg.MaSP == maSp && dg.TrangThai == "DaDuyet")
                .GroupBy(dg => dg.MaSP)
                .Select(g => new
                {
                    DiemTB = g.Average(x => (double)x.SoSao),
                    SoDanhGia = g.Count()
                })
                .FirstOrDefaultAsync();

            double diemTB = rawRating != null ? Math.Round(rawRating.DiemTB, 1) : 5.0;
            int soDanhGia = rawRating?.SoDanhGia ?? 0;

            // Tối đa 4 sản phẩm cùng danh mục đang kinh doanh (không phải chính nó)
            var relatedProductsRaw = await _context.SanPham
                .AsNoTracking()
                .Where(s => s.MaDM == product.MaDM && s.MaSP != maSp && s.TrangThai)
                .OrderByDescending(s => s.NgayTao)
                .ThenBy(s => s.MaSP)
                .Take(4)
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    s.Gia,
                    s.GiaGoc,
                    s.SoLuongTon,
                    s.TrangThai,
                    s.MaDM,
                    TenDanhMuc = s.MaDMNavigation.TenDM,
                    s.MaTH,
                    TenThuongHieu = s.MaTHNavigation.TenTH,
                    AnhChinh = s.AnhSanPham.OrderByDescending(a => a.LaAnhChinh).Select(a => a.DuongDan).FirstOrDefault()
                })
                .ToListAsync();

            var relatedIds = relatedProductsRaw.Select(r => r.MaSP).ToList();
            var rawRelatedRatings = await _context.VW_DanhGia
                .AsNoTracking()
                .Where(dg => relatedIds.Contains(dg.MaSP) && dg.TrangThai == "DaDuyet")
                .GroupBy(dg => dg.MaSP)
                .Select(g => new
                {
                    MaSP = g.Key,
                    DiemTB = g.Average(x => (double)x.SoSao),
                    SoDanhGia = g.Count()
                })
                .ToListAsync();

            var relatedRatingDict = rawRelatedRatings.ToDictionary(
                r => r.MaSP,
                r => new { DiemTB = Math.Round(r.DiemTB, 1), r.SoDanhGia }
            );

            var relatedItems = relatedProductsRaw.Select(p =>
            {
                relatedRatingDict.TryGetValue(p.MaSP, out var r);
                return new SanPhamItemViewModel
                {
                    MaSP = p.MaSP,
                    TenSP = p.TenSP,
                    Gia = p.Gia,
                    GiaGoc = p.GiaGoc,
                    SoLuongTon = p.SoLuongTon,
                    TrangThai = p.TrangThai,
                    MaDM = p.MaDM,
                    TenDanhMuc = p.TenDanhMuc,
                    MaTH = p.MaTH,
                    TenThuongHieu = p.TenThuongHieu,
                    AnhChinh = p.AnhChinh,
                    DiemSaoTrungBinh = r != null ? r.DiemTB : 5.0,
                    SoDanhGia = r != null ? r.SoDanhGia : 0
                };
            }).ToList();

            return new SanPhamChiTietViewModel
            {
                MaSP = product.MaSP,
                TenSP = product.TenSP,
                MoTa = product.MoTa,
                Gia = product.Gia,
                GiaGoc = product.GiaGoc,
                SoLuongTon = product.SoLuongTon,
                TrangThai = product.TrangThai,
                MaDM = product.MaDM,
                TenDanhMuc = product.TenDanhMuc,
                MaTH = product.MaTH,
                TenThuongHieu = product.TenThuongHieu,
                DiemSaoTrungBinh = diemTB,
                SoDanhGia = soDanhGia,
                DanhSachAnh = product.Images,
                DanhGiaMoiNhat = reviews.Select(r => new DanhGiaChiTietItemViewModel
                {
                    MaDG = r.MaDG,
                    TenKhachHang = CheTenNguoiDung(r.TenKhachHang),
                    SoSao = r.SoSao,
                    NoiDung = r.NoiDung,
                    NgayTao = r.NgayTao
                }).ToList(),
                SanPhamLienQuan = relatedItems
            };
        }

        public async Task<TrangChuViewModel> LayDanhSachTrangChuAsync()
        {
            var banners = await LayDanhSachBannerAsync();
            var danhMucs = await LayDanhSachDanhMucAsync();
            var khuyenMais = await _khuyenMaiService.LayKhuyenMaiDangDienRaAsync();

            // 1. Sản phẩm mới: 8 sản phẩm theo NgayTao giảm dần
            var spMoiRaw = await _context.SanPham
                .AsNoTracking()
                .Where(s => s.TrangThai)
                .OrderByDescending(s => s.NgayTao)
                .ThenBy(s => s.MaSP)
                .Take(8)
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    s.Gia,
                    s.GiaGoc,
                    s.SoLuongTon,
                    s.TrangThai,
                    s.MaDM,
                    TenDanhMuc = s.MaDMNavigation.TenDM,
                    s.MaTH,
                    TenThuongHieu = s.MaTHNavigation.TenTH,
                    AnhChinh = s.AnhSanPham.OrderByDescending(a => a.LaAnhChinh).Select(a => a.DuongDan).FirstOrDefault()
                })
                .ToListAsync();

            // 2. Sản phẩm bán chạy: tổng SoLuong trong ChiTietDonHang của đơn HoanThanh giảm dần
            var spBanChayQuery = await _context.SanPham
                .AsNoTracking()
                .Where(s => s.TrangThai)
                .Select(s => new
                {
                    s.MaSP,
                    s.TenSP,
                    s.Gia,
                    s.GiaGoc,
                    s.SoLuongTon,
                    s.TrangThai,
                    s.MaDM,
                    TenDanhMuc = s.MaDMNavigation.TenDM,
                    s.MaTH,
                    TenThuongHieu = s.MaTHNavigation.TenTH,
                    AnhChinh = s.AnhSanPham.OrderByDescending(a => a.LaAnhChinh).Select(a => a.DuongDan).FirstOrDefault(),
                    s.NgayTao,
                    DaBan = s.ChiTietDonHang
                        .Where(ct => ct.MaDHNavigation.TrangThai == TrangThaiDonHangConstant.HoanThanh)
                        .Sum(ct => (int?)ct.SoLuong) ?? 0
                })
                .OrderByDescending(s => s.DaBan)
                .ThenByDescending(s => s.NgayTao)
                .ThenBy(s => s.MaSP)
                .Take(8)
                .ToListAsync();

            // Gom ID để lấy đánh giá 1 lần duy nhất cho toàn bộ trang chủ
            var allIds = spMoiRaw.Select(p => p.MaSP)
                .Union(spBanChayQuery.Select(p => p.MaSP))
                .Distinct()
                .ToList();

            var rawRatings = await _context.VW_DanhGia
                .AsNoTracking()
                .Where(dg => allIds.Contains(dg.MaSP) && dg.TrangThai == "DaDuyet")
                .GroupBy(dg => dg.MaSP)
                .Select(g => new
                {
                    MaSP = g.Key,
                    DiemTB = g.Average(x => (double)x.SoSao),
                    SoDanhGia = g.Count()
                })
                .ToListAsync();

            var ratingDict = rawRatings.ToDictionary(
                r => r.MaSP,
                r => new { DiemTB = Math.Round(r.DiemTB, 1), r.SoDanhGia }
            );

            var spMoiList = spMoiRaw.Select(p =>
            {
                ratingDict.TryGetValue(p.MaSP, out var r);
                return new SanPhamItemViewModel
                {
                    MaSP = p.MaSP,
                    TenSP = p.TenSP,
                    Gia = p.Gia,
                    GiaGoc = p.GiaGoc,
                    SoLuongTon = p.SoLuongTon,
                    TrangThai = p.TrangThai,
                    MaDM = p.MaDM,
                    TenDanhMuc = p.TenDanhMuc,
                    MaTH = p.MaTH,
                    TenThuongHieu = p.TenThuongHieu,
                    AnhChinh = p.AnhChinh,
                    DiemSaoTrungBinh = r != null ? r.DiemTB : 5.0,
                    SoDanhGia = r != null ? r.SoDanhGia : 0
                };
            }).ToList();

            var spBanChayList = spBanChayQuery.Select(p =>
            {
                ratingDict.TryGetValue(p.MaSP, out var r);
                return new SanPhamItemViewModel
                {
                    MaSP = p.MaSP,
                    TenSP = p.TenSP,
                    Gia = p.Gia,
                    GiaGoc = p.GiaGoc,
                    SoLuongTon = p.SoLuongTon,
                    TrangThai = p.TrangThai,
                    MaDM = p.MaDM,
                    TenDanhMuc = p.TenDanhMuc,
                    MaTH = p.MaTH,
                    TenThuongHieu = p.TenThuongHieu,
                    AnhChinh = p.AnhChinh,
                    DiemSaoTrungBinh = r != null ? r.DiemTB : 5.0,
                    SoDanhGia = r != null ? r.SoDanhGia : 0
                };
            }).ToList();

            return new TrangChuViewModel
            {
                Banners = banners,
                DanhMucNoiBat = danhMucs,
                KhuyenMaiDangDienRa = khuyenMais,
                SanPhamMoi = spMoiList,
                SanPhamBanChay = spBanChayList
            };
        }

        public async Task<List<BannerItemViewModel>> LayDanhSachBannerAsync()
        {
            return await _context.Banner
                .AsNoTracking()
                .Where(b => b.TrangThai)
                .OrderBy(b => b.ThuTu)
                .Select(b => new BannerItemViewModel
                {
                    MaBanner = b.MaBanner,
                    TieuDe = b.TieuDe,
                    DuongDanAnh = b.DuongDanAnh,
                    LienKet = b.LienKet,
                    ThuTu = b.ThuTu
                })
                .ToListAsync();
        }

        public async Task<List<DanhMucItemViewModel>> LayDanhSachDanhMucAsync()
        {
            return await _context.DanhMuc
                .AsNoTracking()
                .Where(d => d.TrangThai)
                .OrderBy(d => d.TenDM)
                .Select(d => new DanhMucItemViewModel
                {
                    MaDM = d.MaDM,
                    TenDM = d.TenDM,
                    MoTa = d.MoTa,
                    SoLuongSanPham = d.SanPham.Count(s => s.TrangThai)
                })
                .ToListAsync();
        }

        public async Task<List<ThuongHieuItemViewModel>> LayDanhSachThuongHieuAsync()
        {
            return await _context.ThuongHieu
                .AsNoTracking()
                .Where(t => t.TrangThai)
                .OrderBy(t => t.TenTH)
                .Select(t => new ThuongHieuItemViewModel
                {
                    MaTH = t.MaTH,
                    TenTH = t.TenTH,
                    Logo = t.Logo,
                    MoTa = t.MoTa,
                    SoLuongSanPham = t.SanPham.Count(s => s.TrangThai)
                })
                .ToListAsync();
        }

        /// <summary>
        /// Rút gọn che bớt tên khách hàng khi hiển thị đánh giá công khai, không để lộ email hay họ tên đầy đủ
        /// </summary>
        private static string CheTenNguoiDung(string? hoTen)
        {
            if (string.IsNullOrWhiteSpace(hoTen)) return "Khách hàng";
            var parts = hoTen.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0];
            return $"{parts[0]} {parts[^1][0]}.";
        }
    }
}
