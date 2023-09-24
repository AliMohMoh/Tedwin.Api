using Microsoft.EntityFrameworkCore;
using Tedwin.Api.Data;
using Tedwin.Api.Model;

namespace Tedwin.Api.Services.TagsInfo;

public class TagService : ITagService
{
    private readonly ApplicationDbContext _dbContext;

    public TagService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _dbContext.Tags.ToListAsync();
    }

    public async Task<Tag> GetTagByIdAsync(int id)
    {
        return await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task CreateTagAsync(Tag tag)
    {
        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTagAsync(Tag tag)
    {
        _dbContext.Tags.Update(tag);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(Tag tag)
    {
        _dbContext.Tags.Remove(tag);
        await _dbContext.SaveChangesAsync();
    }
}
