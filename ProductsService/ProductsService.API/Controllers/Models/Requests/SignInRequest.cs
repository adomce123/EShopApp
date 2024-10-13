using System.ComponentModel.DataAnnotations;

namespace ProductsService.Controllers.Models.Requests
{
    public class SignInRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
