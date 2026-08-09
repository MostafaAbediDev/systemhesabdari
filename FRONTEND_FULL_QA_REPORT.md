# گزارش جامع کیفیت فرانت‌اند - پروژه تعادل (Taadol WPF)

**تاریخ:** ۸ مرداد ۱۴۰۵
**وضعیت:** Phase 1 — UI/UX Only
**دامنه بررسی:** تمام Views، UserControls، Styles، Themes، Code-behind

---

## خلاصه اجرایی

| دسته | تعداد |
|------|-------|
| 🔴 Critical | ۱۲ |
| 🟠 High | ۲۴ |
| 🟡 Medium | ۳۵ |
| 🔵 Low | ۲۰ |
| **مجموع** | **۹۱** |

**وضعیت فعلی:** ⚠️ READY WITH MINOR ISSUES

**پیش‌نیاز اصلاح:** دسترسی ویرایش فقط فرانت‌اند. مشکلات بک‌اند به تیم دیگر اطلاع داده می‌شود.

---

## 🔴 CRITICAL (۱۲ مورد)

### C1. Raw SQL در لایه View
- **فایل:** `NewFinancialPeriodView.xaml.cs:164-213`
- **توضیح:** استفاده مستقیم از `SqlConnection` و `SqlCommand` در کد View به جای `IFinancialPeriodApplication`
- **خطر:** آسیب‌پذیری SQL Injection، نقض الگوی CQRS، عدم امکان تست
- **راه‌حل:** حذف Raw SQL و استفاده از Application Layer (`IFinancialPeriodApplication`)

### C2. فراخوانی Sync DB در UI Thread
- **فایل‌ها:**
  - `NewPersonView.xaml.cs:36-66` → `personApplication.Search()` و `personApplication.Create()`
  - `NewCompanyView.xaml.cs:140` → `_companyApplication.Create()`
  - `NewBranchView.xaml.cs:262` → `_branchApplication.GetBranches()`
  - `NewFinancialPeriodView.xaml.cs:130-228` → تمام DB operations
  - `MainWindow.xaml.cs:67` → `_companyApplication.GetCompanies()`
- **خطر:** فریز شدن کامل برنامه هنگام اجرای کوئری
- **راه‌حل:** تبدیل به `async/await` با `Task.Run` برای عملیات دیتابیس

### C3. Connection String Hardcoded
- **فایل:** `App.xaml.cs:61`
- **مقدار:** `@"Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True"`
- **خطر:** لو رفتن اطلاعات سرور، عدم امکان استقرار multi-machine
- **راه‌حل:** انتقال به `appsettings.json` یا Environment Variables

### C4. Static ServiceProvider (Service Locator Anti-pattern)
- **فایل:** `App.xaml.cs:27`
- **توضیح:** تمام Viewها از `App.ServiceProvider` مستقیماً استفاده می‌کنند
- **خطر:** عدم امکان تست واحد، مشکل در Lifecycle سرویس‌ها
- **راه‌حل:** استفاده از Constructor Injection در ViewModelها

### C5. Memory Leak — Event Handler Accumulation
- **فایل:** `MainWindow.xaml.cs:46-51,104`
- **توضیح:** `Sidebar.SidebarWidthChanged` دوبار subscribe می‌شود
- **خطر:** پس از N بار load، callback N بار اجرا می‌شود
- **راه‌حل:** حذف ` -= ` قبل از `+= ` در event subscription

### C6. Memory Leak — ToastManager Static Reference
- **فایل:** `ToastManager.cs:8`
- **توضیح:** ارجاع static به `StackPanel` UI element
- **خطر:** جلوگیری از GC شدن کل درخت بصری MainWindow
- **راه‌حل:** استفاده از `WeakReference` یا پاکسازی در `Unloaded`

### C7. Memory Leak — DI Scope Misuse
- **فایل:** `PersonListView.xaml.cs:34-35` و سایر Viewها
- **توضیح:** Scope با `using` ایجاد شده و فوراً dispose می‌شود
- **خطر:** DbContext instances tracked نیستند و connection leak می‌شود
- **راه‌حل:** نگهداری scope در طول عمر ViewModel

### C8. Dual Navigation Pattern
- **فایل:** `MainWindow.xaml.cs:89-99,116-152`
- **توضیح:** دو الگوی متفاوت ناوبری (NavigationService + دستی)
- **خطر:** عدم یکپارچگی در tracking ناوبری
- **راه‌حل:** یکپارچه‌سازی الگوی ناوبری

