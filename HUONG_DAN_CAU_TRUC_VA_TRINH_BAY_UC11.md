# Hướng dẫn cấu trúc và trình bày code UC11

## 1. Thông tin chung

- **Use Case:** UC11 - Chấm điểm sinh viên thực tập.
- **Tác nhân chính:** Giảng viên hướng dẫn.
- **Công nghệ:** C#, ASP.NET Core Razor Pages, HTML, CSS và JavaScript.
- **Kiểu lưu dữ liệu:** Danh sách trong bộ nhớ (in-memory), không sử dụng database.
- **Project chạy chính:** `ChamDiem/ChamDiem.csproj`.

Ứng dụng cho phép giảng viên:

1. Xem, tìm kiếm và lọc sinh viên được phân công.
2. Xem hồ sơ, báo cáo và đánh giá từ doanh nghiệp.
3. Nhập ba đầu điểm.
4. Tự động tính điểm tổng kết và xếp loại.
5. Lưu nháp khi dữ liệu chưa đầy đủ.
6. Xác nhận điểm và khóa phiếu chấm.

---

## 2. Cấu trúc thư mục project

```text
Usecase11_MaiNamQuoc/
├── README.md
├── HUONG_DAN_CAU_TRUC_VA_TRINH_BAY_UC11.md
├── Usecase11_MaiNamQuoc.slnx
└── ChamDiem/
    ├── ChamDiem.csproj
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Models/
    │   ├── Student.cs
    │   └── GradingInput.cs
    ├── Services/
    │   └── StudentService.cs
    ├── Pages/
    │   ├── Index.cshtml
    │   ├── Index.cshtml.cs
    │   ├── _ViewImports.cshtml
    │   ├── _ViewStart.cshtml
    │   ├── Error.cshtml
    │   ├── Error.cshtml.cs
    │   ├── Privacy.cshtml
    │   ├── Privacy.cshtml.cs
    │   └── Shared/
    │       ├── _Layout.cshtml
    │       ├── _Layout.cshtml.css
    │       └── _ValidationScriptsPartial.cshtml
    ├── Properties/
    │   └── launchSettings.json
    ├── wwwroot/
    │   ├── css/site.css
    │   ├── js/site.js
    │   ├── favicon.ico
    │   └── lib/
    │       ├── bootstrap/
    │       ├── jquery/
    │       ├── jquery-validation/
    │       └── jquery-validation-unobtrusive/
    ├── bin/
    └── obj/
```

---

## 3. Vai trò của từng file và thư mục

### 3.1. File ở thư mục gốc

#### `Usecase11_MaiNamQuoc.slnx`

- Là file solution của Visual Studio/.NET.
- Gom project `ChamDiem` vào một solution để mở, build và chạy thuận tiện.
- Có thể build bằng lệnh:

```powershell
dotnet build .\Usecase11_MaiNamQuoc.slnx
```

#### `README.md`

- Hướng dẫn ngắn gọn cách chạy project.
- Ghi lại các chức năng đã triển khai và bộ điểm dùng để demo.

#### `HUONG_DAN_CAU_TRUC_VA_TRINH_BAY_UC11.md`

- Là tài liệu hiện tại.
- Dùng để ôn cấu trúc code, luồng xử lý và chuẩn bị trả lời khi bảo vệ.

### 3.2. File cấu hình project

#### `ChamDiem/ChamDiem.csproj`

Vai trò:

- Khai báo đây là project web bằng `Microsoft.NET.Sdk.Web`.
- Khai báo target framework là `net10.0`.
- Bật nullable reference types bằng `Nullable`.
- Bật implicit using để giảm các câu lệnh `using` phổ biến.

Project không cần cài package ngoài vì Razor Pages, Dependency Injection, Model Binding và Validation đã có trong ASP.NET Core.

#### `ChamDiem/Program.cs`

Đây là điểm khởi động của ứng dụng.

Các nhiệm vụ chính:

1. Tạo `WebApplicationBuilder`.
2. Đăng ký Razor Pages bằng `AddRazorPages()`.
3. Đăng ký `StudentService` theo vòng đời `Singleton`.
4. Cấu hình xử lý lỗi, HTTPS, static files, routing và authorization.
5. Ánh xạ các Razor Page bằng `MapRazorPages()`.
6. Khởi động ứng dụng bằng `Run()`.

Dòng quan trọng:

```csharp
builder.Services.AddSingleton<StudentService>();
```

`Singleton` đảm bảo chỉ có một `StudentService` trong suốt thời gian ứng dụng chạy. Vì vậy, điểm lưu nháp hoặc điểm đã xác nhận vẫn được giữ khi người dùng chuyển trang. Khi tắt và chạy lại ứng dụng, dữ liệu trở về dữ liệu mẫu ban đầu.

#### `appsettings.json`

- Chứa cấu hình chung của ứng dụng.
- Project demo hiện không có kết nối database hoặc khóa bí mật.
- Có thể mở rộng để chứa cấu hình logging, chuỗi kết nối hoặc tùy chọn hệ thống sau này.

#### `appsettings.Development.json`

- Chứa cấu hình riêng cho môi trường phát triển.
- Thường dùng để điều chỉnh mức logging khi chạy trên máy lập trình viên.

#### `Properties/launchSettings.json`

- Cấu hình profile chạy từ Visual Studio hoặc `dotnet run`.
- Khai báo URL mặc định:
  - `http://localhost:5129`
  - `https://localhost:7148`
- Đặt môi trường chạy là `Development`.

