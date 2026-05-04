using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace PayrollSystemManagement.Domain.Payroll.EmployeeAgg
{
    public class Employees : EntityBase
    {
        public string EmployeeCode { get; private set; }
        public string Department { get; private set; }
        public string JobTitle { get; private set; }
        public string ContractType { get; private set; }
        public string InsuranceNumber { get; private set; }
        public string NationalCode { get; private set; }
        public string BankAccountNumber { get; private set; }
        public string IBAN { get; private set; }
        public string Description { get; private set; }
        public DateTime HireDate { get; private set; }
        public DateTime TerminationDate { get; private set; }
        public decimal BaseSalary { get; private set; }
        public long BranchId { get; private set; }
        public long PersonId { get; private set; }
        public Branches Branches { get; private set; }
        public Persons Persons { get; private set; }
        public List<Payrolls> Payrolls { get; private set; }

        protected Employees()
        {
            Payrolls = new List<Payrolls>();
        }

        public Employees(string employeeCode, string department, string jobTitle, string contractType, string insuranceNumber, 
            string nationalCode, string bankAccountNumber, string iBAN, string description, DateTime hireDate, DateTime terminationDate, decimal baseSalary)
        {
            EmployeeCode = employeeCode;
            Department = department;
            JobTitle = jobTitle;
            ContractType = contractType;
            InsuranceNumber = insuranceNumber;
            NationalCode = nationalCode;
            BankAccountNumber = bankAccountNumber;
            IBAN = iBAN;
            Description = description;
            HireDate = hireDate;
            TerminationDate = terminationDate;
            BaseSalary = baseSalary;
        }

        public void Edit(string employeeCode, string department, string jobTitle, string contractType, string insuranceNumber,
            string nationalCode, string bankAccountNumber, string iBAN, string description, DateTime hireDate, DateTime terminationDate, decimal baseSalary)
        {
            EmployeeCode = employeeCode;
            Department = department;
            JobTitle = jobTitle;
            ContractType = contractType;
            InsuranceNumber = insuranceNumber;
            NationalCode = nationalCode;
            BankAccountNumber = bankAccountNumber;
            IBAN = iBAN;
            Description = description;
            HireDate = hireDate;
            TerminationDate = terminationDate;
            BaseSalary = baseSalary;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Active()
        {
            IsActive = true;
        }

        public void NotActive()
        {
            IsActive = false;
        }
    }
}
