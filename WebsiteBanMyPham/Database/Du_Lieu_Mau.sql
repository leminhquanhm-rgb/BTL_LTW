USE WebsiteBanMyPham;
GO

/* ===== 1. TÀI KHOẢN (mật khẩu: 123456) ===== */
DECLARE @h NVARCHAR(255) = N'$2a$11$EazdNJyrwnZPhjk8UDzM9uNhhz1aHjFEypOcjteJRIfdtGQJQGWZS';
INSERT INTO TaiKhoan (Email, MatKhauHash, HoTen, SoDienThoai, DiaChi, VaiTro, TrangThai) VALUES
(N'admin@mypham.vn', @h, N'Quản trị viên',  '0900000000', N'Hà Nội',                              N'QuanTriVien', 1),
(N'an@gmail.com',    @h, N'Nguyễn Văn An',  '0901000001', N'12 Cầu Giấy, Hà Nội',                 N'KhachHang',   1),
(N'binh@gmail.com',  @h, N'Trần Thị Bình',  '0901000002', N'45 Nguyễn Trãi, Thanh Xuân, Hà Nội',  N'KhachHang',   1),
(N'chi@gmail.com',   @h, N'Lê Minh Chi',    '0901000003', N'8 Lê Lợi, Hải Phòng',                 N'KhachHang',   1),
(N'khoa@gmail.com',  @h, N'Phạm Đăng Khoa', '0901000004', N'20 Trần Phú, Đà Nẵng',                N'KhachHang',   0);
GO

/* ===== 2. DANH MỤC, THƯƠNG HIỆU ===== */
INSERT INTO DanhMuc (TenDM, MoTa) VALUES
(N'Chăm sóc da mặt',  N'Sữa rửa mặt, toner, serum, kem dưỡng, chống nắng'),
(N'Trang điểm',       N'Son, mascara, kem nền, phấn mắt'),
(N'Chăm sóc tóc',     N'Dầu gội, dầu xả, dưỡng tóc'),
(N'Chăm sóc cơ thể',  N'Sữa tắm, tẩy tế bào chết, kem dưỡng thể'),
(N'Nước hoa',         N'Nước hoa và xịt thơm');

INSERT INTO ThuongHieu (TenTH, MoTa) VALUES
(N'La Roche-Posay', N'Dược mỹ phẩm của Pháp'),
(N'Innisfree',      N'Mỹ phẩm thiên nhiên Hàn Quốc'),
(N'Cetaphil',       N'Chăm sóc da nhạy cảm'),
(N'Maybelline',     N'Mỹ phẩm trang điểm'),
(N'L''Oréal Paris', N'Thương hiệu mỹ phẩm Pháp'),
(N'3CE',            N'Thương hiệu trang điểm Hàn Quốc'),
(N'Dove',           N'Chăm sóc cá nhân'),
(N'Cocoon',         N'Mỹ phẩm thuần chay Việt Nam');
GO

