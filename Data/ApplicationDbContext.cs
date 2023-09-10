using Microsoft.EntityFrameworkCore;
using Sample.Data;

namespace Sample.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        public void AddEmployee(Employee employee)
        {
            Employees.Add(employee);
            SaveChanges();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Salary>().ToTable("Salaries");
            modelBuilder.Entity<Attendance>().ToTable("Attendances");
            modelBuilder.Entity<Salary>().Property(s => s.SalaryAmount).HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Attendance>()
                .Property(a => a.EmpId)
                .ValueGeneratedNever(); 

        }

    }
}
