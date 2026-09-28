# Demo UC11 - Chấm điểm sinh viên thực tập

Ứng dụng ASP.NET Core Razor Pages mô phỏng đầy đủ luồng UC11 cho giảng viên hướng dẫn:

- Xem, tìm kiếm và lọc danh sách sinh viên.
- Xem hồ sơ thực tập, báo cáo và đánh giá của doanh nghiệp.
- Nhập ba đầu điểm, tự động tính điểm tổng kết và xếp loại.
- Lưu nháp khi dữ liệu chưa đầy đủ.
- Kiểm tra dữ liệu ở cả trình duyệt và máy chủ.
- Xác nhận điểm, chuyển trạng thái sang `Đã chấm` và khóa phiếu.
- Dữ liệu mẫu được lưu trong bộ nhớ và trở về ban đầu khi khởi động lại.

## Chạy ứng dụng

Yêu cầu .NET 10 SDK (project khung ban đầu đang dùng `net10.0`).

```powershell
cd C:\Users\Quoc\Desktop\DTU\QuanLyPhanMem\Usecase11_MaiNamQuoc
dotnet run --project .\ChamDiem\ChamDiem.csproj
```

Mở địa chỉ được in trong terminal, mặc định là `https://localhost:7148` hoặc `http://localhost:5129`.

## Dữ liệu trình diễn gợi ý

Chọn **Mai Nam Quốc** và nhập:

- Chuyên cần: `9.0`
- Chất lượng báo cáo: `8.5`
- Kết quả và bảo vệ: `8.8`

Điểm tổng kết mong đợi là `8.75`, xếp loại `A - Giỏi`.
