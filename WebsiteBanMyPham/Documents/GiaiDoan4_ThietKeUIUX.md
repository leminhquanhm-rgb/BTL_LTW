# GIAI ĐOẠN 4 – THIẾT KẾ GIAO DIỆN

## 4.1. Mục tiêu

Giai đoạn này xây dựng bản thiết kế UI/UX cho toàn bộ hệ thống, làm cơ sở dựng các View ở giai đoạn sau. Kết quả gồm phong cách thiết kế, Design System, sơ đồ trang khách hàng và quản trị, bố cục từng trang, quy tắc nghiệp vụ và responsive. Prototype HTML tương tác gồm 6 cụm trang khách hàng, 9 mục chức năng quản trị và bộ Design System.

## 4.2. Phong cách và công nghệ giao diện

Phong cách nhẹ nhàng, nữ tính, sạch sẽ, phù hợp ngành mỹ phẩm. Nền trắng kem, màu nhấn hồng pastel và hồng đậm, ảnh sản phẩm lớn, thẻ bo góc mềm và bóng đổ nhẹ.

| Hạng mục | Lựa chọn |
| :--- | :--- |
| CSS | Tailwind CSS (CDN trong prototype) |
| Biểu tượng | Phosphor Icons |
| Font tiêu đề | Playfair Display |
| Font nội dung | Plus Jakarta Sans |
| Layout khách hàng | _Layout.cshtml |
| Layout quản trị | _AdminLayout.cshtml trong Area Admin |
| Tương tác | AJAX: thêm giỏ, đổi số lượng, áp mã, lọc sản phẩm |

*Khách hàng sử dụng header, thanh danh mục và footer; khu vực quản trị sử dụng sidebar bên trái. Hai khu vực dùng chung bảng màu và thành phần.*

## 4.3. Bảng màu

| Tên | Mã màu | Công dụng |
| :--- | :--- | :--- |
| Hồng pastel | #F8DDE5 | Màu nhấn nhẹ, nền khối nổi bật |
| Trắng kem | #FFF9F5 | Nền chính |
| Hồng nude | #E9B8C5 | Nút phụ, sidebar Admin |
| Hồng đậm | #C87991 | Nút chính, liên kết, giá |
| Hồng đậm hover | #B3627A | Hover nút chính |
| Nâu xám | #65545A | Chữ chính |
| Chữ phụ | #9A8B90 | Mô tả, nhãn phụ |
| Viền mảnh | #F0E4E8 | Đường kẻ, viền thẻ |
| Trắng | #FFFFFF | Thẻ sản phẩm, bảng dữ liệu |

Màu trạng thái: thành công #4A9D7C; cảnh báo #D99B52; nguy hiểm #D96B6B; thông tin #5B8FB9.

| Trạng thái đơn hàng | Màu chữ | Màu nền |
| :--- | :--- | :--- |
| 1. Chờ xác nhận | Cảnh báo | #FDF3E7 |
| 2. Đã xác nhận | Thông tin | #EBF3F9 |
| 3. Đang giao | Hồng đậm | Hồng pastel |
| 4. Hoàn thành | Thành công | #E6F4EE |
| Đã hủy | Nguy hiểm | #FCEAEA |

## 4.4. Thành phần dùng chung

| Thành phần | Quy ước |
| :--- | :--- |
| Nút chính | Nền hồng đậm, chữ trắng, bo góc 6px. |
| Nút phụ | Nền hồng nude hoặc viền hồng. |
| Thẻ | Nền trắng, bo góc 10px, bóng đổ nhẹ. |
| Thẻ sản phẩm | Ảnh, nhãn giảm giá, tên, thương hiệu, điểm sao, giá, nút Thêm giỏ và Mua ngay. |
| Voucher | Hiển thị mã, mức giảm, điều kiện tối thiểu, hạn dùng; bấm để chọn. |
| Bảng dữ liệu | Tiêu đề nền kem, đường kẻ mảnh, cột thao tác cuối. |
| Form | Nhãn phía trên, dấu * cho trường bắt buộc, báo lỗi dưới ô nhập. |
| Thông báo | Toast ngắn khi thao tác thành công hoặc lỗi. |
| Thanh tiến trình | 4 bước: Chờ xác nhận, Đã xác nhận, Đang giao, Hoàn thành. |

## 4.5. Sơ đồ trang

### 4.5.1. Khu vực khách hàng (6 cụm trang)

| Cụm trang | Nội dung | Controller/View dự kiến | Actor |
| :--- | :--- | :--- | :--- |
| 1. Trang chủ | Banner, voucher, danh mục chăm sóc, sản phẩm bán chạy/mới, đánh giá | TrangChu | Guest, Customer |
| 2. Sản phẩm | Danh sách, tìm kiếm, lọc danh mục/thương hiệu/giá, sắp xếp | SanPham | Guest, Customer |
| 3. Chi tiết sản phẩm | Ảnh, giá, tồn kho, số lượng, thêm giỏ, mua ngay, mô tả, đánh giá | ChiTietSanPham | Guest, Customer |
| 4. Giỏ hàng | Chọn sản phẩm, đổi số lượng, xóa | GioHang | Customer |
| 5. Đặt hàng và thanh toán | Thông tin nhận hàng, phương thức, mã giảm, tổng tiền, xác nhận | DatHang, ThanhToan | Customer |
| 6. Tài khoản và hỗ trợ | Đơn hàng, đánh giá, khiếu nại, hồ sơ, đổi mật khẩu, đăng nhập/đăng ký/quên mật khẩu | TaiKhoan, DonHang, DanhGia, KhieuNai | Guest, Customer |

