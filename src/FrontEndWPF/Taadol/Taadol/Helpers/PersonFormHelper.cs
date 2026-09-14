using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Province;
using Microsoft.Extensions.DependencyInjection;
using BankManagement.Application.Contracts.BankBranch;
using PersonManagement.Application.Contract.ContactTypes;
using PersonManagement.Application.Contract.PersonCategory;
using PersonManagement.Application.Contract.PersonTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Taadol.Views;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Taadol.Helpers
{

    public static class PersonFormHelper
    {

        public static async Task LoadProvincesAsync(
            ObservableCollection<ProvinceViewModel> provinces,
            Action<Exception> onError,
            CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var items = await Task.Run(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var scope = App.ServiceProvider.CreateScope();
                        var application = scope.ServiceProvider.GetRequiredService<IProvinceApplication>();
                        var result = application.GetProvinces();
                        cancellationToken.ThrowIfCancellationRequested();
                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<ProvinceViewModel>();
                    }
                }, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        provinces.Clear();
                        foreach (var p in items)
                            provinces.Add(p);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                });
                System.Diagnostics.Debug.WriteLine($"[PersonFormHelper] LoadProvincesAsync finished. Count = {provinces.Count}");
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        public static async Task LoadCitiesAsync(
            ObservableCollection<CityViewModel> cities,
            long provinceId,
            CancellationToken? token,
            Func<long> getCurrentProvinceId,
            Func<long> getCurrentCityId,
            Action<long> setSelectedCityId,
            string formName,
            Action<Exception> onError,
            Action onCancelled = null)
        {
            if (provinceId <= 0) return;

            try
            {
                var ct = token ?? CancellationToken.None;
                ct.ThrowIfCancellationRequested();
                var items = await Task.Run(() =>
                {
                    try
                    {
                        ct.ThrowIfCancellationRequested();
                        using var scope = App.ServiceProvider.CreateScope();
                        var application = scope.ServiceProvider.GetRequiredService<ICityApplication>();
                        return application.GetCitiesByProvinceId(provinceId);
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<CityViewModel>();
                    }
                }, ct).ConfigureAwait(false);

                ct.ThrowIfCancellationRequested();

                if (getCurrentProvinceId() != provinceId)
                    return;

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {

                    if (getCurrentProvinceId() != provinceId)
                        return;

                    cities.Clear();
                    foreach (var c in items)
                        cities.Add(c);

                    if (cities.All(c => c.Id != getCurrentCityId()))
                        setSelectedCityId(0);
                });
                System.Diagnostics.Debug.WriteLine($"[PersonFormHelper] LoadCitiesAsync finished. ProvinceId={provinceId}, Cities.Count = {cities.Count}, SelectedCityId={getCurrentCityId()}");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine($"[{formName}] LoadCitiesAsync was cancelled");
                onCancelled?.Invoke();
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        public static async Task LoadContactTypesAsync(
            List<ContactTypeViewModel> contactTypes,
            Dictionary<string, long> contactTypeByName,
            Action<Exception> onError,
            CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var items = await Task.Run(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var scope = App.ServiceProvider.CreateScope();
                        var app = scope.ServiceProvider.GetRequiredService<IContactTypeApplication>();
                        var result = app.GetActive();
                        cancellationToken.ThrowIfCancellationRequested();
                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<ContactTypeViewModel>();
                    }
                }, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                contactTypes.Clear();
                contactTypes.AddRange(items);
                contactTypeByName.Clear();
                foreach (var ct in items)
                    contactTypeByName[ct.Title] = ct.Id;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        public static async Task<List<PersonTypeViewModel>> LoadPersonTypesAsync(
            ObservableCollection<PersonTypeViewModel> personTypes,
            Action<Exception> onError,
            CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var items = await Task.Run(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var scope = App.ServiceProvider.CreateScope();
                        var app = scope.ServiceProvider.GetRequiredService<IPersonTypeApplication>();
                        var result = app.GetPersonTypes();
                        cancellationToken.ThrowIfCancellationRequested();
                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<PersonTypeViewModel>();
                    }
                }, cancellationToken).ConfigureAwait(false);
                if (personTypes != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            personTypes.Clear();
                            foreach (var t in items)
                                personTypes.Add(t);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                    });
                }
                return items;
            }
            catch (OperationCanceledException)
            {
                return new();
            }
            catch (Exception ex)
            {
                onError(ex);
                return new();
            }
        }

        public static async Task<List<BranchComboItem>> LoadBranchesAsync(
            Action<List<BranchComboItem>> replaceAll,
            Action<Exception> onError,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var items = await Task.Run(() =>
                {
                    try
                    {
                        using var scope = App.ServiceProvider.CreateScope();
                        cancellationToken.ThrowIfCancellationRequested();
                        var app = scope.ServiceProvider.GetRequiredService<GeneralInfoManagement.Application.Contract.Branches.IBranchApplication>();
                        var branches = app.GetBranches();
                        if (cancellationToken.IsCancellationRequested)
                            return new List<BranchComboItem>();
                        return branches
                                   .Select(b => new BranchComboItem { Id = b.Id, Title = b.Title })
                                   .ToList();
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<BranchComboItem>();
                    }
                }, cancellationToken).ConfigureAwait(false);
                if (cancellationToken.IsCancellationRequested)
                    return new();

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (!cancellationToken.IsCancellationRequested)
                        replaceAll(items);
                }, System.Windows.Threading.DispatcherPriority.Normal);
                return items;
            }
            catch (OperationCanceledException)
            {
                return new();
            }
            catch (Exception ex)
            {
                onError(ex);
                return new();
            }
        }

        public static async Task LoadBankBranchesAsync(
            ObservableCollection<BankBranchViewModel> bankBranches,
            bool hasBankBranchApp,
            Action<Exception> onError,
            CancellationToken cancellationToken,
            Action<string> onSkipped = null)
        {
            if (!hasBankBranchApp)
            {
                onSkipped?.Invoke("IBankBranchApplication null");
                return;
            }
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var items = await Task.Run(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var scope = App.ServiceProvider.CreateScope();
                        var app = scope.ServiceProvider.GetRequiredService<BankManagement.Application.Contracts.BankBranch.IBankBranchApplication>();
                        var result = app.GetBankBranches();
                        cancellationToken.ThrowIfCancellationRequested();
                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<BankBranchViewModel>();
                    }
                }, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        bankBranches.Clear();
                        foreach (var b in items)
                            bankBranches.Add(b);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                });
            }
            catch (OperationCanceledException)
            {

                return;
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        public static async Task<List<PersonCategoryTreeViewModel>> LoadCategoryTreeAsync(
            long personTypeId,
            CancellationToken? token,
            Action<Exception> onError,
            Action onCancelled = null)
        {
            if (personTypeId <= 0) return null;

            try
            {
                var ct = token ?? CancellationToken.None;
                ct.ThrowIfCancellationRequested();
                var tree = await Task.Run(() =>
                {
                    try
                    {
                        ct.ThrowIfCancellationRequested();
                        using var scope = App.ServiceProvider.CreateScope();
                        var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                        return app.GetTree(personTypeId);
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                }, ct).ConfigureAwait(false);

                ct.ThrowIfCancellationRequested();
                return tree;
            }
            catch (OperationCanceledException)
            {
                onCancelled?.Invoke();
                return null;
            }
            catch (Exception ex)
            {
                onError(ex);
                return null;
            }
        }

        public static async Task<List<PersonCategoryTreeViewModel>> LoadDepartmentsTreeAsync(
            CancellationToken cancellationToken,
            Action<Exception> onError)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var items = await Task.Run(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var scope = App.ServiceProvider.CreateScope();
                        var app = scope.ServiceProvider.GetRequiredService<PayrollSystemManagement.Application.Contracts.Department.IDepartmentApplication>();
                        return app.GetDepartments()?.Where(d => d.IsActive).ToList();
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<PayrollSystemManagement.Application.Contracts.Department.DepartmentViewModel>();
                    }
                }, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
                var root = new PersonCategoryTreeViewModel
                {
                    Id = 0,
                    Title = "دپارتمان",
                    Children = (items ?? new List<PayrollSystemManagement.Application.Contracts.Department.DepartmentViewModel>())
                        .Select(d => new PersonCategoryTreeViewModel
                        {
                            Id = d.Id,
                            Title = d.Name ?? "",
                            Children = new List<PersonCategoryTreeViewModel>()
                        }).ToList()
                };
                return new List<PersonCategoryTreeViewModel> { root };
            }
            catch (OperationCanceledException)
            {
                return new();
            }
            catch (Exception ex)
            {
                onError(ex);
                return new();
            }
        }

        public static async Task<List<PersonCategoryTreeViewModel>> LoadJobTitlesTreeAsync(
            CancellationToken cancellationToken,
            Action<Exception> onError,
            string departmentName = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var items = await Task.Run(() =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var scope = App.ServiceProvider.CreateScope();
                        var app = scope.ServiceProvider.GetRequiredService<PayrollSystemManagement.Application.Contracts.JobTitle.IJobTitleApplication>();
                        var jobTitles = app.GetJobTitles()?.Where(j => j.IsActive).ToList();
                        if (!string.IsNullOrWhiteSpace(departmentName))
                            jobTitles = jobTitles?.Where(j => j.DepartmentName == departmentName).ToList();
                        return jobTitles;
                    }
                    catch (OperationCanceledException)
                    {
                        return new List<PayrollSystemManagement.Application.Contracts.JobTitle.JobTitleViewModel>();
                    }
                }, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
                var root = new PersonCategoryTreeViewModel
                {
                    Id = 0,
                    Title = "عنوان شغلی",
                    Children = (items ?? new List<PayrollSystemManagement.Application.Contracts.JobTitle.JobTitleViewModel>())
                        .Select(j => new PersonCategoryTreeViewModel
                        {
                            Id = j.Id,
                            Title = j.Title ?? "",
                            Children = new List<PersonCategoryTreeViewModel>()
                        }).ToList()
                };
                return new List<PersonCategoryTreeViewModel> { root };
            }
            catch (OperationCanceledException)
            {
                return new();
            }
            catch (Exception ex)
            {
                onError(ex);
                return new();
            }
        }

        public static List<PersonCategoryTreeViewModel> BuildDepartmentTree(
            List<PayrollSystemManagement.Application.Contracts.Department.DepartmentViewModel> items)
        {
            var root = new PersonCategoryTreeViewModel
            {
                Id = 0,
                Title = "دپارتمان",
                Children = (items ?? new List<PayrollSystemManagement.Application.Contracts.Department.DepartmentViewModel>())
                    .Select(d => new PersonCategoryTreeViewModel
                    {
                        Id = d.Id,
                        Title = d.Name ?? "",
                        Children = new List<PersonCategoryTreeViewModel>()
                    }).ToList()
            };
            return new List<PersonCategoryTreeViewModel> { root };
        }

        public static List<PersonCategoryTreeViewModel> BuildJobTitleTree(
            List<PayrollSystemManagement.Application.Contracts.JobTitle.JobTitleViewModel> items)
        {
            var root = new PersonCategoryTreeViewModel
            {
                Id = 0,
                Title = "عنوان شغلی",
                Children = (items ?? new List<PayrollSystemManagement.Application.Contracts.JobTitle.JobTitleViewModel>())
                    .Select(j => new PersonCategoryTreeViewModel
                    {
                        Id = j.Id,
                        Title = j.Title ?? "",
                        Children = new List<PersonCategoryTreeViewModel>()
                    }).ToList()
            };
            return new List<PersonCategoryTreeViewModel> { root };
        }
    }
}
