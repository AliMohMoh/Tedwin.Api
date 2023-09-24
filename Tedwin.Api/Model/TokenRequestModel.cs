using System.ComponentModel.DataAnnotations;

namespace Tedwin.Api.Model;

public class TokenRequestModel
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