/* ===== 3. SẢN PHẨM (SP11 sắp hết hàng, SP20 ngừng kinh doanh) ===== */
INSERT INTO SanPham (TenSP, MoTa, Gia, GiaGoc, SoLuongTon, NguongCanhBao, TrangThai, MaDM, MaTH) VALUES
(N'Sữa rửa mặt Cetaphil Gentle Skin Cleanser 125ml',        N'Làm sạch dịu nhẹ, không xà phòng',    135000,  155000, 50,  5, 1, 1, 3),
(N'Kem chống nắng La Roche-Posay Anthelios SPF50+ 50ml',    N'Chống nắng phổ rộng cho da nhạy cảm', 420000,  483000, 40,  5, 1, 1, 1),
(N'Serum La Roche-Posay Hyaluronic Acid B5 30ml',           N'Cấp ẩm, phục hồi và làm đầy da',      890000, 1024000, 25,  5, 1, 1, 1),
(N'Nước tẩy trang L''Oréal Micellar Water 400ml',           N'Làm sạch lớp trang điểm nhẹ nhàng',   165000,  190000, 60,  5, 1, 1, 5),
(N'Mặt nạ đất sét núi lửa Innisfree 100ml',                 N'Làm sạch sâu lỗ chân lông',           289000,  332000, 35,  5, 1, 1, 2),
(N'Toner Innisfree Green Tea Balancing 200ml',              N'Cân bằng và cấp ẩm từ trà xanh',      320000,  368000, 30,  5, 1, 1, 2),
(N'Kem dưỡng ẩm Cetaphil Moisturizing Cream 250g',          N'Dưỡng ẩm sâu cho da khô',             245000,    NULL, 45,  5, 1, 1, 3),
(N'Son lì Maybelline Super Stay Matte Ink',                 N'Lì lâu, lên màu chuẩn',               175000,  201000, 70,  5, 1, 2, 4),
(N'Mascara Maybelline Lash Sensational',                    N'Làm dài và dày mi',                   199000,  229000, 55,  5, 1, 2, 4),
(N'Kem nền L''Oréal True Match 30ml',                       N'Che phủ tự nhiên, nhiều tông màu',    345000,    NULL, 20,  5, 1, 2, 5),
(N'Son kem 3CE Velvet Lip Tint',                            N'Chất son nhung mịn',                  280000,    NULL,  4,  5, 1, 2, 6),
(N'Bảng phấn mắt 3CE Multi Eye Color Palette',              N'Bảng màu mắt đa dạng',                650000,  748000, 15,  5, 1, 2, 6),
(N'Dầu gội Cocoon Bưởi 310ml',                              N'Giảm gãy rụng, làm sạch dịu nhẹ',     125000,    NULL, 80, 10, 1, 3, 8),
(N'Dầu xả L''Oréal Elseve 170ml',                           N'Mềm mượt, giảm xơ rối',                98000,    NULL, 65, 10, 1, 3, 5),
(N'Dầu gội Dove Phục hồi hư tổn 640g',                      N'Phục hồi tóc hư tổn',                 169000,    NULL, 50, 10, 1, 3, 7),
(N'Sữa tắm Dove Dưỡng ẩm 530g',                             N'Dưỡng ẩm cho da mềm mịn',             145000,    NULL, 90, 10, 1, 4, 7),
(N'Tẩy tế bào chết cơ thể Cocoon Cà phê Đắk Lắk 200ml',     N'Làm sạch da chết cơ thể',              95000,    NULL, 75, 10, 1, 4, 8),
(N'Kem dưỡng thể Cetaphil Moisturizing Lotion 473ml',       N'Dưỡng ẩm toàn thân',                  380000,  437000, 30,  5, 1, 4, 3),
(N'Nước hoa 3CE Eau de Parfum 50ml',                        N'Hương hoa cỏ nhẹ nhàng',              520000,    NULL, 12,  3, 1, 5, 6),
(N'Nước hoa 3CE Mini Perfume 10ml',                         N'Phiên bản mini (đã ngừng kinh doanh)',220000,    NULL,  0,  3, 0, 5, 6);

INSERT INTO AnhSanPham (MaSP, DuongDan, LaAnhChinh) VALUES
(1,  N'/uploads/products/sp01.jpg', 1), (2,  N'/uploads/products/sp02.jpg', 1),
(3,  N'/uploads/products/sp03.jpg', 1), (4,  N'/uploads/products/sp04.jpg', 1),
(5,  N'/uploads/products/sp05.jpg', 1), (6,  N'/uploads/products/sp06.jpg', 1),
(7,  N'/uploads/products/sp07.jpg', 1), (8,  N'/uploads/products/sp08.jpg', 1),
(9,  N'/uploads/products/sp09.jpg', 1), (10, N'/uploads/products/sp10.jpg', 1),
(11, N'/uploads/products/sp11.jpg', 1), (12, N'/uploads/products/sp12.jpg', 1),
(13, N'/uploads/products/sp13.jpg', 1), (14, N'/uploads/products/sp14.jpg', 1),
(15, N'/uploads/products/sp15.jpg', 1), (16, N'/uploads/products/sp16.jpg', 1),
(17, N'/uploads/products/sp17.jpg', 1), (18, N'/uploads/products/sp18.jpg', 1),
(19, N'/uploads/products/sp19.jpg', 1), (20, N'/uploads/products/sp20.jpg', 1);
GO

