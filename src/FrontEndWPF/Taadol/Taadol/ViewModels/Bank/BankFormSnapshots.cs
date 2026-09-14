using System;

namespace Taadol.ViewModels
{

    public abstract class FormSnapshotBase
    {
    }

    public sealed class BankNewFormSnapshot : FormSnapshotBase
    {
        public BankNewFormSnapshot(
            string? uniqueCode,
            bool isUniqueCodeAutomatic,
            string? title,
            string? country,
            string? description,
            string? logo,
            long selectedBankTypeId,
            bool isActive)
        {
            UniqueCode = uniqueCode ?? string.Empty;
            IsUniqueCodeAutomatic = isUniqueCodeAutomatic;
            Title = title ?? string.Empty;
            Country = country ?? string.Empty;
            Description = description ?? string.Empty;
            Logo = logo ?? string.Empty;
            SelectedBankTypeId = selectedBankTypeId;
            IsActive = isActive;
        }

        public string UniqueCode { get; }
        public bool IsUniqueCodeAutomatic { get; }
        public string Title { get; }
        public string Country { get; }
        public string Description { get; }
        public string Logo { get; }
        public long SelectedBankTypeId { get; }
        public bool IsActive { get; }

        public override bool Equals(object? obj) =>
            obj is BankNewFormSnapshot other &&
            string.Equals(UniqueCode, other.UniqueCode, StringComparison.Ordinal) &&
            IsUniqueCodeAutomatic == other.IsUniqueCodeAutomatic &&
            string.Equals(Title, other.Title, StringComparison.Ordinal) &&
            string.Equals(Country, other.Country, StringComparison.Ordinal) &&
            string.Equals(Description, other.Description, StringComparison.Ordinal) &&
            string.Equals(Logo, other.Logo, StringComparison.Ordinal) &&
            SelectedBankTypeId == other.SelectedBankTypeId &&
            IsActive == other.IsActive;

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + UniqueCode.GetHashCode();
                hash = (hash * 23) + IsUniqueCodeAutomatic.GetHashCode();
                hash = (hash * 23) + Title.GetHashCode();
                hash = (hash * 23) + Country.GetHashCode();
                hash = (hash * 23) + Description.GetHashCode();
                hash = (hash * 23) + Logo.GetHashCode();
                hash = (hash * 23) + SelectedBankTypeId.GetHashCode();
                hash = (hash * 23) + IsActive.GetHashCode();
                return hash;
            }
        }
    }

    public sealed class BankEditFormSnapshot : FormSnapshotBase
    {
        public BankEditFormSnapshot(
            string? uniqueCode,
            string? title,
            string? country,
            string? description,
            string? logo,
            long selectedBankTypeId)
        {
            UniqueCode = uniqueCode ?? string.Empty;
            Title = title ?? string.Empty;
            Country = country ?? string.Empty;
            Description = description ?? string.Empty;
            Logo = logo ?? string.Empty;
            SelectedBankTypeId = selectedBankTypeId;
        }

        public string UniqueCode { get; }
        public string Title { get; }
        public string Country { get; }
        public string Description { get; }
        public string Logo { get; }
        public long SelectedBankTypeId { get; }

        public override bool Equals(object? obj) =>
            obj is BankEditFormSnapshot other &&
            string.Equals(UniqueCode, other.UniqueCode, StringComparison.Ordinal) &&
            string.Equals(Title, other.Title, StringComparison.Ordinal) &&
            string.Equals(Country, other.Country, StringComparison.Ordinal) &&
            string.Equals(Description, other.Description, StringComparison.Ordinal) &&
            string.Equals(Logo, other.Logo, StringComparison.Ordinal) &&
            SelectedBankTypeId == other.SelectedBankTypeId;

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + UniqueCode.GetHashCode();
                hash = (hash * 23) + Title.GetHashCode();
                hash = (hash * 23) + Country.GetHashCode();
                hash = (hash * 23) + Description.GetHashCode();
                hash = (hash * 23) + Logo.GetHashCode();
                hash = (hash * 23) + SelectedBankTypeId.GetHashCode();
                return hash;
            }
        }
    }
}
