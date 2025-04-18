namespace BackendProject2.Dto
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
        public string? UserEmail { get; set; }
        public string? Role { get; set; }
        public string? Error { get; set; }
    }
}
