using Microsoft.AspNetCore.Identity;

namespace Arak.DAL.Entities
{
    public class ApplicationUser  : IdentityUser
    {
		public string? Name { get; set; }
		public string? Address { get; set; }
	}
}
