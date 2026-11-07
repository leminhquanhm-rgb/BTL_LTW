# GIAI ĐOẠN 2: PHÂN TÍCH CHỨC NĂNG HỆ THỐNG

**Đề tài:** Xây dựng website quản lý và kinh doanh mỹ phẩm trực tuyến bằng ASP.NET Core MVC (.NET 10)

## 2.1. Mục tiêu

Giai đoạn này tập trung xác định các chức năng của hệ thống, đối tượng sử dụng, quyền thực hiện từng chức năng, mối quan hệ giữa các chức năng và các luồng nghiệp vụ chính.

* Xác định danh sách chức năng đầy đủ của hệ thống.
* Xác định Actor và quyền sử dụng từng chức năng.
* Phân chia chức năng chính, chức năng phụ và chức năng mở rộng.
* Xác định các thao tác CRUD và những nghiệp vụ có quy trình xử lý riêng.
* Mô tả các luồng nghiệp vụ quan trọng làm cơ sở xây dựng sơ đồ Use Case.

## 2.2. Xác định các nhóm chức năng

| Nhóm chức năng | Nội dung |
| --- | --- |
| **1. Quản lý tài khoản và xác thực** | Đăng ký, đăng nhập, đăng xuất, đổi mật khẩu, quên mật khẩu và cập nhật thông tin cá nhân. |
| **2. Tra cứu sản phẩm** | Xem danh sách, xem chi tiết, tìm kiếm và lọc sản phẩm theo giá, danh mục, thương hiệu. |
| **3. Quản lý giỏ hàng** | Xem giỏ hàng, thêm sản phẩm, cập nhật số lượng và xóa sản phẩm. |
| **4. Đặt hàng và thanh toán** | Nhập thông tin nhận hàng, áp dụng mã khuyến mãi, chọn phương thức thanh toán và xác nhận đơn hàng. |
| **5. Quản lý đơn hàng** | Khách hàng theo dõi và hủy đơn theo điều kiện; Admin xem, xác nhận, cập nhật trạng thái, ghi ngày vận đơn và ngày giao hàng thành công. |
| **6. Đánh giá và khiếu nại** | Khách hàng gửi đánh giá, khiếu nại; Admin duyệt đánh giá và xử lý khiếu nại. |
| **7. Quản lý danh mục, thương hiệu và sản phẩm** | Quản lý danh mục, thương hiệu, sản phẩm và hình ảnh sản phẩm. |
| **8. Quản lý tồn kho, thanh toán** | Cập nhật tồn kho, cảnh báo sắp hết hàng, quản lý phương thức thanh toán, giao dịch và hoàn tiền. |
| **9. Quản lý khuyến mãi, banner** | Tạo và quản lý mã khuyến mãi; quản lý banner/slider (chức năng mở rộng). |
| **10. Quản lý khách hàng, tài khoản Admin** | Xem, cập nhật, khóa tài khoản khách hàng; xem, thêm, cập nhật và khóa tài khoản Admin. |
| **11. Dashboard và thống kê** | Hiển thị thông tin tổng quan và báo cáo chi tiết về hoạt động kinh doanh. |

## 2.3. Phân tích chức năng theo từng Actor

### 2.3.1. Guest – Khách vãng lai

| STT | Chức năng | Mô tả |
| --- | --- | --- |
| 1 | Xem danh sách sản phẩm | Hiển thị các sản phẩm đang kinh doanh. |
| 2 | Xem chi tiết sản phẩm | Hiển thị thông tin chi tiết của sản phẩm. |
| 3 | Tìm kiếm sản phẩm | Tìm kiếm theo tên hoặc từ khóa. |
| 4 | Lọc sản phẩm | Lọc theo khoảng giá, danh mục và thương hiệu. |
| 5 | Xem khuyến mãi | Hiển thị chương trình khuyến mãi. |
| 6 | Đăng ký | Tạo tài khoản khách hàng mới. |
| 7 | Đăng nhập | Xác thực tài khoản người dùng. |
| 8 | Quên mật khẩu | Xác minh danh tính và đặt lại mật khẩu. |

