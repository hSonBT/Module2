namespace WebApp.Models;

public class Product
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public string ProductName { get; set; } = null;
    public string Description { get; set; } = null;
    public string Content { get; set; } = null;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } =  null;
    public short Quantity { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}