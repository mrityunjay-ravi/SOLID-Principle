using System;

namespace SOLIDPrinciples.LSP.WithoutLSP
{
    // ? VIOLATES LSP: Base class assumes ALL payments can be processed online
    // This design forces CashOnDelivery to implement methods it cannot support

    /// <summary>
    /// Bad abstraction - assumes all payments work the same way
    /// </summary>
    public abstract class Payment
    {
        public string PaymentId { get; protected set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public Payment(decimal amount)
        {
            PaymentId = Guid.NewGuid().ToString();
            Amount = amount;
            PaymentDate = DateTime.Now;
        }

        // ? Problem: Assumes all payments can be validated online
        public abstract bool ValidatePaymentDetails();

        // ? Problem: Assumes all payments are processed immediately online
        public abstract bool ProcessPayment();

        // ? Problem: Assumes all payments have online transaction IDs
        public abstract string GetTransactionId();

        // ? Problem: Assumes all payments can be refunded immediately
        public abstract bool RefundPayment();
    }

    // ? This works fine - Credit card is processed online
    public class CreditCardPayment : Payment
    {
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        private string _transactionId;

        public CreditCardPayment(decimal amount, string cardNumber, string cvv) 
            : base(amount)
        {
            CardNumber = cardNumber;
            CVV = cvv;
        }

        public override bool ValidatePaymentDetails()
        {
            Console.WriteLine($"? Validating credit card: {CardNumber.Substring(CardNumber.Length - 4)}");
            return !string.IsNullOrEmpty(CardNumber) && CVV.Length == 3;
        }

        public override bool ProcessPayment()
        {
            _transactionId = $"CC-{Guid.NewGuid().ToString().Substring(0, 8)}";
            Console.WriteLine($"? Processing credit card payment of ${Amount}");
            Console.WriteLine($"   Transaction ID: {_transactionId}");
            return true;
        }

        public override string GetTransactionId()
        {
            return _transactionId;
        }

        public override bool RefundPayment()
        {
            Console.WriteLine($"? Refunding ${Amount} to credit card ending in {CardNumber.Substring(CardNumber.Length - 4)}");
            return true;
        }
    }

    // ? This works fine - UPI is processed online
    public class UPIPayment : Payment
    {
        public string UPIId { get; set; }
        private string _transactionId;

        public UPIPayment(decimal amount, string upiId) 
            : base(amount)
        {
            UPIId = upiId;
        }

        public override bool ValidatePaymentDetails()
        {
            Console.WriteLine($"? Validating UPI ID: {UPIId}");
            return UPIId.Contains("@");
        }

        public override bool ProcessPayment()
        {
            _transactionId = $"UPI-{Guid.NewGuid().ToString().Substring(0, 8)}";
            Console.WriteLine($"? Processing UPI payment of ${Amount} to {UPIId}");
            Console.WriteLine($"   Transaction ID: {_transactionId}");
            return true;
        }

        public override string GetTransactionId()
        {
            return _transactionId;
        }

        public override bool RefundPayment()
        {
            Console.WriteLine($"? Refunding ${Amount} to UPI ID: {UPIId}");
            return true;
        }
    }

    // ? THIS VIOLATES LSP!
    // CashOnDelivery cannot fulfill the Payment contract
    public class CashOnDeliveryPayment : Payment
    {
        public string DeliveryAddress { get; set; }

        public CashOnDeliveryPayment(decimal amount, string address) 
            : base(amount)
        {
            DeliveryAddress = address;
        }

        // ? VIOLATION: Cannot validate payment details upfront (cash not received yet)
        public override bool ValidatePaymentDetails()
        {
            throw new NotSupportedException("? Cannot validate COD payment upfront - payment is collected on delivery!");
        }

        // ? VIOLATION: Payment is NOT processed online - it's collected later
        public override bool ProcessPayment()
        {
            throw new NotSupportedException("? COD payment is not processed online - cash is collected at delivery time!");
        }

        // ? VIOLATION: No online transaction ID for COD
        public override string GetTransactionId()
        {
            throw new NotSupportedException("? COD payments don't have online transaction IDs!");
        }

        // ? VIOLATION: Cannot refund something not yet paid
        public override bool RefundPayment()
        {
            throw new NotSupportedException("? Cannot refund COD - payment hasn't been collected yet!");
        }
    }

    /// <summary>
    /// CheckoutService expects all Payment objects to work the same way
    /// This breaks when CashOnDeliveryPayment is used
    /// </summary>
    public class CheckoutService
    {
        public bool CompleteCheckout(Payment payment)
        {
            Console.WriteLine($"\n?? Starting checkout for ${payment.Amount}...");

            try
            {
                // Try to validate payment
                if (!payment.ValidatePaymentDetails())
                {
                    Console.WriteLine("? Payment validation failed!");
                    return false;
                }

                // Try to process payment
                if (!payment.ProcessPayment())
                {
                    Console.WriteLine("? Payment processing failed!");
                    return false;
                }

                // Try to get transaction ID
                string transactionId = payment.GetTransactionId();
                Console.WriteLine($"? Order completed! Transaction: {transactionId}");
                return true;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"? ERROR: {ex.Message}");
                return false;
            }
        }

        public bool ProcessRefund(Payment payment, string orderId)
        {
            Console.WriteLine($"\n?? Processing refund for order {orderId}...");

            try
            {
                payment.RefundPayment();
                return true;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"? ERROR: {ex.Message}");
                return false;
            }
        }
    }

    public class PaymentSystemLSPViolationDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.WriteLine("? LSP VIOLATION DEMO: Payment System (Bad Design)");
            Console.WriteLine("???????????????????????????????????????????????????????\n");

            var checkoutService = new CheckoutService();

            // ? Online payments work fine
            Console.WriteLine("--- 1. Credit Card Payment (Works Fine) ---");
            Payment creditCard = new CreditCardPayment(299.99m, "1234567890123456", "123");
            checkoutService.CompleteCheckout(creditCard);

            Console.WriteLine("\n--- 2. UPI Payment (Works Fine) ---");
            Payment upi = new UPIPayment(499.99m, "user@paytm");
            checkoutService.CompleteCheckout(upi);

            // ? COD breaks the contract
            Console.WriteLine("\n--- 3. Cash On Delivery (BREAKS LSP!) ---");
            Payment cod = new CashOnDeliveryPayment(199.99m, "123 Main St");
            checkoutService.CompleteCheckout(cod); // Will throw exceptions!

            // ? Refund also breaks
            Console.WriteLine("\n--- 4. Attempting Refunds ---");
            checkoutService.ProcessRefund(creditCard, "ORD001");
            checkoutService.ProcessRefund(cod, "ORD002"); // Will throw exception!

            Console.WriteLine("\n????????????????????????????????????????????????????????");
            Console.WriteLine("? WHY THIS VIOLATES LSP:                               ?");
            Console.WriteLine("?                                                      ?");
            Console.WriteLine("? 1. CashOnDelivery cannot substitute Payment         ?");
            Console.WriteLine("? 2. Throws exceptions instead of working properly    ?");
            Console.WriteLine("? 3. CheckoutService breaks with COD payments         ?");
            Console.WriteLine("? 4. Cannot use COD wherever Payment is expected      ?");
            Console.WriteLine("?                                                      ?");
            Console.WriteLine("? THE PROBLEM: Base class assumes all payments are    ?");
            Console.WriteLine("? processed online immediately. COD is different!     ?");
            Console.WriteLine("????????????????????????????????????????????????????????\n");
        }
    }
}
