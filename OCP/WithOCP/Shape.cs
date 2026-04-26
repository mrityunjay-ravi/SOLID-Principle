namespace SOLIDPrinciples.OCP.WithOCP
{
    // This example follows the Open/Closed Principle by using an abstraction (IShape)
    // and allowing extension through new implementations rather than modification
    
    // Abstract shape interface
    public interface IShape
    {
        double CalculateArea();
    }

    public class Rectangle : IShape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double CalculateArea()
        {
            return Width * Height;
        }
    }

    public class Circle : IShape
    {
        public double Radius { get; set; }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public double CalculateArea()
        {
            return Math.PI * Radius * Radius;
        }
    }

    // This class can be extended without modification - just add a new shape implementing IShape
    public class AreaCalculator
    {
        public double CalculateArea(IShape shape)
        {
            return shape.CalculateArea();
        }
    }

    // We can add a new shape without modifying AreaCalculator
    public class Triangle : IShape
    {
        public double Base { get; set; }
        public double Height { get; set; }

        public Triangle(double @base, double height)
        {
            Base = @base;
            Height = height;
        }

        public double CalculateArea()
        {
            return 0.5 * Base * Height;
        }
    }

    public class OCPDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("\n=== With OCP Example ===\n");
            
            var rectangle = new Rectangle(5, 4);
            var circle = new Circle(3);
            var triangle = new Triangle(6, 4);
            
            var calculator = new AreaCalculator();
            
            Console.WriteLine($"Rectangle Area: {calculator.CalculateArea(rectangle)}");
            Console.WriteLine($"Circle Area: {calculator.CalculateArea(circle)}");
            Console.WriteLine($"Triangle Area: {calculator.CalculateArea(triangle)}");
            
            Console.WriteLine("\nWe can add new shapes without modifying the AreaCalculator class!");
        }
    }
}