### 4.5.2. Khu vực quản trị (9 mục chức năng)

| Nhóm | Mục | Nội dung |
| :--- | :--- | :--- |
| Dashboard và báo cáo | 1. Dashboard và Thống kê | Chỉ số tổng quan, doanh thu, đơn hàng, sản phẩm bán chạy. |
| Quản lý dữ liệu và kho | 2. Sản phẩm và Hình ảnh | CRUD sản phẩm, ngừng kinh doanh, quản lý ảnh. |
| Quản lý dữ liệu và kho | 3. Danh mục và Thương hiệu | CRUD danh mục, thương hiệu. |
| Quản lý dữ liệu và kho | 4. Tồn kho và Cảnh báo | Cập nhật số lượng, cảnh báo sắp hết. |
| Quản lý dữ liệu và kho | 5. Quản lý Đơn hàng | Xử lý trạng thái, ngày vận đơn, ngày giao thành công. |
| Quản lý dữ liệu và kho | 6. Thanh toán và Hoàn tiền | Giao dịch, hoàn tiền, phương thức thanh toán. |
| Quản lý dữ liệu và kho | 7. Khuyến mãi và Banner | CRUD khuyến mãi và banner/slider. |
| Quản lý dữ liệu và kho | 8. Tài khoản Khách và Admin | Xem, khóa/mở khóa, quản lý Admin. |
| Quản lý tương tác | 9. Đánh giá và Khiếu nại | Duyệt đánh giá, phản hồi và cập nhật khiếu nại. |

*Mã nguồn quản trị tổ chức trong Area Admin; chức năng quản trị bắt buộc kiểm tra vai trò QuanTriVien.*

## 4.6. Mô tả bố cục từng trang

### 4.6.1. Khu vực khách hàng

**Khung chung:** Header (logo, tìm kiếm, tài khoản, giỏ hàng kèm số lượng) → thanh danh mục → nội dung → Footer (danh mục, chính sách, hỗ trợ, bản quyền).

| Trang | Bố cục chính | Use Case |
| :--- | :--- | :--- |
| Trang chủ | Banner, voucher, danh mục chăm sóc, sản phẩm bán chạy/mới, thương hiệu, đánh giá. | Xem sản phẩm, khuyến mãi. |
| Sản phẩm | Bộ lọc bên trái; sắp xếp, lưới sản phẩm và phân trang bên phải. | Tìm kiếm, lọc. |
| Chi tiết | Ảnh lớn/ảnh nhỏ, thông tin sản phẩm, số lượng, Thêm giỏ/Mua ngay, mô tả, đánh giá. | Xem chi tiết, thêm giỏ, mua ngay. |
| Giỏ hàng | Bảng sản phẩm có chọn, ảnh, giá, số lượng, thành tiền, xóa; tạm tính. | Quản lý giỏ. |
| Đặt hàng/thanh toán | Thông tin nhận hàng, COD/online; sản phẩm, voucher, tiền hàng, tiền giảm, tổng thanh toán. | Đặt hàng UC01, áp mã UC03. |
| Tài khoản/hỗ trợ | Đơn hàng, tiến trình, ngày đặt/vận đơn/giao thành công, hủy, đánh giá, khiếu nại, hồ sơ, xác thực. | Hủy UC02, đánh giá UC05, khiếu nại. |

### 4.6.2. Khu vực quản trị

**Khung chung:** sidebar trái gồm logo, 9 mục chức năng, thông tin Admin, đổi mật khẩu và quay về trang khách hàng; vùng bên phải hiển thị nội dung. Bảng có tìm kiếm, lọc, phân trang và cột thao tác.

| Mục | Bố cục chính |
| :--- | :--- |
| Dashboard | Thẻ chỉ số, biểu đồ doanh thu/trạng thái đơn, sản phẩm bán chạy và sắp hết. |
| Sản phẩm/Hình ảnh | Bảng sản phẩm, form thêm/sửa, tải và quản lý ảnh. |
| Danh mục/Thương hiệu | Hai bảng CRUD và form. |
| Tồn kho | Bảng tồn, cập nhật số lượng, cảnh báo dưới ngưỡng. |
| Đơn hàng | Bảng khách, tổng tiền, trạng thái, ngày vận đơn/giao thành công, chuyển bước. |
| Thanh toán/Hoàn tiền | Bảng giao dịch, trạng thái thanh toán, hoàn tiền, phương thức. |
| Khuyến mãi/Banner | Bảng mã giảm giá và banner. |
| Tài khoản | Bảng tài khoản, khóa/mở khóa, tạo Admin. |
| Đánh giá/Khiếu nại | Duyệt/từ chối đánh giá, phản hồi khiếu nại. |

