

# Detected Code Smells


### RequestHandlerWrapper.cs

1. Class Naming

Line Number: 15, 21
Smell Description: 
هر دو کلاس `RequestHandlerWrapper<TResponse>` و `RequestHandlerWrapper` نام مشابهی دارند و تشخیص اینکه در این Wrap کردن چه functionality یا تغییری در رفتار این کلاس ایجاد شده است، سخت است و بر عهده خواننده کد است تا آن را کشف کند.

Why it's problem:

برای خواندن کد بار شناختی بر روی دوش خواننده ایجاد میکند.


2. Naming Smell
Line Number: 58
Smell Description:
فیلد cancellationToken با نام t تعریف شده است و خب میتواند این متغیر با نام اصلی خود تعریف شود تا باعث بهبود readability شود.

Proposed Refactor:

تغییر نام این متغیر به cancellationToken

3. Obscure Code and Method Chain

```
return serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResponse>) Handler,
                (next, pipeline) => (t) => pipeline.Handle((TRequest) request, next, t == default ? cancellationToken : t))();
```

Line Number: 40
Smell Description: 

متودها به هم chain شدند و دارند یک عملیاتی رو انجام میدن و خب فهمیدن کد رو سخت میکنه.

Why it's a problem:

- اینکه فهم این کد رو سخت میکنه و فهم سخت باعث میشه دیباگ کردن این کد سخت بشه
- اینکه methodها به هم chain شدند باعث میشه تشخیص اینکه الان Type داده‌ای که method داره روش اجرا میشه رو سخت بفهمیم و باز این مورد هم برای دیباگ و غیره باعث دردسر میشه.

```
IEnumerable<IPipelineBehavior<TRequest, TResponse>> pipeLines = serviceProvider
.GetServices<IPipelineBehavior<TRequest, TResponse>>()
.Reverse();

RequestHandlerDelegate<TResponse> next = Handler;

foreach (var pipeline in pipeLines)
{
    RequestHandlerDelegate<TResponse> currentNext = next;

    next = ct =>
    {
        CancellationToken cToken = ct == CancellationToken.None ? cancellationToken : ct;
        return pipeline.Handle((TRequest) request, currentNext, cToken);
    };
}

return next(cancellationToken);
```


### Mediator.cs

1. **Duplicate Code**
Line Number: 57, 75
Smell Description:

قطعه کد زیر در دو خط مربوطه duplicate شده است:
```
var handler = (RequestHandlerWrapper<TResponse>)_requestHandlers.GetOrAdd(request.GetType(), static requestType =>
{
    var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
    var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
    return (RequestHandlerBase)wrapper;
});
```

Why it's a problem:

هر تغییری در signature متودهایی مانند GetOrAdd یا سایر متودها باعث break شدن و شکسته شدن کدهای این خط ها میشود.

Proposed Refactor: بردن این منطق در یک متود واحد


2. **Dependent on Concurrent Dictionary**

Smell Description:

در این کلاس در قسمت‌های مختلفی از متود GetOrAdd استفاده شده است. این متود برای خود Concurrent Dictionary است و عملا ما در قسمت‌های مختلفی از کد به این type وابسته ایم و این وابستگی به هیچ وجه isolate نیست و میتواند باعث break شدن کد شود.

Propose Refactor:

ایزوله کردن این dependency در یک متود.


3. Single Responsibility in Send Method

Smell Description: 
متود send در حال حاضر کارهایی فراتر از کار اصلی خود انجام میدهد. کارهایی مانند ساخت Wrapper Type و غیره.

Proposed Refactor:

اسکترکت متود و سپردن این وظایف به سایر متودها. در نگاه اول شاید این کلاس به طور کل تعداد خط کد کمی داشته باشد، اما خیلی سریع خواننده وارد جزئیاتی میشود که بدون فهیمدین آنها نمیتواند متوجه شود چه اتفاقی در کد در حال رخ دادن است.

# Clean Code Checklist


1. در حوزه نام گذاری
    - دقیق، شفاف و مطابق با زبان کسب و کار باشد
    - از اسامی خلاصه شده و مخفف‌های ناشناخته استفاده نکنیم. مثال: cancellationToken=>t
    - متودهای کامند باید ادبیات دستوری داشته باشند و متودهای کوئری ادبیات غیر دستوری و از جنس کوئری
    - برای متغیرهایی که واحد دارند، حتما واحد ذکر شود. مثال: 

Timeout = 60 ❌
TimeoutInSeconds = 60 ✅

2. شرط‌هایی که از جنس کسب و کار اند، در داخل خود if statement نوشته نشوند و در قالب یک متغیر با مفهوم کسب و کاری قرار بگیرند:

```
if (user.LastTicketCancellationTime > YesterDay)
{
    //Do Some Logic
}
```

```
bool userHasCancelledTicketInLast24Hours = user.LastTicketCancellationTime > YesterDay;

if (userHasCancelledTicketInLast24Hours)
{
    //Do Some Logic
}
```


3. سطح انتزاع خط‌های کد یک متود، کم و زیاد نشود
4. تعداد خط کدهای متودها زیاد نباشد و برای کمتر کردن آن از extract method استفاده شود (با دیدگاه منطقی و بر اساس اصولی مانند Single Responsibility)
5. جایی که حس میکنیم کد نیاز به Comment داره، جایی هست که باید بیشتر راجع به کد فکر کنیم. شاید جایی بد نوشته شده یا مشکل داره. در واقع یک bad smell است که میتونه سرنخ یک مشکل اساسی باشه. و این یک رهیافت مهم است.

6. زمانی که از سمت راست assignment نمیتوان تایپ سمت چپ آن را تشخیص داد، از var استفاده نشود.

```
var cancelledTicket = GetCancelledTicket();
```

```
List<TickentDto> cancelledTicket = GetCancelledTicket();
```

6. متودهای ما تعداد پارامترهای زیادی نداشته باشند (Long Parameter List )
7. متودها  chain نشوند (مرتبط با اصولی مانند LawOfDemeter) (Method Chain bad smell)
8. عنوان Exception ها باید دقیق و مرتبط با اتفاق رخ داده و منطبق با ادبیات کسب و کار باشد و متن پیام هم باید شفاف باشد.
9. ماژول‌ها باید در ارتباط با هم سطح انتزاع بالایی داشته باشند تا کمترین Coupling را نسبت به هم داشته باشند و صرفا در حد دانشی که نیاز دارند، از ماژول دیگر اطلاع داشته باشند، نه بیشتر.