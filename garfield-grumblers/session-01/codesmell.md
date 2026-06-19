
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