/* ===== 4. KHUYẾN MÃI (HETHAN đã hết hạn, MOI15 đã hết lượt) ===== */
INSERT INTO KhuyenMai (Code, TenKM, LoaiGiam, GiaTriGiam, GiamToiDa, DonToiThieu, NgayBatDau, NgayKetThuc, SoLuotToiDa, SoLuotDaDung, TrangThai) VALUES
(N'SALE10',  N'Giảm 10% tối đa 50.000đ',       N'PhanTram', 10,    50000,  200000, '2026-01-01', '2026-12-31 23:59:59', 100,  1, 1),
(N'GIAM50K', N'Giảm 50.000đ cho đơn từ 500K',  N'SoTien',   50000, NULL,   500000, '2026-01-01', '2026-12-31 23:59:59', 50,   1, 1),
(N'HETHAN',  N'Giảm 20% (đã hết hạn)',         N'PhanTram', 20,    100000,      0, '2026-01-01', '2026-06-30 23:59:59', NULL, 30, 1),
(N'MOI15',   N'Giảm 15% khách mới (hết lượt)', N'PhanTram', 15,    80000,  300000, '2026-10-01', '2026-12-31 23:59:59', 10,  10, 1);

/* ===== 5. GIỎ HÀNG ===== */
INSERT INTO GioHang (MaTK, MaSP, SoLuong) VALUES
(2, 10, 1),
(3, 5, 1), (3, 7, 2);
GO

/* ===== 5b. SỔ ĐỊA CHỈ (an có 2 địa chỉ, bình có 1; chi và khoa chưa có) ===== */
INSERT INTO DiaChiNhanHang (MaTK, TenNguoiNhan, SoDienThoai, DiaChi, LaMacDinh) VALUES
(2, N'Nguyễn Văn An',  '0901000001', N'12 Cầu Giấy, Hà Nội',                      1),
(2, N'Nguyễn Văn An',  '0901000011', N'Tầng 5, 99 Trần Duy Hưng, Cầu Giấy, Hà Nội', 0),
(3, N'Trần Thị Bình',  '0901000002', N'45 Nguyễn Trãi, Thanh Xuân, Hà Nội',       1);
GO

/* ===== 6. ĐƠN HÀNG (phương thức thanh toán 1: COD, 2: Online đã có ở file 1) ===== */
INSERT INTO DonHang (MaTK, NgayDat, TenNguoiNhan, SdtNguoiNhan, DiaChiGiao, GhiChu, TongTienHang, MaKM, CodeKM, TienGiam, TongThanhToan, TrangThai, NgayVanDon, NgayGiaoThanhCong, MaVanDon) VALUES
-- DH1: Hoàn thành, dùng SALE10 (10% của 465.000 = 46.500)
(2, '2026-10-05 09:30', N'Nguyễn Văn An', '0901000001', N'12 Cầu Giấy, Hà Nội', NULL, 465000, 1, N'SALE10', 46500, 418500, N'HoanThanh', '2026-10-06 08:00', '2026-10-08 15:20', N'GHN0001'),
-- DH2: Hoàn thành, dùng GIAM50K
(2, '2026-10-12 14:10', N'Nguyễn Văn An', '0901000001', N'12 Cầu Giấy, Hà Nội', N'Giao giờ hành chính', 595000, 2, N'GIAM50K', 50000, 545000, N'HoanThanh', '2026-10-13 08:30', '2026-10-15 10:45', N'GHN0002'),
-- DH3: Đang giao
(3, '2026-10-20 19:45', N'Trần Thị Bình', '0901000002', N'45 Nguyễn Trãi, Thanh Xuân, Hà Nội', NULL, 1089000, NULL, NULL, 0, 1089000, N'DangGiao', '2026-10-22 09:00', NULL, N'GHN0003'),
-- DH4: Đã xác nhận
(3, '2026-10-28 20:15', N'Trần Thị Bình', '0901000002', N'45 Nguyễn Trãi, Thanh Xuân, Hà Nội', NULL, 395000, NULL, NULL, 0, 395000, N'DaXacNhan', NULL, NULL, NULL),
-- DH5: Chờ xác nhận
(4, '2026-10-30 11:20', N'Lê Minh Chi', '0901000003', N'8 Lê Lợi, Hải Phòng', N'Gọi trước khi giao', 930000, NULL, NULL, 0, 930000, N'ChoXacNhan', NULL, NULL, NULL),
-- DH6: Đã hủy (đã thanh toán online, đã hoàn tiền)
(4, '2026-10-18 16:00', N'Lê Minh Chi', '0901000003', N'8 Lê Lợi, Hải Phòng', NULL, 320000, NULL, NULL, 0, 320000, N'DaHuy', NULL, NULL, NULL);

