# گزارش جامع کیفیت بک‌اند — پروژه حسابداری

**تاریخ:** ۸ مرداد ۱۴۰۵
**وضعیت:** Phase 1 — Backend Review
**دامنه بررسی:** Domain, Application, Infrastructure, Configuration/Bootstrapper

---

## خلاصه اجرایی

| دسته | تعداد |
|------|-------|
| 🔴 Critical | ۱۷ |
| 🟠 High | ۱۶ |
| 🟡 Medium | ۲۰ |
| 🔵 Low | ۱۵ |
| **مجموع** | **۶۸** |

---

## 🔴 CRITICAL (۱۷ مورد)

### BC1. تمام Repositoryها از FakeDataContext استفاده می‌کنند
- **فایل‌ها:** تمام Repositoryها (30+ فایل)
- **توضیح:** هر Repository به جای SystemContext، FakeDataContext را inject می‌کند
- **خطر:** عملیات دیتابیس روی دیتابیس اشتباه (TaadolFake به جای TaadolDb)
- **نمونه:** `BankRepository.cs:10` → `BankFakeDataContext` به جای `BankSystemContext`

### BC2. Connection String Hardcoded در کد
- **فایل:** `App.xaml.cs:61` (فرانت‌اند) + تمام Bootstrapperها
- **توضیح:** رشته اتصال SQL Server مستقیماً در کد
- **خطر:** لو رفتن اطلاعات سرور، عدم امکان استقرار multi-machine

### BC3. Raw SQL در لایه View (فرانت‌اند)
- **فایل:** `NewFinancialPeriodView.xaml.cs:164-213`
- **توضیح:** استفاده مستقیم از SqlConnection/SqlCommand به جای IFinancialPeriodApplication
- **خطر:** SQL Injection، نقض CQRS، عدم امکان تست

### BC4. NotImplementedException در Application Layer
- **فایل:** `PersonAddressApplication.cs:139`
- **متد:** `Search()` پیاده‌سازی نشده
- **خطر:** Crash در runtime هنگام فراخوانی

### BC5. Duplicate Relationship Configuration (EF Core)
- **فایل:** `AccountMapping.cs:17-19`
- **توضیح:** رابطه Parent-Children از هر دو طرف تعریف شده
- **خطر:** Runtime exception در EF Core

### BC6. FixedAsset - 3 FK به یک Navigation Property
- **فایل:** `FixedAssetMapping.cs:23-25`
- **توضیح:** سه FK متفاوت به `x.Accounts` اشاره می‌کنند
- **خطر:** EF Core نمی‌تواند FK را تشخیص دهد

### BC7. Anemic Domain Model (مدل بی‌رفتار)
- **دامنه:** تمام Entityها (40+ کلاس)
- **توضیح:** Entityها فقط داده ذخیره می‌کنند، هیچ validation یا business logic ندارند
- **نمونه:** `AccountingEntries` → debit/credit بدون validation، می‌تواند منفی یا هر دو صفر باشد

### BC8. عدم وجود AggregateRoot Base Class
- **دامنه:** سراسری
- **توضیح:** هیچ تمایزی بین Aggregate Root و Child Entity وجود ندارد

### BC9. Namespace اشتباه در AccountSystemContextFactory
- **فایل:** `AccountSystemContextFactory.cs:6`
- **مقدار:** `namespace PayrollSystemManagement.Infrastructure.EFCore`
- **خطر:** ابزارهای migration ممکن است فایل را پیدا نکنند

### BC10. خطا در ApplicationMessages
- **فایل:** `ApplicationMessages.cs:11-12`
- **توضیح:** `DuplicatedRecord` و `RecordNotFound` هر دو پیام یکسانی دارند ("رکورد یافت نشد")
- **خطر:** پیام اشتباه به کاربر نمایش داده می‌شود

### BC11. Namespace تو در تو در PayrollContract
- **فایل:** `IPayrollApplication.cs:3-6`
- **توضیح:** دو namespace یکسان تو در تو تعریف شده
- **خطر:** Using statement اشتباه در `PayrollApplication.cs:3`

### BC12. Data Integrity - Reset قبل از Validation
- **فایل:** `BranchApplication.cs:29-39`
- **توضیح:** `ResetAllMainBranches()` قبل از چک duplicate اجرا می‌شود
- **خطر:** اگر validation شکست بخورد، قبلاً داده تغییر کرده

