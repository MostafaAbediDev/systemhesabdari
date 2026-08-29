using System;
using System.Collections.ObjectModel;
using System.Linq;
using Taadol.ViewModels;
using Taadol.Views;
using Xunit;

namespace Taadol.Tests;

public sealed class FrontEndListViewModelTests
{
    [Fact]
    public void PersonPagination_UsesPageSizeAndGlobalRowNumbers()
    {
        var viewModel = new PersonListViewModel(null!);
        viewModel.AllPersons = new ObservableCollection<PersonItem>(
            Enumerable.Range(1, 31).Select(id => CreatePerson(id)));

        viewModel.ApplyFilters();

        Assert.Equal(15, viewModel.FilteredPersons.Count);
        Assert.Equal(1, viewModel.FilteredPersons[0].RowNumber);
        Assert.Equal(15, viewModel.FilteredPersons[^1].RowNumber);
        Assert.Contains("۱۵", viewModel.PageInfoText);
        Assert.Contains("۳۱", viewModel.PageInfoText);

        viewModel.GoToPage(3);

        Assert.Single(viewModel.FilteredPersons);
        Assert.Equal(31, viewModel.FilteredPersons[0].RowNumber);
        Assert.True(viewModel.Pages.Single(page => page.IsCurrent).PageNumber == 3);
    }

    [Fact]
    public void PersonPageSizeChange_ResetsToFirstPage()
    {
        var viewModel = new PersonListViewModel(null!);
        viewModel.AllPersons = new ObservableCollection<PersonItem>(
            Enumerable.Range(1, 40).Select(id => CreatePerson(id)));
        viewModel.GoToPage(3);

        viewModel.ChangePageSize(20);

        Assert.Equal(1, viewModel.CurrentPage);
        Assert.Equal(20, viewModel.FilteredPersons.Count);
        Assert.Equal(1, viewModel.FilteredPersons[0].RowNumber);
    }

    [Fact]
    public void PersonSearch_ResetsPageAndReturnsMatchingRows()
    {
        var viewModel = new PersonListViewModel(null!);
        viewModel.AllPersons = new ObservableCollection<PersonItem>(new[]
        {
            CreatePerson(1, "علی رضایی"),
            CreatePerson(2, "سارا احمدی"),
            CreatePerson(3, "علی کریمی")
        });
        viewModel.GoToPage(2);

        viewModel.HandleSearchTextChanged("علی");

        Assert.Equal(1, viewModel.CurrentPage);
        Assert.Equal(2, viewModel.FilteredPersons.Count);
        Assert.All(viewModel.FilteredPersons, person => Assert.Contains("علی", person.FullName));
    }

    [Fact]
    public void PersonStatusFilter_OnlyReturnsSelectedStatus()
    {
        var viewModel = new PersonListViewModel(null!);
        viewModel.AllPersons = new ObservableCollection<PersonItem>(new[]
        {
            CreatePerson(1, status: "فعال"),
            CreatePerson(2, status: "غیرفعال"),
            CreatePerson(3, status: "فعال")
        });

        viewModel.SetFilterResult("status", new() { "غیرفعال" });

        Assert.Single(viewModel.FilteredPersons);
        Assert.Equal("غیرفعال", viewModel.FilteredPersons[0].Status);
    }

    [Fact]
    public void PersonSelectAll_SelectsOnlyCurrentFilteredPage()
    {
        var viewModel = new PersonListViewModel(null!);
        viewModel.AllPersons = new ObservableCollection<PersonItem>(
            Enumerable.Range(1, 20).Select(id => CreatePerson(id)));
        viewModel.GoToPage(2);

        viewModel.SelectAll();

        Assert.Equal(5, viewModel.GetSelectedItems().Count);
        Assert.All(viewModel.FilteredPersons, person => Assert.True(person.IsSelected));
        Assert.All(viewModel.AllPersons.Take(15), person => Assert.False(person.IsSelected));
    }

    [Fact]
    public void PersonSummary_SeparatesDebitAndCreditAndCalculatesSelectedNet()
    {
        var viewModel = new PersonListViewModel(null!);
        var debit = CreatePerson(1, accountStatus: "بدهکار", balance: "۱٬۰۰۰");
        var credit = CreatePerson(2, accountStatus: "بستانکار", balance: "۳۰۰");
        debit.IsSelected = true;
        credit.IsSelected = true;
        viewModel.AllPersons = new ObservableCollection<PersonItem> { debit, credit };

        viewModel.ApplyFilters();

        Assert.Contains("۱٬۰۰۰", viewModel.TotalDebitText);
        Assert.Contains("۳۰۰", viewModel.TotalCreditText);
        Assert.Contains("۷۰۰", viewModel.SelectedTotalText);
        Assert.Contains("۲", viewModel.SelectedSummaryText);
    }

    [Fact]
    public void BranchArchivePaginationAndSearch_WorkTogether()
    {
        var viewModel = new BranchArchiveListViewModel(null!);
        viewModel.AllArchives = new ObservableCollection<BranchArchiveItem>(
            Enumerable.Range(1, 26).Select(i => new BranchArchiveItem
            {
                Id = i,
                BranchId = i,
                Title = i == 26 ? "قرارداد ویژه" : $"سند {i}",
                Description = "آرشیو",
                BranchTitle = "شعبه مرکزی",
                CompanyName = "شرکت نمونه",
                File = "file.pdf"
            }));

        viewModel.ApplyFilters();
        Assert.Equal(15, viewModel.FilteredArchives.Count);

        viewModel.ChangePageSize(10);
        viewModel.GoToPage(3);
        Assert.Equal(6, viewModel.FilteredArchives.Count);
        Assert.Equal(21, viewModel.FilteredArchives[0].RowNumber);

        viewModel.HandleSearchTextChanged("ویژه");
        Assert.Equal(1, viewModel.CurrentPage);
        Assert.Single(viewModel.FilteredArchives);
        Assert.Equal(26, viewModel.FilteredArchives[0].Id);
    }

    private static PersonItem CreatePerson(
        int id,
        string? fullName = null,
        string status = "فعال",
        string accountStatus = "بی حساب",
        string balance = "")
    {
        return new PersonItem
        {
            Id = id,
            Code = id.ToString(),
            FullNameText = fullName ?? $"شخص {id}",
            Status = status,
            PersonType = "مشتری",
            AccountStatus = accountStatus,
            BalanceDisplay = balance,
            Company = "شرکت نمونه",
            Province = "تهران",
            City = "تهران",
            Mobile = "09120000000",
            Phone = "02100000000",
            IsEmpty = false
        };
    }
}
