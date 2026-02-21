

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