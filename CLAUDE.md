# CLAUDE.md

Hướng dẫn cho agent lập trình làm việc trong repo này. Repo công khai, là dự án thử nghiệm.

## Lệnh

```powershell
dotnet build NotesApi.slnx -c Release     # phải 0 cảnh báo (TreatWarningsAsErrors)
dotnet test NotesApi.slnx -c Release
dotnet run --project src/NotesApi
```

## Cấu trúc

- `src/NotesApi/Program.cs`: khởi động, đăng ký `NoteStore`, gọi `MapNotes()`.
- `src/NotesApi/Notes.cs`: `Note`, `CreateNoteRequest`, `NoteStore` (trong bộ nhớ), `NoteEndpoints` (các route, kiểm tra đầu vào).
- `tests/NotesApi.Tests/`: xunit 2 với `WebApplicationFactory<Program>`, gọi API thật trong bộ nhớ.

## Quy tắc

1. **Chỉ động vào file cần cho yêu cầu.** Không refactor mã kề bên, không sửa style, không xóa mã chết có sẵn (chỉ báo). Mỗi dòng đổi phải truy về được yêu cầu.
2. **Không rõ thì dừng và hỏi**, nêu các cách hiểu và đánh đổi.
3. Đổi hành vi thì **thêm hoặc sửa test** trước. Sửa lỗi thì viết test tái hiện lỗi rồi mới sửa. Build và test phải xanh trước khi báo xong.
4. **Không thêm package NuGet** khi chưa được yêu cầu.
5. **Repo này công khai.** Không đưa vào repo (code, test, tài liệu, commit message): bí mật, token, chuỗi kết nối, mã hay dữ liệu hay tên nội bộ của bất kỳ công ty nào.
6. **Không `git push`, không tạo PR, không đổi cấu hình GitHub hay workflow CI** nếu không được yêu cầu rõ. Việc đó do chủ repo làm.
7. Khi xong, nêu: đã chạy gì và kết quả, file nào đã đổi, điều gì chưa kiểm chứng.

## Phong cách C#

Namespace theo file (`namespace Foo;`), `record` cho DTO, trường private `_camelCase`, hằng số `PascalCase`, trả sớm thay vì lồng sâu, không "magic number" (đặt tên hằng, ví dụ `MaxTitleLength`), nullable bật. Chú thích giải thích *vì sao*, không tả lại *cái gì*.
