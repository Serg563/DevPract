using System.ComponentModel.DataAnnotations;

namespace DevPract.Models.ViewModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
