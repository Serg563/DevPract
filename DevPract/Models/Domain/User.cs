using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DevPract.Models.Domain
{
    public class User : IdentityUser
    {
       

        [Display(Name = "Full name")]
        public string? FullName { get; set; }
        public int Year { get; set; }
    }
}
