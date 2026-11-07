# GIAI ĐOẠN 3 – THIẾT KẾ CƠ SỞ DỮ LIỆU

**Công nghệ:** ASP.NET Core MVC (.NET 10), SQL Server, Entity Framework Core  
**Năm:** 2026

## 3.1. Mục tiêu và phạm vi

Giai đoạn này thiết kế cơ sở dữ liệu WebsiteBanMyPham trên SQL Server, phục vụ website kinh doanh mỹ phẩm trực tuyến với hai khu vực khách hàng và quản trị viên. Cơ sở dữ liệu lưu trữ tài khoản, sản phẩm, giỏ hàng, đơn hàng, thanh toán, khuyến mãi, đánh giá, khiếu nại và các dữ liệu liên quan.

## 3.2. Danh sách bảng dữ liệu

| STT | Tên bảng | Chức năng |
| :--- | :--- | :--- |
| 1 | TaiKhoan | Lưu tài khoản khách hàng và quản trị viên. |
| 2 | DatLaiMatKhau | Lưu thông tin xác minh/OTP đặt lại mật khẩu. |
| 3 | DanhMuc | Lưu danh mục sản phẩm. |
| 4 | ThuongHieu | Lưu thương hiệu mỹ phẩm. |
| 5 | SanPham | Lưu thông tin, giá, tồn kho và trạng thái sản phẩm. |
| 6 | AnhSanPham | Lưu ảnh của sản phẩm. |
| 7 | GioHang | Lưu sản phẩm và số lượng trong giỏ. |
| 8 | KhuyenMai | Lưu mã giảm giá, điều kiện và thời hạn. |
| 9 | PhuongThucThanhToan | Lưu phương thức thanh toán. |
| 10 | DonHang | Lưu thông tin chung của đơn hàng và người nhận. |
| 11 | ChiTietDonHang | Lưu các sản phẩm thuộc đơn hàng. |
| 12 | ThanhToan | Lưu giao dịch và trạng thái hoàn tiền. |
| 13 | LichSuDonHang | Lưu lịch sử thay đổi trạng thái đơn. |
| 14 | DanhGia | Lưu đánh giá sản phẩm đã mua. |
| 15 | KhieuNai | Lưu khiếu nại và quá trình xử lý. |
| 16 | Banner | Lưu banner/slider trên website. |

## 3.3. Mô tả bảng, khóa và quan hệ

| Bảng | Thuộc tính chính và khóa | Quan hệ/ý nghĩa |
| :--- | :--- | :--- |
| **TaiKhoan** | MaTK (PK), Email (UNIQUE), MatKhauHash, HoTen, SoDienThoai, DiaChi, VaiTro, TrangThai, NgayTao | Một tài khoản có thể có giỏ hàng, đơn hàng và khiếu nại. |
| **DatLaiMatKhau** | MaDL (PK), MaTK (FK), OtpHash, HetHan, DaDung, SoLanSai | Liên kết tài khoản để xác minh đặt lại mật khẩu. |
| **DanhMuc** | MaDM (PK), TenDM (UNIQUE), MoTa, TrangThai | Một danh mục có nhiều sản phẩm. |
| **ThuongHieu** | MaTH (PK), TenTH (UNIQUE), Logo, MoTa, TrangThai | Một thương hiệu có nhiều sản phẩm. |
| **SanPham** | MaSP (PK), TenSP, MoTa, Gia, GiaGoc, SoLuongTon, NguongCanhBao, TrangThai, MaDM (FK), MaTH (FK), NgayTao | Mỗi sản phẩm thuộc một danh mục và một thương hiệu. |
| **AnhSanPham** | MaAnh (PK), MaSP (FK), DuongDan, LaAnhChinh | Một sản phẩm có thể có nhiều ảnh. |
| **GioHang** | MaGH (PK), MaTK (FK), MaSP (FK), SoLuong, NgayThem | UNIQUE(MaTK, MaSP), tránh trùng sản phẩm trong giỏ. |
| **KhuyenMai** | MaKM (PK), Code (UNIQUE), TenKM, LoaiGiam, GiaTriGiam, GiamToiDa, DonToiThieu, NgayBatDau, NgayKetThuc, SoLuotToiDa, SoLuotDaDung, TrangThai | Được đơn hàng tham chiếu khi áp dụng mã. |
| **PhuongThucThanhToan** | MaPT (PK), TenPT (UNIQUE), MoTa, TrangThai | Một phương thức có thể dùng trong nhiều giao dịch. |
| **DonHang** | MaDH (PK), MaTK (FK), NgayDat, thông tin người nhận, TongTienHang, MaKM (FK), CodeKM, TienGiam, TongThanhToan, TrangThai, NgayVanDon, NgayGiaoThanhCong, MaVanDon | Một đơn có nhiều chi tiết, một bản ghi thanh toán và nhiều lịch sử. |
| **ChiTietDonHang** | MaCTDH (PK), MaDH (FK), MaSP (FK), TenSP, DonGia, SoLuong, ThanhTien (computed) | Lưu tên và giá tại thời điểm mua. |
| **ThanhToan** | MaTT (PK), MaDH (FK, UNIQUE), MaPT (FK), SoTien, TrangThai, TrangThaiHoanTien, MaGiaoDich, NgayThanhToan, NgayHoanTien | Mỗi đơn có tối đa một bản ghi thanh toán theo thiết kế. |
| **LichSuDonHang** | MaLS (PK), MaDH (FK), TrangThaiCu, TrangThaiMoi, MaTKThucHien (FK), NgayDoi | Ghi nhận lịch sử thay đổi trạng thái đơn. |
| **DanhGia** | MaDG (PK), MaCTDH (FK, UNIQUE), SoSao, NoiDung, TrangThai, NgayTao | Liên kết đánh giá với chi tiết đơn hàng. |
| **KhieuNai** | MaKN (PK), MaTK (FK), MaDH (FK, NULL), MaSP (FK, NULL), NoiDung, NgayGui, TrangThai, PhanHoi, NgayPhanHoi, MaTKXuLy (FK) | Lưu người gửi và tài khoản quản trị viên xử lý. |
| **Banner** | MaBanner (PK), TieuDe, DuongDanAnh, LienKet, ThuTu, TrangThai | Lưu banner trang chủ. |