### 2.3.2. Customer – Khách hàng

| STT | Nhóm chức năng | Chức năng cụ thể |
| --- | --- | --- |
| 1 | Tài khoản | Xem và cập nhật thông tin cá nhân. |
| 2 | Tài khoản | Đổi mật khẩu. |
| 3 | Tài khoản | Đăng xuất. |
| 4 | Giỏ hàng | Xem giỏ hàng. |
| 5 | Giỏ hàng | Thêm sản phẩm vào giỏ hàng. |
| 6 | Giỏ hàng | Cập nhật số lượng sản phẩm. |
| 7 | Giỏ hàng | Xóa sản phẩm khỏi giỏ hàng. |
| 8 | Đặt hàng | Nhập thông tin nhận hàng. |
| 9 | Đặt hàng | Áp dụng mã khuyến mãi. |
| 10 | Đặt hàng | Chọn phương thức thanh toán. |
| 11 | Đặt hàng | Xác nhận đặt hàng. |
| 12 | Đặt hàng | Mua ngay: đặt hàng trực tiếp một sản phẩm từ trang chi tiết, không thêm vào giỏ. |
| 13 | Đơn hàng | Xem danh sách đơn hàng. |
| 14 | Đơn hàng | Xem chi tiết đơn hàng gồm sản phẩm, thanh toán, ngày đặt, ngày vận đơn. Đơn đã giao thành công thì hiển thị thêm ngày giao hàng thành công. |
| 15 | Đơn hàng | Hủy đơn hàng khi đủ điều kiện. |
| 16 | Đánh giá | Gửi đánh giá sản phẩm đã mua. |
| 17 | Khiếu nại | Gửi khiếu nại về đơn hàng đã Hoàn thành, trong 10 ngày kể từ ngày giao hàng thành công (có thể chọn thêm sản phẩm trong đơn). |
| 18 | Khiếu nại | Theo dõi trạng thái và kết quả xử lý khiếu nại. |
| 19 | Khuyến mãi | Xem chương trình khuyến mãi. |

### 2.3.3. Admin – Quản trị viên

| STT | Nhóm chức năng | Chức năng cụ thể |
| --- | --- | --- |
| 1 | Tài khoản | Đăng nhập, đăng xuất và đổi mật khẩu. |
| 2 | Dashboard | Xem thông tin tổng quan hoạt động kinh doanh. |
| 3 | Danh mục | Thêm, sửa, xóa và tìm kiếm danh mục. |
| 4 | Thương hiệu | Thêm, sửa, xóa và tìm kiếm thương hiệu. |
| 5 | Sản phẩm | Thêm, sửa, ngừng kinh doanh và tìm kiếm sản phẩm. |
| 6 | Hình ảnh | Thêm, thay đổi và xóa hình ảnh sản phẩm. |
| 7 | Tồn kho | Cập nhật số lượng và xem cảnh báo sắp hết hàng. |
| 8 | Đơn hàng | Xem danh sách và chi tiết đơn hàng. |
| 9 | Đơn hàng | Xác nhận và cập nhật trạng thái đơn hàng. |
| 10 | Đơn hàng | Ghi nhận ngày vận đơn khi bàn giao cho đơn vị vận chuyển. |
| 11 | Đơn hàng | Ghi nhận ngày giao hàng thành công khi xác nhận giao thành công. |
| 12 | Thanh toán | Theo dõi giao dịch thanh toán. |
| 13 | Thanh toán | Cập nhật trạng thái hoàn tiền. |
| 14 | Phương thức thanh toán | Thêm, sửa và quản lý phương thức thanh toán. |
| 15 | Khuyến mãi | Tạo, sửa, xóa và quản lý mã khuyến mãi. |
| 16 | Khách hàng | Xem và quản lý tài khoản khách hàng. |
| 17 | Tài khoản Admin | Quản lý tài khoản quản trị viên. |
| 18 | Đánh giá | Duyệt hoặc từ chối đánh giá. |
| 19 | Khiếu nại | Tiếp nhận, phản hồi và cập nhật trạng thái khiếu nại. |
| 20 | Thống kê | Xem báo cáo kinh doanh. |
| 21 | Banner | Thêm, sửa, xóa và quản lý banner/slider. |

