namespace SOLIDPrinciples.LSP.WithLSP
{
    // ✅ FOLLOWS LSP: Proper abstraction hierarchy based on payment characteristics
    // Base interface remains minimal, extended interfaces add specific capabilities

    /// <summary>
    /// Base Payment interface - ONLY common behaviors that ALL payments share
    /// This is the foundation that all payment types implement
    /// </summary>
    public interface IPayment
    {
        string PaymentId { get; }
        decimal Amount { get; }
        DateTime PaymentDate { get; }
        string GetPaymentMethod();
    }

    /// <summary>
    /// OnlinePayment extends Payment - adds online processing capabilities
    /// Only payments that are processed online implement this interface
    /// Includes refund capability as all online payments can be refunded
    /// </summary>
    public interface IOnlinePayment : IPayment
    {
        bool ValidatePaymentDetails();
        bool ProcessPayment();
        string GetTransactionId();
        bool RefundPayment(); // Online payments can be refunded
    }

    /// <summary>
    /// OfflinePayment extends Payment - for payments collected physically
    /// Cash On Delivery and similar payment methods implement this
    /// No online processing or refund capability
    /// </summary>
    public interface IOfflinePayment : IPayment
    {
        string GetDeliveryAddress();
        bool MarkAsCollected();
        bool VerifyCollection(decimal collectedAmount);
    }

    // ✅ Credit Card: Implements OnlinePayment (can process online and refund)
    public class CreditCardPayment : IOnlinePayment
    {
        public string PaymentId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string CardNumber { get; private set; }
        public string CVV { get; private set; }
        private string _transactionId;

        public CreditCardPayment(decimal amount, string cardNumber, string cvv)
        {
            PaymentId = Guid.NewGuid().ToString();
            Amount = amount;
            PaymentDate = DateTime.Now;
            CardNumber = cardNumber;
            CVV = cvv;
        }

        public string GetPaymentMethod() => "Credit Card";

        public bool ValidatePaymentDetails()
        {
            Console.WriteLine($"? Validating credit card: ****{CardNumber.Substring(CardNumber.Length - 4)}");
            return !string.IsNullOrEmpty(CardNumber) && CVV.Length == 3;
        }

        public bool ProcessPayment()
        {
            _transactionId = $"CC-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            Console.WriteLine($"? Processing credit card payment of ${Amount}");
            Console.WriteLine($"   Transaction ID: {_transactionId}");
            return true;
        }

        public string GetTransactionId()
        {
            return _transactionId ?? throw new InvalidOperationException("Payment not processed yet");
        }

        public bool RefundPayment()
        {
            Console.WriteLine($"? Refunding ${Amount} to credit card ****{CardNumber.Substring(CardNumber.Length - 4)}");
            return true;
        }
    }

    // ✅ UPI: Implements OnlinePayment (can process online and refund)
    public class UPIPayment : IOnlinePayment
    {
        public string PaymentId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string UPIId { get; private set; }
        private string _transactionId;

        public UPIPayment(decimal amount, string upiId)
        {
            PaymentId = Guid.NewGuid().ToString();
            Amount = amount;
            PaymentDate = DateTime.Now;
            UPIId = upiId;
        }

        public string GetPaymentMethod() => "UPI";

        public bool ValidatePaymentDetails()
        {
            Console.WriteLine($"? Validating UPI ID: {UPIId}");
            return UPIId.Contains("@");
        }

        public bool ProcessPayment()
        {
            _transactionId = $"UPI-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            Console.WriteLine($"? Processing UPI payment of ${Amount} to {UPIId}");
            Console.WriteLine($"   Transaction ID: {_transactionId}");
            return true;
        }

        public string GetTransactionId()
        {
            return _transactionId ?? throw new InvalidOperationException("Payment not processed yet");
        }

        public bool RefundPayment()
        {
            Console.WriteLine($"? Refunding ${Amount} to UPI ID: {UPIId}");
            return true;
        }
    }

    // ✅ Cash On Delivery: Implements OfflinePayment (no online processing)
    // Only implements IOfflinePayment - doesn't pretend to be an online payment!
    public class CashOnDeliveryPayment : IOfflinePayment
    {
        public string PaymentId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string DeliveryAddress { get; private set; }
        private bool _isCollected;
        private DateTime? _collectionDate;

