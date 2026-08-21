# یافته‌های رفکتور MVVM — NewPersonView

> **وضعیت:** رفکتور متوقف و به baseline `37ccb10` برگردانده شد.
> **برنچ backup:** `backup/newperson-mvvm-attempt` (نقطه پایانی: `4ecf7fa`)
> **تاریخ:** ۲۰ مرداد ۱۴۰۵

---

## ۱. باگ‌های Preexisting کشف‌شده

### ۱.۱ IsValidMobile حذف‌شده در 37ccb10

در نسخه اصلی (`bc6b842`) تابع `IsValidMobile` وجود داشت که اعتبارسنجی پیشرفته‌ای انجام می‌داد:

```csharp
// نسخه اصلی (bc6b842)
private static bool IsValidMobile(string mobile)
{
    if (string.IsNullOrWhiteSpace(mobile)) return false;
    mobile = mobile.Trim().Replace(" ", "").Replace("-", "");
    if (mobile.StartsWith("+98")) mobile = "0" + mobile.Substring(3);
    else if (mobile.StartsWith("0098")) mobile = "0" + mobile.Substring(4);
    return mobile.Length == 11 && mobile.StartsWith("09") && mobile.All(char.IsDigit);
}
```

**مشکل:** در `37ccb10` کامل حذف شد و فقط با `Mobile.Count(char.IsDigit) != 11` جایگزین شد. این یعنی:
- شماره‌های با پیشوند `+98` یا `0098` بدون تبدیل بررسی می‌شوند (و رد می‌شوند چون طولشان != 11)
- شماره‌هایی مثل `12345678901` (بدون پیشوند `09`) قبول می‌شوند
- **وضعیت فعلی:** هنوز برطرف نشده

### ۱.۲ IsValidNationalCode — الگوریتم Check digit حذف‌شده

در `bc6b842` الگوریتم checksum رسمی کد ملی وجود داشت:

```csharp
// نسخه اصلی (bc6b842)
int[] weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
int sum = 0;
for (int i = 0; i < 9; i++)
    sum += (code[i] - '0') * weights[i];
int remainder = sum % 11;
int checkDigit = remainder < 2 ? remainder : 11 - remainder;
return checkDigit == (code[9] - '0');
```

**مشکل:** در `37ccb10` الگوریتم حذف شد و فقط `return true` باقی ماند. یعنی هر کد ۱۰ رقمی‌ای معتبر شمرده می‌شد.
**وضعیت فعلی:** هنوز برطرف نشده. در تلاش رفکتور (کامیت `4ecf7fa`) بازگردانده شد اما با revert از بین رفت.

### ۱.۳ عدم تطابق اعتبارسنجی کد ملی بین NewPersonView و EditPersonView

- `NewPersonView.xaml.cs`: فقط بررسی ۱۰ رقم + `IsValidNationalCode` بدون checksum
- `EditPersonView.xaml.cs`: بررسی ۱۰ رقم + checksum + رد کردن کدهای تکراری

یعنی شخص جدید با کد ملی نامعتبر ثبت می‌شد اما ویرایش آن خطا می‌داد.

### ۱.۴ MainDefaultToggle — Dead Code

تگل شخص حقیقی/حقوقی در کد code-behind وجود داشت اما:
- رویداد `SelectionChanged` به handler متصل بود که فقط `IsLegal` را تنظیم می‌کرد
- نمایش/مخفی کردن پنل‌ها مستقیماً در code-behind انجام می‌شد (خارج از MVVM)

---

## ۲. نکات معماری کشف‌شده

### ۲.۱ الگوی XAML Attribute → Command Binding

حین انتقال از code-behind به MVVM، attributeهای قدیمی مثل `SelectionChanged` در XAML باید حذف شوند وگرنه `XamlParseException` رخ می‌دهد. مثال:

```xml
<!-- قبل از پاک‌سازی - خطای XamlParseException -->
<ComboBox SelectionChanged="OnBranchChanged" ... />

<!-- بعد از پاک‌سازی - صحیح -->
<ComboBox SelectedItem="{Binding SelectedBranch, Mode=TwoWay}" ... />
```

**قانون:** هر attribute رویدادی که به handler در code-behind متصل است، پس از انتقال به Command/Binding باید حذف شود.

### ۲.۲ Pattern: MVVM با Code-Behind Hybrid

برخی UI interactions ذاتاً به code-behind نیاز دارند (مثل اورراید شهر/استان، فیلتر autocomplete). الگوی بهینه:

- **ViewModel:** رویدادهایی مثل `OnSelectedProvinceChanged` تعریف کند
- **Code-behind:** subscribe کند و UI را آپدیت کند
- این hybrid approach بهتر از تلاش برای MVVM خالص 100% است

### ۲.۳ ManualCodeTextBox binding pattern

برای فیلدهایی که `IsEnabled` آن‌ها توسط ViewModel کنترل می‌شود، در `ClearForm` نباید مقدار را hard-code کرد:

```csharp
// ❌ اشتباه
ManualCodeTextBox.IsEnabled = false;

// ✅ صحیح — binding را restore کن
var be = ManualCodeTextBox.GetBindingExpression(UIElement.IsEnabledProperty);
be?.UpdateTarget();
```

### ۲.۴ Persian Digit Conversion

فیلد کد ملی می‌تواند ارقام فارسی تولید کند. قبل از اعتبارسنجی باید تبدیل شوند:

```csharp
if (c >= '۰' && c <= '۹')
    digits.Append((char)('0' + (c - '۰')));
```

این در `bc6b842` نبود و در `37ccb10` اضافه شد (بهبود مثبت).

---

## ۳. وضعیت برنچ Backup

- **نام:** `backup/newperson-mvvm-attempt`
- **نقطه پایانی:** `4ecf7fa` (شامل ۱۰ کامیت رفکتور)
- **شامل:**
  - NewPersonViewModel.cs (communityToolkit MVVM)
  - استخراج کلاس‌های مشترک به Models/ و Converters/
  - رفع رگرسیون‌ها (CodeMode toggle, LegalType, category selection, province cascade)
  - تقویت validation کد ملی (check digit + all-same-digit rejection)
  - تولید خودکار ManualCode

**برای مراجعه بعدی:**
```bash
git checkout backup/newperson-mvvm-attempt
```

---

## ۴. دلیل توقف رفکتور

رفکتور MVVM به خودی خود موفق بود (Build 0 Error، ۱۰ کامیت تمیز)، اما:
- باگ‌های preexisting (مانند IsValidMobile حذف‌شده) هنوز برطرف نشده‌اند
- قبل از ادامه رفکتور، بهتر است این باگ‌ها روی baseline فعلی (`37ccb10`) برطرف شوند
- سپس رفکتور MVVM با پایه سالم‌تری از سر گرفته شود

---

## ۵. Build Baseline

```
Build succeeded.
    872 Warning(s)
    0 Error(s)
```

فرمان: `dotnet build --no-dependencies`
