# Aurora Game Hub

Native Windows Game Launcher xây dựng bằng C# & .NET 8 WPF, tối ưu trải nghiệm và hiệu năng khi chơi game.

## Tính năng chính
- **Giao diện Native mượt mà**: Thiết kế hiện đại, hỗ trợ phóng to/thu nhỏ, kéo thả cửa sổ và hiệu ứng hover 3D.
- **Tự động nhận diện phần cứng**: Đọc thông số CPU, GPU, RAM thực tế theo thời gian thực.
- **Turbo Boost**: Dọn dẹp RAM (EmptyWorkingSet), kích hoạt High Performance Power Plan và nâng mức ưu tiên tiến trình game.
- **Quản lý thư viện game**: Phân loại theo thể loại, tự động kiểm tra trạng thái cài đặt và khởi động game trực tiếp.

## Yêu cầu hệ thống
- Windows 10 / 11 (64-bit)
- .NET 8.0 SDK / Runtime

## Cài đặt & Build
```bash
# Clone repository
git clone https://github.com/meomeobbi/gamelauncher.git
cd gamelauncher

# Build & Run
dotnet run -c Release

# Publish Native Standalone
dotnet publish -c Release -r win-x64 --self-contained true
```
