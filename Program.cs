using SOLIDPrinciples.SRP.WithoutSRP;
using SOLIDPrinciples.SRP.WithSRP;
using SOLIDPrinciples.OCP.WithoutOCP;
using SOLIDPrinciples.OCP.WithOCP;
using SOLIDPrinciples.LSP.WithoutLSP;
using SOLIDPrinciples.LSP.WithLSP;
using SOLIDPrinciples.ISP.WithoutISP;
using SOLIDPrinciples.ISP.WithISP;
using SOLIDPrinciples.DIP.WithoutDIP;
using SOLIDPrinciples.DIP.WithDIP;

Console.WriteLine("SOLID Principles Demonstration");
Console.WriteLine("==============================");

bool continueRunning = true;

while (continueRunning)
{
    Console.WriteLine("\nChoose a SOLID principle to demonstrate:");
    Console.WriteLine("1. Single Responsibility Principle (SRP)");
    Console.WriteLine("2. Open/Closed Principle (OCP)");
    Console.WriteLine("3. Liskov Substitution Principle (LSP) - Payment System (Complete)");
    Console.WriteLine("4. Interface Segregation Principle (ISP)");
    Console.WriteLine("5. Dependency Inversion Principle (DIP)");
    Console.WriteLine("6. Run all examples");
    Console.WriteLine("0. Exit");

    Console.Write("\nEnter your choice: ");
    string? input = Console.ReadLine();

    switch (input)
    {
        case "1":
            SOLIDPrinciples.SRP.WithoutSRP.EmployeeDemo.RunDemo();
            SOLIDPrinciples.SRP.WithSRP.EmployeeDemo.RunDemo();
            break;

        case "2":
            SOLIDPrinciples.OCP.WithoutOCP.OCPDemo.RunDemo();
            SOLIDPrinciples.OCP.WithOCP.OCPDemo.RunDemo();
            break;


        case "3":
            SOLIDPrinciples.LSP.WithoutLSP.PaymentSystemLSPViolationDemo.RunDemo();
            SOLIDPrinciples.LSP.WithLSP.PaymentSystemLSPDemo.RunDemo();
            break;

        case "4":
            SOLIDPrinciples.ISP.WithoutISP.ISPDemo.RunDemo();
            SOLIDPrinciples.ISP.WithISP.ISPDemo.RunDemo();
            break;

        case "5":
            SOLIDPrinciples.DIP.WithoutDIP.DIPDemo.RunDemo();
            SOLIDPrinciples.DIP.WithDIP.DIPDemo.RunDemo();
            break;

        case "6":
            // Run all examples
            SOLIDPrinciples.SRP.WithoutSRP.EmployeeDemo.RunDemo();
            SOLIDPrinciples.SRP.WithSRP.EmployeeDemo.RunDemo();

            SOLIDPrinciples.OCP.WithoutOCP.OCPDemo.RunDemo();
            SOLIDPrinciples.OCP.WithOCP.OCPDemo.RunDemo();

            SOLIDPrinciples.LSP.WithoutLSP.PaymentSystemLSPViolationDemo.RunDemo();
            SOLIDPrinciples.LSP.WithLSP.PaymentSystemLSPDemo.RunDemo();

            SOLIDPrinciples.ISP.WithoutISP.ISPDemo.RunDemo();
            SOLIDPrinciples.ISP.WithISP.ISPDemo.RunDemo();

            SOLIDPrinciples.DIP.WithoutDIP.DIPDemo.RunDemo();
            SOLIDPrinciples.DIP.WithDIP.DIPDemo.RunDemo();
            break;
        
        case "0":
            continueRunning = false;
            Console.WriteLine("\nExiting the application. Goodbye!");
            break;
        
        default:
            Console.WriteLine("\nInvalid choice. Please try again.");
            break;
    }
}