### BC13. Race Condition در Code Generation
- **فایل:** `CodeGeneratorService.cs:18`
- **توضیح:** `Random.Shared.Next(10000, 99999)` فقط ۹۰,۰۰۰ مقدار ممکن
- **خطر:** تولید کدهای تکراری در دسترسی همزمان

### BC14. duplicate registrations در App.xaml.cs
- **فایل:** `App.xaml.cs:75 + 81-84`
- **توضیح:** Person services هم توسط Bootstrapper و هم دستی register شده
- **خطر:** رفتار غیرقابل پیش‌بینی DI

### BC15. EntityBase - Public Setters
- **فایل:** `EntityBase.cs:5-7`
- **توضیح:** تمام propertyها `public set` دارند
- **خطر:** هر کدی می‌تواند IsDeleted/IsActive را تغییر دهد

### BC16. Raw int به جای Enum
- **فایل‌ها:** `Invoices.cs:16-17`، `InventoryTransactions.cs:13-15`، `AssetDisposals.cs:13`
- **توضیح:** InvoiceType, Status, ReferenceType, TransactionType همگی int هستند
- **خطر:** عدم type safety، هر مقداری قبول می‌شود

### BC17. EntityBase از DateTime.Now استفاده می‌کند
- **فایل:** `EntityBase.cs:13`
- **توضیح:** `DateTime.Now` به جای `DateTime.UtcNow`
- **خطر:** مشکلات timezone

---

## 🟠 HIGH (۱۶ مورد)

### BH1. عدم وجود Index روی جداول
- **دامنه:** 55+ جدول، فقط 4 تا Index دارند
- **تаблицه‌های حیاتی:** Accounts (ParentId), AccountingDocuments (BranchId, CreatedBy), Persons (NationalCode, PersonTypeId), Products (CategoryId, Code)
- **خطر:** کندی شدید کوئری‌ها

### BH2. عدم وجود Query Filter روی Soft-Delete
- **دامنه:** فقط PersonCategoryFilter دارد
- **توضیح:** تمام جدول‌ها باید `HasQueryFilter(x => !x.IsDeleted)` داشته باشند
- **خطر:** نشت داده‌های حذف شده

### BH3. Plural Class Names (50+ کلاس)
- **دامنه:** تمام Entityها
- **نمونه:** `Accounts` به جای `Account`، `Persons` به جای `Person`
- **خطر:** نقض naming conventions C# و DDD

### BH4. Inconsistent Active/Deactivate Naming
- **3 الگو:** `Active/NotActive`، `Activate/Deactivate`، `Activate/DeActivate`
- **خطر:** سردرگمی توسعه‌دهندگان

### BH5. Typos در Identifiers
- **فایل‌ها:**
  - `FixedAssets.cs:14` → `PutchaseDate` (باید `PurchaseDate` باشد)
  - `FixedAssets.cs:15` → `PutchasePrice` (باید `PurchasePrice` باشد)
  - `Persons.cs:41` → `perosnCategoryId` (باید `personCategoryId` باشد)
  - `Invoices.cs:28` → `AccountingDoucumentId` (باید `AccountingDocumentId` باشد)

### BH6. Typos در Folder Names
- **پوشه‌ها:**
  - `AccountingEntrieAgg` → `AccountingEntryAgg`
  - `BankBrancheAgg` → `BankBranchAgg`
  - `ProductArrributeValueAgg` → `ProductAttributeValueAgg`
  - `ProductBatcheAgg` → `ProductBatchAgg`
  - `BranchArchice` → `BranchArchive`

### BH7. FinancialPeriodAppliction Typo
- **فایل:** `FinancialPeriodAppliction.cs:8`
- **توضیح:** کلاس `FinancialPeriodAppliction` (بدون i) به جای `FinancialPeriodApplication`

### BH8. OperationResult.Succedded Typo
- **فایل:** `OperationResult.cs:12`
- **توضیح:** `Succedded` به جای `Succeeded`
- **تاثیر:** در 100+ مکان استفاده شده

### BH9. Variable Misnaming - "person" برای غیر Person
- **فایل‌ها:** `BankTypeApplication.cs`، `ChequeApplication.cs`، `ChequeBookApplication.cs`، `CompanyBankAccountApplication.cs`
- **توضیح:** متغیر `person` برای BankType, Cheque, ChequeBook, CompanyBankAccount استفاده شده

