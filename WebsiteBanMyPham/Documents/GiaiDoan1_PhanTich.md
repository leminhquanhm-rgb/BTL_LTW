# GIAI ĐOẠN 1: PHÂN TÍCH ĐỀ TÀI

## 1.1. Giới thiệu đề tài
**Tên đề tài:** Xây dựng website quản lý và kinh doanh mỹ phẩm trực tuyến bằng ASP.NET Core MVC (.NET 10).

### 1.1.1. Mục tiêu đề tài
Đề tài xây dựng một website kinh doanh và quản lý mỹ phẩm trực tuyến, giúp khách hàng dễ dàng tìm kiếm, lựa chọn và đặt mua các sản phẩm mỹ phẩm thông qua Internet. Đồng thời, hệ thống hỗ trợ quản trị viên quản lý hoạt động kinh doanh của cửa hàng, bao gồm quản lý sản phẩm, danh mục, thương hiệu, đơn hàng, khách hàng, khuyến mãi và các nghiệp vụ liên quan.

Hệ thống hướng đến các mục tiêu chính:
* Xây dựng giao diện thân thiện, dễ sử dụng cho khách hàng.
* Hỗ trợ khách hàng tìm kiếm, xem thông tin và đặt mua sản phẩm trực tuyến.
* Hỗ trợ quản lý giỏ hàng, đơn hàng, thanh toán và đánh giá sản phẩm.
* Hỗ trợ quản trị viên quản lý dữ liệu và theo dõi hoạt động kinh doanh.
* Hỗ trợ thống kê doanh thu, đơn hàng và tình hình kinh doanh của cửa hàng.
* Đảm bảo dữ liệu được lưu trữ tập trung, nhất quán và dễ quản lý.

### 1.1.2. Phạm vi đề tài
* Hệ thống được xây dựng phục vụ hoạt động kinh doanh của một cửa hàng mỹ phẩm duy nhất, chuyên cung cấp các sản phẩm mỹ phẩm chính hãng.
* Website không hoạt động theo mô hình sàn thương mại điện tử nhiều nhà bán hàng. Toàn bộ sản phẩm, giá bán, tồn kho, đơn hàng và chương trình khuyến mãi đều do cửa hàng quản lý.
* Hệ thống gồm hai khu vực chính:
    * **Khu vực khách hàng:** Cho phép khách truy cập xem sản phẩm, tìm kiếm, đăng ký tài khoản, đăng nhập, quản lý giỏ hàng, đặt hàng, thanh toán và theo dõi đơn hàng.
    * **Khu vực quản trị:** Cho phép quản trị viên quản lý sản phẩm, danh mục, thương hiệu, khách hàng, đơn hàng, thanh toán, khuyến mãi, đánh giá, khiếu nại và thống kê.
* Website được phát triển trên nền tảng ASP.NET Core MVC với C# và .NET 10, sử dụng SQL Server để lưu trữ dữ liệu.

## 1.2. Đối tượng sử dụng hệ thống (Actor)
Hệ thống có ba đối tượng sử dụng chính:

| Actor | Mô tả |
| :--- | :--- |
| **Guest (Khách vãng lai)** | Người chưa đăng nhập, được phép xem, tìm kiếm, lọc sản phẩm, đăng ký tài khoản, đăng nhập và thực hiện chức năng quên mật khẩu. |
| **Customer (Khách hàng)** | Người đã đăng nhập, được phép quản lý thông tin cá nhân, giỏ hàng, đặt hàng, thanh toán, theo dõi đơn hàng, đánh giá sản phẩm và gửi khiếu nại. |
| **Admin (Quản trị viên)** | Người quản lý hệ thống, được phép quản lý sản phẩm, danh mục, thương hiệu, đơn hàng, khách hàng, khuyến mãi, đánh giá, khiếu nại và xem thống kê. |

Ngoài các đối tượng trên, nếu triển khai thanh toán trực tuyến, hệ thống có thể tương tác với cổng thanh toán bên ngoài để xử lý giao dịch.

