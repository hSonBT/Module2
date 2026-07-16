using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class CalculateController : Controller
{
    // GET: hiển thị form nhập liệu ban đầu, chưa có kết quả nên không truyền model
    public IActionResult Sum()
    {
        return View();
    }

    // POST: nhận num1, num2 từ form (model binding tự động theo tên input),
    // tính tổng rồi truyền kết quả sang View làm model để hiển thị
    [HttpPost]
    public IActionResult Sum(int num1, int num2)
    {
        int total = num1 + num2;
        return View(total); // Truyền data  từ controller => view bằng model
    }


    public IActionResult Triangle()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Triangle(Triangle model)
    {
        return View(model);
    }
}