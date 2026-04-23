namespace BuildingBlocks.Events;

public class UserRegisteredEvent
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string EmailConfirmationLink  { get; set; }
}