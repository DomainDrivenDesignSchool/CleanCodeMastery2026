# Clean Code Mastery 2026 — Session Report

## تسک ۱ — اضافه کردن تخفیف وفاداری بدون ریفکتور

منطق تخفیف ۵٪ برای مشتری‌های returning به کد موجود اضافه شد، بدون تمیزکاری عمدی، تا smell های جدید و قدیمی خودشون رو نشون بدن — که نشون دادن هم.

### کد نهایی

```csharp
public class OrderPriceCalculator : IOrderPriceCalculator
{
    private readonly ITaxCalculator _taxCalculator;

    public OrderPriceCalculator(ITaxCalculator taxCalculator)
    {
        _taxCalculator = taxCalculator;
    }

    public async Task<OrderPriceDto> Calculate(Order order)
    {
        var basePrice = order.GetTotalPrice();
        var tax = await _taxCalculator.Calculate(basePrice);

        decimal payAmount = basePrice + tax;

        if (order.IsReturningCustomer)
        {
            payAmount = payAmount - (payAmount * 0.05M);
        }

        if (payAmount < 0)
            payAmount = 0;

        return new OrderPriceDto
        {
            TaxAmount = tax,
            BaseAmount = basePrice,
            PayAmount = payAmount,
        };
    }
}

public class Order
{
    public int Id { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public bool IsReturningCustomer { get; set; }

    public decimal GetTotalPrice()
    {
        decimal total = 0;
        foreach (var item in Items)
        {
            total += item.Price * item.Quantity;
        }
        return total;
    }
}
```

### لیست Smell های پیدا شده

1. **عدد جادویی (Magic Number)** — `0.05M`, `50_000`, `25_000` مستقیم توی کد، بدون معنی مستند.
2. **نام‌گذاری مبهم/دو‌زبانه** — `MoafAsMaliat` برای کسی که فارسی بلد نیست غیرقابل‌فهمه.
3. **باگ نوع داده در DTO** — `OrderPriceDto.TaxAmount` از نوع `Task<decimal>` بود نه `decimal`.
4. **نوع نامناسب** — `ITaxRepository.GetRatio()` یه `int` برمی‌گردوند، نرخ مالیات همیشه گرد می‌شد.
5. **مسئولیت پراکنده (Feature Envy)** — مالیات سرویس جدا داشت، ولی منطق تخفیف مستقیم توی `Calculate` نوشته شد.
6. **عدم کپسوله‌سازی وضعیت مشتری** — `IsReturningCustomer` یه `bool` با `set` عمومی، بدون قانون کنترل‌کننده.
7. **نبود اعتبارسنجی ورودی** — چک نمی‌شد `order` یا `order.Items` نال نیستن.
8. **کلمپ‌کردن تکراری بدون انتزاع مشترک** — کلمپ مالیات و کلمپ payAmount جدا جدا نوشته شدن.
9. **شرط تو در تو در حال شکل‌گیری** — `Calculate` داره چند مسئولیت (محاسبه + تخفیف + کلمپ) رو تو یه متد جمع می‌کنه.

---

## تسک ۲ — ماتریس Impact/Effort برای Smell ها

برای هر smell پیدا‌شده، Impact (ضرر نادیده‌گرفتنش) و Effort (سختی فیکسش) سنجیده شد و توی ۴ خونه‌ی ماتریس دسته‌بندی شدن.

### 🟢 Quick Wins (High Impact / Low Effort)

| Smell | چرا Impact بالاست | چرا Effort کمه |
|---|---|---|
| `TaxAmount` از نوع `Task<decimal>` | یه باگ خاموشه، تا حالا هم await نشده | فقط تغییر تایپ + await |
| `GetRatio()` نوع `int` | مالیات همیشه گرد می‌شه | تغییر تایپ به decimal |
| نبود اعتبارسنجی ورودی | ریسک کرش در production | چند تا guard clause |
| عدد جادویی | خوانایی پایین، تغییر نرخ ریسکیه | extract به const |
| نام‌گذاری مبهم | برای بقیه‌ی تیم غیرقابل‌فهمه | rename ساده |

