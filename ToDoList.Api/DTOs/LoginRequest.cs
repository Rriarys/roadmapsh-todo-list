using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.DTOs;

public record LoginRequest(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    string Email,

    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "Password must be between 8 and 100 characters.")]
    string Password
    );