### BH10. Duplicate Join Entities
- **فایل‌ها:** `InvoicePayments.cs` + `ReceiptPaymentInvoices.cs`
- **توضیح:** هر دو یک مفهوم هستند (لینک فاکتور به رسید/پرداخت)

### BH11. No String Validation در Domain
- **دامنه:** 30+ Entity
- **توضیح:** هیچ null/empty/whitespace check در constructor یا Edit methods
- **نمونه:** `Accounts.cs:40-57` → Title بدون validation

### BH12. No Numeric Range Validation
- **دامنه:** 10+ Entity
- **نمونه‌ها:**
  - `AccountingEntries` → debit/credit می‌تواند منفی باشد
  - `InvoiceItems` → quantity می‌تواند منفی باشد
  - `Cheques` → Amount می‌تواند صفر یا منفی باشد
  - `PettyCashes` → maxLimit می‌تواند منفی باشد

### BH13. Missing Business Invariants
- **نمونه‌ها:**
  - `FinancialPeriods` → EndDate > StartDate بررسی نمی‌شود
  - `FixedAssets` → SalvageValue < PurchasePrice بررسی نمی‌شود
  - `PettyCashes` → DecreaseBalance CurrentBalance >= amount بررسی نمی‌شود
  - `Cheques` → ChangeStatus بدون state machine

### BH14. Cross-Boundary Aggregate References
- **نمونه:** `ReceiptsPayments.cs` → 7 navigation property از 7 context متفاوت
- **خطر:** Tight coupling بین Bounded Contexts

### BH15. AccountingDocuments - Person به جای User
- **فایل:** `AccountingDocuments.cs:25-26`
- **توضیح:** `Creator` از نوع `Persons` است به جای `User`
- **خطر:** مدل دامنه اشتباه

### BH16. Logs Entity - Remove/Restore غیرمنطقی
- **فایل:** `Logs.cs:29-48`
- **توضیح:** Log entity متدهای Remove/Restore/Active/NotActive دارد
- **خطر:** لاگ‌ها نباید قابل حذف باشند

---

## 🟡 MEDIUM (۲۰ مورد)

### BM1. عدم وجود Pagination
- **دامنه:** تمام Repositoryها
- **توضیح:** تمام `Get*()` و `Search()` methods `List<T>` برمی‌گردانند بدون Take/Skip
- **خطر:** بارگذاری کل جدول در حافظه

### BM2. عدم وجود Async Queries
- **دامنه:** تمام Repositoryها
- **توضیح:** تمام کوئری‌ها synchronous هستند (ToList() به جای ToListAsync())
- **خطر:** block شدن thread اصلی

### BM3. Include Before Select Pattern
- **فایل‌ها:** 18+ Repository
- **توضیح:** `.Include()` قبل از `.Select()` غیرضروری است
- **خطر:** اضافه بار روی حافظه و دیتابیس

### BM4. N+1 Query در PersonBankRepository
- **فایل:** `PersonBankRepository.cs:49`
- **توضیح:** دسترسی به `x.BankBranches.Banks.Title` بدون Include
- **خطر:** هر رکورد 2 کوئری جداگانه ایجاد می‌کند

### BM5. Inconsistent AsNoTracking
- **دامنه:** برخی Repositoryها دارند، برخی ندارند
- **فایل‌های بدون AsNoTracking:** CompanyBankAccountRepository, ChequeRepository, ChequeBookRepository, BankBranchRepository, ReceiptsPaymentRepository, PettyCashRepository, FundRepository, FinancialPeriodRepository, ProvinceRepository, CityRepository, CompanyRepository, BranchArchiveRepository, PictureRepository
- **خطر:** Overhead غیرضروری change tracking

### BM6. FakeDataContext در Production DI
- **فایل‌ها:** تمام Bootstrapperها
- **توضیح:** FakeDataContext alongside real contexts register شده
- **خطر:** استفاده تصادفی از دیتابیس تست در production

### BM7. Inconsistent Error Message Strategy
- **الگوی A:** `ApplicationMessages.RecordNotFound` (برخی فایل‌ها)
- **الگوی B:** Hardcoded Persian strings (بیشتر فایل‌ها)
- **خطر:** یکپارچگی پیام‌ها

### BM8. Inconsistent Error Return Pattern
- **فایل‌ها:** `CompanyApplication.cs:36,49,62,65`، `BranchApplication.cs:149,162,178`
- **توضیح:** `new OperationResult().Failed()` به جای متغیر `operation`