## 1.3. Phân tích chức năng hệ thống

### 1.3.1. Chức năng dành cho khách vãng lai và khách hàng

| STT | Chức năng | Mô tả |
| :--- | :--- | :--- |
| 1 | Xem sản phẩm | Xem danh sách sản phẩm, thông tin chi tiết, hình ảnh và giá bán. |
| 2 | Tìm kiếm sản phẩm | Tìm kiếm sản phẩm theo tên hoặc từ khóa. |
| 3 | Lọc sản phẩm | Lọc sản phẩm theo khoảng giá, danh mục và thương hiệu. |
| 4 | Đăng ký tài khoản | Tạo tài khoản khách hàng mới. |
| 5 | Đăng nhập | Đăng nhập vào hệ thống bằng tài khoản đã đăng ký. |
| 6 | Đăng xuất | Đăng xuất khỏi tài khoản hiện tại. |
| 7 | Đổi mật khẩu | Thay đổi mật khẩu tài khoản sau khi đăng nhập. |
| 8 | Quên mật khẩu | Thực hiện xác minh danh tính và đặt lại mật khẩu khi không nhớ mật khẩu hiện tại. |
| 9 | Quản lý thông tin cá nhân | Xem và cập nhật thông tin cá nhân. |
| 10 | Thêm sản phẩm vào giỏ hàng | Thêm sản phẩm muốn mua vào giỏ hàng. |
| 11 | Quản lý giỏ hàng | Thay đổi số lượng hoặc xóa sản phẩm khỏi giỏ hàng. |
| 12 | Đặt hàng | Nhập thông tin người nhận và xác nhận đặt hàng. |
| 13 | Áp dụng mã khuyến mãi | Nhập mã khuyến mãi để được giảm giá khi đặt hàng. |
| 14 | Chọn phương thức thanh toán | Lựa chọn phương thức thanh toán phù hợp. |
| 15 | Thanh toán | Thực hiện thanh toán theo phương thức đã chọn. |
| 16 | Quản lý đơn hàng cá nhân | Xem lịch sử, chi tiết và trạng thái đơn hàng. |
| 17 | Hủy đơn hàng | Hủy đơn hàng khi đáp ứng điều kiện được phép hủy. |
| 18 | Đánh giá sản phẩm | Gửi đánh giá và nhận xét về sản phẩm đã mua. |
| 19 | Gửi khiếu nại | Gửi khiếu nại về một đơn hàng đã giao thành công (có thể chọn thêm sản phẩm trong đơn), trong thời hạn 10 ngày kể từ ngày giao hàng thành công. |
| 20 | Theo dõi khiếu nại | Xem trạng thái và nội dung phản hồi khiếu nại. |
| 21 | Xem khuyến mãi | Xem các chương trình và mã khuyến mãi đang được áp dụng. |

### 1.3.2. Chức năng dành cho quản trị viên