### 3.3. Thư mục `Models`

#### `Models/Student.cs`

File này chứa các model nghiệp vụ chính.

##### `GradingStatus`

Enum thể hiện trạng thái chấm điểm:

```csharp
Pending // Chờ chấm
Graded  // Đã chấm
```

Dùng enum giúp tránh lỗi gõ sai chuỗi như `"Đã chấm"`, đồng thời làm điều kiện trong code rõ nghĩa hơn.

##### `Student`

Đại diện cho một sinh viên thực tập, gồm:

- Mã sinh viên, họ tên, lớp và ngành.
- Kỳ thực tập.
- Doanh nghiệp, vị trí và cán bộ hướng dẫn.
- Ngày nộp báo cáo.
- Trạng thái chấm điểm.
- Ba đầu điểm và điểm tổng kết.
- Nhận xét của giảng viên.
- Cờ `IsConfirmed` để xác định phiếu đã khóa hay chưa.
- Báo cáo thực tập.
- Đánh giá của doanh nghiệp.

Thuộc tính `Initials` tự tạo chữ viết tắt từ tên sinh viên để hiển thị avatar mà không cần tải ảnh từ Internet.

##### `InternshipReport`

Đại diện cho báo cáo thực tập:

- Tên/tóm tắt đề tài.
- Tên file báo cáo.
- Tên file source code.
- Trạng thái nộp đúng hạn.
- Tỷ lệ đạo văn mẫu.

##### `EnterpriseEvaluation`

Đại diện cho đánh giá của doanh nghiệp:

- Điểm tổng.
- Điểm ý thức và kỷ luật.
- Điểm chuyên môn và tiến độ.
- Điểm kỹ năng làm việc nhóm.
- Nhận xét của cán bộ doanh nghiệp.
- Trạng thái ký số mô phỏng.

#### `Models/GradingInput.cs`

Đây là model chuyên dùng để nhận dữ liệu từ biểu mẫu chấm điểm.

Các trường gồm:

- `StudentId`.
- `AttendanceScore` - điểm chuyên cần và ý thức.
- `ReportScore` - điểm chất lượng báo cáo.
- `DefenseScore` - điểm kết quả và bảo vệ.
- `LecturerFeedback` - nhận xét của giảng viên.

Ba trường điểm là `decimal?` vì khi lưu nháp người dùng được phép chưa nhập đủ điểm.

Mỗi trường điểm có `Range(0, 10)` để ASP.NET Core kiểm tra ở phía server:

```csharp
[Range(typeof(decimal), "0", "10",
    ErrorMessage = "Điểm phải nằm trong khoảng 0 đến 10.")]
```

Không đặt `[Required]` trực tiếp cho ba đầu điểm và nhận xét vì chức năng **Lưu nháp** cho phép dữ liệu chưa đầy đủ. Yêu cầu bắt buộc chỉ được kiểm tra khi người dùng chọn **Xác nhận điểm**.

### 3.4. Thư mục `Services`

#### `Services/StudentService.cs`

Đây là lớp xử lý dữ liệu và nghiệp vụ chính của UC11.

##### Biến `_students`

- Chứa danh sách bốn sinh viên mẫu.
- Được khởi tạo một lần khi `StudentService` được tạo.
- Thay thế database trong phạm vi bản demo.

##### Biến `_syncRoot`

- Dùng với câu lệnh `lock` để tránh hai request cùng sửa danh sách tại một thời điểm.
- Giúp service an toàn hơn vì service được đăng ký `Singleton` và có thể được nhiều request sử dụng.

##### `GetStudents()`

- Trả về danh sách sinh viên để trang hiển thị, tìm kiếm và lọc.
- Trả về một danh sách mới bằng `ToList()` để không cho code bên ngoài thay đổi trực tiếp cấu trúc danh sách gốc.

##### `GetById(int id)`

- Tìm sinh viên theo mã nội bộ.
- Trả về `null` nếu không tìm thấy.

##### `SaveDraft(GradingInput input)`

- Kiểm tra sinh viên có tồn tại hay không.
- Không cho sửa nếu điểm đã được xác nhận.
- Lưu các điểm đã nhập và nhận xét.
- Cho phép để trống điểm hoặc nhận xét.
- Nếu đã có đủ ba đầu điểm thì vẫn tính trước điểm tổng kết.
- Không chuyển trạng thái sang `Graded`.

##### `Confirm(GradingInput input)`

- Kiểm tra sinh viên có tồn tại không.
- Không cho xác nhận lại sinh viên đã chấm.
- Yêu cầu đủ ba đầu điểm.
- Lưu điểm và nhận xét.
- Đặt `IsConfirmed = true`.
- Chuyển trạng thái sang `Graded`.

##### `CalculateFinalScore(...)`

Công thức:

```text
Điểm tổng kết = Chuyên cần × 20%
              + Báo cáo × 30%
              + Bảo vệ × 50%
```

Code dùng `decimal` thay vì `double` vì điểm là dữ liệu thập phân cần kết quả ổn định. Kết quả được làm tròn hai chữ số bằng `Math.Round`.

Ví dụ:

```text
9.0 × 0.2 + 8.5 × 0.3 + 8.8 × 0.5 = 8.75
```

##### `GetClassification(decimal? score)`

Xếp loại bằng switch expression:

- Từ 8.5: `A - Giỏi`.
- Từ 7.0: `B - Khá`.
- Từ 5.5: `C - Trung bình`.
- Từ 4.0: `D - Yếu`.
- Dưới 4.0: `F - Kém`.
- Chưa đủ điểm: `Chưa đủ điểm`.

