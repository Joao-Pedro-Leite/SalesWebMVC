using System.ComponentModel.DataAnnotations;

namespace SalesWebMVC.Models
{
    public class Seller
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome necessário")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Nome o nome deve conter de {2} a {1} caracteres")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Coloque um E-mail válido")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "E-mail necessário")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Data de Nascimento necessário")]
        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Valor do salário necessário")]
        [Range(100.0, 50000.0, ErrorMessage = "O valor do salário deve ser entre {1} e {2}")]
        [Display(Name = "Base Salary")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double BaseSalary { get; set; }

        public Department? Department { get; set; }
        public int DepartmentId { get; set; }
        public ICollection<SalesRecord> Sales { get; set; } = new List<SalesRecord>();

        public Seller()
        {
        }
        public Seller(int id, string name, string email, DateTime birthDate, 
            double baseSalary, Department department)
        {
            Id = id;
            Name = name;
            Email = email;
            BirthDate = birthDate;
            BaseSalary = baseSalary;
            Department = department;
        }
        public void AddSales(SalesRecord sr)
        {
            Sales.Add(sr);
        }
        public void RemoveSales(SalesRecord sr) 
        { 
            Sales.Remove(sr);
        }
        public double TotalSales(DateTime initial, DateTime final)
        {

            return Sales.Where(sr =>  sr.Date >= initial && sr.Date <= final).Sum(sr => sr.Amount); 
        }

        
    }
}