## 4.7. Quy tắc hiển thị theo nghiệp vụ

* Guest chỉ thấy Trang chủ, Sản phẩm, Chi tiết, Đăng nhập, Đăng ký, Quên mật khẩu; chức năng cần đăng nhập sẽ chuyển đến đăng nhập rồi quay lại bước đang làm.
* Sản phẩm hết hàng/ngừng kinh doanh hiển thị nhãn và vô hiệu hóa Thêm giỏ/Mua ngay.
* Giá gốc gạch ngang nếu có; phần trăm giảm theo giá bán và giá gốc, khác với mã khuyến mãi.
* Điểm sao sản phẩm tính từ đánh giá đã duyệt.
* Mua ngay chuyển đến đặt hàng chỉ với sản phẩm đó, không thay đổi giỏ hàng.
* Đặt từ giỏ chỉ lấy sản phẩm được chọn; chỉ xóa sản phẩm đã đặt thành công khỏi giỏ.
* Mỗi đơn dùng tối đa một mã; hiển thị số tiền giảm thực tế và có thể bỏ chọn mã.
* COD luôn có; VNPay/MoMo Sandbox là phần mở rộng.
* Chỉ hiện nút hủy khi đơn Chờ xác nhận; hủy hợp lệ hoàn tồn kho một lần.
* Ngày giao thành công chỉ hiện khi Hoàn thành; ngày vận đơn hiển thị từ trạng thái Đang giao.
* Chỉ đánh giá khi đơn Hoàn thành và sản phẩm chưa được đánh giá; đánh giá mới ở trạng thái Chờ duyệt.
* Chỉ hiện nút Gửi khiếu nại khi đơn Hoàn thành và còn trong 10 ngày kể từ ngày giao thành công; form khiếu nại chỉ liệt kê các đơn đủ điều kiện, nếu không có đơn nào thì hiển thị thông báo giải thích điều kiện.
* Admin chỉ chuyển sang trạng thái kế tiếp hợp lệ; hệ thống tự ghi ngày vận đơn và ngày giao thành công.
* Tài khoản bị khóa không đăng nhập được; Admin thấy trạng thái khóa.
* Thao tác xóa và hủy luôn có hộp thoại xác nhận.

## 4.8. Responsive

| Thiết bị | Chiều rộng | Sản phẩm/hàng | Điều chỉnh |
| :--- | :--- | :--- | :--- |
| Điện thoại | Dưới 640px | 2 | Menu thu gọn, bộ lọc lên trên, bảng cuộn ngang, sidebar Admin thu gọn. |
| Máy tính bảng | 640–1023px | 3 | Giữ phần lớn bố cục, giảm khoảng cách. |
| Máy tính | Từ 1024px | 4 | Bố cục đầy đủ, sidebar Admin cố định. |

## 4.9. Hình minh họa

Cần chèn ảnh chụp màn hình thực tế từ prototype HTML vào các vị trí tương ứng:
* Hình 4.1: Trang chủ.
* Hình 4.2: Danh sách sản phẩm và bộ lọc.
* Hình 4.3: Chi tiết sản phẩm.
* Hình 4.4: Giỏ hàng.
* Hình 4.5: Đặt hàng và thanh toán có mã khuyến mãi.
* Hình 4.6: Đơn hàng của tôi (tiến trình và ngày giao thành công).
* Hình 4.7: Đánh giá, khiếu nại, quên mật khẩu.
* Hình 4.8: Admin – Dashboard và Thống kê.
* Hình 4.9: Admin – Sản phẩm, Tồn kho.
* Hình 4.10: Admin – Quản lý đơn hàng.
* Hình 4.11: Admin – Khuyến mãi, Banner, Đánh giá, Khiếu nại.
* Hình 4.12: Design System – bảng màu.

*Lưu ý: tài liệu này liệt kê vị trí và tên hình; chưa chèn ảnh chụp prototype.*

## 4.10. Kết luận giai đoạn 4

Giai đoạn 4 hoàn thiện thiết kế UI/UX, bảng màu, thành phần dùng chung, sơ đồ trang hai khu vực, bố cục các trang, quy tắc hiển thị theo nghiệp vụ và responsive. Đây là cơ sở để khởi tạo project và dựng giao diện ở các giai đoạn tiếp theo.

## GHI CHÚ KHI CHUYỂN SANG LẬP TRÌNH

* Giữ nguyên database WebsiteBanMyPham và cấu trúc 16 bảng đã thiết kế.
* Prototype HTML là chuẩn UI/UX; dữ liệu JavaScript minh họa phải được thay bằng dữ liệu thật từ SQL Server.
* Area Admin phải được bảo vệ bằng kiểm tra quyền QuanTriVien ở phía server; ẩn nút trên giao diện không thay thế Authorization.
* AJAX chỉ hỗ trợ tương tác; tồn kho, giá, mã khuyến mãi, quyền truy cập và trạng thái đơn phải được kiểm tra lại ở server.
* Không chạy lại script tạo database có lệnh `DROP DATABASE` nếu cần giữ dữ liệu hiện tại.