using System.ComponentModel.DataAnnotations;

namespace TaskCoreCategoryProductsListPage.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm password do not match")]
        public string? ConfirmPassword { get; set; }
        public string? Role { get; set; }
    }
}
