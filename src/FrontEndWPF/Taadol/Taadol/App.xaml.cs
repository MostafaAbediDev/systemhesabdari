using BankManagement.Configuration;
using CodeManagement.Application;
using CodeManagement.Application.Contracts.Code;
using CodeManagement.Configuration;
using GeneralInfoManagement.Configuration;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using GeneralInfoManagement.Infrastructure.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Application;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Configuration;
using PersonManagement.Domain.Person.PersonAgg;
using PersonManagement.Infrastructure.EFCore;
using PersonManagement.Infrastructure.EFCore.Repository;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Taadol
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static string ConnectionString { get; private set; }

        // مسیر فایل لاگ — در پوشه‌ی Temp ویندوز
        public static string LogFilePath { get; } =
            Path.Combine(Path.GetTempPath(), "taadol-startup.log");

        protected override void OnStartup(StartupEventArgs e)
        {
            // ابتدا فایل لاگ رو ریست کن
            try
            {
                File.WriteAllText(LogFilePath,
                    $"=== Taadol Startup Log — {DateTime.Now:yyyy/MM/dd HH:mm:ss} ==={Environment.NewLine}");
            }
            catch
            {
                // اگه نتونست فایل بسازه، حداقل ادامه بده
            }

            // ===== ثبت هندلرهای خطای سراسری =====
            // این هندلرها هر خطایی که در UI Thread رخ بده رو می‌گیرن
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            LogStep("OnStartup entered");

            try
            {
                LogStep("Creating ServiceCollection...");
                var services = new ServiceCollection();

                string connectionString =
                    @"Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True";
                ConnectionString = connectionString;
                LogStep($"Connection string set: {connectionString}");

                LogStep("Registering ICodeGeneratorService...");
                services.AddTransient<ICodeGeneratorService, CodeGeneratorService>();

                LogStep("Configuring CodeManagementBoostrapper...");
                CodeManagementBoostrapper.Configure(services, connectionString);

                LogStep("Configuring GeneralInfoManagementBoostrapper...");
                GeneralInfoManagementBoostrapper.Configure(services, connectionString);

                LogStep("Configuring PersonManagementBoostrapper...");
                PersonManagementBoostrapper.Configure(services, connectionString);

                LogStep("Configuring BankManagementBoostrapper...");
                BankManagementBoostrapper.Configure(services, connectionString);

                LogStep("Registering Person repositories...");
                services.AddTransient<IPersonRepository, PersonRepository>();
                services.AddTransient<IPersonApplication, PersonApplication>();
                services.AddDbContext<PersonSystemContext>(x => x.UseSqlServer(connectionString));
                services.AddDbContext<PersonFakeDataContext>(x => x.UseSqlServer(connectionString));

                LogStep("Registering Province/City repositories...");
                services.AddTransient<IProvinceRepository, ProvinceRepository>();
                services.AddTransient<ICityRepository, CityRepository>();

                LogStep("Building ServiceProvider...");
                ServiceProvider = services.BuildServiceProvider();
                LogStep("ServiceProvider built successfully");

                LogStep("Calling base.OnStartup...");
                base.OnStartup(e);
                LogStep("base.OnStartup completed");

                // ===== ایجاد دستی MainWindow با try-catch =====
                // چون StartupUri رو از App.xaml حذف کردیم، خودمون MainWindow می‌سازیم
                // تا اگه خطایی موقع لود MainWindow رخ داد، ببینیم
                LogStep("Creating MainWindow...");
                try
                {
                    var mainWindow = new MainWindow();
                    LogStep("MainWindow instance created");

                    LogStep("Calling MainWindow.Show()...");
                    mainWindow.Show();
                    LogStep("MainWindow.Show() completed — window should be visible now");
                }
                catch (Exception mwEx)
                {
                    var msg = "🔴 خطا در ایجاد یا نمایش MainWindow:" + Environment.NewLine + Environment.NewLine +
                              BuildExceptionMessage(mwEx);
                    LogStep("!!! MainWindow creation FAILED !!!");
                    LogStep(msg);

                    MessageBox.Show(msg + Environment.NewLine + Environment.NewLine +
                                    $"لاگ کامل در: {LogFilePath}",
                        "خطا در نمایش پنجره اصلی",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    Environment.FailFast(msg);
                }
            }
            catch (Exception ex)
            {
                var fullMessage = BuildExceptionMessage(ex);
                LogStep("!!! EXCEPTION IN OnStartup !!!");
                LogStep(fullMessage);

                try
                {
                    MessageBox.Show(fullMessage + Environment.NewLine + Environment.NewLine +
                                    $"لاگ کامل در: {LogFilePath}",
                        "خطا در راه‌اندازی برنامه",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch
                {
                    // اگر MessageBox هم کار نکرد
                }

                Environment.FailFast(fullMessage);
            }
        }

        // ===== هندلر خطای UI Thread =====
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var msg = "🔴 خطای پردازش‌نشده در UI Thread:" + Environment.NewLine + Environment.NewLine +
                      BuildExceptionMessage(e.Exception);
            LogStep("!!! DispatcherUnhandledException !!!");
            LogStep(msg);

            MessageBox.Show(msg + Environment.NewLine + Environment.NewLine +
                            $"لاگ کامل در: {LogFilePath}",
                "خطای پردازش‌نشده",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // جلوگیری از بسته شدن برنامه
            e.Handled = true;
        }

        // ===== هندلر خطای سراسری CLR =====
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            var msg = "🔴 خطای پردازش‌نشده در AppDomain:" + Environment.NewLine + Environment.NewLine +
                      (ex != null ? BuildExceptionMessage(ex) : e.ExceptionObject?.ToString() ?? "null");
            LogStep("!!! CurrentDomain.UnhandledException !!!");
            LogStep(msg);

            try
            {
                MessageBox.Show(msg + Environment.NewLine + Environment.NewLine +
                                $"لاگ کامل در: {LogFilePath}",
                    "خطای بحرانی",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch
            {
                // اگر نتونست MessageBox نشون بده
            }

            Environment.FailFast(msg);
        }

        // ===== هندلر خطای Task های غیرمشاهده =====
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var msg = "🔴 خطای Task غیرمشاهده:" + Environment.NewLine + Environment.NewLine +
                      BuildExceptionMessage(e.Exception);
            LogStep("!!! TaskScheduler.UnobservedTaskException !!!");
            LogStep(msg);

            e.SetObserved();
        }

        /// <summary>
        /// نوشتن یک خط در فایل لاگ با timestamp
        /// </summary>
        private static void LogStep(string message)
        {
            try
            {
                var line = $"[{DateTime.Now:HH:mm:ss.fff}] {message}{Environment.NewLine}";
                File.AppendAllText(LogFilePath, line);

                // همزمان در Debug Output ویندوز هم بنویس
                Debug.WriteLine(line);
                Trace.WriteLine(line);
            }
            catch
            {
                // اگر نتونست در فایل بنویسه
            }
        }

        /// <summary>
        /// ساخت پیام خطای کامل شامل Inner Exception ها و Stack Trace
        /// </summary>
        private static string BuildExceptionMessage(Exception ex)
        {
            if (ex == null) return "خطای ناشناخته.";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("🔴 خطا در اجرای برنامه:");
            sb.AppendLine();
            sb.AppendLine($"Type: {ex.GetType().FullName}");
            sb.AppendLine();
            sb.AppendLine("Message:");
            sb.AppendLine(ex.Message);
            sb.AppendLine();

            var inner = ex.InnerException;
            int depth = 1;
            while (inner != null && depth <= 10)
            {
                sb.AppendLine($"--- Inner Exception #{depth} ---");
                sb.AppendLine($"Type: {inner.GetType().FullName}");
                sb.AppendLine($"Message: {inner.Message}");
                sb.AppendLine();
                inner = inner.InnerException;
                depth++;
            }

            sb.AppendLine("--- Stack Trace ---");
            sb.AppendLine(ex.StackTrace);

            return sb.ToString();
        }
    }
}