| STT | Chức năng | Mô tả |
| :--- | :--- | :--- |
| 1 | Dashboard | Hiển thị tổng quan hoạt động kinh doanh như doanh thu, đơn hàng, khách hàng và sản phẩm. |
| 2 | Quản lý danh mục | Thêm, sửa, xóa và tìm kiếm danh mục sản phẩm. |
| 3 | Quản lý thương hiệu | Thêm, sửa, xóa và tìm kiếm thương hiệu mỹ phẩm. |
| 4 | Quản lý sản phẩm | Thêm, sửa, ngừng kinh doanh và tìm kiếm sản phẩm. |
| 5 | Quản lý hình ảnh sản phẩm | Thêm, thay đổi và quản lý hình ảnh của sản phẩm. |
| 6 | Quản lý tồn kho | Cập nhật số lượng tồn kho và theo dõi cảnh báo sản phẩm sắp hết hàng. |
| 7 | Quản lý đơn hàng | Xem danh sách, chi tiết và xử lý đơn hàng của khách hàng. |
| 8 | Cập nhật trạng thái đơn hàng | Thay đổi trạng thái đơn hàng theo quy trình xử lý. |
| 9 | Quản lý giao dịch thanh toán | Theo dõi giao dịch, kiểm tra trạng thái thanh toán và cập nhật trạng thái hoàn tiền. |
| 10 | Quản lý phương thức thanh toán | Xem danh sách, sửa mô tả và bật/tắt phương thức thanh toán (COD luôn bật; không thêm phương thức mới). |
| 11 | Quản lý khuyến mãi | Tạo, sửa, xóa và quản lý mã khuyến mãi. |
| 12 | Quản lý khách hàng | Xem thông tin, cập nhật trạng thái và quản lý tài khoản khách hàng. |
| 13 | Quản lý tài khoản Admin | Không quản lý trên web; tài khoản Admin được tạo trực tiếp trong CSDL khi triển khai để tránh Admin tự khóa chính mình hoặc mất quyền quản trị. |
| 14 | Quản lý đánh giá | Kiểm tra, duyệt hoặc từ chối đánh giá của khách hàng. |
| 15 | Quản lý khiếu nại | Tiếp nhận, phản hồi và cập nhật trạng thái xử lý khiếu nại. |
| 16 | Thống kê, báo cáo | Thống kê doanh thu, đơn hàng, khách hàng, sản phẩm bán chạy và các số liệu kinh doanh. |
| 17 | Quản lý banner/slider | Thêm, sửa, xóa và quản lý banner hiển thị trên website. |

## 1.4. Bảng phân quyền hệ thống
**Ký hiệu:**
* **✓:** Được phép thực hiện.
* **—:** Không được phép thực hiện.

| Chức năng | Guest | Customer | Admin |
| :--- | :---: | :---: | :---: |
| Xem danh sách và chi tiết sản phẩm | ✓ | ✓ | ✓ |
| Tìm kiếm, lọc sản phẩm | ✓ | ✓ | ✓ |
| Đăng ký tài khoản | ✓ | — | — |
| Đăng nhập | ✓ | — | ✓ |
| Đăng xuất | — | ✓ | ✓ |
| Đổi mật khẩu | — | ✓ | ✓ |
| Quên mật khẩu | ✓ | — | — |
| Xem, cập nhật thông tin cá nhân | — | ✓ | ✓ |
| Quản lý giỏ hàng | — | ✓ | — |
| Đặt hàng | — | ✓ | — |
| Áp dụng mã khuyến mãi | — | ✓ | — |
| Thanh toán | — | ✓ | — |
| Xem lịch sử và chi tiết đơn hàng | — | ✓ | ✓ |
| Hủy đơn hàng theo điều kiện | — | ✓ | — |
| Đánh giá sản phẩm | — | ✓ | — |
| Gửi khiếu nại | — | ✓ | — |
| Theo dõi xử lý khiếu nại | — | ✓ | — |
| Xem chương trình khuyến mãi | ✓ | ✓ | ✓ |
| Quản lý danh mục | — | — | ✓ |
| Quản lý thương hiệu | — | — | ✓ |
| Quản lý sản phẩm và hình ảnh | — | — | ✓ |
| Cập nhật số lượng tồn kho | — | — | ✓ |
| Quản lý và xử lý đơn hàng | — | — | ✓ |
| Quản lý giao dịch thanh toán | — | — | ✓ |
| Quản lý phương thức thanh toán | — | — | ✓ |
| Quản lý chương trình khuyến mãi | — | — | ✓ |
| Quản lý tài khoản khách hàng | — | — | ✓ |
| Quản lý tài khoản Admin (không làm trên web, tài khoản Admin tạo trong CSDL khi triển khai) | — | — | — |
| Duyệt đánh giá sản phẩm | — | — | ✓ |
| Xử lý khiếu nại | — | — | ✓ |
| Xem Dashboard | — | — | ✓ |
| Xem báo cáo thống kê | — | — | ✓ |
| Quản lý banner/slider | — | — | ✓ |