## 2.4. Phân loại chức năng CRUD và nghiệp vụ

| Đối tượng | Chức năng CRUD hoặc thao tác | Actor |
| --- | --- | --- |
| Tài khoản khách hàng | Đăng ký, xem, cập nhật và khóa tài khoản | Guest, Customer, Admin |
| Danh mục | Thêm, xem, sửa, xóa | Admin |
| Thương hiệu | Thêm, xem, sửa, xóa | Admin |
| Sản phẩm | Thêm, xem, sửa, ngừng kinh doanh | Admin |
| Hình ảnh sản phẩm | Thêm, xem, sửa, xóa | Admin |
| Giỏ hàng | Thêm, xem, sửa số lượng, xóa | Customer |
| Đơn hàng | Tạo, xem, cập nhật trạng thái, ghi ngày vận đơn, ghi ngày giao thành công, hủy theo điều kiện | Customer, Admin |
| Chi tiết đơn hàng | Tạo và xem | Customer, Admin |
| Khuyến mãi | Thêm, xem, sửa, xóa | Admin |
| Đánh giá | Tạo, xem, duyệt, từ chối | Customer, Admin |
| Khiếu nại | Tạo, xem, phản hồi, cập nhật trạng thái | Customer, Admin |
| Thanh toán | Tạo và theo dõi giao dịch, cập nhật hoàn tiền | Customer, Admin |
| Phương thức thanh toán | Thêm, xem, sửa, quản lý | Admin |
| Banner | Thêm, xem, sửa, xóa | Admin |
| Tài khoản Admin | Xem, thêm, cập nhật và khóa tài khoản | Admin |

*Lưu ý: Không phải mọi đối tượng đều cần đủ bốn thao tác CRUD. Đơn hàng đã phát sinh giao dịch không nên bị xóa tùy tiện; sản phẩm đã được mua nên chuyển sang trạng thái ngừng kinh doanh thay vì xóa vật lý.*

## 2.5. Các luồng nghiệp vụ quan trọng

Các luồng dưới đây mô tả trình tự xử lý ở mức nghiệp vụ. Sơ đồ Use Case tổng quát, các sơ đồ chi tiết và đặc tả Use Case được trình bày tại mục 2.9 và 2.10.

### 2.5.1. Luồng đăng ký và đăng nhập
`Truy cập website` → `Đăng ký nếu chưa có tài khoản` → `Kiểm tra thông tin` → `Tạo tài khoản` → `Nhập thông tin đăng nhập` → `Xác thực tài khoản` → `Kiểm tra trạng thái tài khoản` → `Đăng nhập thành công`
*(Nếu thông tin đăng ký hoặc đăng nhập không hợp lệ, hệ thống thông báo lỗi và yêu cầu người dùng nhập lại.)*

### 2.5.2. Luồng mua hàng và đặt hàng
`Xem sản phẩm` → `Chọn sản phẩm` → `Thêm vào giỏ hàng` → `Xem giỏ hàng` → `Kiểm tra sản phẩm và số lượng` → `Nhập thông tin nhận hàng` → `Áp dụng mã khuyến mãi nếu có` → `Chọn phương thức thanh toán` → `Xác nhận đặt hàng` → `Kiểm tra tồn kho và dữ liệu` → `Tạo đơn hàng và chi tiết đơn hàng` → `Trừ tồn kho` → `Lưu thông tin thanh toán` → `Thông báo đặt hàng thành công`
*(Nếu dữ liệu hoặc tồn kho không hợp lệ, hệ thống thông báo lỗi và yêu cầu khách hàng điều chỉnh trước khi xác nhận đơn hàng.)*

### 2.5.3. Luồng xử lý đơn hàng
`Chờ xác nhận` → `Đã xác nhận` → `Đang giao` *(Admin bàn giao đơn vị vận chuyển và ghi nhận ngày vận đơn)* → `Hoàn thành` *(Admin ghi nhận ngày giao hàng thành công)*.
**Nhánh hủy:** `Chờ xác nhận` → `Đã hủy`. Khách hàng chỉ được hủy đơn khi đơn hàng đang ở trạng thái Chờ xác nhận.