##### `CreateSeedData()`

- Khởi tạo dữ liệu mẫu cho bốn sinh viên.
- Có hai sinh viên chờ chấm và hai sinh viên đã chấm.
- Mỗi sinh viên có hồ sơ, báo cáo và đánh giá doanh nghiệp riêng.

##### `SaveResult`

- Là record dùng để trả kết quả thành công hoặc thất bại từ service.
- Giúp PageModel không phải xử lý exception cho các lỗi nghiệp vụ thông thường.

### 3.5. Thư mục `Pages`

#### `Pages/Index.cshtml.cs`

Đây là PageModel của màn hình UC11. File này xử lý request, gọi service và chuẩn bị dữ liệu cho giao diện.

##### Các thuộc tính nhận từ URL

```csharp
StudentId
Search
Status
```

- `StudentId`: sinh viên đang được chọn.
- `Search`: từ khóa tìm theo tên hoặc MSSV.
- `Status`: bộ lọc `all`, `pending` hoặc `graded`.

Các thuộc tính này dùng `[BindProperty(SupportsGet = true)]` để ASP.NET tự nhận dữ liệu từ query string.

Ví dụ URL:

```text
/?studentId=1&search=Mai%20Nam%20Quốc&status=pending
```

##### Thuộc tính `Input`

- Nhận dữ liệu từ biểu mẫu POST.
- Có kiểu `GradingInput`.
- Được dùng chung cho lưu nháp và xác nhận điểm.

##### `OnGet()`

- Xử lý khi mở trang hoặc thay đổi tìm kiếm/bộ lọc/sinh viên.
- Gọi `LoadPageData(false)` để tải danh sách, thống kê và thông tin sinh viên.

##### `OnPostSaveDraft()`

Luồng xử lý:

1. Lấy `StudentId` từ form.
2. Kiểm tra mỗi điểm chỉ có tối đa một chữ số thập phân.
3. Nếu điểm đã nhập không hợp lệ, hiển thị lỗi.
4. Gọi `StudentService.SaveDraft()`.
5. Dùng `TempData` lưu thông báo thành công.
6. Redirect về trang GET.

Lưu nháp không yêu cầu đủ ba điểm và nhận xét.

##### `OnPostConfirmScore()`

Luồng xử lý:

1. Kiểm tra độ chính xác của điểm.
2. Yêu cầu đủ ba đầu điểm.
3. Yêu cầu nhận xét không rỗng và tối thiểu 30 ký tự.
4. Nếu có lỗi, trả lại trang cùng `ModelState`.
5. Nếu hợp lệ, gọi `StudentService.Confirm()`.
6. Redirect và hiển thị thông báo thành công.

##### `LoadPageData(bool useBoundInput)`

Phương thức này:

- Tính tổng sinh viên.
- Tính số đã chấm và chờ chấm.
- Tính điểm trung bình từ dữ liệu hiện có.
- Lọc danh sách theo trạng thái.
- Tìm theo họ tên hoặc MSSV.
- Xác định sinh viên đang chọn.
- Nạp dữ liệu điểm cũ vào biểu mẫu.

Tham số `useBoundInput` giúp giữ lại chính xác dữ liệu người dùng vừa nhập khi server trả về lỗi, thay vì ghi đè bằng dữ liệu trong service.

##### `ValidateScorePrecision()`

- Kiểm tra mỗi đầu điểm chỉ có tối đa một chữ số sau dấu thập phân.
- Đây là kiểm tra phía server, không phụ thuộc thuộc tính HTML `step="0.1"`.

##### `ValidateConfirmationRequirements()`

- Chỉ được gọi khi xác nhận điểm.
- Kiểm tra ba điểm bắt buộc.
- Kiểm tra nhận xét bắt buộc và tối thiểu 30 ký tự.

##### `TempData`

`SuccessMessage` và `ErrorMessage` dùng `TempData` để giữ thông báo qua một lần redirect.

Đây là mẫu **POST - Redirect - GET**:

```text
POST biểu mẫu → xử lý dữ liệu → Redirect → GET trang kết quả
```

Ưu điểm là người dùng bấm F5 sẽ không gửi lại biểu mẫu lần nữa.

#### `Pages/Index.cshtml`

Đây là giao diện Razor chính của UC11.

File chịu trách nhiệm hiển thị:

- Tiêu đề và tiến trình UC11.
- Bốn thẻ thống kê.
- Bộ lọc và ô tìm kiếm.
- Danh sách sinh viên.
- Hồ sơ sinh viên đang chọn.
- Báo cáo và sản phẩm.
- Đánh giá doanh nghiệp.
- Biểu mẫu nhập điểm.
- Điểm tổng kết và xếp loại.
- Thông báo lỗi hoặc thành công.

Các cú pháp Razor thường gặp:

```razor
@Model.TotalStudents
@if (selected is null) { ... }
@foreach (var student in Model.Students) { ... }
```

Các Tag Helper quan trọng:

```razor
<input asp-for="Input.AttendanceScore" />
<span asp-validation-for="Input.AttendanceScore"></span>
<button asp-page-handler="SaveDraft">Lưu nháp</button>
```

Khi sinh viên đã được xác nhận, biến `isLocked` làm các trường bị `disabled` và không hiển thị nút lưu/xác nhận.

#### `Pages/Shared/_Layout.cshtml`

