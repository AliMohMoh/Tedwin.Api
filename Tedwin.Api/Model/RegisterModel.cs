using System.ComponentModel.DataAnnotations;

namespace Tedwin.Api.Model;

public class RegisterModel
{
    [Required,StringLength(250)]
    public string UserName { get; set; }
    [Required, StringLength(250)]
    public string Email { get; set; }
    [Required, StringLength(250)]
    public string Password { get; set; }
}