### 2.5.4. Luồng đánh giá sản phẩm
`Khách hàng xem đơn hàng` → `Kiểm tra đơn hàng đã hoàn thành` → `Chọn sản phẩm đã mua` → `Nhập số sao và nhận xét` → `Gửi đánh giá` → `Chờ duyệt` → `Admin kiểm tra` → `Duyệt và hiển thị hoặc từ chối đánh giá`

### 2.5.5. Luồng xử lý khiếu nại
`Khách hàng xem đơn hàng` → `Kiểm tra đơn Hoàn thành và còn trong 10 ngày kể từ ngày giao thành công` → `Chọn sản phẩm trong đơn (không bắt buộc)` → `Nhập nội dung khiếu nại` → `Gửi khiếu nại` → `Hệ thống lưu khiếu nại` → `Chờ xử lý` → `Admin tiếp nhận` → `Đang xử lý` → `Đã giải quyết hoặc Từ chối` → `Khách hàng theo dõi kết quả`
*(Nếu đơn chưa Hoàn thành, đã quá 10 ngày, không thuộc khách hàng, hoặc khách hàng đang có quá 10 khiếu nại chưa xử lý xong, hệ thống từ chối và thông báo lý do.)*

## 2.6. Phân loại mức độ ưu tiên chức năng

| Mức độ | Chức năng |
| --- | --- |
| **Bắt buộc** | Đăng ký, đăng nhập, đăng xuất, phân quyền; đổi mật khẩu, quên mật khẩu; xem, tìm kiếm, lọc sản phẩm; giỏ hàng; đặt hàng; thanh toán COD; hủy đơn khi đủ điều kiện; CRUD danh mục, thương hiệu, sản phẩm; quản lý đơn hàng và ghi nhận ngày vận đơn, ngày giao hàng thành công. |
| **Quan trọng** | Quản lý khuyến mãi; quản lý tồn kho và cảnh báo; quản lý khách hàng và tài khoản Admin; quản lý giao dịch thanh toán; đánh giá, khiếu nại; Dashboard và thống kê. |
| **Mở rộng** | Thanh toán trực tuyến Sandbox; quy trình hoàn tiền tự động (nếu không thực hiện được thì Admin cập nhật thủ công); banner/slider; các báo cáo nâng cao. |

*Các chức năng bắt buộc cần được hoàn thiện trước. Những chức năng mở rộng chỉ triển khai khi các nghiệp vụ cốt lõi đã hoạt động ổn định.*

## 2.7. Kết quả giai đoạn 2
Qua quá trình phân tích, hệ thống đã xác định các Actor, nhóm chức năng, quyền hạn, thao tác CRUD, các luồng nghiệp vụ, sơ đồ Use Case tổng quát, bảy sơ đồ Use Case chi tiết và đặc tả năm Use Case chính.
Như vậy, các nội dung phân tích, sơ đồ và đặc tả chức năng của Giai đoạn 2 đã được hoàn thiện; đây là cơ sở để chuyển sang giai đoạn thiết kế cơ sở dữ liệu.

## 2.8. Các quyết định đã chốt
Các nội dung đã được nhóm thống nhất và chốt phạm vi như sau:
* **Quên mật khẩu:** Áp dụng cho cả Customer và Admin. Người dùng chưa đăng nhập có thể yêu cầu xác minh danh tính và đặt lại mật khẩu.
* **Banner/slider:** Thuộc nhóm chức năng Mở rộng; chỉ triển khai sau khi các nghiệp vụ cốt lõi đã hoàn thiện.
* **Khiếu nại:** Bắt buộc gắn với một đơn hàng của khách hàng ở trạng thái Hoàn thành và chỉ gửi được trong 10 ngày kể từ ngày giao hàng thành công; sản phẩm là tùy chọn và phải thuộc đơn đó. Nội dung từ 10 đến 1000 ký tự; tối đa 10 khiếu nại đang mở mỗi khách hàng. Trường hợp giao trễ được khiếu nại sau khi nhận hàng.