*Lưu ý: Admin có quyền xem và xử lý đơn hàng của khách hàng nhưng không thực hiện nghiệp vụ mua hàng với tư cách Customer. Chức năng đăng nhập được sử dụng bởi cả Guest và Admin; sau khi đăng nhập, hệ thống xác định vai trò để điều hướng và phân quyền phù hợp.*

## 1.5. Các quy tắc nghiệp vụ

### 1.5.1. Quy tắc tài khoản
* Guest được phép xem, tìm kiếm và lọc sản phẩm mà không cần đăng nhập.
* Người dùng phải đăng nhập mới được đặt hàng và sử dụng các chức năng dành riêng cho khách hàng.
* Email đăng ký tài khoản phải là duy nhất trong hệ thống.
* Mật khẩu phải được mã hóa bằng thuật toán băm an toàn trước khi lưu vào cơ sở dữ liệu.
* Tài khoản bị khóa không được phép đăng nhập.
* Đổi mật khẩu yêu cầu người dùng cung cấp mật khẩu hiện tại.
* Quên mật khẩu yêu cầu xác minh danh tính trước khi đặt mật khẩu mới.

### 1.5.2. Quy tắc sản phẩm và tồn kho
* Mỗi sản phẩm thuộc một danh mục và một thương hiệu.
* Giá bán và số lượng tồn kho không được nhỏ hơn 0.
* Sản phẩm ngừng kinh doanh không được thêm vào giỏ hàng hoặc đặt mua.
* Sản phẩm đang bán được định nghĩa là SanPham.TrangThai = 1, DanhMuc.TrangThai = 1 và ThuongHieu.TrangThai = 1. Khi danh mục hoặc thương hiệu ngừng hoạt động, mọi sản phẩm thuộc danh mục/thương hiệu đó được đối xử tương tự sản phẩm ngừng kinh doanh ở phía khách hàng.
* Không xóa vật lý sản phẩm đã phát sinh đơn hàng; thay vào đó, chuyển trạng thái sang ngừng kinh doanh để bảo đảm lịch sử đơn hàng.
* Admin có quyền cập nhật số lượng tồn kho.
* Khi số lượng tồn kho thấp hơn hoặc bằng ngưỡng cảnh báo, hệ thống hiển thị cảnh báo cho Admin.
* Hệ thống trừ số lượng tồn kho ngay khi đơn hàng được tạo thành công. Khi đơn hàng được hủy hợp lệ, hệ thống hoàn lại số lượng sản phẩm tương ứng vào kho.
* Việc trừ và hoàn tồn kho phải được thực hiện chính xác, tránh trừ hoặc hoàn số lượng nhiều lần đối với cùng một đơn hàng.

### 1.5.3. Quy tắc giỏ hàng
* Chỉ Customer đã đăng nhập mới được sử dụng giỏ hàng.
* Giỏ hàng được lưu trong cơ sở dữ liệu và gắn với tài khoản khách hàng. Khi khách hàng đăng xuất rồi đăng nhập lại, thông tin giỏ hàng vẫn được giữ nguyên.
* Khách hàng được thêm sản phẩm, thay đổi số lượng và xóa sản phẩm khỏi giỏ hàng.
* Số lượng sản phẩm trong giỏ phải lớn hơn 0.
* Sản phẩm phải đang kinh doanh và còn hàng mới được phép đặt mua.
* Khi đặt hàng, hệ thống phải kiểm tra lại số lượng tồn kho để tránh trường hợp sản phẩm đã hết hàng sau khi được thêm vào giỏ.
* Giỏ hàng hiển thị giá bán hiện tại của sản phẩm. Giá sản phẩm được cố định tại thời điểm tạo đơn hàng và lưu trong chi tiết đơn hàng.