- Là bố cục dùng chung của toàn ứng dụng.
- Chứa thanh điều hướng trái, top bar và thông tin giảng viên.
- Nạp Bootstrap, CSS tùy chỉnh, jQuery, Bootstrap JavaScript và `site.js`.
- Chứa vùng `@RenderBody()` để render nội dung của `Index.cshtml`.
- Chứa toast cho các chức năng mô phỏng.

#### `Pages/Shared/_ValidationScriptsPartial.cshtml`

- Nạp `jquery.validate` và `jquery.validate.unobtrusive`.
- Chuyển các Data Annotation của C# thành quy tắc kiểm tra phía trình duyệt.
- Validation phía client chỉ giúp phản hồi nhanh; server vẫn kiểm tra lại để đảm bảo an toàn.

#### `Pages/_ViewImports.cshtml`

- Import namespace và Tag Helper dùng chung cho các Razor Page.
- Nhờ Tag Helper, Razor hiểu các thuộc tính như `asp-for`, `asp-page` và `asp-validation-for`.

#### `Pages/_ViewStart.cshtml`

- Chỉ định layout mặc định cho các trang Razor.
- Giúp các trang không phải khai báo `_Layout` lặp lại.

#### `Pages/Error.cshtml` và `Error.cshtml.cs`

- Trang xử lý lỗi mặc định của template ASP.NET Core.
- Được dùng trong môi trường không phải Development khi có lỗi chưa xử lý.

#### `Pages/Privacy.cshtml` và `Privacy.cshtml.cs`

- File mặc định của template Razor Pages.
- Không tham gia vào UC11 và không được đặt trên menu chính.
- Có thể xóa nếu muốn làm sạch project, nhưng giữ lại cũng không ảnh hưởng nghiệp vụ.

#### `Pages/Shared/_Layout.cshtml.css`

- File CSS scoped được tạo sẵn bởi template.
- Giao diện UC11 hiện dùng `wwwroot/css/site.css`, vì vậy file này không giữ vai trò chính.

### 3.6. Thư mục `wwwroot`

`wwwroot` chứa các tài nguyên tĩnh mà trình duyệt có thể truy cập.

#### `wwwroot/css/site.css`

Chứa toàn bộ style chính:

- Màu sắc, font chữ và biến CSS.
- Sidebar và top bar.
- Hero, progress và thẻ thống kê.
- Danh sách sinh viên.
- Hồ sơ, báo cáo và đánh giá doanh nghiệp.
- Biểu mẫu điểm và thông báo validation.
- Responsive cho laptop, tablet và mobile.

Giao diện dùng CSS nội bộ, không phụ thuộc Google Fonts hoặc ảnh từ URL bên ngoài. Điều này giúp demo ổn định khi không có Internet.

#### `wwwroot/js/site.js`

Chứa JavaScript phía trình duyệt:

1. Hiển thị toast cho các chức năng mô phỏng.
2. Đọc ba đầu điểm.
3. Tự động tính điểm tổng kết.
4. Tự động cập nhật xếp loại.
5. Đếm ký tự nhận xét.
6. Bật/tắt nút xác nhận dựa trên dữ liệu hợp lệ.
7. Hiển thị hộp thoại xác nhận cuối trước khi khóa điểm.

JavaScript giúp trải nghiệm nhanh hơn nhưng không thay thế validation và tính toán phía server.

#### `wwwroot/lib`

Chứa thư viện frontend cục bộ:

- Bootstrap.
- jQuery.
- jQuery Validation.
- jQuery Validation Unobtrusive.

Các file được lưu trong project nên không phụ thuộc CDN khi trình bày.

#### `wwwroot/favicon.ico`

- Biểu tượng hiển thị trên tab trình duyệt.

### 3.7. Thư mục sinh tự động

#### `bin`

- Chứa file kết quả sau khi build như `.dll`, `.exe`, `.pdb`.
- Có thể tạo lại bằng `dotnet build`.
- Không viết code trực tiếp trong thư mục này.

#### `obj`

- Chứa file trung gian do .NET/MSBuild tạo ra.
- Hỗ trợ restore, compile Razor và quản lý static assets.
- Có thể tạo lại và không phải source code nghiệp vụ.

#### `ChamDiem.csproj.user`

- Cấu hình cục bộ của Visual Studio cho người dùng hiện tại.
- Không chứa nghiệp vụ UC11.

---

## 4. Luồng xử lý tổng quát

```text
Trình duyệt
    │
    │ GET: mở trang, tìm kiếm, lọc hoặc chọn sinh viên
    ▼
Index.cshtml.cs / OnGet
    │
    │ gọi StudentService
    ▼
Danh sách dữ liệu trong bộ nhớ
    │
    ▼
Index.cshtml render giao diện
```

Luồng lưu điểm:

```text
Người dùng nhập điểm
    │
    ├─ JavaScript tính tạm và kiểm tra nhanh
    │
    ▼
POST SaveDraft hoặc ConfirmScore
    │
    ├─ Model Binding tạo GradingInput
    ├─ Data Annotation + kiểm tra nghiệp vụ phía server
    │
    ▼
StudentService cập nhật dữ liệu
    │
    ▼
Redirect về GET + TempData thông báo kết quả
```

---

## 5. Những quyết định thiết kế và lý do lựa chọn

### 5.1. Vì sao dùng Razor Pages?

- UC11 chủ yếu có một màn hình master-detail.
- Razor Pages gom giao diện và PageModel gần nhau, dễ học và dễ trình bày.
- Ít cấu hình hơn MVC Controller/View cho một demo nhỏ.
- Hỗ trợ sẵn Model Binding, Tag Helper, Validation và Dependency Injection.