## 2.9. Sơ đồ Use Case
Sơ đồ Use Case tổng quát thể hiện phạm vi chức năng của hệ thống và tương tác giữa ba Actor chính: Guest, Customer và Admin. Cổng thanh toán là tác nhân ngoài, chỉ tham gia khi triển khai thanh toán trực tuyến.

*(Hình 2.1. Sơ đồ Use Case tổng quát)*

### 2.9.1. Use Case quản lý tài khoản và xác thực
*(Hình 2.2. Use Case quản lý tài khoản và xác thực)*

### 2.9.2. Use Case tra cứu sản phẩm và giỏ hàng
*(Hình 2.3. Use Case tra cứu sản phẩm và giỏ hàng)*

### 2.9.3. Use Case đặt hàng và thanh toán
*(Hình 2.4. Use Case đặt hàng và thanh toán)*

### 2.9.4. Use Case quản lý đơn hàng
*(Hình 2.5. Use Case quản lý đơn hàng)*

### 2.9.5. Use Case quản trị danh mục, sản phẩm và tồn kho
*(Hình 2.6. Use Case quản trị danh mục, sản phẩm và tồn kho)*

### 2.9.6. Use Case khuyến mãi, thanh toán, khách hàng, thống kê
*(Hình 2.7. Use Case khuyến mãi, thanh toán, khách hàng, thống kê)*

### 2.9.7. Use Case đánh giá và khiếu nại
*(Hình 2.8. Use Case đánh giá và khiếu nại)*

### 2.9.8. Các quan hệ \<\<include\>\> và \<\<extend\>\>
* **Quan hệ \<\<include\>\> (bắt buộc):**
  * Đặt hàng \<\<include\>\> Nhập thông tin nhận hàng; Chọn phương thức thanh toán; Kiểm tra tồn kho; Tính tổng tiền; Lưu đơn hàng và chi tiết đơn. Đây là các bước luôn phải thực hiện khi đặt hàng.
  * Hủy đơn hàng \<\<include\>\> Hoàn lại tồn kho. Khi hủy hợp lệ, hệ thống luôn hoàn lại số lượng đã trừ, chỉ thực hiện một lần.
  * Gửi đánh giá \<\<include\>\> Kiểm tra đơn hoàn thành / sản phẩm đã mua.
  * Gửi khiếu nại \<\<include\>\> Kiểm tra đơn hoàn thành trong thời hạn khiếu nại (10 ngày kể từ ngày giao hàng thành công).
* **Quan hệ \<\<extend\>\> (tùy chọn):**
  * Áp mã khuyến mãi \<\<extend\>\> Đặt hàng, vì khách hàng có thể đặt hàng mà không cần mã giảm giá.
  * Thanh toán trực tuyến \<\<extend\>\> Đặt hàng, chỉ phát sinh khi Customer chọn thanh toán online và hệ thống triển khai phần mở rộng (có Cổng thanh toán tham gia).
  * Mua ngay \<\<extend\>\> Đặt hàng: khách có thể đặt hàng trực tiếp một sản phẩm mà không qua giỏ hàng; các bước kiểm tra tồn kho, tính tiền, thanh toán vẫn giống Đặt hàng.
* **Quan hệ \<\<extend\>\> (theo điều kiện):**
  * Hủy đơn hàng \<\<extend\>\> Xem chi tiết đơn hàng; chỉ thực hiện được khi đơn ở trạng thái Chờ xác nhận.
  * Ghi ngày vận đơn \<\<extend\>\> Cập nhật trạng thái đơn, khi chuyển sang Đang giao.
  * Ghi ngày giao thành công \<\<extend\>\> Cập nhật trạng thái đơn, khi chuyển sang Hoàn thành.
  * *Lưu ý: ngày vận đơn và ngày giao thành công do hệ thống tự ghi theo thay đổi trạng thái đơn, không tạo thêm trạng thái đơn hàng.*

## 2.10. Đặc tả các Use Case chính

