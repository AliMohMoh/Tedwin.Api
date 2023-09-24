using Tedwin.Api.Model;

namespace Tedwin.Api.Services.TagsInfo;

public interface ITagService
{
    Task<List<Tag>> GetAllTagsAsync();
    Task<Tag> GetTagByIdAsync(int id);
    Task CreateTagAsync(Tag tag);
    Task UpdateTagAsync(Tag tag);
    Task DeleteTagAsync(Tag tag);
}