### 5.2. Vì sao không dùng database?

- Đặc tả demo không yêu cầu lưu dữ liệu lâu dài.
- Trọng tâm là minh họa đúng quy trình nghiệp vụ UC11.
- Dữ liệu trong bộ nhớ giúp project chạy ngay, không cần cài SQL Server hay chạy migration.
- Nếu mở rộng, có thể thay `StudentService` bằng repository dùng Entity Framework Core mà không cần thay đổi nhiều giao diện.

### 5.3. Vì sao `StudentService` dùng Singleton?

- Dữ liệu phải tồn tại trong suốt phiên chạy của ứng dụng.
- Nếu dùng `Transient`, mỗi request có thể tạo lại dữ liệu mẫu và mất bản nháp.
- Nếu dùng `Scoped`, mỗi request cũng có một service mới và không phù hợp với danh sách in-memory của demo.

### 5.4. Vì sao kiểm tra ở cả client và server?

- Client validation giúp người dùng thấy lỗi ngay, trải nghiệm tốt hơn.
- Dữ liệu client có thể bị sửa hoặc gửi trực tiếp, nên không thể tin hoàn toàn.
- Server validation mới là lớp kiểm tra bắt buộc trước khi lưu.

### 5.5. Vì sao tách `GradingInput` khỏi `Student`?

- Form chỉ cần nhận một số trường liên quan đến chấm điểm.
- Tránh bind toàn bộ `Student`, giảm nguy cơ người dùng sửa các trường không được phép như tên, doanh nghiệp hoặc trạng thái.
- Validation của biểu mẫu được quản lý độc lập với model dữ liệu.

### 5.6. Vì sao dùng `decimal` cho điểm?

- `decimal` phù hợp với số thập phân cần độ chính xác ổn định.
- `double` lưu theo dạng nhị phân và có thể sinh sai số biểu diễn nhỏ.
- Với nghiệp vụ điểm số, `decimal` dễ kiểm soát và giải thích hơn.

### 5.7. Vì sao điểm doanh nghiệp không tham gia công thức?

- Theo phạm vi demo, điểm doanh nghiệp chỉ dùng tham khảo.
- Công thức chính thức chỉ gồm ba đầu điểm của giảng viên: 20%, 30% và 50%.

### 5.8. Vì sao phải có `IsConfirmed` ngoài `Status`?

- `Status` phục vụ hiển thị và lọc nghiệp vụ.
- `IsConfirmed` diễn tả trực tiếp việc phiếu đã bị khóa.
- Hai điều kiện cùng được kiểm tra giúp tránh chỉnh sửa ngoài ý muốn.

### 5.9. Vì sao dùng POST - Redirect - GET?

- Tránh gửi lại form khi người dùng refresh.
- URL sau xử lý sạch và có thể tiếp tục tìm kiếm/chọn sinh viên.
- Thông báo thành công được chuyển qua `TempData`.

### 5.10. Vì sao project là .NET 10 thay vì .NET 8?

- Project khung ban đầu trong thư mục đích đã được tạo với `TargetFramework` là `net10.0`.
- Máy phát triển hiện có .NET 10 SDK nên giữ nguyên giúp build và chạy ngay, tránh thay đổi môi trường ngoài phạm vi UC11.
- Kiến trúc và API đang dùng tương thích về tư duy với ASP.NET Core 8.
- Nếu máy chấm chỉ có .NET 8, có thể đổi `TargetFramework` thành `net8.0` sau khi cài .NET 8 SDK/targeting pack và build kiểm tra lại.

---

## 6. Kịch bản trình bày code đề xuất

### Bước 1 - Giới thiệu kiến trúc

Có thể nói:

> Em xây dựng UC11 bằng ASP.NET Core Razor Pages. Em chia project thành Model để mô tả dữ liệu, Service để xử lý nghiệp vụ và Razor Page để nhận request, kiểm tra dữ liệu rồi hiển thị giao diện. Dữ liệu demo được lưu trong bộ nhớ bằng Singleton service.

### Bước 2 - Trình bày model

Mở:

- `Models/Student.cs`.
- `Models/GradingInput.cs`.

Nói rõ:

- `Student` là dữ liệu đầy đủ.
- `GradingInput` chỉ là dữ liệu được phép nhập từ form.
- Điểm là nullable để hỗ trợ lưu nháp.
- Range 0-10 được kiểm tra ở server.

### Bước 3 - Trình bày service

Mở `Services/StudentService.cs` và trình bày:

- Dữ liệu mẫu nằm trong `CreateSeedData()`.
- `SaveDraft()` giữ trạng thái chờ chấm.
- `Confirm()` chuyển sang đã chấm và khóa phiếu.
- `CalculateFinalScore()` chứa công thức duy nhất, tránh lặp logic.
- `GetClassification()` xếp loại theo bảng cố định.

### Bước 4 - Trình bày PageModel

Mở `Pages/Index.cshtml.cs` và trình bày:

- `OnGet()` tải danh sách.
- `OnPostSaveDraft()` và `OnPostConfirmScore()` là hai hành động riêng.
- `LoadPageData()` xử lý thống kê, tìm kiếm và lọc.
- `ModelState` giữ lỗi tại từng trường.
- Khi lỗi, dữ liệu vừa nhập không bị mất.

### Bước 5 - Trình bày giao diện

Mở `Pages/Index.cshtml` và chỉ ra:

