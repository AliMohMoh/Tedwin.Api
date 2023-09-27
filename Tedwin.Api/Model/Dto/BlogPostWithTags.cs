namespace Tedwin.Api.Model.Dto;

public class BlogPostWithTags
{
    public BlogPost BlogPost { get; set; }
    public List<Tag> Tags { get; set; }
}