Các đặc tả dưới đây mô tả Actor, điều kiện trước, luồng chính, luồng thay thế/ngoại lệ và điều kiện sau. Tên trạng thái đơn hàng được giữ thống nhất với Giai đoạn 1.

### UC01 – Đặt hàng

| Thuộc tính | Mô tả |
| --- | --- |
| **Actor chính** | Customer |
| **Điều kiện trước** | Customer đã đăng nhập; giỏ hàng có ít nhất một sản phẩm; sản phẩm đang kinh doanh. |
| **Luồng chính** | 1. Customer mở giỏ hàng và chọn chức năng Đặt hàng.<br>2. Hệ thống hiển thị sản phẩm, số lượng, giá hiện tại và tổng tiền tạm tính.<br>3. Customer nhập/chọn thông tin người nhận và địa chỉ giao hàng.<br>4. Customer có thể áp dụng mã khuyến mãi (nếu có).<br>5. Customer chọn phương thức thanh toán; COD là phương thức bắt buộc phải có.<br>6. Customer xác nhận đặt hàng.<br>7. Hệ thống kiểm tra dữ liệu, trạng thái sản phẩm và tồn kho.<br>8. Hệ thống tính tổng tiền, tạo đơn hàng và chi tiết đơn hàng; lưu giá tại thời điểm mua.<br>9. Hệ thống trừ tồn kho trong cùng giao dịch cơ sở dữ liệu và lưu thông tin thanh toán.<br>10. Hệ thống thông báo đặt hàng thành công và hiển thị mã đơn hàng. |
| **Luồng thay thế / ngoại lệ** | • Giỏ hàng rỗng: hệ thống yêu cầu thêm sản phẩm trước khi đặt hàng.<br>• Số lượng vượt tồn kho/sản phẩm ngừng kinh doanh: thông báo lỗi, yêu cầu điều chỉnh giỏ hàng.<br>• Thông tin nhận hàng không hợp lệ: yêu cầu nhập lại.<br>• Mã khuyến mãi không hợp lệ: thông báo lý do; khách hàng có thể bỏ mã và tiếp tục đặt hàng. |
| **Điều kiện sau** | Đơn hàng được tạo ở trạng thái Chờ xác nhận; chi tiết đơn, giá mua, thanh toán và tồn kho được lưu nhất quán. |

### UC02 – Hủy đơn hàng

| Thuộc tính | Mô tả |
| --- | --- |
| **Actor chính** | Customer |
| **Điều kiện trước** | Customer đã đăng nhập và là chủ sở hữu đơn hàng; đơn đang ở trạng thái Chờ xác nhận. |
| **Luồng chính** | 1. Customer mở danh sách đơn hàng của tôi.<br>2. Customer chọn đơn hàng cần hủy và xem chi tiết.<br>3. Customer chọn Hủy đơn hàng và xác nhận thao tác.<br>4. Hệ thống kiểm tra chủ sở hữu và trạng thái hiện tại của đơn.<br>5. Hệ thống cập nhật trạng thái đơn thành Đã hủy.<br>6. Hệ thống hoàn lại số lượng tồn kho đã trừ khi tạo đơn (chỉ thực hiện một lần).<br>7. Hệ thống thông báo kết quả hủy đơn. |
| **Luồng thay thế / ngoại lệ** | • Đơn không thuộc Customer: từ chối thao tác.<br>• Đơn không còn ở trạng thái Chờ xác nhận: không cho phép hủy; thông báo trạng thái hiện tại.<br>• Đơn đã hủy trước đó: không hoàn tồn kho lần thứ hai. |
| **Điều kiện sau** | Đơn chuyển sang Đã hủy; tồn kho được hoàn lại đúng một lần; lịch sử đơn được giữ lại. |

### UC03 – Áp mã khuyến mãi

