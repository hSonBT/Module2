using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

// Trong OOP gọi là class, nhưng trong lập trình web (MVC) gọi là Controller
public class WelcomeController : Controller
{
    // Trong OOP gọi là method, nhưng trong lập trình web (MVC) gọi là Action
    public string Hello()
    {
        return "Chào các bạn học viên lớp LTV Dot Net";
    }
}