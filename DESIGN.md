---
version: alpha
name: Taadol-Design-System
description: |
  Tali design system for the Taadol WPF (Tadol) accounting application. A Persian (Farsi) Right-to-Left desktop finance application built on WPF. The system uses a professional blue-and-navy financial aesthetic: navy brand anchors, a blue accent for interactivity, green for positive monetary states, red for destructive/errors, gold for highlights/warnings. Everything is rendered in the IRANSans font family, FlowDirection RightToLeft is global. Visual language is clean, card-based, flat with subtle shadows and 4-8px rounded corners.

colors:
  primary: "#0D2159"
  primary-dark: "#000B2E"
  primary-light: "#E9F0FF"
  accent: "#2667FF"
  accent-dark: "#1E52CC"
  accent-deep: "#173E99"
  accent-light: "#D4E1FF"
  accent-super-light: "#E9F0FF"
  green: "#02CF54"
  green-dark: "#02A844"
  green-light: "#E0FBEC"
  danger: "#C1121F"
  danger-light: "#FCE8EA"
  gold: "#F5A623"
  gold-light: "#FFF8E7"
  gray-05: "#E9F0FF"
  gray-1: "#D4E1FF"
  gray-15: "#BED1FF"
  gray-2: "#A8C2FF"
  gray-3: "#7DA4FF"
  gray-4: "#5185FF"
  gray-6: "#1E52CC"
  gray-7: "#173E99"
  gray-055: "#F8FAFC"
  background: "#F2F5FA"
  card-background: "#FFFFFF"
  sidebar-bg: "#F8FAFF"
  text-ink: "#0D2159"
  text-mute: "#64748B"
  text-subtle: "#94A3B8"

typography:
  family: IRANSansXFaNum
  default-size: 13px
  button-size: 13px
  filter-tab-size: 13px
  small-size: 11px
  label-size: 15px

rounded:
  default: 4px
  chip: 8px

---

# سیستم دیزاین تعادل (Taadol) — اپ حسابداری WPF

## نمای کلی

این یک اپ دسکتاپ حسابداری فارسی به زبان فارسی (Persian / Farsi) است. کل UI با `FlowDirection="RightToLeft"` کار می‌کند و فونت همه‌ی متن‌ها `IRANSans` است. زبان بصری: کارت‌محور، تخت، تمیز، با گوشه‌های گرد کوچک (۴ تا ۸ پیکسل) و سایه‌های ملایم. هیچ فونت خارجی یا متن لاتین برای محتوای UI استفاده نمی‌شود (مقادیر عددی با `IRANSansXFaNum` اعداد فارسی نمایش داده می‌شوند).

هویت رنگی از ترکیب سرمه‌ای برند + آبی تعاملی نشئت می‌گیرد — همان الگوی اپ‌های مالی حرفه‌ای و بانکی.

## اصول (Do's and Don'ts)

### Do
- همیشه `FlowDirection="RightToLeft"` را حفظ کن.
- همه‌ی متن‌ها با فونت `IRANSans` (resource key: `IRANSans`).
- `#0D2159` (نخودی‌تیره / سرمه‌ای برند) را برای تیترها و عناصر اصلی استفاده کن.
- آبی `#2667FF` را برای حالت فعال/باز و لینک/تعامل بگذار.
- سبز `#02CF54` برای وضعیت مثبت مالی (بستانکار، مانده‌ی بدهکار حساب‌ها، تأیید).
- قرمز `#C1121F` فقط برای خطا/حذف/وضعیت منفی.
- طلایی `#F5A623` برای هشدار/نکته/برجسته‌سازی.
- برای کامپوننت‌های دکمه/تب/اینپوت از گوشه‌ی گرد ۴px و برای چیپ‌های شمارنده ۸px استفاده کن.
- اعداد را با «اعداد فارسی» نمایش بده (فونت `IRANSansXFaNum` این کار را خودکار انجام می‌دهد). دستی `ToPersianNumber(...)` در کد برای تبدیل long/int.
- دسترسی داده‌ها در viewها فقط از مسیر `Task.Run` + `App.ServiceProvider.CreateScope()` — هرگز در UI thread مستقیم کار DB نکن.

### Don't
- از فونت یا سایز ۲۴px+ برای متن بدنه استفاده نکن؛ بدنه ۱۳px است.
- الگوی `HorizontalAlignment` در RTL را اشتباه نگیر — «چپ» و «راست» در RTL **قرینه** می‌شوند.
- بدون `Task.Run`/`CreateScope` با دیتابیس از view تماس نگیر.
- در `edit` / `create` از باگ تکرار کد ملی/کد اقتصادی غافل نشو (همان منطق `Create` را در `Edit` هم بررسی کن).

## رنگ‌ها

### برند (Primary)
- **primary** `#0D2159` — سرمه‌ای برند؛ تیترها، نوار بالایی، دکمه‌ی اصلی.
- **primary-dark** `#000B2E` — حالت فشرده/عمیق برند.
- **primary-light** `#E9F0FF` — پس‌زمینه‌ی ملایم برند.

### تعاملی (Accent)
- **accent** `#2667FF` — حالت فعال (checked)، لینک، هاور دکمه‌ی اصلی.
- **accent-dark** `#1E52CC` — هاور accent.
- **accent-light** `#D4E1FF` / **accent-super-light** `#E9F0FF` — پس‌زمینه‌ی حالت فعال تب/دکمه.

