namespace Tedwin.Api.Model.Dto;

public class Response<T>
{
    public int Status { get; set; }
    public string Title { get; set; }
    public Dictionary<string, object> Errors { get; set; }
    public T Data { get; set; }
}
