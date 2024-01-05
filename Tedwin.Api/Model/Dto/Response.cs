using Microsoft.AspNetCore.Http;

namespace Tedwin.Api.Model.Dto;

public class Response<T>
{
    public int Status { get; set; }
    public string Title { get; set; }
    public Dictionary<string, object> Errors { get; set; }
    public T Data { get; set; }
    public PaginationInfo? PageInfo { get; set; }
   
}
public class PaginationInfo
{
    public int? TotalPages { get; set; }
    public int? TotalItems { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

public class SearchWithFilter
{
    public string? SearchTerm { get; set; }
    public List<string>? TagNames { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

