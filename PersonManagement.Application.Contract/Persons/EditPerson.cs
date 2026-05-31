namespace PersonManagement.Application.Contract.Persons
{
    public class EditPerson : CreatePerson
    {
        public long Id { get; set; }
        public string? CurrentCode { get; set; }


    }
}
