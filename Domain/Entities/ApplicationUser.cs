using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? CocheraId { get; set; }
        public Cochera Cochera { get; set; }
    }
}
