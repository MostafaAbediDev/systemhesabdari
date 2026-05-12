using Bogus;
using Microsoft.EntityFrameworkCore;
using PersonManagement.Domain.Person.ContactTypeAgg;
using PersonManagement.Domain.Person.PersonAgg;
using PersonManagement.Domain.Person.PersonAddressAgg;
using PersonManagement.Domain.Person.PersonBankAgg;
using PersonManagement.Domain.Person.PersonContactAgg;
using PersonManagement.Domain.Person.PersonTypeAgg;
using GeneralInfoManagement.Infrastructure.EFCore;

namespace PersonManagement.Infrastructure.EFCore.Seed;

public class PersonFakeSeeder
{
    private readonly PersonFakeDataContext _context;
    private readonly GeneralInfoFakeDataContext _generalContext;

    public PersonFakeSeeder(
        PersonFakeDataContext context,
        GeneralInfoFakeDataContext generalContext)
    {
        _context = context;
        _generalContext = generalContext;
    }

    public async Task SeedAsync()
    {
        if (await _context.Persons.AnyAsync())
            return;

        var faker = new Faker("fa");

        // -------------------------
        // ContactTypes
        // -------------------------

        if (!await _context.ContactTypes.AnyAsync())
        {
            await _context.ContactTypes.AddRangeAsync(
                new ContactTypes("موبایل"),
                new ContactTypes("ایمیل"),
                new ContactTypes("تلفن")
            );

            await _context.SaveChangesAsync();
        }

        var personTypeIds = await _context.PersonTypes
            .Select(x => x.Id)
            .ToListAsync();

        var mobileTypeId = await _context.ContactTypes
            .Where(x => x.Title == "موبایل")
            .Select(x => x.Id)
            .FirstAsync();

        // -------------------------
        // Persons
        // -------------------------

        var branches = await _generalContext.Branches.ToListAsync();

        if (!branches.Any())
            throw new Exception("No branch found in database.");

        var personFaker = new Faker<Persons>("fa")
            .CustomInstantiator(f =>
            {
                var branch = f.PickRandom(branches);

                return new Persons(
                    code: f.Random.Replace("P#####"),
                    fullName: f.Name.FullName(),
                    nationalCode: f.Random.ReplaceNumbers("##########"),
                    economicCode: f.Random.ReplaceNumbers("ECO#####"),
                    registrationNumber: f.Random.ReplaceNumbers("REG#####"),
                    personTypeId: f.PickRandom(personTypeIds),
                    branchId: branch.Id
                );
            });

        var persons = personFaker.Generate(500);

        await _context.Persons.AddRangeAsync(persons);

        await _context.SaveChangesAsync();


        // -------------------------
        // Addresses
        // -------------------------

        var cities = await _generalContext.Cities
            .Select(c => new
            {
                c.Id,
                c.ProvinceId
            })
            .ToListAsync();

        var addresses = persons.Select(p =>
        {
            var city = faker.PickRandom(cities);

            return new PersonAddresses(
                personId: p.Id,
                provinceId: city.ProvinceId,
                cityId: city.Id,
                address: faker.Address.FullAddress(),
                postalCode: faker.Random.ReplaceNumbers("##########")
            );

        }).ToList();

        await _context.PersonAddresses.AddRangeAsync(addresses);

        // -------------------------
        // Banks
        // -------------------------

        var bankBins = new Dictionary<string, string[]>
        {
            ["ملت"] = new[] { "610433" },
            ["ملی"] = new[] { "603799" },
            ["صادرات"] = new[] { "603769" },
            ["تجارت"] = new[] { "627353" },
            ["کشاورزی"] = new[] { "603770" },
            ["سامان"] = new[] { "621986" },
            ["پاسارگاد"] = new[] { "502229" },
            ["پارسیان"] = new[] { "622106" },
            ["آینده"] = new[] { "636214" }
        };

        var bankNames = bankBins.Keys.ToArray();

        var banks = persons.Select(p =>
        {
            var bankName = faker.PickRandom(bankNames);
            var bin = faker.PickRandom(bankBins[bankName]);

            return new PersonBanks(
                bankName: bankName,
                accountNumber: faker.Random.ReplaceNumbers("################"),
                cardNumber: GenerateCardNumber(faker, bin),
                shaba: "IR" + faker.Random.ReplaceNumbers("########################"),
                personId : p.Id
            );

        }).ToList();

        await _context.PersonBanks.AddRangeAsync(banks);

        // -------------------------
        // Contacts
        // -------------------------

        var contacts = persons.Select(p =>
            new PersonContacts(
                personId: p.Id,
                contactTypeId: mobileTypeId,
                value: faker.Phone.PhoneNumber("09#########"),
                description: "شماره تماس"
                )).ToList();

        await _context.PersonContacts.AddRangeAsync(contacts);


        await _context.SaveChangesAsync();
    }

    // -------------------------
    // Card Generator
    // -------------------------

    static string GenerateCardNumber(Faker faker, string bin)
    {
        var first15 = bin + faker.Random.ReplaceNumbers("#########");
        var check = LuhnCheckDigit(first15);
        return first15 + check;
    }

    static int LuhnCheckDigit(string number)
    {
        int sum = 0;
        bool doubleDigit = true;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int d = number[i] - '0';

            if (doubleDigit)
            {
                d *= 2;

                if (d > 9)
                    d -= 9;
            }

            sum += d;

            doubleDigit = !doubleDigit;
        }

        return (10 - (sum % 10)) % 10;
    }
}
