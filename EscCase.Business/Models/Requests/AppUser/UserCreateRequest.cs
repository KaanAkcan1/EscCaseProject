using EscCase.Common.Entities.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace EscCase.Business.Models.Requests.AppUser
{
    public class UserCreateRequest
    {
        [SwaggerSchema("The full name of the user.")]
        public string? Name { get; set; }

        [SwaggerSchema("The user's email address.")]
        public string? Email { get; set; }

        [SwaggerSchema("The status identifier. Default is the active status. Status Types: Deleted = -1,Undefined = 0,Active = 1,Passive = 2")]
        public int? StatusId { get; set; } = RepositoryDefaults.DefaultEntityStatus;

        [SwaggerSchema("The user's phone number.")]
        public string? PhoneNumber { get; set; }
    }
}
