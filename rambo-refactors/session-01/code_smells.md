# Code Smells — فایل‌های `routing.py` و `utils.py`

---

## ۱. تکرار ۳,۰۰۰ خطی متدهای HTTP در `routing.py`

**محل:** خط ۱۴۳۵ تا ۴۴۷۴  

متدهای `get()`, `put()`, `post()`, `delete()`, `options()`, `head()`, `patch()`, `trace()` کاملاً کپی‌پیست هم‌دیگن. هر کدوم حدود ۳۷۵ خطن و تنها تفاوتشون یه خطه:

```python
methods=["GET"]    # توی get()
methods=["PUT"]    # توی put()
methods=["POST"]   # توی post()
# ...
```

از ۴,۵۰۰ خط کل فایل، حدود ۳,۰۰۰ خطش (۶۸٪) فقط تکراره. اگه پارامتر جدید اضافه بشه یا باگی فیکس بشه، باید ۸ جا عوض بشه.

---

## ۲. God Function: `get_request_handler()`

**محل:** `routing.py` خط ۲۴۸ تا ۴۰۳ (~۱۵۵ خط)  

یه تابع که ۶ تا مسئولیت مختلف داره: parse کردن body، تشخیص content-type، مدیریت خطای JSON، حل dependency ها، اجرای endpoint، و ساختن response.

---

## ۳. try/except سه‌لایه تو در تو در `ensure_multipart_is_installed()`

**محل:** `utils.py` خط ۸۵ تا ۱۰۹  

سه لایه try/except تو در تو که دنبال کردن flow خطاهاش سخته. علاوه بر این وقتی پکیج اصلی نصب نیست، بیخودی version پکیج legacy رو چک میکنه .

---

## ۴. متد `get_dependant()` بزرگ و چندوظیفه‌ای

**محل:** `utils.py` خط ۲۵۱ تا ۳۲۳  

توی یه حلقه `for` سه تا کار متفاوت انجام میده با الگوی if/continue:
- هندل کردن `Depends` (با validate scope و استخراج OAuth)
- تشخیص تایپ‌های خاص مثل `Request` و `WebSocket`
- مسیریابی فیلدهای عادی به body یا query/path/header/cookie

---

## ۵. صدا زدن بازگشتی بدون مکانیزم قطع

**محل:** `utils.py` — توابع `get_dependant()` خط ۲۹۸، `get_flat_dependant()` خط ۱۶۴، `solve_dependencies()` خط ۶۱۴  

هر سه تابع خودشون رو recursive صدا میزنن تا nested dependency ها رو پردازش کنن ولی depth limit یا چک circular dependency مشخصی ندارن. اگه یه dependency به صورت دایره‌ای به خودش اشاره کنه، بدون پیام خطای واضح میره توی infinite recursion.

نکته: `get_flat_dependant()` یه لیست `visited` داره ولی فقط وقتی `skip_repeats=True` باشه ازش استفاده میکنه. توی حالت پیش‌فرض عملاً بلااستفاده‌ست.

با دقت بیشتر بررسی نکردیم — ممکنه جای دیگه‌ای محافظتی وجود داشته باشه.

---

## ۶. توابع recursive بدون محدودیت

**فایل:** `utils.py`

سه تا تابع خودشون رو recursive صدا میزنن:
- `get_dependant()` خط ۲۹۸ خودش رو صدا میزنه
- `get_flat_dependant()` خط ۱۶۴ خودش رو صدا میزنه
- `solve_dependencies()` خط ۶۱۴ خودش رو صدا میزنه

هیچ‌کدوم depth limit ندارن. یعنی اگه یه dependency دایره‌ای باشه و به خودش اشاره کنه، میره توی infinite recursion و stack overflow میده بدون اینکه خطای واضحی بده.

`get_flat_dependant()` یه لیست `visited` داره ولی فقط وقتی `skip_repeats=True` باشه چکش میکنه. حالت پیش‌فرضش `False` هست یعنی عملاً بلااستفاده‌ست.

البته با دقت بیشتر ندیدیم، شاید جای دیگه‌ای یه محافظتی باشه.

---