| Thuộc tính | Mô tả |
| --- | --- |
| **Actor chính** | Customer |
| **Điều kiện trước** | Customer đang thực hiện đặt hàng; mã khuyến mãi tồn tại trong hệ thống. |
| **Luồng chính** | 1. Customer nhập mã khuyến mãi tại màn hình đặt hàng.<br>2. Hệ thống tìm mã và kiểm tra trạng thái hoạt động, thời hạn sử dụng.<br>3. Hệ thống kiểm tra giá trị đơn tối thiểu và giới hạn lượt sử dụng.<br>4. Hệ thống xác định loại giảm giá (theo phần trăm hoặc số tiền cố định).<br>5. Hệ thống tính số tiền giảm, áp dụng mức giảm tối đa nếu có và không để giảm vượt giá trị hàng hóa.<br>6. Hệ thống hiển thị số tiền được giảm và tổng tiền sau giảm. |
| **Luồng thay thế / ngoại lệ** | • Mã không tồn tại, hết hạn, bị khóa hoặc hết lượt: thông báo không áp dụng được.<br>• Đơn chưa đạt giá trị tối thiểu: thông báo điều kiện còn thiếu.<br>• Customer xóa mã: hệ thống tính lại tổng tiền không áp dụng giảm giá. |
| **Điều kiện sau** | Mã hợp lệ được áp dụng cho đơn hiện tại; mã và số tiền giảm thực tế được lưu cùng đơn khi đơn được tạo. |

### UC04 – Cập nhật trạng thái đơn hàng

| Thuộc tính | Mô tả |
| --- | --- |
| **Actor chính** | Admin |
| **Điều kiện trước** | Admin đã đăng nhập và có quyền quản lý đơn hàng; đơn hàng tồn tại. |
| **Luồng chính** | 1. Admin mở danh sách đơn hàng và chọn một đơn.<br>2. Admin xem chi tiết đơn và trạng thái hiện tại.<br>3. Admin xác nhận đơn: hệ thống chuyển Chờ xác nhận thành Đã xác nhận.<br>4. Khi bàn giao cho đơn vị vận chuyển, Admin chuyển đơn sang Đang giao; hệ thống tự ghi ngày vận đơn (và Admin có thể nhập mã vận đơn).<br>5. Khi xác nhận giao hàng thành công, Admin chuyển đơn sang Hoàn thành; hệ thống tự ghi ngày giao hàng thành công.<br>6. Hệ thống kiểm tra thứ tự trạng thái hợp lệ trước khi lưu.<br>7. Hệ thống cập nhật đơn hàng và hiển thị kết quả. |
| **Luồng thay thế / ngoại lệ** | • Không đúng thứ tự trạng thái: từ chối cập nhật.<br>• Đơn Đã hủy: không cho phép chuyển sang Đang giao hoặc Hoàn thành. |
| **Điều kiện sau** | Đơn có một trong bốn trạng thái Chờ xác nhận, Đã xác nhận, Đang giao, Hoàn thành; ngày vận đơn và ngày giao thành công được ghi nhận đúng thời điểm. |

### UC05 – Đánh giá sản phẩm

| Thuộc tính | Mô tả |
| --- | --- |
| **Actor chính** | Customer |
| **Điều kiện trước** | Customer đã đăng nhập; có đơn hàng ở trạng thái Hoàn thành và sản phẩm được đánh giá thuộc đơn đó. |
| **Luồng chính** | 1. Customer mở đơn hàng đã hoàn thành.<br>2. Customer chọn sản phẩm đã mua và chức năng Đánh giá.<br>3. Customer nhập số sao từ 1 đến 5 và nội dung nhận xét (không bắt buộc).<br>4. Customer gửi đánh giá.<br>5. Hệ thống kiểm tra đơn đã hoàn thành, quyền sở hữu và sản phẩm thuộc chi tiết đơn.<br>6. Hệ thống lưu đánh giá ở trạng thái Chờ duyệt.<br>7. Admin kiểm tra và duyệt hoặc từ chối đánh giá.<br>8. Nếu được duyệt, đánh giá được hiển thị công khai tại sản phẩm. |
| **Luồng thay thế / ngoại lệ** | • Đơn chưa hoàn thành hoặc sản phẩm không thuộc đơn: từ chối đánh giá.<br>• Số sao ngoài khoảng 1–5: yêu cầu nhập lại.<br>• Đã có đánh giá cho cùng sản phẩm trong cùng đơn: không cho tạo trùng.<br>• Admin từ chối: đánh giá không hiển thị công khai. |
| **Điều kiện sau** | Đánh giá được lưu; chỉ đánh giá được Admin duyệt mới xuất hiện công khai. |