INSERT INTO ChiTietDonHang (MaDH, MaSP, TenSP, DonGia, SoLuong) VALUES
(1, 1,  N'Sữa rửa mặt Cetaphil Gentle Skin Cleanser 125ml',     135000, 1),   -- CT1
(1, 4,  N'Nước tẩy trang L''Oréal Micellar Water 400ml',        165000, 2),   -- CT2
(2, 2,  N'Kem chống nắng La Roche-Posay Anthelios SPF50+ 50ml', 420000, 1),   -- CT3
(2, 8,  N'Son lì Maybelline Super Stay Matte Ink',              175000, 1),   -- CT4
(3, 3,  N'Serum La Roche-Posay Hyaluronic Acid B5 30ml',        890000, 1),   -- CT5
(3, 9,  N'Mascara Maybelline Lash Sensational',                 199000, 1),   -- CT6
(4, 13, N'Dầu gội Cocoon Bưởi 310ml',                           125000, 2),   -- CT7
(4, 16, N'Sữa tắm Dove Dưỡng ẩm 530g',                          145000, 1),   -- CT8
(5, 11, N'Son kem 3CE Velvet Lip Tint',                         280000, 1),   -- CT9
(5, 12, N'Bảng phấn mắt 3CE Multi Eye Color Palette',           650000, 1),   -- CT10
(6, 6,  N'Toner Innisfree Green Tea Balancing 200ml',           320000, 1);   -- CT11
GO

/* ===== 7. THANH TOÁN ===== */
INSERT INTO ThanhToan (MaDH, MaPT, SoTien, TrangThai, TrangThaiHoanTien, MaGiaoDich, NgayThanhToan, NgayHoanTien) VALUES
(1, 1, 418500,  N'DaThanhToan',   N'KhongCan', NULL,        '2026-10-08 15:20', NULL),
(2, 2, 545000,  N'DaThanhToan',   N'KhongCan', N'SBX-0002', '2026-10-12 14:12', NULL),
(3, 1, 1089000, N'ChuaThanhToan', N'KhongCan', NULL,        NULL, NULL),
(4, 1, 395000,  N'ChuaThanhToan', N'KhongCan', NULL,        NULL, NULL),
(5, 1, 930000,  N'ChuaThanhToan', N'KhongCan', NULL,        NULL, NULL),
(6, 2, 320000,  N'DaThanhToan',   N'DaHoan',   N'SBX-0006', '2026-10-18 16:02', '2026-10-20 10:00');

