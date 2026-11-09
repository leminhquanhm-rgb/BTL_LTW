USE master;
GO
IF DB_ID(N'WebsiteBanMyPham') IS NOT NULL
BEGIN
    ALTER DATABASE WebsiteBanMyPham SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE WebsiteBanMyPham;
END
GO
CREATE DATABASE WebsiteBanMyPham;
GO
USE WebsiteBanMyPham;
GO

/* ===== 1. TÀI KHOẢN ===== */
CREATE TABLE TaiKhoan (
    MaTK        INT IDENTITY(1,1) PRIMARY KEY,
    Email       NVARCHAR(150) NOT NULL UNIQUE,
    MatKhauHash NVARCHAR(255) NOT NULL,
    HoTen       NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15)   NULL,
    DiaChi      NVARCHAR(255) NULL,
    VaiTro      NVARCHAR(20)  NOT NULL DEFAULT N'KhachHang'
                CHECK (VaiTro IN (N'KhachHang', N'QuanTriVien')),
    TrangThai   BIT NOT NULL DEFAULT 1,              -- 1: hoạt động, 0: bị khóa
    NgayTao     DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE DatLaiMatKhau (
    MaDL      INT IDENTITY(1,1) PRIMARY KEY,
    MaTK      INT NOT NULL REFERENCES TaiKhoan(MaTK) ON DELETE CASCADE,
    OtpHash   NVARCHAR(255) NOT NULL,
    HetHan    DATETIME2 NOT NULL,
    DaDung    BIT NOT NULL DEFAULT 0,
    SoLanSai  INT NOT NULL DEFAULT 0,
    NgayTao   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT CK_DLRMK_SoLanSai CHECK (SoLanSai >= 0)
);

/* ===== 2. DANH MỤC, THƯƠNG HIỆU, SẢN PHẨM ===== */
CREATE TABLE DanhMuc (
    MaDM      INT IDENTITY(1,1) PRIMARY KEY,
    TenDM     NVARCHAR(100) NOT NULL UNIQUE,
    MoTa      NVARCHAR(500) NULL,
    TrangThai BIT NOT NULL DEFAULT 1
);

CREATE TABLE ThuongHieu (
    MaTH      INT IDENTITY(1,1) PRIMARY KEY,
    TenTH     NVARCHAR(100) NOT NULL UNIQUE,
    Logo      NVARCHAR(255) NULL,
    MoTa      NVARCHAR(500) NULL,
    TrangThai BIT NOT NULL DEFAULT 1
);

CREATE TABLE SanPham (
    MaSP          INT IDENTITY(1,1) PRIMARY KEY,
    TenSP         NVARCHAR(200) NOT NULL,
    MoTa          NVARCHAR(MAX) NULL,
    Gia           DECIMAL(18,0) NOT NULL CHECK (Gia >= 0),
    GiaGoc        DECIMAL(18,0) NULL,                -- giá gốc hiển thị gạch ngang
    SoLuongTon    INT NOT NULL DEFAULT 0 CHECK (SoLuongTon >= 0),
    NguongCanhBao INT NOT NULL DEFAULT 5 CHECK (NguongCanhBao >= 0),
    TrangThai     BIT NOT NULL DEFAULT 1,            -- 1: đang kinh doanh, 0: ngừng
    MaDM          INT NOT NULL REFERENCES DanhMuc(MaDM),
    MaTH          INT NOT NULL REFERENCES ThuongHieu(MaTH),
    NgayTao       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT CK_SanPham_GiaGoc CHECK (GiaGoc IS NULL OR GiaGoc >= Gia)
);
CREATE INDEX IX_SanPham_MaDM  ON SanPham(MaDM);
CREATE INDEX IX_SanPham_MaTH  ON SanPham(MaTH);
CREATE INDEX IX_SanPham_TenSP ON SanPham(TenSP);

CREATE TABLE AnhSanPham (
    MaAnh      INT IDENTITY(1,1) PRIMARY KEY,
    MaSP       INT NOT NULL REFERENCES SanPham(MaSP) ON DELETE CASCADE,
    DuongDan   NVARCHAR(255) NOT NULL,
    LaAnhChinh BIT NOT NULL DEFAULT 0
);