### وضعیت (Semantic)
- **green** `#02CF54` / **green-dark** `#02A844` / **green-light** `#E0FBEC` — تأیید و وضعیت مثبت مالی.
- **danger** `#C1121F` / **danger-light** `#FCE8EA` — خطا، حذف، وضعیت منفی.
- **gold** `#F5A623` / **gold-light** `#FFF8E7` — هشدار/نکته.

### سطوح و پس‌زمینه
- **background** `#F2F5FA` — پشت‌زمینه‌ی کل app.
- **card-background** `#FFFFFF` — کارت‌ها، فرم‌ها، DataGrid.
- **sidebar-bg** `#F8FAFF` — سایدبار/منوی کناری.
- **gray-055** `#F8FAFC` — حالت هاور ملایم، ردیف‌های متناوب جدول.

### متن
- **text-ink** `#0D2159` — متن اصلی/تیتر.
- **text-mute** `#64748B` — متن ثانویه/توضیح.
- **text-subtle** `#94A3B8` — کم‌اهمیت‌ترین متن (شمارنده‌ها در حالت عادی).

## تایپوگرافی

- **فونت:** `IRANSans` (که فایل font های embedded با نام `IRANSansXFaNum` است). برای اعداد فارسی؛ حتماً استفاده شود.
- **اندازه‌ها:** بدنه و دکمه `13px` · تب های فیلتر `13px` · متن کوچک (برچسب/شمارنده) `11px` · چک‌باکس و عنوان فرم `15px`.
- تیترهای صفحه عمدتاً `13–15px` با `FontWeight` متوسط/بولد.

## کامپوننت‌ها

### دکمه‌های فیلتر (Filter Tab)
استایل‌های `FilterTabStyle` (RadioButton) و `FilterTabStyle2` (ToggleButton).
- حالت عادی: متن `text-mute` `#64748B`، بدون پس‌زمینه.
- حالت فعال (IsChecked): متن و بردر آبی `accent`، پس‌زمینه‌ی `accent-super-light` `#E9F0FF`، متن بولد.
- شمارنده: چیپ با گوشه‌ی ۸px (`CornerRadius="8"`)؛ پس‌زمینه‌ی `#F1F5F9` در حالت عادی و `#DBEAFE` با متن `#2563EB` در حالت فعال.
- باکس جستجو و تب‌های فیلتر باید در **یک ردیف** قرار بگیرند و با هم به‌طور متناسب کوچک/بزرگ شوند (ستون‌های `*` در Grid)؛ اگر عرض کم شد می‌توان `Padding` را کم کرد — ارتفاع را تغییر نده.

### DataGrid (لیست اشخاص و غیره)
- پس‌زمینه‌ی سفید `card-background`، خطوط عمودی خاکستری `#E5E7EB`.
- `RowHeight="36"` ثابت.
- ستون آخر نباید `Width="*"` (انعطاف‌پذیر تمام‌عرض) باشد اگر اسکرول افقی می‌خواهی — برای اسکرول افقی ستون‌ها را عرض ثابت کن و اسکرول‌بار را روی ScrollViewer بیرونی بگذار، نه اینکه `Border.Clip` بگذاری (کلیپ باعث بریده شدن اسکرول‌بار می‌شود).
- اسکرول‌بار از استایل `MinimalScrollBar` استفاده می‌کند.

### فرم ورود اطلاعات (New/Edit Person)
- پالت رنگی: برند/آبی برای دکمه‌های اصلی، سبز برای تأیید، قرمز برای انصراف/خطا.
- همه‌ی اعتبارسنجی‌ها قبل از ذخیره: کد ملی (الگوریتم checksum)، موبایل، ایمیل، شماره شبا.
- DB در `Save` = `async Task ... SaveAsync()` با `Task.Run` + `CreateScope` + فلاگ `_isSaving` (debounce) در `try/finally`.

### منو / سایدبار
- پس‌زمینه‌ی `sidebar-bg` `#F8FAFF`.
- آیتم فعال: رخ‌نمای آبی `MenuActiveBrush` `#2667FF`؛ آیتم غیرفعال: خاکستری `MenuInactiveBrush` `#AFAFAF`.

## مسئولیت‌پذیری (Responsive)
- از Grid با ستون‌های `*` استفاده کن تا عناصر با عرض پنجره کشیده/فشرده شوند.
- تب‌ها و باکس جستجو را در یک ردیف نگه دار («responsive» یعنی هم‌زمان جمع شوند، نه wrap و نه پنهان شدن).
- برای فضای تنگ، به‌جای پنهان کردن، `Padding` داخلی را کم کن.
- برای جدول‌های پهن از اسکرول افقی (ScrollViewer) به‌جای کوچک‌کردن بیش‌ازحد ستون‌ها استفاده کن.

## ضوابط فنی (Technical Constraints)
- پروژه: WPF (.NET، فرانت‌اند فقط — **بدون تغییر بک‌اند**).
- فایل مرجع استایل‌ها: `src/FrontEndWPF/Taadol/Taadol/Themes/Styles.xaml`.
- هر تماس DB در view: `Task.Run` + `App.ServiceProvider.CreateScope()`.
- اعداد فارسی: `ToPersianNumber(long)` موجود در `PersonListView.xaml.cs`.
- RTL: `HorizontalAlignment="Left/Right"` در RTL برعکس ظاهر می‌شود — برای راست‌چین واقعی از `Stretch` یا در نظر گرفتن آینه‌گی استفاده کن.