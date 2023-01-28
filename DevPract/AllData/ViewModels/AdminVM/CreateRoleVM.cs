using System.ComponentModel.DataAnnotations;

namespace DevPract.AllData.ViewModels.AdminVM
{
    public class CreateRoleVM
    {
        [Required]
       // [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