### 1.5.4. Quy tắc đơn hàng
* Khách hàng phải đăng nhập mới được đặt hàng.
* Khi đặt hàng, hệ thống kiểm tra thông tin người nhận, sản phẩm, số lượng tồn kho và mã khuyến mãi.
* Một đơn hàng gồm thông tin chung của đơn hàng và danh sách chi tiết sản phẩm.
* Chi tiết đơn hàng phải lưu tên sản phẩm và đơn giá tại thời điểm mua. Khi Admin thay đổi tên hoặc giá bán của sản phẩm trong tương lai, thông tin đơn hàng cũ vẫn giữ nguyên giá trị tại thời điểm giao dịch.
* Tổng tiền hàng được tính từ số lượng và đơn giá của từng sản phẩm trong chi tiết đơn hàng.
* Hệ thống tính số tiền giảm giá và tổng tiền thanh toán trước khi xác nhận đơn hàng.
* Khi tạo đơn hàng thành công, hệ thống lưu thông tin đơn hàng, chi tiết đơn hàng và thông tin thanh toán tương ứng.
* Các thao tác tạo đơn hàng và trừ tồn kho phải được thực hiện trong một giao dịch cơ sở dữ liệu để bảo đảm tính nhất quán dữ liệu.
* **Trạng thái đơn hàng:**
  * Chờ xác nhận → Đã xác nhận → Đang giao → Hoàn thành.
  * Nhánh hủy: Chờ xác nhận → Đã hủy.
* Khách hàng chỉ được phép hủy đơn khi đơn hàng đang ở trạng thái Chờ xác nhận. Admin có quyền cập nhật trạng thái đơn hàng theo quy trình xử lý.

### 1.5.5. Quy tắc thanh toán
* COD (thanh toán khi nhận hàng) là phương thức thanh toán bắt buộc phải có trong phiên bản đầu tiên.
* Thanh toán trực tuyến được mô phỏng hoặc tích hợp môi trường Sandbox nếu còn thời gian thực hiện.
* Đối với COD, trạng thái thanh toán được cập nhật theo quá trình nhận và xác nhận thanh toán.
* Đối với thanh toán trực tuyến, giao dịch chỉ được ghi nhận là thành công khi hệ thống nhận được kết quả xác nhận thành công.
* Đối với đơn hàng đã thanh toán trực tuyến nhưng bị hủy hợp lệ, Admin thực hiện hoàn tiền thủ công và cập nhật trạng thái hoàn tiền trên hệ thống. Hệ thống lưu lại trạng thái giao dịch và trạng thái hoàn tiền để phục vụ việc theo dõi, đối soát.
* Hệ thống lưu thông tin phương thức thanh toán, số tiền, thời gian và trạng thái giao dịch.

### 1.5.6. Quy tắc khuyến mãi
* Mỗi chương trình khuyến mãi có mã riêng, tên chương trình, loại giảm giá, giá trị giảm, thời gian bắt đầu, thời gian kết thúc và trạng thái hoạt động.
* Khuyến mãi có hai loại:
  * Giảm theo phần trăm (%).
  * Giảm theo số tiền cố định (VNĐ).
* Mỗi chương trình khuyến mãi có thể quy định mức giảm tối đa.
* Khách hàng chỉ được sử dụng mã khuyến mãi còn hiệu lực, đạt điều kiện đơn hàng tối thiểu và chưa vượt quá giới hạn sử dụng.
* Mỗi đơn hàng chỉ được áp dụng tối đa một mã khuyến mãi.
* Số tiền giảm không vượt mức giảm tối đa và không vượt tổng tiền hàng.
* Thông tin mã khuyến mãi và số tiền giảm thực tế phải được lưu trong đơn hàng để bảo đảm lịch sử giao dịch không thay đổi khi Admin chỉnh sửa chương trình khuyến mãi.

### 1.5.7. Quy tắc đánh giá sản phẩm
* Chỉ khách hàng đã mua sản phẩm và có đơn hàng ở trạng thái Hoàn thành mới được phép đánh giá sản phẩm đó.
* Mỗi sản phẩm chỉ được đánh giá một lần trong cùng một đơn hàng.
* Điểm đánh giá nằm trong khoảng từ 1 đến 5 sao.
* Khách hàng có thể nhập nội dung nhận xét kèm theo đánh giá.
* Đánh giá sau khi gửi sẽ ở trạng thái Chờ duyệt. Admin kiểm tra và duyệt trước khi đánh giá được hiển thị công khai.
* Admin có quyền từ chối hoặc ẩn đánh giá không phù hợp, kể cả đánh giá đã duyệt; đánh giá đã từ chối có thể được duyệt lại. Đánh giá không quay về trạng thái Chờ duyệt, Admin không sửa nội dung và không xóa đánh giá.

