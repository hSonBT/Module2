using Microsoft.AspNetCore.Mvc;
using WebApp.Repositories;

namespace WebApp.Controllers;

public class CategoryController : Controller
{
    CategoryRepository categoryRepository;

    public CategoryController(IConfiguration configuration)
    {
        categoryRepository = new CategoryRepository(configuration);
    }

    // GET
    public IActionResult Index()
    {
        return View(categoryRepository.GetCategories());
    }
}