- Razor lấy dữ liệu từ `Model`.
- `foreach` hiển thị danh sách sinh viên.
- Tag Helper bind dữ liệu và validation.
- Trường bị khóa khi `IsConfirmed` là `true`.

Sau đó mở `wwwroot/js/site.js`:

- JavaScript tính điểm tức thời.
- Server vẫn tính lại khi lưu để không tin dữ liệu phía client.

### Bước 6 - Demo chức năng

1. Tìm `Mai Nam Quốc`.
2. Chọn sinh viên và xem hồ sơ.
3. Nhập điểm `11` để minh họa lỗi.
4. Nhập lại `9.0`, `8.5`, `8.8`.
5. Cho thấy kết quả `8.75`, xếp loại `A - Giỏi`.
6. Thử lưu nháp.
7. Xác nhận điểm.
8. Cho thấy sinh viên chuyển sang `Đã chấm`, thống kê thay đổi và form bị khóa.

---

## 7. Câu hỏi và trả lời khi bảo vệ

### Câu 1: UC11 giải quyết bài toán gì?

**Trả lời:** UC11 cho phép giảng viên xem hồ sơ thực tập, báo cáo và đánh giá doanh nghiệp của sinh viên, sau đó nhập điểm, lưu nháp hoặc xác nhận kết quả. Khi xác nhận, sinh viên chuyển sang trạng thái đã chấm và phiếu bị khóa.

### Câu 2: Vì sao em chọn Razor Pages thay vì MVC?

**Trả lời:** UC11 chỉ có một màn hình nghiệp vụ chính nên Razor Pages phù hợp hơn, cấu trúc gọn và ít boilerplate. Mỗi trang có file giao diện và PageModel đi kèm, nhưng vẫn sử dụng đầy đủ Dependency Injection, Model Binding và Validation của ASP.NET Core.

### Câu 3: Project của em có theo mô hình MVC không?

**Trả lời:** Razor Pages được xây dựng trên nền ASP.NET Core MVC nhưng tổ chức theo từng trang. `Index.cshtml` tương đương phần View, `Index.cshtml.cs` xử lý request, Model và Service tách riêng nghiệp vụ. Vì vậy tư duy phân lớp vẫn được giữ.

### Câu 4: Vì sao không kết nối database?

**Trả lời:** Phạm vi UC11 là bản demo nghiệp vụ và đặc tả không yêu cầu database. Em dùng dữ liệu in-memory để project chạy độc lập. Khi cần triển khai thật, em có thể thay service hiện tại bằng repository và Entity Framework Core.

### Câu 5: Dữ liệu có mất khi tắt ứng dụng không?

**Trả lời:** Có. Đây là chủ đích của demo in-memory. Dữ liệu được giữ trong lúc ứng dụng chạy nhưng trở về mẫu ban đầu sau khi khởi động lại.

### Câu 6: Tại sao đăng ký `StudentService` là Singleton?

**Trả lời:** Vì danh sách in-memory cần được dùng chung giữa các request. Singleton giúp bản nháp và trạng thái xác nhận được giữ trong suốt phiên chạy. Em có dùng `lock` để bảo vệ thao tác đọc/ghi cơ bản.

### Câu 7: Nếu dùng Scoped thì có được không?

**Trả lời:** Scoped phù hợp khi dùng `DbContext` vì mỗi request có một context. Tuy nhiên với danh sách in-memory nằm trực tiếp trong service, tạo lại service theo request sẽ làm mất thay đổi nếu dữ liệu cũng được khởi tạo trong constructor. Vì vậy demo này dùng Singleton.

### Câu 8: Vì sao tách `GradingInput` khỏi `Student`?

**Trả lời:** Đây là input model chỉ cho phép bind các trường cần chấm. Nếu bind thẳng toàn bộ `Student`, client có thể gửi thêm các trường như trạng thái, tên hoặc doanh nghiệp. Tách input model giúp rõ trách nhiệm và hạn chế over-posting.

### Câu 9: Hệ thống chống nhập điểm 11 như thế nào?

**Trả lời:** HTML đặt `min="0"`, `max="10"`, JavaScript kiểm tra để phản hồi nhanh, và quan trọng nhất là Data Annotation `[Range]` kiểm tra lại trên server. Nếu không hợp lệ, handler không gọi service để lưu.

### Câu 10: Nếu người dùng tắt JavaScript thì sao?

**Trả lời:** Các kiểm tra phía server vẫn hoạt động. JavaScript chỉ hỗ trợ tính trước và cải thiện trải nghiệm; dữ liệu không hợp lệ vẫn bị server từ chối.

### Câu 11: Vì sao tính điểm ở cả JavaScript và C#?

**Trả lời:** JavaScript giúp người dùng thấy kết quả ngay khi nhập. C# là kết quả chính thức và đáng tin cậy khi lưu. Công thức nghiệp vụ chuẩn nằm trong `StudentService.CalculateFinalScore()`.

### Câu 12: Có bị lặp công thức không?

**Trả lời:** Phía server không bị lặp; chỉ có một hàm `CalculateFinalScore()`. JavaScript có cùng công thức để preview trên giao diện, nhưng kết quả cuối cùng luôn được server tính lại.

### Câu 13: Vì sao dùng `decimal` thay vì `double`?

**Trả lời:** `decimal` biểu diễn số thập phân ổn định hơn và phù hợp với điểm số. `double` dùng biểu diễn nhị phân nên có thể xuất hiện sai số nhỏ không mong muốn.

### Câu 14: Điểm được làm tròn như thế nào?

