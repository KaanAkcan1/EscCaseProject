using EscCase.Common.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace EscCase.Business.Models.Requests.AppUser
{
    public class LoginRequest
    {
        [Required(ErrorMessage = RepositoryDefaults.ValidationMessages.Required)]
        public string Username { get; set; }

        [Required(ErrorMessage = RepositoryDefaults.ValidationMessages.Required)]
        public string Password { get; set; }
    }
}
