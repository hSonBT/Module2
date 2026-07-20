# Module2 - WebApp (ASP.NET Core MVC)

## Cấu trúc thư mục

```
Module2/
└── WebApp/
    ├── Program.cs                     # Điểm khởi động ứng dụng, cấu hình DI, middleware, routing
    ├── appsettings.json               # Cấu hình chung (connection string, logging...)
    ├── appsettings.Development.json   # Cấu hình riêng cho môi trường Development
    │
    ├── Controllers/                   # Xử lý request, gọi Repository, trả về View
    │   ├── CategoryController.cs      # Controller cho chức năng quản lý Category
    │   ├── CalculateController.cs     # Controller cho chức năng tính toán (Sum, Triangle)
    │   └── WelcomeController.cs       # Controller trang chào mừng
    │
    ├── Models/                        # Định nghĩa cấu trúc dữ liệu (POCO)
    │   ├── Category.cs                # Model Category (CategoryId, CategoryName)
    │   └── Triangle.cs                # Model Triangle
    │
    ├── Repositories/                  # Lớp truy cập dữ liệu (Dapper + ADO.NET)
    │   └── CategoryRepository.cs      # CRUD Category với SQL Server qua Dapper
    │
    └── Views/                         # Giao diện Razor (.cshtml)
        ├── _ViewImports.cshtml        # using + TagHelper dùng chung cho mọi view
        ├── _ViewStart.cshtml          # Thiết lập Layout mặc định trước khi render view
        │
        ├── Shared/
        │   └── _Layout.cshtml         # Layout khung chung (header, css, js, @RenderBody())
        │
        ├── Category/                  # View tương ứng CategoryController
        │   ├── index.cshtml           # Danh sách category
        │   ├── Add.cshtml             # Form thêm category
        │   ├── Edit.cshtml            # Form sửa category
        │   └── Error.cshtml           # Trang lỗi
        │
        └── Calculate/                 # View tương ứng CalculateController
            ├── Sum.cshtml
            └── Triangle.cshtml
```

## Giải thích các thành phần chính

### Program.cs
Điểm khởi động của ứng dụng ASP.NET Core: đăng ký service (MVC, DI...), cấu hình middleware pipeline (routing, static files...) và khởi chạy server.

### Controllers/
Nhận HTTP request, gọi xuống `Repositories` để lấy/ghi dữ liệu, sau đó chọn `View` tương ứng để trả về theo convention `Views/{TênController}/{TênAction}.cshtml`.

### Models/
Chứa các class đại diện cho dữ liệu nghiệp vụ (entity), dùng để bind dữ liệu từ form (`asp-for`) và truyền qua các tầng Controller ↔ Repository ↔ View.

### Repositories/
Tầng truy cập cơ sở dữ liệu, dùng **Dapper** (micro-ORM) để thực thi câu lệnh SQL và map kết quả trực tiếp sang các Model (`Query`, `QueryFirstOrDefault`, `Execute`...).

### Views/
- **`_ViewImports.cshtml`**: khai báo dùng chung (namespace, TagHelper) cho toàn bộ view, tránh phải lặp lại `@addTagHelper` ở từng file.
- **`_ViewStart.cshtml`**: chạy trước mỗi view, dùng để set `Layout` mặc định (`_Layout`) cho tất cả các trang.
- **`Shared/_Layout.cshtml`**: khung giao diện chung (HTML, CSS, JS dùng chung), có `@RenderBody()` là nơi nội dung của view con được chèn vào.
- **Thư mục con theo tên Controller** (`Category/`, `Calculate/`): mỗi action trong controller tương ứng với một file `.cshtml` cùng tên trong thư mục con đó.