**Trả lời:** Server dùng `Math.Round(..., 2, MidpointRounding.AwayFromZero)` để làm tròn đến hai chữ số thập phân theo quy tắc rõ ràng.

### Câu 15: Tại sao điểm chỉ cho nhập một chữ số thập phân nhưng tổng kết có hai chữ số?

**Trả lời:** Ba đầu điểm là dữ liệu do giảng viên nhập theo bước 0.1. Khi nhân trọng số, tổng kết có thể sinh hai chữ số thập phân nên kết quả được giữ và hiển thị hai chữ số.

### Câu 16: Tại sao lưu nháp được phép thiếu dữ liệu?

**Trả lời:** Vì giảng viên có thể chưa hoàn thành việc đánh giá. Các trường điểm dùng kiểu nullable và nhận xét không bắt buộc khi lưu nháp. Tuy nhiên, nếu đã nhập điểm thì điểm đó vẫn phải trong khoảng 0-10.

### Câu 17: Lưu nháp khác xác nhận như thế nào?

**Trả lời:** Lưu nháp cập nhật dữ liệu nhưng giữ trạng thái `Pending` và vẫn cho sửa. Xác nhận yêu cầu đủ ba điểm, nhận xét ít nhất 30 ký tự, chuyển trạng thái sang `Graded` và đặt `IsConfirmed = true` để khóa phiếu.

### Câu 18: Vì sao phải hỏi xác nhận lần cuối?

**Trả lời:** Xác nhận điểm là thao tác không cho sửa lại trong phạm vi UC11. Hộp thoại giúp tránh việc giảng viên bấm nhầm trước khi khóa dữ liệu.

### Câu 19: Nếu gửi lại request xác nhận cho sinh viên đã chấm thì sao?

**Trả lời:** Service kiểm tra cả `IsConfirmed` và trạng thái `Graded`. Dù người dùng sửa HTML hoặc gửi request trực tiếp, service vẫn từ chối cập nhật.

### Câu 20: Tìm kiếm được thực hiện như thế nào?

**Trả lời:** `LoadPageData()` dùng `Contains` với `StringComparison.OrdinalIgnoreCase` trên họ tên và MSSV. Tìm kiếm thực hiện phía server vì dữ liệu nhỏ và giúp URL lưu được điều kiện tìm kiếm.

### Câu 21: Bộ lọc trạng thái hoạt động ra sao?

**Trả lời:** Query string `status` có ba giá trị: `all`, `pending`, `graded`. PageModel dùng switch expression để lọc danh sách trước khi render.

### Câu 22: Nếu tìm kiếm không có kết quả thì sao?

**Trả lời:** Danh sách trả về rỗng, giao diện hiển thị thông báo `Không tìm thấy sinh viên phù hợp` và không phát sinh lỗi null.

### Câu 23: Các số liệu thống kê có viết cứng không?

**Trả lời:** Không. Tổng số sinh viên, số đã chấm, số chờ chấm và điểm trung bình đều được tính lại từ danh sách trong `StudentService`. Sau khi xác nhận, thống kê tự cập nhật.

### Câu 24: Điểm doanh nghiệp có được tính vào điểm cuối không?

**Trả lời:** Không. Theo phạm vi demo, đánh giá doanh nghiệp chỉ là thông tin tham khảo. Điểm cuối chỉ gồm ba đầu điểm của giảng viên với trọng số 20%, 30% và 50%.

### Câu 25: `ModelState` có vai trò gì?

**Trả lời:** `ModelState` lưu kết quả bind và validation của từng trường. Khi có lỗi, PageModel trả lại trang và Razor hiển thị lỗi tại đúng ô nhập mà không thực hiện lưu/xác nhận.

### Câu 26: Vì sao khi lỗi dữ liệu vừa nhập không bị mất?

**Trả lời:** Khi handler phát hiện lỗi, nó gọi `LoadPageData(true)`. Giá trị `true` báo rằng không được ghi đè `Input` đang có từ form. Razor sẽ render lại các giá trị trong `ModelState`.

### Câu 27: `TempData` dùng để làm gì?

**Trả lời:** `TempData` giữ thông báo thành công qua đúng một lần redirect. Nó phù hợp với mẫu POST - Redirect - GET và tránh gửi lại form khi refresh.

### Câu 28: Project có bảo mật gì?

**Trả lời:** Razor Pages tự tạo anti-forgery token cho form POST, server không bind toàn bộ entity và kiểm tra lại quyền sửa dựa trên trạng thái. Đăng nhập/phân quyền thật nằm ngoài phạm vi demo. Nếu triển khai thật, em sẽ bổ sung ASP.NET Core Identity và policy cho giảng viên hướng dẫn.

### Câu 29: Anti-forgery token là gì?

**Trả lời:** Đây là token giúp chống CSRF. Form hợp lệ phải gửi token do server tạo. Request giả mạo từ website khác không có token phù hợp sẽ bị từ chối.

### Câu 30: Vì sao giao diện không dùng ảnh và font từ Internet?

**Trả lời:** Để demo ổn định trong phòng học khi mạng yếu hoặc không có mạng. Avatar được tạo bằng chữ viết tắt, CSS và thư viện cần thiết đều nằm trong project.

### Câu 31: Các nút xem/tải báo cáo có hoạt động thật không?

**Trả lời:** Theo phạm vi UC11, đây là chức năng mô phỏng. Khi bấm, hệ thống hiển thị thông báo mẫu. Việc lưu và tải file thật nằm ngoài phạm vi.

### Câu 32: Vì sao có `lock` trong service?

