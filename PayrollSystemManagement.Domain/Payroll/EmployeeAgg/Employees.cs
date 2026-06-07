using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using PayrollSystemManagement.Domain.Payroll.DepartmentAgg;
using PayrollSystemManagement.Domain.Payroll.JobTitleAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace PayrollSystemManagement.Domain.Payroll.EmployeeAgg
{
    public class Employees : EntityBase
    {
        public string EmployeeCode { get; private set; }
        public string InsuranceNumber { get; private set; }
        public string? Description { get; private set; }
        public DateTime HireDate { get; private set; }
        public DateTime? TerminationDate { get; private set; }
        public decimal BaseSalary { get; private set; }
        public long BranchId { get; private set; }
        public long PersonId { get; private set; }
        public long DepartmentId { get; private set; }
        public long JobTitleId { get; private set; }
        public EmployeeContractType ContractType { get; private set; }
        public Branches Branches { get; private set; }
        public Persons Persons { get; private set; }
        public Departments Departments { get; private set; }
        public JobTitles JobTitles { get; private set; }
        public List<Payrolls> Payrolls { get; private set; }

        protected Employees()
        {
            Payrolls = new List<Payrolls>();
        }

        public Employees(string employeeCode, long personId, long branchId, long departmentId, long jobTitleId,
        string insuranceNumber, DateTime hireDate, decimal baseSalary, string? description,
            EmployeeContractType contractType)
        {
            if (string.IsNullOrWhiteSpace(employeeCode))
                throw new ArgumentException("EmployeeCode is required");

            if (personId <= 0)
                throw new ArgumentException("Person is invalid");

            if (branchId <= 0)
                throw new ArgumentException("Branch is invalid");

            if (departmentId <= 0)
                throw new ArgumentException("Department is invalid");

            if (jobTitleId <= 0)
                throw new ArgumentException("JobTitle is invalid");

            if (baseSalary <= 0)
                throw new ArgumentException("BaseSalary must be greater than zero");

            EmployeeCode = employeeCode;
            PersonId = personId;
            BranchId = branchId;
            DepartmentId = departmentId;
            JobTitleId = jobTitleId;
            InsuranceNumber = insuranceNumber;
            HireDate = hireDate;
            BaseSalary = baseSalary;
            Description = description;
            ContractType = contractType;
            IsActive = true;
        }

        public void Edit(string employeeCode, long departmentId, long jobTitleId, string insuranceNumber, string? description, decimal baseSalary)
        {
            if (string.IsNullOrWhiteSpace(employeeCode))
                throw new ArgumentException("EmployeeCode is required");

            if (departmentId <= 0)
                throw new ArgumentException("Department is invalid");

            if (jobTitleId <= 0)
                throw new ArgumentException("JobTitle is invalid");

            if (baseSalary <= 0)
                throw new ArgumentException("BaseSalary must be greater than zero");

            EmployeeCode = employeeCode;

            DepartmentId = departmentId;
            JobTitleId = jobTitleId;

            InsuranceNumber = insuranceNumber;

            Description = description?.Trim();
            BaseSalary = baseSalary;
        }

        public void ChangeContractType(EmployeeContractType contractType)
        {
            ContractType = contractType;
        }
        public void Terminate(DateTime date)
        {
            TerminationDate = date;
            IsActive = false;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