/* ===== 3. GIỎ HÀNG ===== */
CREATE TABLE GioHang (
    MaGH     INT IDENTITY(1,1) PRIMARY KEY,
    MaTK     INT NOT NULL REFERENCES TaiKhoan(MaTK) ON DELETE CASCADE,
    MaSP     INT NOT NULL REFERENCES SanPham(MaSP),
    SoLuong  INT NOT NULL CHECK (SoLuong > 0),
    NgayThem DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_GioHang UNIQUE (MaTK, MaSP)
);

/* ===== 3b. SỔ ĐỊA CHỈ NHẬN HÀNG (mỗi khách nhiều địa chỉ, tối đa 1 địa chỉ mặc định) ===== */
CREATE TABLE DiaChiNhanHang (
    MaDC         INT IDENTITY(1,1) PRIMARY KEY,
    MaTK         INT NOT NULL REFERENCES TaiKhoan(MaTK) ON DELETE CASCADE,
    TenNguoiNhan NVARCHAR(100) NOT NULL,
    SoDienThoai  VARCHAR(15)   NOT NULL,
    DiaChi       NVARCHAR(255) NOT NULL,
    LaMacDinh    BIT NOT NULL DEFAULT 0,
    NgayTao      DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
CREATE INDEX IX_DiaChiNhanHang_MaTK ON DiaChiNhanHang(MaTK);
-- Mỗi tài khoản chỉ có tối đa 1 địa chỉ mặc định
CREATE UNIQUE INDEX UX_DiaChiNhanHang_MacDinh ON DiaChiNhanHang(MaTK) WHERE LaMacDinh = 1;

/* ===== 4. KHUYẾN MÃI, PHƯƠNG THỨC THANH TOÁN ===== */
CREATE TABLE KhuyenMai (
    MaKM         INT IDENTITY(1,1) PRIMARY KEY,
    Code         NVARCHAR(30)  NOT NULL UNIQUE,
    TenKM        NVARCHAR(150) NOT NULL,
    LoaiGiam     NVARCHAR(10)  NOT NULL CHECK (LoaiGiam IN (N'PhanTram', N'SoTien')),
    GiaTriGiam   DECIMAL(18,0) NOT NULL CHECK (GiaTriGiam > 0),
    GiamToiDa    DECIMAL(18,0) NULL CHECK (GiamToiDa IS NULL OR GiamToiDa > 0),
    DonToiThieu  DECIMAL(18,0) NOT NULL DEFAULT 0 CHECK (DonToiThieu >= 0),
    NgayBatDau   DATETIME2 NOT NULL,
    NgayKetThuc  DATETIME2 NOT NULL,
    SoLuotToiDa  INT NULL CHECK (SoLuotToiDa IS NULL OR SoLuotToiDa > 0),   -- NULL: không giới hạn
    SoLuotDaDung INT NOT NULL DEFAULT 0 CHECK (SoLuotDaDung >= 0),
    TrangThai    BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_KM_Ngay     CHECK (NgayKetThuc >= NgayBatDau),
    CONSTRAINT CK_KM_PhanTram CHECK (LoaiGiam = N'SoTien' OR GiaTriGiam <= 100),
    CONSTRAINT CK_KM_Luot     CHECK (SoLuotToiDa IS NULL OR SoLuotDaDung <= SoLuotToiDa)
);

CREATE TABLE PhuongThucThanhToan (
    MaPT      INT IDENTITY(1,1) PRIMARY KEY,
    TenPT     NVARCHAR(100) NOT NULL UNIQUE,
    MoTa      NVARCHAR(255) NULL,
    TrangThai BIT NOT NULL DEFAULT 1
);

/* ===== 5. ĐƠN HÀNG ===== */
CREATE TABLE DonHang (
    MaDH              INT IDENTITY(1,1) PRIMARY KEY,
    MaTK              INT NOT NULL REFERENCES TaiKhoan(MaTK),
    NgayDat           DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    TenNguoiNhan      NVARCHAR(100) NOT NULL,
    SdtNguoiNhan      VARCHAR(15)   NOT NULL,
    DiaChiGiao        NVARCHAR(255) NOT NULL,
    GhiChu            NVARCHAR(500) NULL,
    TongTienHang      DECIMAL(18,0) NOT NULL CHECK (TongTienHang >= 0),
    MaKM              INT NULL REFERENCES KhuyenMai(MaKM),
    CodeKM            NVARCHAR(30) NULL,             -- lưu lại mã tại thời điểm đặt
    TienGiam          DECIMAL(18,0) NOT NULL DEFAULT 0 CHECK (TienGiam >= 0),
    TongThanhToan     DECIMAL(18,0) NOT NULL,
    TrangThai         NVARCHAR(20) NOT NULL DEFAULT N'ChoXacNhan'
                      CHECK (TrangThai IN (N'ChoXacNhan', N'DaXacNhan', N'DangGiao', N'HoanThanh', N'DaHuy')),
    NgayVanDon        DATETIME2 NULL,
    NgayGiaoThanhCong DATETIME2 NULL,
    MaVanDon          NVARCHAR(50) NULL,
    CONSTRAINT CK_DH_Tien CHECK (TienGiam <= TongTienHang AND TongThanhToan = TongTienHang - TienGiam),
    CONSTRAINT CK_DH_TrangThai_Ngay CHECK (
           (TrangThai IN (N'ChoXacNhan', N'DaXacNhan', N'DaHuy')
                AND NgayVanDon IS NULL AND NgayGiaoThanhCong IS NULL)
        OR (TrangThai = N'DangGiao'
                AND NgayVanDon IS NOT NULL AND NgayGiaoThanhCong IS NULL)
        OR (TrangThai = N'HoanThanh'
                AND NgayVanDon IS NOT NULL AND NgayGiaoThanhCong IS NOT NULL
                AND NgayGiaoThanhCong >= NgayVanDon)
    )
);
CREATE INDEX IX_DonHang_MaTK      ON DonHang(MaTK, NgayDat DESC);
CREATE INDEX IX_DonHang_TrangThai ON DonHang(TrangThai);

CREATE TABLE ChiTietDonHang (
    MaCTDH    INT IDENTITY(1,1) PRIMARY KEY,
    MaDH      INT NOT NULL REFERENCES DonHang(MaDH) ON DELETE CASCADE,
    MaSP      INT NOT NULL REFERENCES SanPham(MaSP),
    TenSP     NVARCHAR(200) NOT NULL,                         -- tên lúc mua
    DonGia    DECIMAL(18,0) NOT NULL CHECK (DonGia >= 0),     -- giá lúc mua
    SoLuong   INT NOT NULL CHECK (SoLuong > 0),
    ThanhTien AS (DonGia * SoLuong) PERSISTED,
    CONSTRAINT UQ_CTDH UNIQUE (MaDH, MaSP)
);
CREATE INDEX IX_CTDH_MaSP ON ChiTietDonHang(MaSP);

CREATE TABLE ThanhToan (
    MaTT              INT IDENTITY(1,1) PRIMARY KEY,
    MaDH              INT NOT NULL UNIQUE REFERENCES DonHang(MaDH),
    MaPT              INT NOT NULL REFERENCES PhuongThucThanhToan(MaPT),
    SoTien            DECIMAL(18,0) NOT NULL CHECK (SoTien >= 0),
    TrangThai         NVARCHAR(20) NOT NULL DEFAULT N'ChuaThanhToan'
                      CHECK (TrangThai IN (N'ChuaThanhToan', N'DaThanhToan', N'ThatBai')),
    TrangThaiHoanTien NVARCHAR(20) NOT NULL DEFAULT N'KhongCan'
                      CHECK (TrangThaiHoanTien IN (N'KhongCan', N'ChoHoan', N'DaHoan')),
    MaGiaoDich        NVARCHAR(100) NULL,
    NgayThanhToan     DATETIME2 NULL,
    NgayHoanTien      DATETIME2 NULL,
    NgayTao           DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT CK_TT_NgayThanhToan CHECK (
           (TrangThai = N'DaThanhToan' AND NgayThanhToan IS NOT NULL)
        OR (TrangThai <> N'DaThanhToan' AND NgayThanhToan IS NULL)),
    CONSTRAINT CK_TT_HoanTien CHECK (TrangThaiHoanTien = N'KhongCan' OR TrangThai = N'DaThanhToan'),
    CONSTRAINT CK_TT_NgayHoanTien CHECK (
           (TrangThaiHoanTien = N'DaHoan' AND NgayHoanTien IS NOT NULL)
        OR (TrangThaiHoanTien <> N'DaHoan' AND NgayHoanTien IS NULL))
);

CREATE TABLE LichSuDonHang (
    MaLS         INT IDENTITY(1,1) PRIMARY KEY,
    MaDH         INT NOT NULL REFERENCES DonHang(MaDH) ON DELETE CASCADE,
    TrangThaiCu  NVARCHAR(20) NULL,
    TrangThaiMoi NVARCHAR(20) NOT NULL,
    MaTKThucHien INT NULL REFERENCES TaiKhoan(MaTK),
    NgayDoi      DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

/* ===== 6. ĐÁNH GIÁ (chỉ lưu MaCTDH; khách và sản phẩm lấy qua ChiTietDonHang -> DonHang) ===== */
CREATE TABLE DanhGia (
    MaDG      INT IDENTITY(1,1) PRIMARY KEY,
    MaCTDH    INT NOT NULL UNIQUE REFERENCES ChiTietDonHang(MaCTDH),   -- 1 sản phẩm / 1 đơn: 1 đánh giá
    SoSao     TINYINT NOT NULL CHECK (SoSao BETWEEN 1 AND 5),
    NoiDung   NVARCHAR(1000) NULL,
    TrangThai NVARCHAR(10) NOT NULL DEFAULT N'ChoDuyet'
              CHECK (TrangThai IN (N'ChoDuyet', N'DaDuyet', N'TuChoi')),
    NgayTao   DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
CREATE INDEX IX_DanhGia_TrangThai ON DanhGia(TrangThai);

/* ===== 7. KHIẾU NẠI, BANNER ===== */
CREATE TABLE KhieuNai (
    MaKN         INT IDENTITY(1,1) PRIMARY KEY,
    MaTK         INT NOT NULL REFERENCES TaiKhoan(MaTK),        -- khách gửi
    MaDH         INT NULL REFERENCES DonHang(MaDH),
    MaSP         INT NULL REFERENCES SanPham(MaSP),
    NoiDung      NVARCHAR(1000) NOT NULL,
    NgayGui      DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    TrangThai    NVARCHAR(15) NOT NULL DEFAULT N'ChoXuLy'
                 CHECK (TrangThai IN (N'ChoXuLy', N'DangXuLy', N'DaGiaiQuyet', N'TuChoi')),
    PhanHoi      NVARCHAR(1000) NULL,
    NgayPhanHoi  DATETIME2 NULL,
    MaTKXuLy     INT NULL REFERENCES TaiKhoan(MaTK)             -- Admin xử lý
);

CREATE TABLE Banner (
    MaBanner    INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe      NVARCHAR(150) NULL,
    DuongDanAnh NVARCHAR(255) NOT NULL,
    LienKet     NVARCHAR(255) NULL,
    ThuTu       INT NOT NULL DEFAULT 0,
    TrangThai   BIT NOT NULL DEFAULT 1
);
GO

/* ===== VIEW: đánh giá kèm khách hàng và sản phẩm ===== */
CREATE VIEW VW_DanhGia AS
SELECT dg.MaDG, dg.MaCTDH, dh.MaTK, ct.MaSP,
       dg.SoSao, dg.NoiDung, dg.TrangThai, dg.NgayTao
FROM DanhGia dg
JOIN ChiTietDonHang ct ON ct.MaCTDH = dg.MaCTDH
JOIN DonHang dh        ON dh.MaDH  = ct.MaDH;
GO

/* ===== DỮ LIỆU BẮT BUỘC ===== */
INSERT INTO PhuongThucThanhToan (TenPT, MoTa) VALUES
(N'COD',    N'Thanh toán khi nhận hàng'),
(N'Online', N'Thanh toán trực tuyến (mô phỏng/Sandbox)');
GO