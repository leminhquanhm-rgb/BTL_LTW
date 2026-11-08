using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanMyPham.Models;

public partial class WebsiteBanMyPhamContext : DbContext
{
    public WebsiteBanMyPhamContext(DbContextOptions<WebsiteBanMyPhamContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnhSanPham> AnhSanPham { get; set; }

    public virtual DbSet<Banner> Banner { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHang { get; set; }

    public virtual DbSet<DanhGia> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMuc { get; set; }

    public virtual DbSet<DatLaiMatKhau> DatLaiMatKhau { get; set; }

    public virtual DbSet<DiaChiNhanHang> DiaChiNhanHang { get; set; }

    public virtual DbSet<DonHang> DonHang { get; set; }

    public virtual DbSet<GioHang> GioHang { get; set; }

    public virtual DbSet<KhieuNai> KhieuNai { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMai { get; set; }

    public virtual DbSet<LichSuDonHang> LichSuDonHang { get; set; }

    public virtual DbSet<PhuongThucThanhToan> PhuongThucThanhToan { get; set; }

    public virtual DbSet<SanPham> SanPham { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoan { get; set; }

    public virtual DbSet<ThanhToan> ThanhToan { get; set; }

    public virtual DbSet<ThuongHieu> ThuongHieu { get; set; }

    public virtual DbSet<VW_DanhGia> VW_DanhGia { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnhSanPham>(entity =>
        {
            entity.HasKey(e => e.MaAnh).HasName("PK__AnhSanPh__356240DFA9A249A1");

            entity.Property(e => e.DuongDan).HasMaxLength(255);

            entity.HasOne(d => d.MaSPNavigation).WithMany(p => p.AnhSanPham)
                .HasForeignKey(d => d.MaSP)
                .HasConstraintName("FK__AnhSanPham__MaSP__5812160E");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.MaBanner).HasName("PK__Banner__508B4A495E525118");

            entity.Property(e => e.DuongDanAnh).HasMaxLength(255);
            entity.Property(e => e.LienKet).HasMaxLength(255);
            entity.Property(e => e.TieuDe).HasMaxLength(150);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaCTDH).HasName("PK__ChiTietD__1E4E40F0ECFFDEF4");

            entity.HasIndex(e => e.MaSP, "IX_CTDH_MaSP");

            entity.HasIndex(e => new { e.MaDH, e.MaSP }, "UQ_CTDH").IsUnique();

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TenSP).HasMaxLength(200);
            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("([DonGia]*[SoLuong])", true)
                .HasColumnType("decimal(29, 0)");

            entity.HasOne(d => d.MaDHNavigation).WithMany(p => p.ChiTietDonHang)
                .HasForeignKey(d => d.MaDH)
                .HasConstraintName("FK__ChiTietDon__MaDH__01142BA1");

            entity.HasOne(d => d.MaSPNavigation).WithMany(p => p.ChiTietDonHang)
                .HasForeignKey(d => d.MaSP)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDon__MaSP__02084FDA");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.MaDG).HasName("PK__DanhGia__27258660042BC61E");

            entity.HasIndex(e => e.TrangThai, "IX_DanhGia_TrangThai");

            entity.HasIndex(e => e.MaCTDH, "UQ__DanhGia__1E4E40F187A735B4").IsUnique();

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NoiDung).HasMaxLength(1000);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(10)
                .HasDefaultValue("ChoDuyet");

            entity.HasOne(d => d.MaCTDHNavigation).WithOne(p => p.DanhGia)
                .HasForeignKey<DanhGia>(d => d.MaCTDH)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaCTDH__19DFD96B");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDM).HasName("PK__DanhMuc__2725866EDA53C308");

            entity.HasIndex(e => e.TenDM, "UQ__DanhMuc__4CF965596FAB224C").IsUnique();

            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenDM).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<DatLaiMatKhau>(entity =>
        {
            entity.HasKey(e => e.MaDL).HasName("PK__DatLaiMa__2725866D77AAFFD8");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.OtpHash).HasMaxLength(255);

            entity.HasOne(d => d.MaTKNavigation).WithMany(p => p.DatLaiMatKhau)
                .HasForeignKey(d => d.MaTK)
                .HasConstraintName("FK__DatLaiMatK__MaTK__3E52440B");
        });

        modelBuilder.Entity<DiaChiNhanHang>(entity =>
        {
            entity.HasKey(e => e.MaDC);

            entity.ToTable("DiaChiNhanHang");

            entity.HasIndex(e => e.MaTK, "IX_DiaChiNhanHang_MaTK");

            entity.HasIndex(e => e.MaTK, "UX_DiaChiNhanHang_MacDinh")
                .IsUnique()
                .HasFilter("([LaMacDinh]=(1))");

            entity.Property(e => e.TenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.LaMacDinh).HasDefaultValue(false);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.MaTKNavigation).WithMany(p => p.DiaChiNhanHang)
                .HasForeignKey(d => d.MaTK)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDH).HasName("PK__DonHang__27258661A81F5D83");

            entity.HasIndex(e => new { e.MaTK, e.NgayDat }, "IX_DonHang_MaTK").IsDescending(false, true);

            entity.HasIndex(e => e.TrangThai, "IX_DonHang_TrangThai");

            entity.Property(e => e.CodeKM).HasMaxLength(30);
            entity.Property(e => e.DiaChiGiao).HasMaxLength(255);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.MaVanDon).HasMaxLength(50);
            entity.Property(e => e.NgayDat).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SdtNguoiNhan)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.TienGiam).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TongThanhToan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TongTienHang).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ChoXacNhan");

            entity.HasOne(d => d.MaKMNavigation).WithMany(p => p.DonHang)
                .HasForeignKey(d => d.MaKM)
                .HasConstraintName("FK__DonHang__MaKM__778AC167");

            entity.HasOne(d => d.MaTKNavigation).WithMany(p => p.DonHang)
                .HasForeignKey(d => d.MaTK)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaTK__74AE54BC");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGH).HasName("PK__GioHang__2725AE8517FB16EC");

            entity.HasIndex(e => new { e.MaTK, e.MaSP }, "UQ_GioHang").IsUnique();

            entity.Property(e => e.NgayThem).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.MaSPNavigation).WithMany(p => p.GioHang)
                .HasForeignKey(d => d.MaSP)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaSP__5DCAEF64");

            entity.HasOne(d => d.MaTKNavigation).WithMany(p => p.GioHang)
                .HasForeignKey(d => d.MaTK)
                .HasConstraintName("FK__GioHang__MaTK__5CD6CB2B");
        });

        modelBuilder.Entity<KhieuNai>(entity =>
        {
            entity.HasKey(e => e.MaKN).HasName("PK__KhieuNai__2725CF14EE7B192F");

            entity.Property(e => e.NgayGui).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NoiDung).HasMaxLength(1000);
            entity.Property(e => e.PhanHoi).HasMaxLength(1000);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(15)
                .HasDefaultValue("ChoXuLy");

            entity.HasOne(d => d.MaDHNavigation).WithMany(p => p.KhieuNai)
                .HasForeignKey(d => d.MaDH)
                .HasConstraintName("FK__KhieuNai__MaDH__2180FB33");

            entity.HasOne(d => d.MaSPNavigation).WithMany(p => p.KhieuNai)
                .HasForeignKey(d => d.MaSP)
                .HasConstraintName("FK__KhieuNai__MaSP__22751F6C");

            entity.HasOne(d => d.MaTKNavigation).WithMany(p => p.KhieuNaiMaTKNavigation)
                .HasForeignKey(d => d.MaTK)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhieuNai__MaTK__208CD6FA");

            entity.HasOne(d => d.MaTKXuLyNavigation).WithMany(p => p.KhieuNaiMaTKXuLyNavigation)
                .HasForeignKey(d => d.MaTKXuLy)
                .HasConstraintName("FK__KhieuNai__MaTKXu__2645B050");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKM).HasName("PK__KhuyenMa__2725CF157A4A9B77");

            entity.HasIndex(e => e.Code, "UQ__KhuyenMa__A25C5AA7873F6543").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(30);
            entity.Property(e => e.DonToiThieu).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.GiaTriGiam).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.GiamToiDa).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.LoaiGiam).HasMaxLength(10);
            entity.Property(e => e.TenKM).HasMaxLength(150);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<LichSuDonHang>(entity =>
        {
            entity.HasKey(e => e.MaLS).HasName("PK__LichSuDo__2725C7720FCBCF86");

            entity.Property(e => e.NgayDoi).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TrangThaiCu).HasMaxLength(20);
            entity.Property(e => e.TrangThaiMoi).HasMaxLength(20);

            entity.HasOne(d => d.MaDHNavigation).WithMany(p => p.LichSuDonHang)
                .HasForeignKey(d => d.MaDH)
                .HasConstraintName("FK__LichSuDonH__MaDH__14270015");

            entity.HasOne(d => d.MaTKThucHienNavigation).WithMany(p => p.LichSuDonHang)
                .HasForeignKey(d => d.MaTKThucHien)
                .HasConstraintName("FK__LichSuDon__MaTKT__151B244E");
        });

        modelBuilder.Entity<PhuongThucThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaPT).HasName("PK__PhuongTh__2725E7F613912271");

            entity.HasIndex(e => e.TenPT, "UQ__PhuongTh__4CF9C7FB11BD375F").IsUnique();

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenPT).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSP).HasName("PK__SanPham__2725081CCA2EA327");

            entity.HasIndex(e => e.MaDM, "IX_SanPham_MaDM");

            entity.HasIndex(e => e.MaTH, "IX_SanPham_MaTH");

            entity.HasIndex(e => e.TenSP, "IX_SanPham_TenSP");

            entity.Property(e => e.Gia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.GiaGoc).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NguongCanhBao).HasDefaultValue(5);
            entity.Property(e => e.TenSP).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaDMNavigation).WithMany(p => p.SanPham)
                .HasForeignKey(d => d.MaDM)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaDM__52593CB8");

            entity.HasOne(d => d.MaTHNavigation).WithMany(p => p.SanPham)
                .HasForeignKey(d => d.MaTH)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaTH__534D60F1");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTK).HasName("PK__TaiKhoan__272500706D7E529E");

            entity.HasIndex(e => e.Email, "UQ__TaiKhoan__A9D1053452BA99E1").IsUnique();

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhauHash).HasMaxLength(255);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .HasDefaultValue("KhachHang");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaTT).HasName("PK__ThanhToa__272500798E9572AF");

            entity.HasIndex(e => e.MaDH, "UQ__ThanhToa__27258660119BC542").IsUnique();

            entity.Property(e => e.MaGiaoDich).HasMaxLength(100);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ChuaThanhToan");
            entity.Property(e => e.TrangThaiHoanTien)
                .HasMaxLength(20)
                .HasDefaultValue("KhongCan");

            entity.HasOne(d => d.MaDHNavigation).WithOne(p => p.ThanhToan)
                .HasForeignKey<ThanhToan>(d => d.MaDH)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__MaDH__07C12930");

            entity.HasOne(d => d.MaPTNavigation).WithMany(p => p.ThanhToan)
                .HasForeignKey(d => d.MaPT)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__MaPT__08B54D69");
        });

        modelBuilder.Entity<ThuongHieu>(entity =>
        {
            entity.HasKey(e => e.MaTH).HasName("PK__ThuongHi__27250075D8EE4300");

            entity.HasIndex(e => e.TenTH, "UQ__ThuongHi__4CF9E74A02E177ED").IsUnique();

            entity.Property(e => e.Logo).HasMaxLength(255);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenTH).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<VW_DanhGia>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_DanhGia");

            entity.Property(e => e.NoiDung).HasMaxLength(1000);
            entity.Property(e => e.TrangThai).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
