namespace Together.DTOs.User
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public bool? IsFemale { get; set; }
    }
}
