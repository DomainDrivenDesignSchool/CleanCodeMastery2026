public class OrderPriceCalculator : IOrderPriceCalculator
{
    private readonly ITaxCalculator _taxCalculator;

    public OrderPriceCalculator(ITaxCalculator taxCalculator)
    {
        _taxCalculator = taxCalculator ?? throw new ArgumentNullException(nameof(taxCalculator));
    }

    public async Task<OrderPriceDto> Calculate(Order order)
    {
        // --- Precondition ---
        if (order is null)
            throw new ArgumentNullException(nameof(order), "Order نمی‌تونه null باشه.");
        if (order.Items is null || !order.Items.Any())
            throw new InvalidOperationException("Order باید حداقل یه آیتم داشته باشه.");

        var basePrice = order.GetTotalPrice();
        var tax = await _taxCalculator.Calculate(basePrice);

        decimal payAmount = basePrice + tax;

        if (order.IsReturningCustomer)
        {
            payAmount -= payAmount * DiscountRules.LoyaltyDiscountRate;
        }

        if (payAmount < 0)
            payAmount = 0;

        var result = new OrderPriceDto
        {
            TaxAmount = tax,
            BaseAmount = basePrice,
            PayAmount = payAmount,
        };

        // --- Postcondition ---
        if (result.PayAmount < 0)
            throw new InvalidOperationException("Postcondition نقض شد: PayAmount نباید منفی باشه.");
        if (result.TaxAmount > TaxRules.MaximumTaxAmount)
            throw new InvalidOperationException("Postcondition نقض شد: TaxAmount نباید از سقف مجاز بیشتر بشه.");

        return result;
    }
}

public interface ITaxCalculator
{
    Task<decimal> Calculate(decimal basePrice);
}

public static class TaxRules
{
    public const decimal MinimumTaxExemptAmount = 50_000M;
    public const decimal MaximumTaxAmount = 25_000M;
}

public static class DiscountRules
{
    public const decimal LoyaltyDiscountRate = 0.05M;
}

public class TaxCalculator : ITaxCalculator
{
    private readonly ITaxRepository _taxRepository;

    public TaxCalculator(ITaxRepository taxRepository)
    {
        _taxRepository = taxRepository ?? throw new ArgumentNullException(nameof(taxRepository));
    }

    public async Task<decimal> Calculate(decimal basePrice)
    {
        // --- Precondition ---
        if (basePrice < 0)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "basePrice نمی‌تونه منفی باشه.");

        if (IsTaxExempt(basePrice))
            return decimal.Zero;

        var calculatedTaxAmount = await CalculateTaxAmount(basePrice);
        var result = calculatedTaxAmount > TaxRules.MaximumTaxAmount
            ? TaxRules.MaximumTaxAmount
            : calculatedTaxAmount;

        // --- Postcondition ---
        if (result < 0 || result > TaxRules.MaximumTaxAmount)
            throw new InvalidOperationException("Postcondition نقض شد: مالیات باید بین صفر و سقف مجاز باشه.");

        return result;
    }

    private async Task<decimal> CalculateTaxAmount(decimal basePrice)
    {
        decimal taxRatio = await _taxRepository.GetRatio();
        return basePrice * taxRatio;
    }

    private static bool IsTaxExempt(decimal basePrice)
    {
        return basePrice < TaxRules.MinimumTaxExemptAmount;
    }
}

public interface IOrderPriceCalculator
{
    Task<OrderPriceDto> Calculate(Order order);
}

public class Order
{
    private decimal _totalAmount;

    public int Id { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public bool IsReturningCustomer { get; set; }

    public decimal GetTotalPrice()
    {
        EnsureInvariant();

        decimal total = 0;
        foreach (var item in Items)
        {
            if (item.Quantity < 0)
                throw new InvalidOperationException(
                    $"Invariant نقض شد: Quantity آیتم {item.ProductId} نمی‌تونه منفی باشه.");
            if (item.Price < 0)
                throw new InvalidOperationException(
                    $"Invariant نقض شد: Price آیتم {item.ProductId} نمی‌تونه منفی باشه.");

            total += item.Price * item.Quantity;
        }

        _totalAmount = total;
        EnsureInvariant();

        return total;
    }

    private void EnsureInvariant()
    {
        // Invariant: مجموع سفارش هیچ‌وقت نباید منفی باشه
        if (_totalAmount < 0)
            throw new InvalidOperationException("Invariant نقض شد: TotalAmount نمی‌تونه منفی باشه.");
    }
}

public class OrderItem
{
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
}

public interface ITaxRepository
{
    Task<decimal> GetRatio();
}

public record OrderPriceDto
{
    public decimal TaxAmount { get; internal set; }
    public decimal BaseAmount { get; internal set; }
    public decimal PayAmount { get; internal set; }
}
