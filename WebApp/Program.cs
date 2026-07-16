var builder = WebApplication.CreateBuilder(args);

// Đăng ký service cho MVC vào DI container - chọn 1 trong 3 tuỳ nhu cầu:
// builder.Services.AddControllers();            // Chỉ Web API, không có View, trả JSON
// builder.Services.AddControllersWithViews();    // MVC có View (Controller + Razor View)
builder.Services.AddMvc();                        // Đầy đủ nhất: Controllers + Views + Razor Pages

var app = builder.Build();

// app.MapGet("/", () => "Hello World!");

// Thiết lập route mặc định theo quy ước {controller=Home}/{action=Index}/{id?}
// Nhờ đó URL như /Welcome/Hello sẽ tự động được ánh xạ tới WelcomeController.Hello()
app.MapDefaultControllerRoute();

app.Run();