### BM9. Missing Validation در Application Methods
- **دامنه:** 20+ متد
- **نمونه:** BankApplication, ChequeApplication, CompanyBankAccountApplication, PersonApplication, PayrollApplication, DepartmentApplication, JobTitleApplication

### BM10. Inconsistent Duplicate-Check Strategy
- **الگوی A:** `_repository.Exists()` (اکثر)
- **الگوی B:** `_repository.Search().Any()` (FundApplication, PettyCashApplication)
- **الگوی C:** Double check (ContactTypeApplication)

### BM11. No try-catch در Application Methods
- **دامنه:** تمام 28 Application classes
- **توضیح:** Database exceptions بدون handling به بالا bubble می‌شوند

### BM12. All Services Registered as Transient
- **دامنه:** تمام Bootstrapperها
- **توضیح:** Application و Repository هر دو Transient هستند
- **خطر:** ممکن است DbContext scoped lifetime مشکل ایجاد کند

### BM13. Missing ICodeGeneratorService Registration در Bootstrapper
- **فایل:** `CodeManagementBoostrapper.cs`
- **توضیح:** فقط در App.xaml.cs register شده
- **خطر:** هر host غیر WPF نمی‌تواند resolve کند

### BM14. IBankTypeApplication ناقص
- **فایل:** `IBankTypeApplication.cs`
- **توضیح:** فقط Remove, Restore, Activate, Deactivate, GetBankTypes
- **خطر:** Create و Edit وجود ندارد

### BM15. Missing PersonApplication.Edit EconomicCode Check
- **فایل:** `PersonApplication.cs:60-87`
- **توضیح:** در Create چک EconomicCode وجود دارد، در Edit نیست

### BM16. PayrollApplication - Status Validation ندارد
- **فایل:** `PayrollApplication.cs:82-95`
- **توضیح:** Pay و Cancel بدون بررسی وضعیت فعلی

### BM17. Missing ORM Constructors
- **فایل‌ها:** `FinancialPeriods.cs`، `PersonAddresses.cs`، `PersonBanks.cs`، `PersonContacts.cs`
- **توضیح:** `protected` parameterless constructor برای ORM ندارند

### BM18. ReceiptsPaymentMapping - Amount == Comparison
- **فایل:** `ReceiptsPaymentRepository.cs:131`
- **توضیح:** مقایسه decimal با `==` ممکن است precision issues داشته باشد

### BM19. PersonCategoryApplication - Duplicate Check ممکن است خودش را شامل شود
- **فایل:** `PersonCategoryApplication.cs:49`
- **توضیح:** `ExistsByTitle` ممکن است رکورد فعلی را هم شامل شود

### BM20. Unused Computed Variables
- **فایل:** `PayrollDetailApplication.cs:30,62`
- **توضیح:** `var amount = command.Quantity * command.Rate` محاسبه شده ولی استفاده نشده

---

## 🔵 LOW (۱۵ مورد)

### BL1. DRY Violation - Remove/Restore/Active/NotActive
- **دامنه:** 40+ Entity
- **توضیح:** کد تکراری در تمام Entityها
- **راه‌حل:** انتقال به EntityBase یا IActivateable/ISoftDeletable interface

### BL2. Missing Value Objects
- **مفهوم:** Money, Address, NationalCode, EconomicCode, Shaba, CardNumber, PersonName
- **توضیح:** تمام این‌ها به صورت bare string/decimal پراکنده هستند

### BL3. Location - عدم وجود Equals/GetHashCode
- **فایل:** `Location.cs:4-15`
- **توضیح:** Value object بدون override equality

### BL4. God Class - Products
- **فایل:** `Products.cs:18-57`
- **توضیح:** 17 property + 7 child collections
- **خطر:** Concurrency conflicts، aggregate بیش از حد بزرگ

### BL5. Inconsistent String Trimming
- **دامنه:** فقط 3 از 25+ Application class string را trim می‌کنند

### BL6. Cross-Context Assembly Dependency
- **فایل:** `PersonSystemContext.cs:1,35-36`
- **توضیح:** Person context assembly BankManagement را load می‌کند

### BL7. RepositoryBase بدون IDisposable
- **فایل:** `RepositoryBase.cs:7`
- **توضیح:** DbContext lifecycle کاملاً به DI بستگی دارد

