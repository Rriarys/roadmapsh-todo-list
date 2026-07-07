using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.DTOs;

public record RegisterRequest(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    string Name,

    [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
    string Email,

    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "Password must be between 8 and 100 characters.")]
    string Password
    );