### C9. BranchListView Resource Key Typo
- **فایل:** `BranchListView.xaml:19`
- **مقدار:** `"BooleanToVisibili tyConverter"` (فاصله اضافی)
- **خطر:** Runtime XamlParseException
- **راه‌حل:** اصلاح به `"BooleanToVisibilityConverter"`

### C10. Double try/finally Nesting
- **فایل‌ها:** `NewPersonView.xaml.cs:860-869`، `EditPersonView.xaml.cs:860-874`
- **توضیح:** کد پاکسازی تکراری در بلوک‌های finally تو در تو
- **خطر:** اجرای چندباره کد پاکسازی
- **راه‌حل:** حذف لایه اضافی try/finally

### C11. Exit Without Dirty State Check
- **فایل:** `MainWindow.xaml.cs:166-177`
- **توضیح:** خروج بدون بررسی تغییرات ذخیره نشده
- **خطر:** از دست رفتن سکوت کاربر
- **راه‌حل:** اضافه کردن `MessageBox` تأیید قبل از خروج

### C12. MessageBox به جای ModernDialog
- **فایل:** `MainWindow.xaml.cs:168`
- **توضیح:** استفاده از `MessageBox.Show` به جای دیالوگ تمدار
- **خطر:** شکستن یکپارچگی بصری و عدم پشتیبانی RTL صحیح
- **راه‌حل:** استفاده از `ModernDialog`

---

## 🟠 HIGH (۲۴ مورد)

### Responsive / Layout

| # | فایل:خط | مشکل |
|---|---------|-------|
| H1 | `ProductListView.xaml` | ۱۳ ستون → overflow در صفحات کوچک |
| H2 | `CompanyListView.xaml` | ۹ ستون → overflow |
| H3 | `BranchListView.xaml` | ۷ ستون + حذف → overflow |
| H4 | `NewPersonView.xaml` | فرم بدون scroll → clip در ابعاد کوچک |
| H5 | `EditPersonView.xaml` | فرم بدون scroll → clip |
| H6 | `FilterTabStyle2` (5 views) | `Width="215"` ثابت → overflow افقی |
| H7 | `ProductListView.xaml:75` | `Height="420"` ثابت → clip عمودی |
| H8 | `ModernDialog.xaml:13` | `Width="440"` ثابت |
| H9 | `MainWindow.xaml:55` | Header `Height="90"` ثابت |
| H10 | `MainWindow.xaml:22` | Sidebar `Width="240"` ثابت |
| H11 | `PersonListView.xaml` | `Margin="20"` + `HorizontalAlignment="Center"` |

### Accessibility

| # | فایل:خط | مشکل |
|---|---------|-------|
| H12 | `MainWindow.xaml` (تمام فایل) | عدم وجود `AutomationProperties` |
| H13 | `UnifiedListView.xaml:106-114` | Select All بدون accessibility |
| H14 | `PaginationBar.xaml:31-67,149-184` | دکمه‌های prev/next بدون accessibility |
| H15 | `ToastNotification.xaml:70-101` | دکمه close بدون keyboard support |

### Code Architecture

| # | فایل:خط | مشکل |
|---|---------|-------|
| H16 | `SidebarControl.xaml.cs:279-302` | DispatcherTimer به جای WPF DoubleAnimation |
| H17 | `TreeComboBox.xaml.cs:172-187` | FilterTree بدون debounce |
| H18 | `UnifiedListView.xaml.cs:331-381` | ایجاد brush جدید در هر scroll |
| H19 | `ModernPersianTextBox.xaml.cs:136` | Regex بدون `Compiled` option |
| H20 | `UnifiedListView.xaml.cs:393-395` | Scroll handler بدون debounce |
| H21 | `SidebarControl.xaml.cs:434-488` | Race condition در ToggleSubMenu |
| H22 | `UnifiedListView.xaml.cs:217-227` | جلوگیری از row selection |
| H23 | `App.xaml.cs:80-84` | Duplicate service registrations |
| H24 | `App.xaml.cs:91` | BuildServiceProvider بدون ValidateOnBuild |

---

## 🟡 MEDIUM (۳۵ مورد)

### UI/Design