### 🔵 Strategic Refactors (High Impact / High Effort)

| Smell | چرا Impact بالاست | چرا Effort زیاده |
|---|---|---|
| مسئولیت پراکنده | فیچرهای مالی جدید همین‌جا روی هم انباشته می‌شن | نیاز به یه انتزاع جدید داره (`IDiscountPolicy`) |
| شرط تو در تو در حال شکل‌گیری | نشونه‌ی یه God Method در حال تولده | نیاز به Extract Method + تست رگرسیون |

### ⚪ Low Priority (Low Impact / Low Effort)

| Smell | توضیح |
|---|---|
| `IsReturningCustomer` بدون کپسوله‌سازی | امروز مشکلی نمی‌سازه، ولی ارزش فیکس شدن رو داره |

### 🔴 Avoid/Defer

موردی شناسایی نشد.

**نتیجه:** ۵ مورد Quick Win باید اول فیکس بشن، ۲ مورد Strategic باید planned بشن.

---

## تسک ۳ — ماتریس Impact/Effort برای تکنیک‌های ریفکتورینگ

به‌جای مشکلات، خود تکنیک‌های ریفکتورینگ (راه‌حل‌ها) بر اساس تاثیرشون و سختی اجراشون رتبه‌بندی شدن تا بعداً با smell های مناسب جفت بشن.

### 🟢 Quick Wins

| تکنیک | توضیح کوتاه | کاربرد در کد ما |
|---|---|---|
| Replace Magic Number with Symbolic Constant | عدد خام → const معنادار | `0.05M` → `LoyaltyDiscountRate` |
| Rename Method/Variable | اسم مبهم → اسم گویا | `MoafAsMaliat` → `IsTaxExempt` |
| Introduce Guard Clause | چک‌های اعتبارسنجی اول متد | nullcheck روی `order`/`Items` |
| Fix Type Mismatch | تایپ غلط → تایپ درست | `Task<decimal>` → `decimal`، `int` → `decimal` |
| Consolidate Duplicate Conditional Fragments | منطق تکراری یه‌جا جمع بشه | کلمپ‌های تکراری |

### 🔵 Strategic Refactors

| تکنیک | توضیح کوتاه | کاربرد در کد ما |
|---|---|---|
| Extract Class / Introduce Strategy | مسئولیت قاطی‌شده → سرویس مستقل | `IDiscountCalculator` جدا از `OrderPriceCalculator` |
| Replace Conditional with Polymorphism | if/else روی نوع → کلاس‌های جدا | وقتی چند نوع تخفیف داشته باشیم |
| Extract Method | متد بزرگ → چند متد کوچیک | شکستن `Calculate` |
| Introduce Value Object | primitive → کلاس دامنه | `Money` Value Object |
| Encapsulate Field | فیلد عمومی → private set + متد | `IsReturningCustomer` |

**نتیجه:** هر Quick Win smell یه Quick Win refactor مستقیم داره؛ smell های Strategic هم refactor Strategic نیاز دارن — نمی‌شه میان‌بر زد.

---

## تسک ۴ — نگاشت Smell ها به سطوح بیولوژیک

هر smell بر اساس محل وقوعش (فیلد تا کل سیستم) به یکی از ۵ سطح Atom/Molecule/Cell/Tissue/Organism نگاشت شد تا مشخص بشه مشکلات محلی‌ان یا معماری کل پروژه رو تحت تاثیر می‌ذارن.