### 2.10.6. Quy tắc nhất quán khi triển khai
* Trạng thái đơn hàng chỉ gồm bốn trạng thái chính: Chờ xác nhận, Đã xác nhận, Đang giao, Hoàn thành; nhánh hủy là Đã hủy.
* Bàn giao đơn vị vận chuyển là hành động nghiệp vụ khi chuyển đơn sang Đang giao, không phải một trạng thái độc lập.
* Ngày vận đơn được ghi khi chuyển sang Đang giao; ngày giao hàng thành công được ghi khi chuyển sang Hoàn thành.
* COD là phương thức thanh toán bắt buộc phải có trong hệ thống. Thanh toán trực tuyến chỉ là phần mở rộng.
* Quên mật khẩu áp dụng cho Customer và Admin. Banner/slider thuộc nhóm Mở rộng.

### Bảng tổng hợp quan hệ giữa các Use Case

| Use Case gốc | Use Case liên quan | Quan hệ | Giải thích |
| --- | --- | --- | --- |
| Đặt hàng | Kiểm tra tồn kho | \<\<include\>\> | Luôn phải kiểm tra số lượng tồn trước khi tạo đơn. |
| Đặt hàng | Tính tổng tiền | \<\<include\>\> | Luôn phải tính tiền hàng, giảm giá (nếu có) và tổng thanh toán. |
| Đặt hàng | Lưu đơn hàng và chi tiết đơn | \<\<include\>\> | Luôn lưu đơn và các dòng sản phẩm sau khi xác nhận hợp lệ. |
| Áp mã khuyến mãi | Đặt hàng | \<\<extend\>\> | Chỉ phát sinh khi Customer tự nguyện nhập mã; có thể đặt hàng không dùng mã. |
| Hủy đơn hàng | Xem chi tiết đơn hàng | \<\<extend\>\> | Chỉ được thực hiện khi đơn đang ở trạng thái Chờ xác nhận. |
| Gửi đánh giá | Kiểm tra đơn hoàn thành / sản phẩm đã mua | \<\<include\>\> | Phải xác minh đơn đã Hoàn thành và sản phẩm thuộc đơn của Customer. |
| Gửi khiếu nại | Kiểm tra đơn hoàn thành trong thời hạn khiếu nại | \<\<include\>\> | Phải xác minh đơn thuộc Customer, đã Hoàn thành và chưa quá 10 ngày kể từ ngày giao hàng thành công. |
| Đặt hàng | Nhập thông tin nhận hàng | \<\<include\>\> | Luôn phải có thông tin người nhận và địa chỉ giao hàng. |
| Đặt hàng | Chọn phương thức thanh toán | \<\<include\>\> | Luôn phải chọn phương thức thanh toán (COD là bắt buộc phải có). |
| Mua ngay | Đặt hàng | \<\<extend\>\> | Đặt trực tiếp một sản phẩm, không dùng giỏ hàng; giỏ hàng giữ nguyên. |
| Thanh toán trực tuyến | Đặt hàng | \<\<extend\>\> | Chỉ phát sinh khi Customer chọn thanh toán online và hệ thống triển khai phần mở rộng; có Cổng thanh toán tham gia. |
| Hủy đơn hàng | Hoàn lại tồn kho | \<\<include\>\> | Khi hủy hợp lệ, luôn hoàn lại số lượng đã trừ, chỉ thực hiện một lần. |
| Ghi ngày vận đơn | Cập nhật trạng thái đơn | \<\<extend\>\> | Hệ thống tự ghi khi đơn chuyển sang Đang giao. |
| Ghi ngày giao thành công | Cập nhật trạng thái đơn | \<\<extend\>\> | Hệ thống tự ghi khi đơn chuyển sang Hoàn thành. |