| # | فایل:خط | مشکل |
|---|---------|-------|
| M1 | `ModernDialog.xaml:119-121` | Confirm button `Width="130"` ثابت |
| M2 | `ToastNotification.xaml:7` | `Width="500"` ثابت |
| M3 | `NewPersonView.xaml:141-143` | ارتفاع ثابت bank account fields |
| M4 | `Styles.xaml:1215` | ModernButton بدون disabled state |
| M5 | `Styles.xaml:519` | OutlinedButtonLarge بدون disabled state |
| M6 | `Styles.xaml:1649` | TabToggleButton بدون IsMouseOver trigger |
| M7 | `GridStyles.xaml:48-85` | UnifiedGridColumnHeaderStyle بدون triggers |
| M8 | `ModernDialog.xaml:95,173` | Confirm button رنگ hardcoded |
| M9 | `NewFinancialPeriodView.xaml:316-317` | FilterTabStyle2 بدون `BasedOn` |
| M10 | `PersonListView.xaml:170` | FilterTabStyle2 بدون `BasedOn` |

### RTL

| # | فایل:خط | مشکل |
|---|---------|-------|
| M11 | `PaginationBar.xaml:29` | فلش‌ها ممکن است در RTL جابجا باشند |
| M12 | `ModernPersianTextBox.xaml:28` | `HorizontalContentAlignment="Left"` hardcoded |
| M13 | `PaginationBar.xaml:11` | FlowDirection hardcoded |

### Code Quality

| # | فایل:خط | مشکل |
|---|---------|-------|
| M14 | 6+ فایل | `ToPersianNumber` تکرار شده |
| M15 | 5 فایل | `FilterTabStyle2` تکرار شده |
| M16 | List Viewها | `PageItem` class تکرار شده |
| M17 | `NewFinancialPeriodView` | `BranchComboItem` class تکرار شده |
| M18 | `NewProductView.xaml.cs` | `IsOperationSucceeded` با reflection |
| M19 | `MainWindow.xaml.cs:95-99` | `InvalidateVisual()` + `UpdateLayout()` غیرضروری |
| M20 | `MainWindow.xaml.cs:132,141` | نمایش پیام خطا خام به کاربر |
| M21 | `ModernDialog.xaml.cs:202-213` | Enter key callback incorrectly |
| M22 | `ModernDialog.xaml.cs:230-253` | Empty input → shake animation بدون پیام |
| M23 | `ModernPersianTextBox.xaml.cs:153-158` | Infinite recursion risk در OnTextChanged |
| M24 | `TreeComboBox.xaml.cs:175-176` | FilteredItems جایگزین کل collection |
| M25 | `ToastNotification.xaml.cs:82-97` | Dismiss double-call risk |
| M26 | `UnifiedListView.xaml.cs:316-329` | FindDescendantByName بدون depth limit |
| M27 | `UnifiedListView.xaml.cs:201-205` | DataGridView_SelectionChanged → nullifies selection |

### Data Consistency

| # | فایل:خط | مشکل |
|---|---------|-------|
| M28 | `ModernPersianTextBox.xaml.cs:136` | اجازه ترکیب اعداد فارسی و انگلیسی |
| M29 | `TreeComboBox.xaml.cs:195` | عدم پشتیبانی از diacritics فارسی |
| M30 | `ModernPersianTextBox.xaml.cs:195-199` | ConvertToPersianDigits برای Text type هم اعمال می‌شود |

### Accessibility

| # | فایل:خط | مشکل |
|---|---------|-------|
| M31 | `ModernPersianTextBox.xaml:20-33` | TextBox بدون AutomationProperties.Name |
| M32 | `ModernDialog.xaml:47-67` | Icon circle بدون alt text |
| M33 | `SidebarControl.xaml:33-89` | Toggle button بدون accessibility |

### Misc

| # | فایل:خط | مشکل |
|---|---------|-------|
| M34 | `App.xaml.cs:218-221` | Silent exception swallowing |
| M35 | `App.xaml.cs:31-32` | Log file بدون rotation |

---

## 🔵 LOW (۲۰ مورد)

