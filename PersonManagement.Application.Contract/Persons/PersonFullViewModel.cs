using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Application.Contract.PersonBank;
using PersonManagement.Application.Contract.PersonContact;

namespace PersonManagement.Application.Contract.Persons
{
    public class PersonFullViewModel
    {
        public PersonViewModel Person { get; set; }
        public List<PersonContactViewModel> Contacts { get; set; }
        public List<PersonAddressViewModel> Addresses { get; set; }
        public List<PersonBankViewModel> Banks { get; set; }
    }
}