| Smell | سطح | چرا |
|---|---|---|
| عدد جادویی | **Atom** | فقط یه مقدار خامه، مستقل از بقیه |
| نام‌گذاری مبهم | **Atom** | مشکل محلیه، فقط روی یه اسم |
| `Task<decimal> TaxAmount` | **Atom** | تایپ غلط، محدود به تعریف فیلد |
| `GetRatio()` نوع `int` | **Atom → Molecule** | اثرش می‌ره روی متد محاسبه هم |
| نبود اعتبارسنجی ورودی | **Molecule** | رفتار کل متد رو تحت تاثیر می‌ذاره |
| کلمپ تکراری | **Molecule** | یه الگوی تکراری داخل متده |
| شرط تو در تو | **Molecule** | ساختار داخلی متد رو پیچیده می‌کنه |
| `IsReturningCustomer` بدون کپسوله‌سازی | **Cell** | به کل کلاس `Order` مربوطه |
| مسئولیت پراکنده | **Cell → Tissue** | داره مرز بین Pricing و Discount Policy رو تهدید می‌کنه |

**نتیجه:** اکثر smell ها Atom/Molecule (محلی)‌ان. فقط «مسئولیت پراکنده» داره به سمت Tissue می‌ره — هم‌خوان با نتیجه‌ی ماتریس Impact/Effort که همین مورد Strategic بود.

---

## تسک ۵ — Precondition / Postcondition / Invariant

برای متدهای اصلی، قوانین ورودی (precondition)، تضمین خروجی (postcondition) و قانون همیشگی کلاس (invariant) با `if/throw` مستقیم توی کد پیاده‌سازی شدن، نه فقط کامنت.

### کد کامل نهایی

```csharp
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
```

### جدول قراردادها

| نوع قرارداد | مثال از کد ما | چی رو تضمین می‌کنه |
|---|---|---|
| Precondition | `order != null`, `basePrice >= 0` | ورودی متد قبل از اجرا معتبره |
| Postcondition | `PayAmount >= 0`, `TaxAmount <= MaximumTaxAmount` | خروجی متد قوانین کسب‌وکار رو نقض نکرده |
| Invariant | `TotalAmount >= 0` در `Order` | همیشه، در هر لحظه از حیات کلاس، برقراره |

---

## تسک ۶ — بررسی کد Payment از Session ۱
## تسک ۷ — تداخل Basket و Order

بررسی شد که وقتی Basket (سبد قبل از پرداخت) و Order (سفارش نهایی) با هم قاطی بشن چه state های نامعتبری ممکن می‌شه، با ۵ مثال عینی از گذارهای وضعیت پنهان‌شده.

1. **فیلد `IsSubmitted` روی خود Basket** — یعنی یه Basket می‌تونه هم «هنوز در حال خریده» باشه هم «ثبت شده». این‌ها در واقع دو تا state کاملاً متفاوتن که باید با دو کلاس جدا مدل بشن، نه یه bool.
2. **`PaymentStatus` نال‌پذیر روی Basket** — قبل از پرداخت این فیلد باید نال باشه؛ یعنی «نال بودن» عملاً یه state ضمنیه (unpaid) که هیچ‌جا صریح تعریف نشده.
3. **`Order.Items` که مستقیماً از `Basket.Items` کپی می‌شه، بدون snapshot جدا** — اگه بعداً قیمت محصول عوض بشه، سفارش قدیمی هم عوض می‌شه؛ گذار «سبد خرید» به «سفارش نهایی‌شده» باید قیمت رو freeze کنه، ولی این الان مخفیه.
4. **متد `Basket.Checkout()` که هم Order می‌سازه هم خود Basket رو خالی می‌کنه** — دو تا مسئولیت (ساخت سفارش + پاک کردن سبد) قاطی یه متد شدن؛ اگه یکیش fail بشه، معلوم نیست state نهایی سبد و سفارش سازگاره یا نه.
5. **`OrderId` روی خود Basket (nullable FK)** — یعنی یه Basket می‌تونه به یه Order اشاره کنه یا نکنه؛ این عملاً گذار «سبد فعال» → «سبد تبدیل‌شده به سفارش» رو با یه فیلد نال‌پذیر پنهون می‌کنه، به‌جای این‌که این گذار به‌صورت صریح (مثلاً یه متد `ConvertToOrder()` که یه Order برمی‌گردونه و Basket رو غیرفعال می‌کنه) مدل بشه.
