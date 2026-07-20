using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;
using WebApp.Repositories;
using WebApp.Services;

namespace WebApp.Controllers;

public class ProductController : Controller
{
    private readonly ProductRepository _productRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _productRepository = new ProductRepository(configuration);
        _categoryRepository = new CategoryRepository(configuration);
        _webHostEnvironment = webHostEnvironment;
    }

    // GET
    public IActionResult Index()
    {
        var list = _productRepository.GetProducts();
        return View(list);
    }

    //GET
    public IActionResult Add()
    {
        ViewBag.Categories = new SelectList(_categoryRepository.GetCategories(), "CategoryId", "CategoryName");
        return View();
    }

    [HttpPost]
    public IActionResult Add(Product product, IFormFile? file)
    {
        ModelState.Remove(nameof(product.ImageUrl));

        if (file is null)
        {
            ModelState.AddModelError("file", "Please choose an image");
        }

        if (ModelState.IsValid && file is not null)
        {
            var imageDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(imageDir))
            {
                Directory.CreateDirectory(imageDir);
            }

            var ext = Path.GetExtension(file.FileName);
            var fileName = Helper.RandomString(32 - ext.Length) + ext;
            var filePath = Path.Combine(imageDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            product.ImageUrl = fileName;


            var ret = _productRepository.Add(product);
            if (ret > 0)
            {
                TempData["Msg"] = "Add Success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("Error", "Insert Failed");
        }

        ViewBag.Categories = new SelectList(_categoryRepository.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
        return View(product);
    }


    public IActionResult Delete(int id)
    {
        Product? product = _productRepository.GetProduct(id);
        if (product is null) return RedirectToAction("Index");
        return View(product);
    }

    [HttpPost]
    public IActionResult Delete(int id, string imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl))
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(imageUrl));
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        var ret = _productRepository.Delete(id);
        if (ret > 0)
        {
            TempData["Msg"] = "Delete Success";
            return RedirectToAction("Index", "Product");
        }

        TempData["Msg"] = "Delete Failed";
        return RedirectToAction("Delete", new { id });
    }

    public IActionResult Edit(int id)
    {
        Product? product = _productRepository.GetProduct(id);
        if(product is null) return RedirectToAction("Index");
        ViewBag.Categies = new SelectList(_categoryRepository.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(int id, Product product, IFormFile? file)
    {

        if (ModelState.IsValid)
        {
             
            if (file is not null)
            {
                string root = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string oldPath = Path.Combine(root, product.ImageUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                // File mới

                string ext = Path.GetExtension(file.FileName);
                string fileName = Helper.RandomString(32 - ext.Length) + ext;
                string path = Path.Combine(root, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                product.ImageUrl = fileName;
            }
        
            // Update data
            int ret = _productRepository.Edit(product);
            if (ret > 0)
            {
                TempData["Msg"] = "Edit Success";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("Error", "Edit Failed");
        }

        ViewBag.Categies = new SelectList(_categoryRepository.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
        return View(product);
    }
}