### 1.5.8. Quy tắc khiếu nại
* Khách hàng đã đăng nhập mới được phép gửi khiếu nại.
* Chỉ khách hàng đã mua và đã nhận hàng mới được khiếu nại. Mỗi khiếu nại bắt buộc gắn với một đơn hàng của chính khách hàng đó; khách hàng có thể chọn thêm một sản phẩm, khi đó sản phẩm phải nằm trong chính đơn hàng được chọn.
* Đơn hàng được khiếu nại phải ở trạng thái Hoàn thành (đã giao hàng thành công). Đơn Chờ xác nhận, Đã xác nhận, Đang giao hoặc Đã hủy không được khiếu nại.
* Thời hạn khiếu nại là 10 ngày kể từ ngày giao hàng thành công. Quá thời hạn này, hệ thống không cho phép gửi khiếu nại mới cho đơn hàng đó.
* Trường hợp giao hàng trễ, khách hàng gửi khiếu nại sau khi nhận hàng, trong thời hạn 10 ngày nêu trên.
* Nội dung khiếu nại có độ dài từ 10 đến 1000 ký tự.
* Mỗi khách hàng có tối đa 10 khiếu nại đang mở (Chờ xử lý hoặc Đang xử lý) cùng lúc, nhằm hạn chế gửi khiếu nại hàng loạt.
* Khách hàng được xem nội dung và trạng thái xử lý khiếu nại của mình; không xem được thông tin cá nhân của quản trị viên xử lý.
* Admin có quyền tiếp nhận, phản hồi và cập nhật trạng thái khiếu nại.
* **Các trạng thái khiếu nại:**
  * Chờ xử lý.
  * Đang xử lý.
  * Đã giải quyết.
  * Từ chối.
* **Luồng xử lý thông thường:**
  * Chờ xử lý → Đang xử lý → Đã giải quyết.
* **Trường hợp xử lý nhanh hoặc không đủ điều kiện:**
  * Chờ xử lý → Đã giải quyết (giải quyết ngay, không cần bước tiếp nhận).
  * Chờ xử lý hoặc Đang xử lý → Từ chối.
* Khi chuyển sang Đã giải quyết hoặc Từ chối, Admin bắt buộc nhập phản hồi từ 10 đến 1000 ký tự. Đã giải quyết và Từ chối là trạng thái cuối, không thay đổi được nữa.

### 1.5.9. Quy tắc thống kê
* Admin có quyền xem các thông tin thống kê:
  * Tổng doanh thu.
  * Tổng số đơn hàng.
  * Tổng số khách hàng.
  * Doanh thu theo tháng.
  * Số lượng đơn hàng theo trạng thái.
  * Sản phẩm bán chạy.
  * Sản phẩm sắp hết hàng.
* Doanh thu được tính theo những đơn hàng đã hoàn thành và đáp ứng điều kiện ghi nhận doanh thu, không tính tất cả đơn hàng vừa được tạo.

## 1.6. Kết luận giai đoạn 1
Qua quá trình phân tích, đề tài đã xác định được mục tiêu, phạm vi, đối tượng sử dụng, các chức năng chính, quyền hạn của từng đối tượng và những quy tắc nghiệp vụ cơ bản của hệ thống website kinh doanh mỹ phẩm trực tuyến.

Hệ thống tập trung giải quyết hai nhóm nghiệp vụ chính: hỗ trợ khách hàng mua sắm trực tuyến và hỗ trợ quản trị viên quản lý hoạt động kinh doanh của cửa hàng.

Các nội dung phân tích ở giai đoạn này là cơ sở để thực hiện phân tích chức năng, xây dựng sơ đồ Use Case và xác định các yêu cầu chi tiết của hệ thống ở giai đoạn tiếp theo.