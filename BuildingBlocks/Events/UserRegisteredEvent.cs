namespace BuildingBlocks.Events;

public class UserRegisteredEvent
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string EmailConfirmationLink  { get; set; }
}