**Trả lời:** Service là Singleton nên có thể được nhiều request truy cập đồng thời. `lock` giúp các thao tác đọc và cập nhật danh sách không chạy xen kẽ gây dữ liệu không nhất quán trong demo.

### Câu 33: Nếu hai giảng viên cùng sửa một sinh viên thì sao?

**Trả lời:** Demo giả định không có nhiều người cùng sửa. `lock` chỉ bảo vệ thao tác trong một tiến trình, chưa giải quyết optimistic concurrency. Bản thật nên dùng database, cột version/timestamp và kiểm tra quyền phân công.

### Câu 34: Nếu muốn dùng database thì thay đổi như thế nào?

**Trả lời:** Em sẽ tạo `AppDbContext`, ánh xạ các model thành entity, dùng Entity Framework Core và repository/service dạng Scoped. Các handler trong PageModel vẫn gọi service nên phần giao diện thay đổi ít.

### Câu 35: Nếu muốn lưu file báo cáo thật thì làm thế nào?

**Trả lời:** Em sẽ dùng `IFormFile`, kiểm tra loại và dung lượng file, đổi tên an toàn, lưu vào vùng ngoài `wwwroot` hoặc object storage, rồi lưu metadata vào database. Khi tải phải kiểm tra quyền truy cập.

### Câu 36: Nếu muốn thêm đăng nhập và phân quyền thì sao?

**Trả lời:** Em sẽ dùng ASP.NET Core Identity, thêm `[Authorize]` cho trang và policy/claim dành cho vai trò giảng viên. Khi lấy sinh viên, service chỉ truy vấn các sinh viên được phân công cho tài khoản hiện tại.

### Câu 37: Làm sao kiểm thử công thức điểm?

**Trả lời:** Có thể kiểm thử hàm `CalculateFinalScore()` độc lập. Trường hợp nghiệm thu là `9.0`, `8.5`, `8.8` phải cho `8.75`. Cũng cần kiểm thử thiếu một điểm trả về `null` và các mức biên xếp loại như 8.5, 7.0, 5.5, 4.0.

### Câu 38: Project đã được kiểm thử những gì?

**Trả lời:** Project đã build thành công không có warning/error; đã kiểm tra tìm kiếm trả một kết quả, lọc chờ chấm trả hai kết quả ban đầu, lưu nháp rỗng, chặn điểm ngoài 0-10, công thức 8.75, xác nhận thành công, cập nhật thống kê và khóa form.

### Câu 39: Hạn chế hiện tại của project là gì?

**Trả lời:** Dữ liệu chưa lưu lâu dài, chưa có đăng nhập/phân quyền thật, file chỉ mô phỏng, chưa hỗ trợ nhiều người sửa đồng thời và chưa có test project tự động riêng. Những phần này nằm ngoài phạm vi demo nhưng có hướng mở rộng rõ ràng.

### Câu 40: Nếu có thêm thời gian, em sẽ cải tiến gì?

**Trả lời:** Em sẽ thêm Entity Framework Core và SQL Server, ASP.NET Core Identity, kiểm thử unit/integration, audit log lịch sử sửa điểm, optimistic concurrency, upload file an toàn và chức năng xuất bảng điểm.

---

## 8. Một số câu trả lời ngắn cần nhớ

| Chủ đề | Trả lời ngắn |
|---|---|
| Kiến trúc | Razor Pages + Model + Service |
| Dữ liệu | In-memory, giữ trong một phiên chạy |
| Vòng đời service | Singleton để giữ dữ liệu giữa các request |
| Công thức | 20% chuyên cần + 30% báo cáo + 50% bảo vệ |
| Kiểu điểm | `decimal?` để chính xác và hỗ trợ lưu nháp |
| Validation | Client để phản hồi nhanh, server để đảm bảo đúng |
| Khóa điểm | `Status = Graded` và `IsConfirmed = true` |
| Thông báo | `TempData` kết hợp POST - Redirect - GET |
| Tìm kiếm | Theo tên hoặc MSSV, không phân biệt hoa thường |
| Database | Không dùng vì ngoài phạm vi demo |
| Điểm doanh nghiệp | Chỉ tham khảo, không tham gia công thức |
| Framework | Giữ `net10.0` theo project khung và SDK của máy |

---

## 9. Lệnh chạy và build

Từ thư mục `Usecase11_MaiNamQuoc`:

```powershell
dotnet build .\Usecase11_MaiNamQuoc.slnx
dotnet run --project .\ChamDiem\ChamDiem.csproj
```

Sau đó mở:

```text
http://localhost:5129
```

hoặc URL được hiển thị trong terminal.

---

## 10. Checklist trước khi trình bày

- Build project trước buổi trình bày.
- Đảm bảo cổng `5129` không bị ứng dụng khác sử dụng.
- Khởi động lại ứng dụng để dữ liệu trở về trạng thái mẫu.
- Tìm đúng sinh viên `Mai Nam Quốc`.
- Ghi nhớ bộ điểm `9.0`, `8.5`, `8.8` và kết quả `8.75`.
- Chuẩn bị một nhận xét dài hơn 30 ký tự.
- Demo lỗi điểm `11` trước khi nhập điểm hợp lệ.
- Lưu nháp trước, sau đó mới xác nhận.
- Sau khi xác nhận, chỉ ra ba thay đổi: trạng thái, thống kê và form bị khóa.
- Nếu demo sai dữ liệu, tắt và chạy lại ứng dụng để reset dữ liệu mẫu.
