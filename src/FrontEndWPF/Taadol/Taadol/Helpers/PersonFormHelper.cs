using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Province;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
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
    /// <summary>
    /// Shared helpers for Person form views (NewPersonView, EditPersonView).
    /// Extracted to eliminate duplicated LoadProvincesAsync and LoadCitiesAsync code.
    /// </summary>
    public static class PersonFormHelper
    {
        /// <summary>
        /// Loads provinces into the given collection via IProvinceRepository.
        /// </summary>
        /// <param name="provinces">Target collection to populate (will be cleared first).</param>
        /// <param name="onError">Error handler — each view uses a different reporter (ToastManager vs Debug).</param>
        public static async Task LoadProvincesAsync(
            ObservableCollection<ProvinceViewModel> provinces,
            Action<Exception> onError)
        {
            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IProvinceRepository>();
                    return repo.GetProvincesForSelectList();
                }).ConfigureAwait(false);

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    provinces.Clear();
                    foreach (var p in items)
                        provinces.Add(p);
                });
                System.Diagnostics.Debug.WriteLine($"[PersonFormHelper] LoadProvincesAsync finished. Count = {provinces.Count}");
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        /// <summary>
        /// Loads cities for a given province via ICityRepository, with staleness check and city-reset logic.
        /// </summary>
        /// <param name="cities">Target collection to populate (will be cleared first).</param>
        /// <param name="provinceId">Province to load cities for.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <param name="getCurrentProvinceId">Returns the view's current SelectedProvinceId (for staleness check).</param>
        /// <param name="getCurrentCityId">Returns the view's current SelectedCityId (for reset check).</param>
        /// <param name="setSelectedCityId">Sets the view's SelectedCityId (to reset if no longer valid).</param>
        /// <param name="formName">View name for Debug.WriteLine messages (e.g. "NewPersonView").</param>
        /// <param name="onError">Error handler for non-cancellation exceptions.</param>
        /// <param name="onCancelled">Handler called when OperationCanceledException is caught (usually just Debug.WriteLine).</param>
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
                var items = await Task.Run(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ICityRepository>();
                    return repo.GetCitiesByProvince(provinceId);
                }, ct).ConfigureAwait(false);

                ct.ThrowIfCancellationRequested();
                // ✅ Staleness check: اگر کاربر استان را عوض کرده، نتایج قدیمی را نادیده بگیر
                if (getCurrentProvinceId() != provinceId)
                    return;

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // The selection may change after the background query completes but
                    // before this UI callback runs, so check again on the UI thread.
                    if (getCurrentProvinceId() != provinceId)
                        return;

                    cities.Clear();
                    foreach (var c in items)
                        cities.Add(c);

                    // اگر شهر انتخاب‌شده دیگر متعلق به این استان نیست، ریستش کن
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

        /// <summary>
        /// Fetches active contact types, populates the given list and dictionary.
        /// No Dispatcher needed — dictionary operations are not UI-bound.
        /// </summary>
        public static async Task LoadContactTypesAsync(
            List<ContactTypeViewModel> contactTypes,
            Dictionary<string, long> contactTypeByName,
            Action<Exception> onError)
        {
            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IContactTypeApplication>();
                    return app.GetActive();
                }).ConfigureAwait(false);
                contactTypes.Clear();
                contactTypes.AddRange(items);
                contactTypeByName.Clear();
                foreach (var ct in items)
                    contactTypeByName[ct.Title] = ct.Id;
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        /// <summary>
        /// Fetches person types. Optionally populates the given collection (with Dispatcher).
        /// Returns the items list so the caller can do view-specific post-processing.
        /// </summary>
        public static async Task<List<PersonTypeViewModel>> LoadPersonTypesAsync(
            ObservableCollection<PersonTypeViewModel> personTypes,
            Action<Exception> onError)
        {
            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonTypeApplication>();
                    return app.GetPersonTypes();
                }).ConfigureAwait(false);
                if (personTypes != null)
                {
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        personTypes.Clear();
                        foreach (var t in items)
                            personTypes.Add(t);
                    });
                }
                return items;
            }
            catch (Exception ex)
            {
                onError(ex);
                return new();
            }
        }

        /// <summary>
        /// Fetches branches and populates the collection (with Dispatcher).
        /// Returns items so the caller can do view-specific post-processing (e.g. SelectedBranchId).
        /// Note: branches must be BulkObservableCollection to support ReplaceAll.
        /// </summary>
        public static async Task<List<BranchComboItem>> LoadBranchesAsync(
            Action<List<BranchComboItem>> replaceAll,
            Action<Exception> onError)
        {
            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<GeneralInfoManagement.Application.Contract.Branches.IBranchApplication>();
                    return app.GetBranches()
                               .Select(b => new BranchComboItem { Id = b.Id, Title = b.Title })
                               .ToList();
                }).ConfigureAwait(false);
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => replaceAll(items));
                return items;
            }
            catch (Exception ex)
            {
                onError(ex);
                return new();
            }
        }

        /// <summary>
        /// Fetches bank branches and populates the collection (with Dispatcher).
        /// </summary>
        public static async Task LoadBankBranchesAsync(
            ObservableCollection<BankBranchViewModel> bankBranches,
            bool hasBankBranchApp,
            Action<Exception> onError,
            Action<string> onSkipped = null)
        {
            if (!hasBankBranchApp)
            {
                onSkipped?.Invoke("IBankBranchApplication null");
                return;
            }
            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<BankManagement.Application.Contracts.BankBranch.IBankBranchApplication>();
                    return app.GetBankBranches();
                }).ConfigureAwait(false);
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    bankBranches.Clear();
                    foreach (var b in items)
                        bankBranches.Add(b);
                });
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        /// <summary>
        /// Loads the category tree for a given person type via IPersonCategoryApplication.
        /// Returns the tree DTO so the caller can populate its own CategorySearch controls.
        /// No UI dependencies — each view sets PersonTypeId and calls LoadFromTreeDto itself.
        /// </summary>
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
                var tree = await Task.Run(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                    return app.GetTree(personTypeId);
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
    }
}
