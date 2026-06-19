internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Enter Base Price: must be more than 0");
        decimal basePrice = decimal.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter Discount:");
        decimal discount = decimal.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter Tax Ratio (example: 0.20):");
        decimal taxRatio = decimal.Parse(Console.ReadLine()!);

        Console.WriteLine("Is customer returning? (true/false):");
        bool customerReturn = bool.Parse(Console.ReadLine()!);

        // Precondition
        if (basePrice < 0)
            throw new ArgumentException("BasePrice must be non-negative");

        var order = new Order(basePrice, discount);

        var discountService = new DiscountService(order);

        decimal priceAfterDiscount = discountService.CalculateDiscount(customerReturn);

        var taxService = new TaxService();

        var taxRequest = new CalculateTaxRequest(priceAfterDiscount, taxRatio);

        decimal tax = taxService.CalculateTax(taxRequest);

        decimal total = priceAfterDiscount + tax;

        if (total < 0)
            throw new Exception("Total cannot be less than zero");

        Console.WriteLine("=================================");
        Console.WriteLine($"Base Price: {basePrice}");
        Console.WriteLine($"Discount: {discount}");
        Console.WriteLine($"Price After Discount: {priceAfterDiscount}");
        Console.WriteLine($"Tax: {tax}");
        Console.WriteLine($"Final Total: {total}");
    }
}

/* ======================= DOMAIN ======================= */

public record CalculateTaxRequest(decimal BasePrice, decimal TaxRatio);

public class TaxService
{
    //invariant condition
    private const decimal MaxTax = 25M;
    private const decimal MinimumPrice = 50M;

    public decimal CalculateTax(CalculateTaxRequest request)
    {
        if (request.BasePrice < MinimumPrice)
            return 0;

        decimal taxableAmount = request.BasePrice ;
        decimal tax = taxableAmount * request.TaxRatio;

        if (tax > MaxTax)
            tax = MaxTax;

        return tax;
    }
}

public class DiscountService
{
    //invariant condition
    private const decimal LoyaltyDiscountRate = 0.05M;
    private readonly Order _order;

    public DiscountService(Order order)
    {
        _order = order;
    }

    public decimal CalculateDiscount(bool customerReturn)
    {

        if (customerReturn)
            return _order.BasePrice - LoyaltyDiscount();

        return _order.BasePrice - _order.Discount;
    }

    private decimal LoyaltyDiscount()
    {
        return _order.BasePrice * LoyaltyDiscountRate;
    }
}

public class Order
{
    public decimal BasePrice { get; }
    public decimal Discount { get; }

    public Order(decimal basePrice, decimal discount)
    {
        BasePrice = basePrice;
        Discount = discount;
    }
}