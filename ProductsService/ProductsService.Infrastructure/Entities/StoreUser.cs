using Microsoft.AspNetCore.Identity;

namespace ProductsService.Infrastructure.Entities
{
    public class StoreUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