### BL8. PictureRepository SaveChanges مستقیم
- **فایل:** `PictureRepository.cs:61`
- **توضیح:** UoW pattern نقض شده

### BL9. IRepository ناقص
- **فایل:** `IRepository.cs:5-13`
- **توضیح:** فاقد async methods, filtering, pagination

### BL10. No Query Filter روی Logs
- **فایل:** `LogMapping.cs`
- **توضیح:** Logs بدون soft-delete filter

### BL11. LogMapping - DateTime.Now به جای UtcNow
- **فایل:** `EntityBase.cs:13`

### BL12. ApplicationMessages Unused Usings
- **فایل:** `ApplicationMessages.cs:1-5`
- **توضیح:** 5 using statement غیرضروری

### BL13. Excessive Blank Lines
- **فایل‌ها:** `FundApplication.cs`، `PettyCashApplication.cs`، `ReceiptsPaymentApplication.cs`

### BL14. Contract vs Contracts Naming Inconsistency
- **PersonManagement, GeneralInfoManagement:** Contract (singular)
- **BankManagement, CodeManagement, FinancialManagement, Payroll:** Contracts (plural)

### BL15. Using After Namespace در PersonManagementBoostrapper
- **فایل:** `PersonManagementBoostrapper.cs:1-4`
- **توضیح:** using statements بعد از namespace declaration

---

## نقشه راه اصلاح بک‌اند

### فاز ۱: ثبات و امنیت (هفته ۱)
| # | وظیفه | اولویت |
|---|-------|--------|
| 1 | اصلاح FakeDataContext → SystemContext در Repositoryها | 🔴 |
| 2 | اصلاح Duplicate Relationship در AccountMapping | 🔴 |
| 3 | اصلاح FixedAsset FK Navigation | 🔴 |
| 4 | اصلاح Namespace اشتباه در AccountSystemContextFactory | 🔴 |
| 5 | اصلاح ApplicationMessages.DuplicatedRecord | 🔴 |
| 6 | حذف Namespace تو در تو در IPayrollApplication | 🔴 |
| 7 | اصلاح BranchApplication Data Integrity | 🔴 |

### فاز ۲: Domain Model (هفته ۲-۳)
| # | وظیفه | اولویت |
|---|-------|--------|
| 8 | اضافه کردن AggregateRoot base class | 🟠 |
| 9 | encapsulation کردن EntityBase setters | 🟠 |
| 10 | تبدیل Raw int به Enum | 🟠 |
| 11 | اضافه کردن validation به Domain entities | 🟠 |
| 12 | اضافه کردن business invariants | 🟠 |
| 13 | اصلاح plural class names | 🟡 |

### فاز ۳: Performance (هفته ۳-۴)
| # | وظیفه | اولویت |
|---|-------|--------|
| 14 | اضافه کردن Index به جداول پرتردد | 🟠 |
| 15 | اضافه کردن Query Filter برای Soft-Delete | 🟠 |
| 16 | اضافه کردن Pagination | 🟡 |
| 17 | اضافه کردن Async Queries | 🟡 |
| 18 | حذف Include Before Select | 🟡 |
| 19 | اصلاح N+1 Query | 🟡 |

### فاز ۴: Application Layer (هفته ۴)
| # | وظیفه | اولویت |
|---|-------|--------|
| 20 | اضافه کردن try-catch به Application methods | 🟡 |
| 21 | یکپارچه‌سازی Error Messages | 🟡 |
| 22 | اضافه کردن validation به Application methods | 🟡 |
| 23 | اصلاح DI registrations (Transient → Scoped) | 🟡 |
| 24 | حذف FakeDataContext از Production DI | 🟡 |

---

## مشکلات فرانت‌اند مرتبط با بک‌اند

| # | مشکل | فایل | نیاز به تغییر بک‌اند |
|---|-------|------|---------------------|
| 1 | Raw SQL در View | `NewFinancialPeriodView.xaml.cs` | ✅ بله - اضافه کردن CreateWithActivation |
| 2 | Sync DB calls | 5 فایل | ✅ بله - async methods |
| 3 | Connection String | `App.xaml.cs` | ✅ بله - appsettings.json |
| 4 | IFinancialPeriodApplication.Create | `FinancialPeriodAppliction.cs` | ✅ بله - منطق activate/deactivate |

---

*تهیه شده توسط تیم فرانت‌اند — پروژه تعادل*
