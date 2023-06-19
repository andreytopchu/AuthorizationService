namespace Identity.Models.Requests;

public class PaginationFilterRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}