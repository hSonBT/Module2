namespace WebApp.Models;

public class Triangle
{
    public double A { get; set; }
    public double B { get; set; }
    public double C { get; set; }

    public double Perimeter
    {
        get { return A + B + C; }
    }

    public double Area {
        get
        {
            double p = Perimeter / 2;
            return p * (p - A) * (p - B) * (p - C);
        }
    }
}