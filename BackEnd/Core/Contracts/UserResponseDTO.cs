namespace Contracts
{
    public class UserResponseDTO
    {
        public UserResponseDTO()
        {
            Roles = new List<string>();
        }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ContactNo { get; set; }
        public int Status { get; set; }
        public string PrimaryRole { get; set; }
        public List<string> Roles { get; set; }
    }
}
