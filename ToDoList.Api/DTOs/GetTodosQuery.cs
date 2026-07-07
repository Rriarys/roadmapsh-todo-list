using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.DTOs;

public record GetTodosQuery
{
    private const int MaximumPageSize = 50; // hardcoded maximum page size to prevent excessive data retrieval

    private int _page = 1;
    private int _pageSize = 10;

    [Range(1, int.MaxValue)]
    public int Page
    {
        get => _page;
        init => _page = value < 1 ? 1 : value; // fallback to 1 if the provided value is less than 1
    }

    [Range(1, MaximumPageSize)]
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaximumPageSize ? MaximumPageSize : value < 1 ? 10 : value; // fallback to 10 if the provided value is less than 1
    }
}
