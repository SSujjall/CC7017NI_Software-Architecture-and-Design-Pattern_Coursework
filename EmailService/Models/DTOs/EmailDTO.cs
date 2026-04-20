using System.ComponentModel.DataAnnotations;

namespace EmailService.Models.DTOs;

public class EmailRequestDTO
{
    [EmailAddress]
    public string To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}