## 3.4. Ràng buộc nghiệp vụ và toàn vẹn dữ liệu

* Email tài khoản là duy nhất; VaiTro gồm KhachHang hoặc QuanTriVien; tài khoản bị khóa không được đăng nhập.
* Giá và tồn kho không âm; GiaGoc nếu có phải lớn hơn hoặc bằng Gia.
* Mỗi sản phẩm thuộc một danh mục và một thương hiệu; ảnh sản phẩm phụ thuộc sản phẩm.
* Số lượng giỏ hàng lớn hơn 0; mỗi tài khoản chỉ có một dòng cho cùng một sản phẩm.
* Mã khuyến mãi là duy nhất; ngày kết thúc không trước ngày bắt đầu; giá trị giảm hợp lệ; số lượt sử dụng không vượt giới hạn.
* TongThanhToan = TongTienHang - TienGiam; chi tiết đơn hàng lưu tên và đơn giá tại thời điểm mua.
* Trạng thái đơn hàng: ChoXacNhan, DaXacNhan, DangGiao, HoanThanh, DaHuy.
* NgayVanDon được ghi khi chuyển DangGiao; NgayGiaoThanhCong được ghi khi chuyển HoanThanh; đơn hủy không có ngày giao thành công.
* Thanh toán có trạng thái ChuaThanhToan, DaThanhToan, ThatBai; trạng thái hoàn tiền được quản lý riêng.
* Đánh giá gắn với một chi tiết đơn hàng; SoSao từ 1 đến 5; trạng thái ChoDuyet, DaDuyet hoặc TuChoi.
* Ứng dụng phải kiểm tra đơn hàng thuộc khách hàng gửi khiếu nại và người xử lý khiếu nại có vai trò QuanTriVien.
* Cột KhieuNai.MaDH cho phép NULL ở mức cơ sở dữ liệu để giữ tương thích với dữ liệu khiếu nại cũ. Với khiếu nại tạo mới, ứng dụng bắt buộc có MaDH; đơn phải ở trạng thái HoanThanh và thời điểm gửi không quá 10 ngày kể từ NgayGiaoThanhCong; MaSP (nếu có) phải thuộc ChiTietDonHang của đơn đó; NoiDung từ 10 đến 1000 ký tự. Các quy tắc này được kiểm tra ở tầng ứng dụng, không thay đổi cấu trúc bảng.

## 3.5. Dữ liệu mẫu

Bộ dữ liệu mẫu gồm 05 tài khoản (01 quản trị viên, 04 khách hàng, có một tài khoản bị khóa); 05 danh mục; 08 thương hiệu; 20 sản phẩm (có sản phẩm tồn kho thấp và sản phẩm ngừng kinh doanh); 20 ảnh sản phẩm; 04 mã khuyến mãi; 03 dòng giỏ hàng; 06 đơn hàng; 11 chi tiết đơn hàng; 06 bản ghi thanh toán; 16 dòng lịch sử đơn hàng; 04 đánh giá; 03 khiếu nại và 03 banner.

## 3.6. Kiểm thử cơ sở dữ liệu

Nhóm đã thực hiện các kiểm tra sau:
* Đối chiếu TongTienHang của DonHang với tổng ThanhTien trong ChiTietDonHang.
* Đối chiếu TongThanhToan với TongTienHang trừ TienGiam.
* Đối chiếu SoTien trong ThanhToan với TongThanhToan của đơn hàng.
* Kiểm tra tính nhất quán giữa trạng thái đơn hàng, NgayVanDon và NgayGiaoThanhCong.
* Kiểm tra trạng thái thanh toán, hoàn tiền và các mốc thời gian liên quan.
* Chạy `DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS` để kiểm tra ràng buộc.

Kết quả kiểm thử đã thực hiện thành công: tổng tiền khớp nhau, các mốc ngày phù hợp trạng thái đơn hàng, thanh toán/hoàn tiền nhất quán và DBCC CHECKCONSTRAINTS không trả về vi phạm ràng buộc.

## 3.7. Kết luận giai đoạn 3

Cơ sở dữ liệu WebsiteBanMyPham gồm 16 bảng và view VW_DanhGia, có khóa chính, khóa ngoại, chỉ mục và các ràng buộc nghiệp vụ cần thiết. Dữ liệu mẫu và truy vấn kiểm thử đã được thực hiện, làm cơ sở kết nối với ASP.NET Core MVC.