namespace Sample.Data
{
    public class Salary
    {
        public int SalaryId { get; set; }
        public int EmpId { get; set; }
        public string? EmployeeName { get; set; } 
        public string? Designation { get; set; }
        public DateTime SalaryDate { get; set; }

        public decimal SalaryAmount { get; set; }

        public decimal TaxAmount { get; set; } 
        public decimal TotalPFAmount { get; set; } 
        public decimal NetSalary { get; set; } 

        public Salary(int empId)
        {
            EmpId = empId;
        }

    }
}
