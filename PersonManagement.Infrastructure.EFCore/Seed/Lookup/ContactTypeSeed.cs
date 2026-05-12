using PersonManagement.Domain.Person.ContactTypeAgg;

namespace PersonManagement.Infrastructure.EFCore.Seed.Lookup
{
    public class ContactTypeSeed
    {
        public static void Seed(PersonSystemContext context)
        {
            if (context.ContactTypes.Any())
                return;

            context.ContactTypes.AddRange(
                new ContactTypes("موبایل"),
                new ContactTypes("تلفن ثابت"),
                new ContactTypes("ایمیل"),
                new ContactTypes("فکس")
            );

            context.SaveChanges();
        }
    }
}
