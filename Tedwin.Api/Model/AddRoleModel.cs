using System.ComponentModel.DataAnnotations;

namespace Tedwin.Api.Model;

public class AddRoleModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Role { get; set; }
}
