using SOLIDPrinciples.OCP.WithOCP;
using System;
using System.Collections.Generic;

namespace SOLIDPrinciples.OCP.WithoutOCP
{
    // This example violates the Open/Closed Principle because whenever a new shape is added,
    // the AreaCalculator class needs to be modified to handle the new shape
    
    public class Rectangle
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }

    public class Circle
    {
        public double Radius { get; set; }

        public Circle(double radius)
        {
            Radius = radius;
        }
    }

    // This class violates OCP because it must be modified whenever a new shape is added
    public class AreaCalculator
    {
        public double CalculateArea(object shape)
        {
            if (shape is Rectangle rectangle)
            {
                return rectangle.Width * rectangle.Height;
            }
            else if (shape is Circle circle)
            {
                return Math.PI * circle.Radius * circle.Radius;
            }

                throw new ArgumentException("Unsupported shape type");
        }
    }

    public class OCPDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("\n=== Without OCP Example ===\n");
            
            var rectangle = new Rectangle(5, 4);
            var circle = new Circle(3);
            
            var calculator = new AreaCalculator();
            
            Console.WriteLine($"Rectangle Area: {calculator.CalculateArea(rectangle)}");
            Console.WriteLine($"Circle Area: {calculator.CalculateArea(circle)}");
            
            Console.WriteLine("\nIf we want to add a Triangle class, we must modify the AreaCalculator class!");
            
            // Adding a new shape requires modifying the AreaCalculator class
            // (This won't compile without modifying AreaCalculator)
            // var triangle = new Triangle(3, 4, 5);
            // Console.WriteLine($"Triangle Area: {calculator.CalculateArea(triangle)}");
        }
    }
}