        public CashOnDeliveryPayment(decimal amount, string address)
        {
            PaymentId = Guid.NewGuid().ToString();
            Amount = amount;
            PaymentDate = DateTime.Now;
            DeliveryAddress = address;
            _isCollected = false;
        }

        public string GetPaymentMethod() => "Cash On Delivery";

        public string GetDeliveryAddress()
        {
            return DeliveryAddress;
        }

        public bool MarkAsCollected()
        {
            _isCollected = true;
            _collectionDate = DateTime.Now;
            Console.WriteLine($"? COD payment of ${Amount} marked as collected");
            Console.WriteLine($"   Collection Date: {_collectionDate}");
            return true;
        }

        public bool VerifyCollection(decimal collectedAmount)
        {
            if (collectedAmount == Amount)
            {
                Console.WriteLine($"? COD amount verified: ${collectedAmount}");
                return true;
            }
            else
            {
                Console.WriteLine($"? Amount mismatch! Expected: ${Amount}, Collected: ${collectedAmount}");
                return false;
            }
        }

        public bool IsCollected()
        {
            return _isCollected;
        }
    }

    /// <summary>
    /// CheckoutService that properly handles different payment types
    /// Uses LSP-compliant design - each payment type works within its contract
    /// </summary>
    public class CheckoutService
    {
        // Handle online payments (credit card, UPI, etc.)
        public bool CompleteOnlineCheckout(IOnlinePayment payment)
        {
            Console.WriteLine($"\n?? Processing online checkout for ${payment.Amount}...");
            Console.WriteLine($"   Payment Method: {payment.GetPaymentMethod()}");

            // Validate payment details
            if (!payment.ValidatePaymentDetails())
            {
                Console.WriteLine("? Payment validation failed!");
                return false;
            }

            // Process payment immediately
            if (!payment.ProcessPayment())
            {
                Console.WriteLine("? Payment processing failed!");
                return false;
            }

            // Get transaction ID
            string transactionId = payment.GetTransactionId();
            Console.WriteLine($"? Online order completed!");
            Console.WriteLine($"   Order ID: ORD-{Guid.NewGuid().ToString().Substring(0, 6)}");
            Console.WriteLine($"   Transaction ID: {transactionId}");
            return true;
        }

        // Handle offline payments (COD)
        public bool CompleteOfflineCheckout(IOfflinePayment payment)
        {
            Console.WriteLine($"\n?? Processing COD checkout for ${payment.Amount}...");
            Console.WriteLine($"   Payment Method: {payment.GetPaymentMethod()}");
            Console.WriteLine($"   Delivery Address: {payment.GetDeliveryAddress()}");

            string orderId = $"ORD-{Guid.NewGuid().ToString().Substring(0, 6)}";
            Console.WriteLine($"? COD order placed successfully!");
            Console.WriteLine($"   Order ID: {orderId}");
            Console.WriteLine($"   ?? Payment will be collected on delivery");
            return true;
        }

        // Generic method that works with any payment type
        public bool ProcessAnyPayment(IPayment payment)
        {
            Console.WriteLine($"\n?? Processing payment using {payment.GetPaymentMethod()}...");

            // Use pattern matching to handle different payment types correctly
            if (payment is IOnlinePayment onlinePayment)
            {
                return CompleteOnlineCheckout(onlinePayment);
            }
            else if (payment is IOfflinePayment offlinePayment)
            {
                return CompleteOfflineCheckout(offlinePayment);
            }
            else
            {
                Console.WriteLine("? Unknown payment type!");
                return false;
            }
        }

        // Refund only works with online payments (which support refunds)
        public bool ProcessRefund(IOnlinePayment payment, string orderId)
        {
            Console.WriteLine($"\n💰 Processing refund for order {orderId}...");
            Console.WriteLine($"   Amount: ${payment.Amount}");
            Console.WriteLine($"   Payment Method: {payment.GetPaymentMethod()}");

            bool success = payment.RefundPayment();
            if (success)
            {
                Console.WriteLine($"✅ Refund completed successfully!");
            }
            return success;
        }

        // Simulate delivery and cash collection for COD
        public bool CompleteDelivery(IOfflinePayment payment, decimal collectedAmount)
        {
            Console.WriteLine($"\n?? Completing delivery...");

            if (payment.VerifyCollection(collectedAmount))
            {
                payment.MarkAsCollected();
                Console.WriteLine($"? Delivery and payment collection completed!");
                return true;
            }
            else
            {
                Console.WriteLine($"? Delivery failed - payment amount mismatch!");
                return false;
            }
        }
    }

