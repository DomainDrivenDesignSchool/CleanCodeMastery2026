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
        bool isReturningCustomer = bool.Parse(Console.ReadLine()!);

        var order = new Order(basePrice, discount);

        var discountService = new DiscountService(order);

        decimal priceAfterDiscount = discountService.CalculateDiscount(isReturningCustomer);

        var taxService = new TaxService();

        var taxRequest = new CalculateTaxRequest(priceAfterDiscount, taxRatio);

        decimal tax = taxService.CalculateTax(taxRequest);

        decimal total = Math.Max(0, priceAfterDiscount + tax);

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
    private const decimal MaxTax = 25_000M;
    private const decimal MinimumPrice = 50M;

    public decimal CalculateTax(CalculateTaxRequest request)
    {
        if (request.BasePrice < 0)
            throw new ArgumentException("Tax base price must be non-negative", nameof(request));

        if (request.TaxRatio < 0)
            throw new ArgumentException("Tax ratio must be non-negative", nameof(request));

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

    public decimal CalculateDiscount(bool isReturningCustomer)
    {

        if (isReturningCustomer)
            return Math.Max(0, _order.BasePrice - LoyaltyDiscount());

        return Math.Max(0, _order.BasePrice - _order.Discount);
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
        if (basePrice < 0)
            throw new ArgumentException("Base price must be non-negative", nameof(basePrice));

        if (discount < 0)
            throw new ArgumentException("Discount must be non-negative", nameof(discount));

        BasePrice = basePrice;
        Discount = discount;
    }
}
