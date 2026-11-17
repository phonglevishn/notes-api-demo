# notes-api-demo

API ghi chú nhỏ (ASP.NET Core minimal API, .NET 10) để thử quy trình làm việc với agent lập trình: giao việc cho agent, nó làm trên một nhánh `agent/<task>`, người xem diff, mở PR, CI chạy, rồi gộp.

Đây là dự án thử nghiệm, không phải sản phẩm. Dữ liệu nằm trong bộ nhớ và mất khi khởi động lại.

## Chạy

```powershell
dotnet run --project src/NotesApi
```

| Method | Đường dẫn | Ý nghĩa |
|---|---|---|
| GET | `/health` | Kiểm tra sống |
| GET | `/notes` | Danh sách ghi chú, cũ trước |
| GET | `/notes/{id}` | Một ghi chú, 404 nếu không có |
| POST | `/notes` | Tạo ghi chú `{ "title": "...", "body": "..." }`; `title` bắt buộc, tối đa 100 ký tự; 201 hoặc 400 |

## Build và test

```powershell
dotnet build NotesApi.slnx -c Release
dotnet test NotesApi.slnx -c Release
```

Build bật `TreatWarningsAsErrors`, nên cảnh báo cũng làm hỏng build.

## CI

`.github/workflows/ci.yml` chạy restore, build, test trên `ubuntu-latest` cho mỗi PR và mỗi lần đẩy vào `main`. Các action được ghim theo commit SHA; Dependabot đề xuất nâng phiên bản hằng tuần.

## Cho agent

Xem [CLAUDE.md](CLAUDE.md).