/* ===== 8. LỊCH SỬ TRẠNG THÁI ĐƠN ===== */
INSERT INTO LichSuDonHang (MaDH, TrangThaiCu, TrangThaiMoi, MaTKThucHien, NgayDoi) VALUES
(1, NULL,          N'ChoXacNhan', 2, '2026-10-05 09:30'),
(1, N'ChoXacNhan', N'DaXacNhan',  1, '2026-10-05 13:00'),
(1, N'DaXacNhan',  N'DangGiao',   1, '2026-10-06 08:00'),
(1, N'DangGiao',   N'HoanThanh',  1, '2026-10-08 15:20'),
(2, NULL,          N'ChoXacNhan', 2, '2026-10-12 14:10'),
(2, N'ChoXacNhan', N'DaXacNhan',  1, '2026-10-12 16:00'),
(2, N'DaXacNhan',  N'DangGiao',   1, '2026-10-13 08:30'),
(2, N'DangGiao',   N'HoanThanh',  1, '2026-10-15 10:45'),
(3, NULL,          N'ChoXacNhan', 3, '2026-10-20 19:45'),
(3, N'ChoXacNhan', N'DaXacNhan',  1, '2026-10-21 08:30'),
(3, N'DaXacNhan',  N'DangGiao',   1, '2026-10-22 09:00'),
(4, NULL,          N'ChoXacNhan', 3, '2026-10-28 20:15'),
(4, N'ChoXacNhan', N'DaXacNhan',  1, '2026-10-29 08:45'),
(5, NULL,          N'ChoXacNhan', 4, '2026-10-30 11:20'),
(6, NULL,          N'ChoXacNhan', 4, '2026-10-18 16:00'),
(6, N'ChoXacNhan', N'DaHuy',      4, '2026-10-19 09:00');
GO

/* ===== 9. ĐÁNH GIÁ (chỉ MaCTDH; khách và sản phẩm lấy qua VW_DanhGia) ===== */
INSERT INTO DanhGia (MaCTDH, SoSao, NoiDung, TrangThai, NgayTao) VALUES
(1, 5, N'Rửa sạch mà không bị khô da, rất ưng.', N'DaDuyet',  '2026-10-09 10:00'),
(2, 4, N'Tẩy trang sạch, mùi dễ chịu.',          N'DaDuyet',  '2026-10-09 10:05'),
(3, 5, N'Chống nắng tốt, không bết dính.',       N'ChoDuyet', '2026-10-16 09:30'),
(4, 3, N'Màu hơi khác ảnh một chút.',            N'TuChoi',   '2026-10-16 09:35');

/* ===== 10. KHIẾU NẠI ===== */
INSERT INTO KhieuNai (MaTK, MaDH, MaSP, NoiDung, NgayGui, TrangThai, PhanHoi, NgayPhanHoi, MaTKXuLy) VALUES
(3, 3,    NULL, N'Đơn hàng giao chậm hơn dự kiến.',            '2026-10-25 08:00', N'ChoXuLy',     NULL, NULL, NULL),
(2, 2,    8,    N'Son có màu khác so với hình trên website.',  '2026-10-16 10:00', N'DaGiaiQuyet', N'Cửa hàng xin lỗi và đã gửi đổi sản phẩm cho bạn.', '2026-10-18 09:00', 1),
(4, NULL, 11,   N'Cho mình hỏi thành phần của son kem này.',   '2026-10-31 14:30', N'DangXuLy',    NULL, NULL, 1);

/* ===== 11. BANNER ===== */
INSERT INTO Banner (TieuDe, DuongDanAnh, LienKet, ThuTu, TrangThai) VALUES
(N'Ưu đãi mùa thu - Giảm đến 20%',   N'/images/banners/banner1.jpg', N'/SanPham',                1, 1),
(N'Chăm sóc da cùng La Roche-Posay', N'/images/banners/banner2.jpg', N'/SanPham?thuongHieu=1',   2, 1),
(N'Trang điểm Maybelline - Son mới', N'/images/banners/banner3.jpg', N'/SanPham?danhMuc=2',      3, 1);
GO


SELECT COUNT(*) AS SoBang FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
SELECT 'TaiKhoan' AS Bang, COUNT(*) AS SoDong FROM TaiKhoan UNION ALL
SELECT 'SanPham', COUNT(*) FROM SanPham UNION ALL
SELECT 'DonHang', COUNT(*) FROM DonHang UNION ALL
SELECT 'ChiTietDonHang', COUNT(*) FROM ChiTietDonHang UNION ALL
SELECT 'DiaChiNhanHang', COUNT(*) FROM DiaChiNhanHang UNION ALL
SELECT 'VW_DanhGia', COUNT(*) FROM VW_DanhGia;