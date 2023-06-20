namespace Identity.Abstractions;

public interface IPaginationFilter
{
    public int Page { get; init; }
    public int PageSize { get; init; }
}