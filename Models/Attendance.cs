using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sample.Data
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceId { get; set; }

        public int EmpId { get; set; }
        public string? EmployeeName { get; set; } 
        public DateTime AttendanceDate { get; set; }

        public Attendance()
        {
            AttendanceDate = DateTime.Now;
        }

        public Attendance(int empId) : this()
        {
            EmpId = empId;
        }
    }
}