| # | فایل:خط | مشکل |
|---|---------|-------|
| L1 | `Styles.xaml` | ۲۴ Brush تعریف شده ولی استفاده نشده |
| L2 | `Styles.xaml` | ۱۵ Style تعریف شده ولی استفاده نشده |
| L3 | `Styles.xaml:1118` | `twoway` style بدون استفاده |
| L4 | `Styles.xaml:1128` | `ModernTextBox` style بدون استفاده |
| L5 | `Styles.xaml` | نام `Gray*` برای رنگ‌های آبی گمراه‌کننده |
| L6 | `Styles.xaml` | ۳ سبک سبز مختلف بدون تفکیک معنایی |
| L7 | `UnifiedListView.xaml:11` | `Background="#FFFFFF"` hardcoded |
| L8 | `UnifiedListView.xaml:66` | `RowHeight="36"` hardcoded |
| L9 | `UnifiedListView.xaml:210` | Loading card ابعاد hardcoded |
| L10 | `MainWindow.xaml:139,144` | اطلاعات کاربر hardcoded |
| L11 | `MainWindow.xaml:13,33,55,73` | رنگ‌های hardcoded به جای StaticResource |
| L12 | `PaginationBar.xaml:31,39-40` | ابعاد دکمه تکراری |
| L13 | `UnifiedListView.xaml.cs:310-313` | SolidColorBrush ایجاد در هر call |
| L14 | `ModernPersianTextBox.xaml.cs:374-384` | `ConvertToPersianDigits` string allocation |
| L15 | `TreeComboBox.xaml.cs:153` | `SelectedItemChanged` بدون unsubscribe |
| L16 | `ModernDialog.xaml.cs:92` | `KeyDown` handler بدون unsubscribe |
| L17 | `App.xaml.cs:17,19` | Unused using statements |
| L18 | `MainWindow.xaml.cs:85,120,123,125,127` | `Debug.WriteLine` در production |
| L19 | `MainWindow.xaml.cs:81-87` | `CompanySelector` handler بدون عمل |
| L20 | `MainWindow.xaml.cs:160-164` | `ModalOverlay` handler خالی |

---

## نقشه راه اصلاح

### فاز ۱: ثبات و امنیت (هفته ۱)
| # | وظیفه | فایل‌ها |
|---|-------|---------|
| 1 | حذف Raw SQL از لایه View | `NewFinancialPeriodView.xaml.cs` |
| 2 | تبدیل DB calls به async/await | 5 فایل ذکر شده در C2 |
| 3 | انتقال Connection String | `App.xaml.cs` |
| 4 | اصلاح Resource Key Typo | `BranchListView.xaml` |
| 5 | حذف Double try/finally | `NewPersonView`, `EditPersonView` |

### فاز ۲: Memory & Performance (هفته ۲)
| # | وظیفه | فایل‌ها |
|---|-------|---------|
| 6 | اصلاح DI Scope Management | تمام ViewModels |
| 7 | اصلاح Event Handler Accumulation | `MainWindow.xaml.cs` |
| 8 | اصلاح ToastManager Static Reference | `ToastManager.cs` |
| 9 | اضافه کردن Debounce | `TreeComboBox`, `UnifiedListView` |
| 10 | اصلاح Brush Caching | `UnifiedListView.xaml.cs` |

### فاز ۳: Responsive Layout (هفته ۳)
| # | وظیفه | فایل‌ها |
|---|-------|---------|
| 11 | تبدیل HorizontalAlignment | تمام List Views |
| 12 | اضافه کردن ScrollViewer | تمام Form Views |
| 13 | تبدیل Width ثابت | FilterTabStyle2, ModernDialog |
| 14 | اصلاح ProductListView Grid | `ProductListView.xaml` |

### فاز ۴: Accessibility (هفته ۴)
| # | وظیفه | فایل‌ها |
|---|-------|---------|
| 15 | اضافه کردن AutomationProperties | تمام interactive elements |
| 16 | اضافه کردن Keyboard Support | دکمه‌ها، PaginationBar |
| 17 | اضافه کردن LiveSetting | پیام‌ها و Toast |

### فاز ۵: Design System Cleanup (هفته ۵)
| # | وظیفه | فایل‌ها |
|---|-------|---------|
| 18 | حذف Styles غیرفعال | `Styles.xaml` |
| 19 | حذف Brushes غیرفعال | `Styles.xaml` |
| 20 | یکپارچه‌سازی رنگ‌ها | تمام فایل‌ها |
| 21 | اضافه کردن Disabled States | 12 Style |

---

## مشکلات بک‌اند (برای اطلاع‌رسانی به تیم دیگر)

| # | مشکل | فایل |
|---|-------|------|
| B1 | `PersonSystemContext` و `PersonFakeDataContext` هر دو register شده‌اند | `App.xaml.cs:83-84` |
| B2 | Duplicate service registrations ممکن است با Boostrapper تداخل داشته باشد | `App.xaml.cs:80-84` |
| B3 | Connection String برای هر دو DbContext یکسان است | `App.xaml.cs:61` |

---

*تهیه شده توسط تیم فرانت‌اند — پروژه تعادل*