    /// <summary>
    /// Order management system demonstrating proper usage
    /// </summary>
    public class OrderManagementSystem
    {
        private List<IPayment> _completedPayments = new List<IPayment>();

        public void ProcessOrder(IPayment payment)
        {
            var checkoutService = new CheckoutService();
            bool success = checkoutService.ProcessAnyPayment(payment);

            if (success)
            {
                _completedPayments.Add(payment);
            }
        }

        public void DisplayPaymentSummary()
        {
            Console.WriteLine("\n?? Payment Summary:");
            Console.WriteLine($"   Total Payments: {_completedPayments.Count}");

            int onlineCount = 0;
            int offlineCount = 0;
            decimal totalAmount = 0;

            foreach (var payment in _completedPayments)
            {
                totalAmount += payment.Amount;
                if (payment is IOnlinePayment)
                    onlineCount++;
                else if (payment is IOfflinePayment)
                    offlineCount++;
            }

            Console.WriteLine($"   Online Payments: {onlineCount}");
            Console.WriteLine($"   Offline Payments: {offlineCount}");
            Console.WriteLine($"   Total Amount: ${totalAmount:F2}");
        }
    }

    public class PaymentSystemLSPDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.WriteLine("? LSP COMPLIANCE DEMO: Payment System (Good Design)");
            Console.WriteLine("???????????????????????????????????????????????????????\n");

            var checkoutService = new CheckoutService();
            var orderSystem = new OrderManagementSystem();

            // Process online payments
            Console.WriteLine("? SCENARIO 1: Online Payments");
            Console.WriteLine("?????????????????????????????????????????????????????");

            CreditCardPayment creditCard = new CreditCardPayment(299.99m, "1234567890123456", "123");
            orderSystem.ProcessOrder(creditCard);

            UPIPayment upi = new UPIPayment(499.99m, "user@paytm");
            orderSystem.ProcessOrder(upi);

            // Process offline payment
            Console.WriteLine("\n\n? SCENARIO 2: Cash On Delivery");
            Console.WriteLine("?????????????????????????????????????????????????????");

            IOfflinePayment cod = new CashOnDeliveryPayment(199.99m, "123 Main St, City");
            orderSystem.ProcessOrder(cod);

            // Simulate delivery and payment collection
            Console.WriteLine("\n\n? SCENARIO 3: Delivery & Cash Collection");
            Console.WriteLine("?????????????????????????????????????????????????????");
            checkoutService.CompleteDelivery(cod, 199.99m);

            // Process refunds - only for refundable payments
            Console.WriteLine("\n\n? SCENARIO 4: Refunds");
            Console.WriteLine("?????????????????????????????????????????????????????");

            checkoutService.ProcessRefund(creditCard, "ORD-001");
            checkoutService.ProcessRefund(upi, "ORD-002");

            // COD cannot be refunded (doesn't implement IOnlinePayment)
            // This line would not compile - caught at compile time!
            // checkoutService.ProcessRefund(cod, "ORD-003"); // ❌ Compiler error!

            Console.WriteLine("\n⚠️ Note: COD cannot be refunded because it doesn't implement IOnlinePayment");
            Console.WriteLine("   This is caught at COMPILE TIME, not runtime!");

            // Display summary
            orderSystem.DisplayPaymentSummary();

            // Demonstrate LSP compliance
            Console.WriteLine("\n\n????????????????????????????????????????????????????????");
            Console.WriteLine("? WHY THIS FOLLOWS LSP:                                ?");
            Console.WriteLine("?                                                      ?");
            Console.WriteLine("? ? Each payment type fulfills its interface contract ?");
            Console.WriteLine("? ? COD doesn't pretend to be an online payment      ?");
            Console.WriteLine("? ? No exceptions thrown - all methods work properly ?");
            Console.WriteLine("? ? Type-safe at compile time                        ?");
            Console.WriteLine("? ? Can substitute any IPayment implementation       ?");
            Console.WriteLine("?                                                      ?");
            Console.WriteLine("? THE SOLUTION: Separate interfaces for different     ?");
            Console.WriteLine("? payment characteristics (online vs offline)        ?");
            Console.WriteLine("????????????????????????????????????????????????????????\n");
        }
    }
}
