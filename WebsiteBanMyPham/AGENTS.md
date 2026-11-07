# AGENTS.md – QUY CHUẨN PHÁT TRIỂN WEBSITE BÁN MỸ PHẨM

## 1. VAI TRÒ VÀ MỤC TIÊU

Bạn là lập trình viên chuyên nghiệp có kinh nghiệm với C#, ASP.NET Core MVC, SQL Server và Entity Framework Core.

Bạn tham gia phát triển bài tập lớn:

XÂY DỰNG WEBSITE QUẢN LÝ VÀ KINH DOANH MỸ PHẨM TRỰC TUYẾN

Tên project: WebsiteBanMyPham

Mục tiêu:

-   Xây dựng website có giao diện đẹp, hiện đại.
-   Các chức năng hoạt động thực tế.
-   Dữ liệu được lưu trữ trong SQL Server.
-   Nghiệp vụ đúng với yêu cầu đã thống nhất.
-   Giao diện bám sát prototype HTML và tài liệu UI/UX.
-   Có validation, xử lý lỗi và bảo mật.
-   Code dễ đọc, dễ hiểu, phù hợp với sinh viên CNTT.
-   Có thể kiểm thử, trình bày và bảo vệ trước giảng viên.

Không tự ý mở rộng phạm vi hoặc thay đổi các quyết định đã thống nhất.

## 2. CÔNG NGHỆ BẮT BUỘC

### 2.1. Backend

-   C#.
-   ASP.NET Core MVC.
-   .NET 10.
-   Visual Studio 2026.
-   Microsoft SQL Server 2022.
-   Entity Framework Core 10.

Không tự ý đổi phiên bản hoặc công nghệ.

### 2.2. Database

Sử dụng phương pháp Database-First.

-   Database đã được thiết kế và tạo trước.
-   Sử dụng EF Core Scaffold để sinh DbContext và Model.
-   Không sử dụng EF Core Migrations.
-   Không tự tạo database thay thế.
-   Không tự thay đổi cấu trúc database.

Khi scaffold:

-   Dùng đúng phiên bản EF Core 10 tương thích với project.
-   Không ghi đè Model hoặc DbContext đang tồn tại nếu chưa kiểm tra; không dùng tùy chọn --force khi người dùng chưa đồng ý.
-   Không đưa Connection String chứa thông tin bí mật vào mã nguồn: dùng tùy chọn --no-onconfiguring (hoặc xóa OnConfiguring sau khi scaffold) và cấu hình kết nối qua Program.cs với appsettings.Development.json, User Secrets hoặc biến môi trường.
-   Sau khi scaffold phải đối chiếu Model với cấu trúc SQL thực tế.

### 2.3. Xác thực và mật khẩu

Sử dụng:

-   Cookie Authentication tự triển khai trên bảng TaiKhoan.
-   BCrypt.Net-Next để hash và kiểm tra mật khẩu.
-   Claims để lưu thông tin tài khoản và vai trò.
-   [Authorize] để kiểm soát quyền.
-   [Authorize(Roles = "QuanTriVien")] để bảo vệ Admin.

Không sử dụng ASP.NET Identity.

Không lưu mật khẩu dạng văn bản thuần.

Hash mật khẩu phải tương thích với dữ liệu mẫu hiện tại, bao gồm định dạng BCrypt có tiền tố $2a$ nếu xuất hiện.

### 2.4. Frontend

Bắt buộc sử dụng:

-   Razor View.
-   HTML5.
-   Tailwind CSS qua CDN.
-   JavaScript.
-   Phosphor Icons.
-   Google Fonts.
-   CSS tùy chỉnh.

Font chữ:

-   Tiêu đề: Playfair Display.
-   Nội dung: Plus Jakarta Sans.

Không sử dụng Bootstrap để thay thế thiết kế hiện tại.

Không chuyển sang React, Angular, Vue hoặc Node.js.

### 2.5. Cấu trúc Workspace

Workspace dự kiến:

```text
WebsiteBanMyPham/                 <-- Workspace mở trong Antigravity (cũng là thư mục chứa Program.cs)
│
├── AGENTS.md
│
├── Database/
│   ├── CreateSQL_CHI_CHAY_MOT_LAN.sql
│   └── Du_Lieu_Mau.sql
│
├── Documents/
│   ├── GiaiDoan1_PhanTich.md
│   ├── GiaiDoan2_PhanTichChucNang.md
│   ├── GiaiDoan3_ThietKeCSDL.md
│   ├── GiaiDoan4_ThietKeUIUX.md
│   └── Prototype.html
│
├── Controllers/
├── Models/
├── Views/
├── wwwroot/
├── Properties/
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── WebsiteBanMyPham.csproj
└── WebsiteBanMyPham.slnx
```

Project nằm trực tiếp trong Workspace, không có thư mục project lồng bên trong. File solution là .slnx (Visual Studio 2026 và .NET 10 mặc định dùng .slnx); nếu thực tế là .sln thì vẫn chấp nhận. Documents/ ưu tiên file Markdown; nếu chưa chuyển thì dùng file .docx cùng tên.

Đây là cấu trúc định hướng. Phải kiểm tra cấu trúc thực tế trước khi tạo hoặc di chuyển file.

Nếu cấu trúc thực tế khác, báo cáo trước khi thay đổi.

Không tự ý tạo lại project hoặc ghi đè file đang hoạt động.

### 2.6. Cấu trúc code bên trong project

Định hướng:

```text
WebsiteBanMyPham/
│
├── Areas/
│   └── Admin/
│       ├── Controllers/
│       └── Views/
│
├── Constants/
├── Helpers/
├── Resources/
├── Controllers/
│   └── Api/
├── Models/
├── ViewModels/
│   └── Api/
├── Services/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── uploads/
│
├── Program.cs
└── appsettings.json
```

Vai trò:

-   Controllers/: xử lý request.
-   Areas/Admin/Controllers/: chức năng quản trị.
-   Areas/Admin/Views/: giao diện Admin.
-   Models/: Model scaffold từ database.
-   ViewModels/: dữ liệu phục vụ giao diện và form.
-   Services/: xử lý nghiệp vụ phức tạp.
-   Views/: giao diện khách hàng.
-   wwwroot/: CSS, JavaScript, hình ảnh.

Services dành cho nghiệp vụ phức tạp như đặt hàng, hủy đơn, khuyến mãi, thanh toán và thống kê.

CRUD đơn giản có thể sử dụng DbContext trực tiếp trong Controller.

Không over-engineering.

Tên Controller khách hàng theo tài liệu: TrangChu, SanPham, ChiTietSanPham, GioHang, DatHang, ThanhToan, TaiKhoan, DonHang, DanhGia, KhieuNai. Controller quản trị đặt trong Areas/Admin/Controllers.

### 2.7. Đa ngôn ngữ (Internationalization)

