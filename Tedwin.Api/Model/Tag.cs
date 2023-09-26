using System.ComponentModel.DataAnnotations;

namespace Tedwin.Api.Model;

public class Tag
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }

}