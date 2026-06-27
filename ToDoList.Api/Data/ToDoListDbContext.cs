using Microsoft.EntityFrameworkCore;

namespace ToDoList.Api.Data;

public class ToDoListDbContext : DbContext
{
    public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options) : base(options)
    {
    }

}