-   Hỗ trợ hai ngôn ngữ: tiếng Việt (vi-VN, mặc định) và tiếng Anh (en-US).
-   Dùng cơ chế chuẩn của ASP.NET Core: AddLocalization (ResourcesPath = "Resources"), AddViewLocalization, AddDataAnnotationsLocalization, UseRequestLocalization.
-   Tệp tài nguyên chung: Resources/SharedResource.resx (tiếng Việt, mặc định) và Resources/SharedResource.en.resx (tiếng Anh), kèm lớp đánh dấu SharedResource. Hai tệp luôn phải có CÙNG tập khóa.
-   Khóa là mã định danh tiếng Anh theo nhóm, ví dụ Nav_Home, Common_Save, Validation_Required, Msg_Order_OutOfStock, Status_Order_ChoXacNhan. KHÔNG dùng câu tiếng Việt làm khóa.
-   Ngôn ngữ được lưu bằng cookie (CookieRequestCultureProvider). Hành động đổi ngôn ngữ chỉ chấp nhận returnUrl cục bộ (Url.IsLocalUrl) để tránh open redirect. Không dùng Accept-Language để tự đổi ngôn ngữ: mặc định luôn là tiếng Việt cho đến khi người dùng chọn.
-   Mọi chữ hiển thị cho người dùng (View, nhãn và thông báo validation của ViewModel, thông báo trong Service, nhãn trạng thái, tiêu đề trang, thông báo lỗi, nội dung OTP demo) đều lấy từ tài nguyên; không viết cứng chuỗi.
-   Thông báo trả về từ Service dùng IStringLocalizer<SharedResource>. Thông báo ghi log (ILogger) dành cho lập trình viên nên giữ nguyên, không cần dịch.
-   Dữ liệu lấy từ database (tên sản phẩm, mô tả, thương hiệu, banner, khuyến mãi) giữ nguyên tiếng Việt, không dịch. Ngoại lệ duy nhất: tên danh mục được dịch bằng khóa tài nguyên DanhMuc_{MaDM} (qua ILocalizationHelper); nếu chưa có khóa thì dùng tên gốc trong database. Danh mục mới do Admin thêm sau này cần thêm khóa tương ứng vào cả hai tệp .resx.
-   Định dạng theo ngôn ngữ: tiền tiếng Việt "135.000đ", tiếng Anh "135,000 VND"; ngày giờ tiếng Việt dd/MM/yyyy HH:mm, tiếng Anh MM/dd/yyyy h:mm tt. Dùng helper tập trung trong thư mục Helpers/, không viết định dạng rải rác.
-   Khi thêm chữ mới ở bất kỳ bước nào: thêm khóa vào CẢ HAI tệp .resx trong cùng nhiệm vụ. Báo cáo cuối mỗi nhiệm vụ nêu số khóa của mỗi tệp (phải bằng nhau).
-   Không nhúng chuỗi tài nguyên trực tiếp vào chuỗi JavaScript bằng @L["..."] (Razor mã hóa HTML các ký tự có dấu thành dạng &#x1EA1;, trong khối script hoặc hộp thoại confirm sẽ hiện nguyên mã). Truyền chữ cho JavaScript bằng thuộc tính data-* (ví dụ data-confirm="@L["Cart_ConfirmDelete"]", JavaScript đọc bằng dataset) hoặc serialize JSON bằng System.Text.Json.
-   Kiểm tra khóa thiếu phải quét ĐỦ ba loại tệp: .cs, .cshtml và .js, gồm cả khóa ghép chuỗi động (Status_Order_*, DanhMuc_*, Payment_Method_*...). Sau mỗi nhiệm vụ có giao diện, phải mở từng trang mới ở CẢ HAI ngôn ngữ và dùng Ctrl+F tìm các tiền tố Common_, Nav_, Cart_, Product_, Account_, Msg_, Status_, Label_, Home_, Payment_: không được còn chữ dạng Nhóm_Tên hiện ra trên giao diện (đó là khóa chưa có bản dịch).

### 2.8. Web API (RESTful)

-   Dùng ASP.NET Core Web API ([ApiController]) đặt trong Controllers/Api/, tiền tố /api/v1/, tên tài nguyên số nhiều bằng tiếng Việt không dấu dạng kebab-case: san-pham, danh-muc, thuong-hieu, khuyen-mai, gio-hang, don-hang; API quản trị dưới /api/v1/admin/.
-   Đúng ngữ nghĩa HTTP: GET đọc; POST tạo (201 kèm Location); PUT cập nhật toàn phần; DELETE xóa (204). Mã trạng thái dùng đúng: 200, 201, 204, 400, 401, 403, 404, 409. Lỗi trả về ProblemDetails (RFC 7807), thông báo theo ngôn ngữ hiện tại.
-   Chỉ trả DTO (ViewModels/Api/), KHÔNG trả entity EF; JSON camelCase; danh sách có phân trang: pageIndex, pageSize (tối đa 50), và kết quả gồm items, pageIndex, pageSize, totalItems, totalPages.
-   Xác thực API dùng chung Cookie của website (gọi từ trình duyệt bằng fetch/AJAX cùng nguồn gốc). Chưa đăng nhập: trả 401 dạng JSON (không chuyển hướng sang trang đăng nhập); sai vai trò: 403 dạng JSON. Cấu hình OnRedirectToLogin và OnRedirectToAccessDenied cho đường dẫn bắt đầu bằng /api.
-   Mọi POST, PUT, PATCH, DELETE phải gửi header RequestVerificationToken (AntiForgery). Layout phát token qua thẻ meta; wwwroot/js/site.js có hàm apiFetch tự gắn token, xử lý lỗi và hiện toast.
-   Controller API mỏng như MVC controller: nghiệp vụ và kiểm tra quyền sở hữu dùng lại Services.
-   Tài liệu OpenAPI: Microsoft.AspNetCore.OpenApi cùng giao diện Scalar.AspNetCore (hoặc Swashbuckle nếu Scalar không tương thích .NET 10); chỉ bật ở môi trường Development; mô tả endpoint bằng XML comment và ProducesResponseType. Báo cáo phiên bản package đã dùng.
-   Các thao tác AJAX làm thay đổi dữ liệu của website (thêm giỏ, đổi số lượng, áp mã, hủy đơn...) gọi qua các endpoint này.

### 2.9. Soạn thảo văn bản (Rich text)

-   Dùng Quill 2.x qua CDN ghim phiên bản (giấy phép MIT, không cần khóa) cho mô tả sản phẩm (SanPham.MoTa) ở trang Admin. Thanh công cụ giới hạn: in đậm, nghiêng, gạch chân, tiêu đề (h2, h3), danh sách, liên kết, xóa định dạng; không cho chèn ảnh hay video, không cho dán ảnh base64.
-   BẮT BUỘC làm sạch HTML ở server bằng package HtmlSanitizer (Ganss.Xss) trước khi lưu, với danh sách cho phép: p, br, strong, em, u, h2, h3, ul, ol, li, blockquote, a (href chỉ http, https, mailto; tự thêm rel="noopener noreferrer"). Đóng gói trong IHtmlSanitizerService dùng chung.
-   Không dùng Html.Raw cho bất kỳ HTML nào chưa qua làm sạch. Khi hiển thị mô tả ở trang khách hàng cũng đi qua helper làm sạch.
-   Giới hạn độ dài mô tả (tối đa 20.000 ký tự) và kiểm tra ở server.

### 2.10. Xuất Excel

-   Dùng ClosedXML (MIT) trong IExcelExportService; trả về file .xlsx (MIME application/vnd.openxmlformats-officedocument.spreadsheetml.sheet).
-   Chức năng xuất (chỉ QuanTriVien): báo cáo doanh thu theo tháng, danh sách đơn hàng theo bộ lọc hiện tại, danh sách sản phẩm và tồn kho (có sản phẩm sắp hết hàng).
-   Có dòng tiêu đề, định dạng tiền và ngày theo ngôn ngữ hiện tại, tự điều chỉnh độ rộng cột, tên file kèm ngày giờ.
-   Chống formula injection: ô văn bản bắt đầu bằng ký tự =, +, -, @ phải được ghi dạng văn bản, không để Excel hiểu là công thức.
-   Doanh thu theo đúng quy tắc: đơn HoanThanh, tính theo NgayGiaoThanhCong. Giới hạn tối đa 50.000 dòng mỗi lần xuất, quá thì thông báo.

## 3. TÀI LIỆU CHÍNH THỨC

### 3.1. Database

Database/CreateSQL_CHI_CHAY_MOT_LAN.sql

Chứa cấu trúc database, bảng, khóa, constraint, index và view.

Database/Du_Lieu_Mau.sql

Chứa dữ liệu mẫu để kiểm thử.

### 3.2. Documents

Documents/ chứa tài liệu các giai đoạn:

-   Giai đoạn 1: Phân tích đề tài.
-   Giai đoạn 2: Phân tích chức năng.
-   Giai đoạn 3: Thiết kế cơ sở dữ liệu.
-   Giai đoạn 4: Thiết kế UI/UX.

Ưu tiên sử dụng Markdown để AI đọc tài liệu.

Nếu chưa chuyển sang Markdown thì dùng bản .docx cùng tên; khi đó phải báo cáo nếu không đọc được nội dung.

File HTML prototype phải được giữ nguyên.

### 3.3. Thứ tự ưu tiên khi có mâu thuẫn

1.  File SQL hiện tại: cấu trúc bảng, cột, khóa và constraint.
2.  AGENTS.md: các quyết định mới nhất của nhóm, bao gồm Mua ngay, hoàn lượt mã, OTP và các quy tắc bổ sung.
3.  Tài liệu Giai đoạn 1 và 2: nghiệp vụ gốc.
4.  Tài liệu Giai đoạn 3.
5.  Prototype HTML và tài liệu Giai đoạn 4.

Nếu phát hiện mâu thuẫn giữa các tài liệu:

-   Không tự ý lựa chọn phương án.
-   Báo cáo nội dung mâu thuẫn.
-   Giải thích ảnh hưởng.
-   Đưa ra phương án đề xuất.
-   Chờ người dùng xác nhận.

## 4. DATABASE HIỆN TẠI

Tên database:

WebsiteBanMyPham

Có 16 bảng:

1.  TaiKhoan
2.  DatLaiMatKhau
3.  DanhMuc
4.  ThuongHieu
5.  SanPham
6.  AnhSanPham
7.  GioHang
8.  KhuyenMai
9.  PhuongThucThanhToan

10. DonHang

11. ChiTietDonHang

12. ThanhToan

13. LichSuDonHang

14. DanhGia

15. KhieuNai

16. Banner

Có View:

VW_DanhGia

### 4.1. Quy tắc bảo toàn database

-   Đọc file SQL trước khi scaffold.
-   Giữ nguyên tên database.
-   Giữ nguyên tên bảng và cột.
-   Giữ nguyên kiểu dữ liệu.
-   Giữ nguyên Primary Key.
-   Giữ nguyên Foreign Key.
-   Giữ nguyên quan hệ.
-   Giữ nguyên CHECK Constraint.
-   Không tự ý thêm hoặc xóa cột.
-   Không tự ý xóa bảng.
-   Không tự ý thay đổi kiểu dữ liệu.
-   Không tự ý tạo database thay thế.

Nếu cần thay đổi database, phải báo cáo lý do, bảng bị ảnh hưởng, phương án và rủi ro. Chỉ thực hiện sau khi người dùng đồng ý.

### 4.2. Cảnh báo SQL

CreateSQL_CHI_CHAY_MOT_LAN.sql có lệnh DROP và CREATE DATABASE.

Đây là file chỉ dùng để tạo database ban đầu.

TUYỆT ĐỐI KHÔNG TỰ Ý CHẠY LẠI FILE NÀY.

Không tự ý chạy script có khả năng xóa database hoặc dữ liệu.

## 5. ĐỐI TƯỢNG VÀ CHỨC NĂNG

Hệ thống có hai khu vực chính.

### 5.1. Khu vực khách hàng

Trang chủ

-   Hiển thị banner.
-   Hiển thị danh mục.
-   Hiển thị thương hiệu.
-   Hiển thị sản phẩm mới.
-   Hiển thị sản phẩm bán chạy/nổi bật.
-   Hiển thị chương trình khuyến mãi.

Quy tắc lấy dữ liệu:

-   Sản phẩm mới: sắp xếp theo NgayTao giảm dần.
-   Sản phẩm bán chạy/nổi bật: tính tổng SoLuong trong ChiTietDonHang của các đơn HoanThanh, sắp xếp giảm dần.
-   Nếu chưa có đơn hàng hoàn thành hoặc chưa đủ dữ liệu bán hàng, lấy sản phẩm theo NgayTao giảm dần.
-   Không tự ý thêm cột đánh dấu nổi bật vào bảng SanPham.
-   Điểm sao và số lượt đánh giá: tính từ VW_DanhGia, chỉ lấy đánh giá có trạng thái DaDuyet.

Sản phẩm

-   Xem danh sách.
-   Xem chi tiết.
-   Tìm kiếm.
-   Lọc theo danh mục.
-   Lọc theo thương hiệu.
-   Lọc theo giá.
-   Sắp xếp.
-   Xem hình ảnh.

Tài khoản

-   Đăng ký.
-   Đăng nhập.
-   Đăng xuất.
-   Quên mật khẩu.
-   Đổi mật khẩu.
-   Xem và cập nhật thông tin cá nhân.

Giỏ hàng

-   Thêm sản phẩm.
-   Tăng số lượng.
-   Giảm số lượng.
-   Xóa sản phẩm.
-   Chọn sản phẩm muốn mua.
-   Tính tổng tiền.
-   Tiếp tục mua sắm.

Đặt hàng và thanh toán

-   Nhập thông tin nhận hàng.
-   Chọn phương thức thanh toán.
-   Áp dụng mã khuyến mãi.
-   Xác nhận đơn hàng.
-   Thanh toán COD.
-   Thanh toán Online theo phạm vi triển khai.

Đơn hàng

-   Xem danh sách.
-   Xem chi tiết.
-   Theo dõi trạng thái.
-   Hủy đơn khi đủ điều kiện.
-   Xem ngày vận đơn.
-   Xem ngày giao thành công.

Đánh giá

-   Gửi đánh giá.
-   Xem trạng thái đánh giá.
-   Chỉ hiển thị đánh giá đã duyệt.

Khiếu nại

-   Gửi khiếu nại.
-   Liên kết đơn hàng hoặc sản phẩm nếu phù hợp.
-   Theo dõi trạng thái.
-   Xem phản hồi.

### 5.2. Khu vực quản trị

Dashboard

-   Tổng doanh thu.
-   Tổng đơn hàng.
-   Tổng khách hàng.
-   Tổng sản phẩm.
-   Biểu đồ doanh thu theo tháng.
-   Thống kê trạng thái đơn hàng.
-   Sản phẩm bán chạy.
-   Sản phẩm sắp hết hàng.

Quản lý danh mục

-   Xem.
-   Thêm.
-   Sửa.
-   Xóa khi đủ điều kiện.
-   Chuyển trạng thái ngừng hoạt động khi không thể xóa.

Quản lý thương hiệu

-   Xem.
-   Thêm.
-   Sửa.
-   Xóa khi đủ điều kiện.
-   Chuyển trạng thái ngừng hoạt động khi không thể xóa.

Quản lý sản phẩm

-   Xem.
-   Thêm.
-   Sửa.
-   Quản lý hình ảnh.
-   Cập nhật giá.
-   Cập nhật tồn kho.
-   Ngừng bán.

Quản lý tồn kho

-   Xem tồn kho.
-   Cảnh báo sắp hết hàng.
-   Cập nhật số lượng.
-   Theo dõi sản phẩm hết hàng.

Quản lý đơn hàng

-   Xem danh sách.
-   Xem chi tiết.
-   Xác nhận đơn.
-   Chuyển trạng thái.
-   Cập nhật mã vận đơn.
-   Xem ngày vận đơn.
-   Xem ngày giao thành công.
-   Xem lịch sử thay đổi.

Quản lý thanh toán

-   Xem thanh toán.
-   Xem trạng thái.
-   Xử lý hoàn tiền.

Quản lý phương thức thanh toán

-   Xem.
-   Thêm.
-   Sửa.
-   Bật/tắt phương thức thanh toán.
-   Phương thức COD bắt buộc không được tắt.

Quản lý khuyến mãi

-   Xem.
-   Thêm.
-   Sửa.
-   Xóa khi đủ điều kiện.
-   Chuyển trạng thái ngừng hoạt động khi không thể xóa.

Quản lý banner

-   Thêm.
-   Sửa.
-   Xóa hoặc ngừng hiển thị theo thiết kế.
-   Thay đổi thứ tự.

Quản lý tài khoản

-   Xem khách hàng.
-   Khóa tài khoản.
-   Mở khóa.
-   Quản lý tài khoản Admin theo quyền được phép.

Quản lý đánh giá

-   Xem.
-   Duyệt.
-   Từ chối.

Quản lý khiếu nại

-   Xem.
-   Tiếp nhận.
-   Đang xử lý.
-   Giải quyết.
-   Từ chối.
-   Phản hồi.

Quy tắc xóa dữ liệu

Đối với danh mục, thương hiệu, khuyến mãi và các bản ghi có thể có dữ liệu liên quan:

-   Chỉ xóa khi bản ghi chưa có dữ liệu liên quan và không vi phạm khóa ngoại.
-   Nếu đã có dữ liệu liên quan, chuyển trạng thái ngừng hoạt động thay vì xóa.
-   Không xóa cứng dữ liệu lịch sử đơn hàng và thanh toán.
-   Không xóa dữ liệu làm ảnh hưởng lịch sử giao dịch.

## 6. QUY TẮC NGHIỆP VỤ

### 6.1. Tài khoản

-   Hai vai trò: KhachHang, QuanTriVien.
-   Email duy nhất.
-   Mật khẩu được hash bằng BCrypt.
-   Tài khoản bị khóa không được đăng nhập.
-   Đăng nhập thành công tạo Claims phù hợp.
-   Phân quyền phải được kiểm tra ở server.
-   Khách vãng lai (Guest) được xem, tìm kiếm, lọc sản phẩm. Thêm vào giỏ, Mua ngay, đặt hàng và các chức năng của khách hàng yêu cầu đăng nhập: chuyển sang trang đăng nhập và quay lại đúng trang đang làm sau khi đăng nhập thành công.
-   Admin không sử dụng giỏ hàng và không đặt hàng với tư cách khách hàng.

### 6.2. Sản phẩm

-   Một sản phẩm thuộc một danh mục và một thương hiệu.
-   Giá và tồn kho không được âm.
-   Sản phẩm ngừng bán không được đặt mua.
-   Không xóa cứng sản phẩm có lịch sử đơn hàng.
-   Nếu có GiaGoc thì hiển thị gạch ngang cạnh giá bán và tính nhãn phần trăm giảm = (GiaGoc - Gia) / GiaGoc. Đây là giảm giá hiển thị, khác với mã khuyến mãi.
-   Sản phẩm hết hàng hoặc ngừng kinh doanh phải hiển thị nhãn tương ứng và vô hiệu nút Thêm giỏ, Mua ngay.

### 6.3. Giỏ hàng

-   Lưu trong database.
-   Liên kết với tài khoản khách hàng.
-   Một khách không có hai dòng trùng sản phẩm.
-   Số lượng lớn hơn 0.
-   Giỏ hàng vẫn còn sau khi đăng xuất và đăng nhập lại.

Kiểm tra giỏ hàng khi hiển thị:

-   Khi hiển thị giỏ hàng, phải kiểm tra tình trạng kinh doanh và tồn kho hiện tại của từng sản phẩm.
-   Sản phẩm hết hàng hoặc ngừng bán: hiển thị thông báo rõ ràng, không cho chọn để đặt và không cho tiếp tục đặt sản phẩm đó.
-   Số lượng trong giỏ lớn hơn tồn kho hiện tại: thông báo số lượng còn lại tối đa và yêu cầu khách điều chỉnh; không tự ý xóa hoặc đổi dữ liệu giỏ hàng.
-   Việc kiểm tra này không thay thế kiểm tra tồn kho trong transaction khi tạo đơn.

### 6.4. Hai cách đặt hàng

Đặt hàng từ giỏ hàng

-   Chỉ lấy dòng được chọn.
-   Không lấy toàn bộ giỏ nếu khách chỉ chọn một phần.
-   Đặt thành công chỉ xóa các dòng đã đặt.
-   Dòng chưa chọn phải giữ nguyên.

Mua ngay

-   Đặt trực tiếp một sản phẩm.
-   Không bắt buộc thêm vào giỏ trước.
-   Không thay đổi giỏ hàng hiện tại.
-   Không xóa sản phẩm khỏi giỏ hàng.

Hai cách phải sử dụng chung nghiệp vụ tạo đơn hàng.

### 6.5. Tạo đơn hàng

Tạo đơn trong một transaction:

1.  Kiểm tra khách hàng.
2.  Kiểm tra người nhận.
3.  Kiểm tra sản phẩm.
4.  Kiểm tra trạng thái kinh doanh.
5.  Kiểm tra tồn kho.
6.  Lấy giá từ database.
7.  Tính tổng tiền.
8.  Kiểm tra mã khuyến mãi.
9.  Tính tiền giảm.

10. Tạo DonHang.

11. Tạo ChiTietDonHang.

12. Tạo ThanhToan.

13. Tạo LichSuDonHang.

14. Trừ tồn kho.

15. Tăng lượt dùng khuyến mãi nếu có.

16. Xóa dòng giỏ hàng đã đặt nếu đơn xuất phát từ giỏ hàng.

17. Commit transaction.

Nếu bất kỳ bước nào thất bại, rollback toàn bộ.

Nếu giao dịch thất bại:

-   Rollback toàn bộ thay đổi (đơn hàng, chi tiết, thanh toán, lịch sử, tồn kho, lượt mã).
-   Ghi nhận lỗi phục vụ debug (ILogger), không ghi mật khẩu hoặc dữ liệu nhạy cảm.
-   Không hiển thị thông tin lỗi database nhạy cảm cho khách hàng.
-   Trả về thông báo dễ hiểu và giữ lại dữ liệu khách đã nhập để thử lại.
-   Không để lại đơn hàng hoặc dữ liệu thanh toán chưa hoàn chỉnh.

Không tin giá, tổng tiền và tồn kho gửi từ trình duyệt.

Khi tạo ChiTietDonHang phải lưu TenSP và DonGia tại thời điểm mua; việc sửa tên hoặc giá sản phẩm sau này không được làm thay đổi đơn cũ. Giỏ hàng hiển thị giá hiện tại, giá chỉ được chốt khi tạo đơn. Cột ThanhTien là cột tính toán, không gán giá trị khi thêm dữ liệu.

### 6.6. Chống bán vượt tồn kho

Dùng cập nhật có điều kiện tương đương:

```sql
UPDATE SanPham
SET SoLuongTon = SoLuongTon - @sl
WHERE MaSP = @maSP
AND SoLuongTon >= @sl
AND TrangThai = 1;
```

Kiểm tra số dòng bị ảnh hưởng.

Nếu không cập nhật đúng một dòng:

-   Dừng đặt hàng.
-   Rollback transaction.
-   Thông báo hết hàng hoặc không còn kinh doanh.

Phải kiểm soát trường hợp nhiều người mua đồng thời.

### 6.7. Khuyến mãi

-   Mỗi đơn tối đa một mã.
-   Kiểm tra mã tồn tại.
-   Kiểm tra trạng thái.
-   Kiểm tra thời hạn.
-   Kiểm tra số lượt sử dụng.
-   Kiểm tra giá trị đơn tối thiểu.
-   Kiểm tra loại giảm.
-   Kiểm tra giới hạn giảm tối đa.

Công thức:

TongThanhToan = TongTienHang - TienGiam

Lưu MaKM, CodeKM, TienGiam trong DonHang.

Tính lại trên server.

Kiểm soát lượt sử dụng đồng thời.

Hủy đơn hợp lệ phải hoàn lượt mã đúng một lần.

### 6.8. Trạng thái đơn hàng

Các trạng thái:

-   ChoXacNhan
-   DaXacNhan
-   DangGiao
-   HoanThanh
-   DaHuy

Luồng:

```text
ChoXacNhan
|
v
DaXacNhan
|
v
DangGiao
|
v
HoanThanh
```

Nhánh hủy:

```text
ChoXacNhan
|
v
DaHuy
```

Không cho chuyển trạng thái tùy ý.

### 6.9. Ngày vận đơn và ngày giao thành công

Trong DonHang có:

-   NgayVanDon
-   NgayGiaoThanhCong
-   MaVanDon

Quy tắc:

-   Chuyển DangGiao: tự động ghi NgayVanDon.
-   Ghi MaVanDon nếu có.
-   Chuyển HoanThanh: tự động ghi NgayGiaoThanhCong.
-   Không cho người dùng tự nhập hai ngày qua form thông thường.
-   Đơn hủy không có ngày giao thành công.

Trạng thái và ngày phải được lưu cùng một lần cập nhật để không vi phạm CK_DH_TrangThai_Ngay.

Mỗi lần thay đổi trạng thái phải ghi LichSuDonHang.

### 6.10. Hủy đơn

-   Chỉ chủ sở hữu đơn được hủy.
-   Chỉ hủy khi ChoXacNhan.
-   Không hủy đơn đã xác nhận, đang giao hoặc hoàn thành.
-   Hoàn kho đúng một lần.
-   Hoàn lượt mã đúng một lần.
-   Ghi lịch sử thay đổi.

Nếu đơn Online đã thanh toán:

-   Chuyển TrangThaiHoanTien sang ChoHoan.
-   Admin xác nhận hoàn tiền.
-   Chuyển DaHoan khi hoàn tiền được xác nhận.
-   Ghi NgayHoanTien.

### 6.11. Thanh toán COD và Online

COD:

-   Tạo ThanhToan với ChuaThanhToan.
-   Khi đơn hoàn thành, chuyển sang DaThanhToan.
-   Ghi NgayThanhToan.

Online:

-   Có thể mô phỏng hoặc sử dụng Sandbox.
-   Không tự đánh dấu thành công chỉ vì khách nhấn nút thanh toán.
-   Phải kiểm tra kết quả thanh toán.

Các CHECK Constraint bắt buộc:

-   Khi chuyển ThanhToan sang DaThanhToan, phải ghi NgayThanhToan trong cùng lần lưu.
-   Khi chuyển TrangThaiHoanTien sang DaHoan, phải ghi NgayHoanTien trong cùng lần lưu.
-   Chỉ được đặt TrangThaiHoanTien khác KhongCan khi TrangThai = DaThanhToan.
-   ThanhToan.SoTien phải bằng DonHang.TongThanhToan.

Phải tuân thủ:

-   CK_TT_NgayThanhToan
-   CK_TT_NgayHoanTien
-   CK_TT_HoanTien

Không lưu trạng thái mà thiếu ngày tương ứng.

### 6.12. Quên mật khẩu

Áp dụng cho cả khách hàng và Admin.

-   OTP 6 chữ số.
-   Lưu hash trong DatLaiMatKhau.
-   Hết hạn sau 10 phút.
-   Chỉ sử dụng một lần.
-   Tối đa 5 lần nhập sai.
-   Nhập sai tăng SoLanSai.
-   OTP hết hạn hoặc đã sử dụng không được chấp nhận.
-   Không tiết lộ email có tồn tại hay không.

Trong Development có thể in OTP ra Console để demo.

Không hiển thị OTP trên giao diện production.

### 6.13. Đánh giá

Bảng DanhGia chỉ liên kết qua MaCTDH.

Không thêm MaTK, MaSP vào bảng này.

Lấy thông tin qua VW_DanhGia.

-   Khách phải đăng nhập.
-   Đơn phải thuộc khách.
-   Đơn phải HoanThanh.
-   Chỉ đánh giá sản phẩm đã mua.
-   Mỗi chi tiết đơn hàng chỉ đánh giá một lần.
-   Số sao từ 1 đến 5.
-   Mới gửi có trạng thái ChoDuyet.
-   Chỉ DaDuyet được hiển thị công khai.

### 6.14. Khiếu nại

-   Khách phải đăng nhập.
-   Đơn hàng gắn kèm phải thuộc khách.
-   Kiểm tra sản phẩm gắn kèm theo nghiệp vụ.
-   Khách không xem khiếu nại của người khác.
-   Chỉ Admin được xử lý.

Trạng thái:

-   ChoXuLy
-   DangXuLy
-   DaGiaiQuyet
-   TuChoi

### 6.15. Doanh thu

Chỉ tính đơn HoanThanh.

Dựa trên NgayGiaoThanhCong.

Không tính đơn đang xử lý, đang giao hoặc đã hủy.

## 7. GIAO DIỆN

### 7.1. Phong cách

Hồng pastel – trắng kem – hiện đại – tối giản – thanh lịch.

Bám sát prototype HTML và tài liệu Giai đoạn 4.

### 7.2. Bảng màu

| Thành phần | Mã màu |
|---|---|
| Background | #FFF9F5 |
| Pastel Pink | #F8DDE5 |
| Nude | #E9B8C5 |
| Primary | #C87991 |
| Text | #65545A |
| Muted | #9A8B90 |
| Surface | #FFFFFF |

### 7.3. Layout khách hàng

Views/Shared/_Layout.cshtml

### 7.4. Layout Admin

Areas/Admin/Views/Shared/_AdminLayout.cshtml

Admin có giao diện riêng, sidebar và dashboard.

### 7.5. Responsive

-   Dưới 640px: 2 sản phẩm mỗi hàng.
-   640–1023px: 3 sản phẩm mỗi hàng.
-   Từ 1024px: 4 sản phẩm mỗi hàng.

### 7.6. Nguyên tắc

-   Có loading.
-   Có thông báo thành công/thất bại.
-   Có trạng thái dữ liệu trống.
-   Có xác nhận thao tác quan trọng.
-   Có validation rõ ràng.
-   Không để nút bấm chỉ trang trí.
-   Không dùng dữ liệu mock khi đã kết nối database.
-   Không trộn giao diện khách hàng và Admin.

## 8. BẢO MẬT

### 8.1. Authentication và Authorization

-   Cookie Authentication.
-   Claims.
-   Role.
-   Đăng xuất xóa Cookie.
-   Tài khoản khóa không được đăng nhập.
-   Admin phải được bảo vệ ở server bằng [Authorize(Roles = "QuanTriVien")].

Cấu hình Role Claims:

-   Khi đăng nhập thành công, tạo claim ClaimTypes.NameIdentifier chứa MaTK, ClaimTypes.Name hoặc họ tên, ClaimTypes.Email và ClaimTypes.Role lấy từ TaiKhoan.VaiTro (KhachHang hoặc QuanTriVien), rồi SignIn bằng Cookie Authentication.
-   Phải cấu hình Role Claims đúng để [Authorize(Roles = "QuanTriVien")] hoạt động chính xác.
-   Cấu hình LoginPath và AccessDeniedPath: chưa đăng nhập thì chuyển sang đăng nhập, sai vai trò thì báo không có quyền.
-   Kiểm tra quyền tại Controller hoặc Action ở phía server; ẩn nút trên giao diện không thay thế việc này.
-   Phải thử: Admin vào được khu vực Admin; khách hàng và người chưa đăng nhập không vào được.
-   Khi dựng đăng nhập ở Bước 5: dùng CookieAuthenticationEvents.OnValidatePrincipal kiểm tra TrangThai của tài khoản trong database; tài khoản bị khóa giữa phiên thì RejectPrincipal và đăng xuất ngay.
-   Chức năng đăng nhập chỉ kiểm tra khóa sau khi mật khẩu đúng; ViewModel đăng nhập chỉ yêu cầu bắt buộc nhập và độ dài tối đa, không áp quy tắc độ dài tối thiểu của mật khẩu mới.

### 8.2. Kiểm tra quyền sở hữu

Khách chỉ được thao tác dữ liệu của mình:

-   Giỏ hàng.
-   Đơn hàng.
-   Chi tiết đơn hàng.
-   Đánh giá.
-   Khiếu nại.
-   Thông tin tài khoản.

Không tin ID từ URL hoặc Form.

Lấy tài khoản đăng nhập từ Claims và đối chiếu database.

### 8.3. Anti-Forgery

-   Mọi POST phải có [ValidateAntiForgeryToken] hoặc cơ chế tương đương.
-   Form phải gửi Anti-Forgery Token.
-   Không bỏ qua kiểm tra khi thay đổi dữ liệu.

### 8.4. Validation

Kiểm tra ở server:

-   Trường bắt buộc.
-   Độ dài.
-   Email.
-   Số điện thoại.
-   Giá trị số.
-   Số lượng.
-   Trạng thái.
-   Dữ liệu trùng.
-   Khóa ngoại.
-   Quyền sở hữu.
-   Điều kiện nghiệp vụ.

Không chỉ dựa vào JavaScript.

### 8.5. Upload

-   Kiểm tra phần mở rộng.
-   Kiểm tra dung lượng.
-   Không tin tên file người dùng gửi.
-   Đổi tên ngẫu nhiên.
-   Không cho upload file thực thi.
-   Lưu đường dẫn hợp lệ.
-   Xử lý file không hợp lệ.

### 8.6. Cookie và bí mật

-   HttpOnly.
-   Secure phù hợp HTTPS.
-   SameSite phù hợp.
-   Không lưu mật khẩu thuần.
-   Không đưa thông tin bí mật vào Git.
-   Không hiển thị Connection String.
-   Không hiển thị Stack Trace trên Production.

### 8.7. Connection String

Ưu tiên:

-   appsettings.Development.json.
-   User Secrets.
-   Biến môi trường.
-   Cấu hình riêng cho Production.

Không hardcode thông tin bí mật trong code.

## 9. QUY TRÌNH TRIỂN KHAI

Thực hiện theo 7 bước kỹ thuật sau:

Bước 1 – Khởi tạo và chuẩn hóa project

-   Tạo .gitignore chuẩn cho .NET (bin/, obj/, .vs/, *.user...); không bỏ qua Database/, Documents/, AGENTS.md, wwwroot/uploads, wwwroot/images.
-   Đổi HomeController thành TrangChuController (kèm Views/Home thành Views/TrangChu, route mặc định, liên kết menu).
-   Tạo khung thư mục: Areas/Admin/Controllers, Areas/Admin/Views/Shared, Services, ViewModels, wwwroot/css, wwwroot/js.
-   Cấu hình route Area Admin trong Program.cs, đặt trước route mặc định.
-   Giữ nguyên layout và Bootstrap mẫu cho đến Bước 4.
-   Chưa cài package, chưa kết nối database, chưa cấu hình đăng nhập.
-   Không tạo project mới. Kết quả: dotnet build thành công, trang chủ chạy.

Bước 2 – Kết nối database

-   Kiểm tra SQL Server và database WebsiteBanMyPham đã được tạo bằng script.
-   Cấu hình Connection String (appsettings.Development.json, User Secrets hoặc biến môi trường).
-   Cài package: Microsoft.EntityFrameworkCore.SqlServer và Microsoft.EntityFrameworkCore.Design (cùng phiên bản 10.0.x), công cụ dotnet-ef.
-   Scaffold DbContext và toàn bộ Model từ database với --no-onconfiguring --use-database-names --no-pluralize (giữ đúng tên bảng và cột như MaTK, MaCTDH, TaiKhoan, VW_DanhGia; nếu không, EF đổi thành MaTk, MaCtdh, VwDanhGia); không tạo Model thủ công.
-   Đăng ký DbContext trong Program.cs; kiểm tra quan hệ và kết nối.

Bước 3 – ViewModel và Services (chia hai phần: 3A tài khoản và đăng nhập; 3B giỏ hàng, khuyến mãi, đơn hàng)

-   Đối chiếu Model đã scaffold với SQL (chỉ kiểm tra, không viết lại Model).
-   Cài BCrypt.Net-Next; cấu hình Cookie Authentication và Authorization trong Program.cs (LoginPath, AccessDeniedPath, Role Claims).
-   Tạo ViewModel.
-   Tạo cấu trúc Services khi cần.
-   Không cấu hình Session: giỏ hàng lưu trong database.
-   Không tự ý sửa Model scaffold.
-   Không dùng Migrations.

Bước 3C – Đa ngôn ngữ nền tảng

-   Cấu hình localization trong Program.cs (AddLocalization, AddViewLocalization, AddDataAnnotationsLocalization, UseRequestLocalization với vi-VN mặc định và en-US, cookie provider).
-   Tạo lớp SharedResource, Resources/SharedResource.resx (vi) và SharedResource.en.resx (en).
-   Tạo CultureController đổi ngôn ngữ (chỉ chấp nhận returnUrl cục bộ).
-   Tạo Helpers/ cho định dạng tiền, ngày giờ và nhãn trạng thái theo ngôn ngữ.
-   Chuyển thông báo trong Services và nhãn, thông báo validation trong ViewModel sang tài nguyên.
-   Không tạo View hoặc giao diện (thuộc Bước 4).

Bước 4 – Giao diện chung

-   Nút chuyển ngôn ngữ VI | EN trên header của cả layout khách hàng và layout Admin.
-   Mọi chữ trong layout và View lấy từ tài nguyên, không viết cứng.
-   Layout khách hàng.
-   Layout Admin.
-   Header.
-   Footer.
-   Sidebar.
-   Menu.
-   CSS.
-   JavaScript.
-   Responsive.

Bước 5 – Chức năng khách hàng (chia thành 5A đến 5F)

Bước 5A – Tài khoản: đăng ký, đăng nhập, đăng xuất, quên mật khẩu bằng OTP, hồ sơ, đổi mật khẩu.

Bước 5B – Sản phẩm: trang chủ (banner từ database, sản phẩm mới, bán chạy, khuyến mãi), danh sách sản phẩm với tìm kiếm và lọc bằng AJAX (trả về partial), chi tiết sản phẩm (điểm sao và đánh giá đã duyệt), và Web API công khai (san-pham, danh-muc, thuong-hieu, khuyen-mai).

Bước 5C – Giỏ hàng và mã khuyến mãi: thêm, sửa số lượng, xóa bằng AJAX qua Web API gio-hang và khuyen-mai.

Bước 5D – Đặt hàng, Mua ngay và thanh toán: COD, Online mô phỏng; chống bấm đặt hàng hai lần.

Bước 5E – Đơn hàng của tôi, hủy đơn (qua Web API), đánh giá, khiếu nại; tạo IDanhGiaService và IKhieuNaiService.

Bước 5F – Hoàn thiện Web API và tài liệu OpenAPI: rà soát chuẩn REST, ProblemDetails, 401 và 403 dạng JSON, phân trang, header chống giả mạo, API quản trị danh-muc và thuong-hieu (CRUD đầy đủ).

Bước 6 – Chức năng quản trị (chia thành 6A đến 6E)

Bước 6A – Dashboard, thống kê và xuất Excel (tạo IThongKeService, IExcelExportService).

Bước 6B – Sản phẩm (soạn thảo rich text, tải ảnh), hình ảnh, danh mục, thương hiệu, tồn kho và cảnh báo.

Bước 6C – Đơn hàng, thanh toán, hoàn tiền, phương thức thanh toán.

Bước 6D – Khuyến mãi, banner, tài khoản (khóa và mở khóa phải xóa cache của CookieValidator).

Bước 6E – Đánh giá (duyệt, từ chối) và khiếu nại (phản hồi, đổi trạng thái).

Bước 7 – Kiểm thử và hoàn thiện

-   Kiểm thử chức năng.
-   Kiểm thử nghiệp vụ.
-   Kiểm thử phân quyền.
-   Kiểm thử bảo mật.
-   Kiểm thử dữ liệu.
-   Kiểm thử responsive.
-   Kiểm tra giao diện.
-   Hoàn thiện báo cáo.
-   Chuẩn bị bảo vệ.

Lưu ý: Đây là thứ tự triển khai kỹ thuật. Khi lập báo cáo, vẫn sử dụng hệ thống đánh số Giai đoạn 1 đến Giai đoạn 12 theo kế hoạch báo cáo của nhóm. Không tự ý đánh lại số giai đoạn trong tài liệu báo cáo.

## 10. QUY TRÌNH LÀM VIỆC

### 10.1. Trước khi code

1.  Đọc yêu cầu.
2.  Đọc file liên quan.
3.  Kiểm tra code hiện tại.
4.  Phân tích phương án.
5.  Liệt kê file cần tạo.
6.  Liệt kê file cần sửa.
7.  Giải thích lý do.
8.  Nêu điểm chưa rõ.

Nếu mâu thuẫn, phải hỏi người dùng.

### 10.2. Trong khi code

-   Chỉ thực hiện nhiệm vụ được giao.
-   Không tự ý làm thêm chức năng.
-   Không xóa code đang hoạt động.
-   Không tự ý đổi tên file, Controller, Action, Model hoặc thuộc tính.
-   Không tự ý đổi database, bảng hoặc cột.
-   Không tự ý đổi công nghệ.

Ngoại lệ: Ở Bước 1, được phép đổi HomeController thành TrangChuController khi người dùng giao việc này. Khi thực hiện phải cập nhật đồng bộ route mặc định, View tương ứng và các liên kết menu. Không được đổi riêng lẻ khiến project lỗi.

### 10.3. Sau khi code

1.  Build project.
2.  Kiểm tra lỗi biên dịch.
3.  Kiểm tra namespace.
4.  Kiểm tra Model Binding.
5.  Kiểm tra database nếu có.
6.  Kiểm tra nghiệp vụ.
7.  Kiểm tra validation.
8.  Kiểm tra quyền.
9.  Báo cáo kết quả.

Không tuyên bố hoàn thành nếu chưa kiểm tra.

Nếu không thể kiểm thử, phải nói rõ.

### 10.4. Báo cáo sau mỗi nhiệm vụ

1. Công việc đã thực hiện

2. File đã tạo

3. File đã sửa

4. Cách hoạt động

5. Database

6. Kiểm thử

7. Lỗi còn tồn tại

8. Hướng dẫn người dùng kiểm tra

9. Bước tiếp theo

Sau báo cáo phải dừng và chờ người dùng xác nhận.

## 11. QUY ƯỚC CHUNG

-   Giao diện hỗ trợ tiếng Việt có dấu (mặc định) và tiếng Anh theo mục 2.7.
-   Tiền và ngày giờ hiển thị theo ngôn ngữ hiện tại (xem mục 2.7), qua helper trong Helpers/.
-   Các cột ngày giờ lưu vào database dùng DateTime.Now (giờ địa phương, khớp SYSDATETIME() của SQL Server); không trộn DateTime.UtcNow (trừ ExpiresUtc của cookie).
-   Hằng số vai trò và trạng thái đặt trong thư mục Constants/ (ví dụ VaiTroConstant).
-   Tên trạng thái trong code phải khai báo tập trung trong static class hằng số phù hợp.
-   Ví dụ: TrangThaiDonHang.ChoXacNhan, TrangThaiDanhGia.DaDuyet.
-   Không viết chuỗi trạng thái rải rác trong nhiều Controller hoặc Service.
-   Các giá trị hằng phải thống nhất với dữ liệu SQL hiện tại.
-   Không tự ý thay đổi giá trị lưu trong database.
-   Không tự động Git commit hoặc Git push.
-   Sau mỗi nhiệm vụ, chỉ gợi ý nội dung commit để người dùng tự quyết định.
-   Khi cần chạy dotnet build hoặc dotnet run, nhắc người dùng tắt phiên Debug đang chạy trong Visual Studio nếu có nguy cơ khóa file.
-   Không xóa dữ liệu hoặc file chỉ để xử lý lỗi build khi chưa xác định nguyên nhân.

## 12. NGUYÊN TẮC VIẾT CODE

-   Code dễ đọc.
-   Tên biến, class và method rõ nghĩa.
-   Mỗi hàm thực hiện một nhiệm vụ chính.
-   Tách hàm nhỏ khi cần.
-   Không viết nghiệp vụ phức tạp trong Razor View.
-   Không lặp code không cần thiết.
-   Không sử dụng thư viện không cần thiết.
-   Không over-engineering.
-   Có xử lý ngoại lệ.
-   Có thông báo lỗi dễ hiểu.
-   Không che giấu lỗi bằng dữ liệu giả.

Phải giải thích:

-   File nằm ở đâu?
-   File dùng để làm gì?
-   Action hoạt động thế nào?
-   Model được sử dụng ra sao?
-   Dữ liệu đi từ View đến Controller và database như thế nào?
-   Thành phần nào liên kết với nhau?

Mục tiêu là giúp sinh viên hiểu code và tự trình bày được.

## 13. QUY TẮC KIỂM THỬ

Mỗi chức năng phải kiểm tra trường hợp đúng và sai.

Đăng nhập

-   Đúng thông tin.
-   Sai mật khẩu.
-   Email không tồn tại.
-   Tài khoản bị khóa.
-   Sai vai trò.

Giỏ hàng

-   Thêm sản phẩm.
-   Thêm sản phẩm trùng.
-   Số lượng bằng 0.
-   Vượt tồn kho.
-   Sản phẩm ngừng bán.
-   Xóa sản phẩm.
-   Đăng xuất và đăng nhập lại.

Đặt hàng

-   Đặt hàng thành công.
-   Không đủ tồn kho.
-   Sản phẩm ngừng bán.
-   Mã không tồn tại.
-   Mã hết hạn.
-   Mã hết lượt.
-   Không đủ điều kiện đơn tối thiểu.
-   Đặt một phần giỏ hàng.
-   Mua ngay.
-   Rollback khi thất bại.
-   Hai người cùng mua sản phẩm cuối cùng.

Đơn hàng

-   Xem đơn của chính mình.
-   Không xem được đơn người khác.
-   Hủy khi chờ xác nhận.
-   Không hủy đơn đã giao.
-   Hoàn kho đúng một lần.
-   Hoàn lượt mã đúng một lần.
-   Ghi ngày vận đơn.
-   Ghi ngày giao thành công.

Thanh toán

-   COD chưa thanh toán.
-   COD đã thanh toán.
-   Online thành công.
-   Online thất bại.
-   Hoàn tiền chờ xử lý.
-   Hoàn tiền thành công.
-   Đảm bảo các ngày thanh toán và hoàn tiền hợp lệ.

Admin

-   Admin truy cập được.
-   Khách không truy cập được.
-   Người chưa đăng nhập không truy cập được.
-   Không thể thay đổi dữ liệu trái quyền.

## 14. NHỮNG ĐIỀU TUYỆT ĐỐI KHÔNG ĐƯỢC LÀM

1.  Không tạo project mới.
2.  Không đổi công nghệ.
3.  Không đổi database.
4.  Không đổi tên bảng hoặc cột.
5.  Không tự ý xóa dữ liệu.
6.  Không tự chạy script DROP DATABASE.
7.  Không sử dụng EF Core Migrations.
8.  Không sử dụng ASP.NET Identity.
9.  Không thay Tailwind bằng Bootstrap.

10. Không tự ý thay prototype bằng thiết kế khác.

11. Không dùng dữ liệu giả thay database khi chức năng đã kết nối.

12. Không bỏ qua kiểm tra quyền sở hữu.

13. Không cho đặt sản phẩm ngừng bán.

14. Không bán vượt tồn kho.

15. Không đặt hàng thiếu transaction.

16. Không hoàn kho hoặc lượt mã nhiều lần.

17. Không tự ý thêm cột vào DanhGia.

18. Không bỏ CHECK Constraint.

19. Không lưu mật khẩu dạng văn bản thuần.

20. Không chỉ bảo vệ Admin bằng giao diện.

21. Không tự ý làm nhiệm vụ tiếp theo.

22. Không báo cáo hoàn thành khi chưa kiểm thử.

23. Không tự động commit hoặc push Git.

24. Không viết cứng (hard-code) chuỗi hiển thị trong View, ViewModel hoặc Service; phải dùng tài nguyên đa ngôn ngữ.

25. Không dùng Html.Raw với HTML chưa qua làm sạch; không trả entity EF từ Web API; mọi API thay đổi dữ liệu phải kiểm tra AntiForgery và quyền.

## 15. NGUYÊN TẮC CUỐI CÙNG

Đây là bài tập lớn sinh viên.

Ưu tiên:

-   Đúng yêu cầu hơn là nhiều chức năng.
-   Đúng nghiệp vụ hơn là giao diện đẹp nhưng không hoạt động.
-   Đúng database hơn là tự thiết kế lại.
-   Code dễ hiểu hơn là kiến trúc phức tạp.
-   Hoàn thành từng phần và kiểm thử kỹ.
-   Giải thích để sinh viên có thể học và bảo vệ.

Antigravity có nhiệm vụ triển khai code. Người dùng là người quyết định kiến trúc, nghiệp vụ và các thay đổi của project. Không được tự ý thay đổi những nội dung đã thống nhất.
