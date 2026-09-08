using SlientMoon.Domain.Common;

namespace SlientMoon.Domain.Entities
{
    public class Admin : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
