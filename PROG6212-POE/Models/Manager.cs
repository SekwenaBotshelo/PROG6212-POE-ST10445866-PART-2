namespace PROG6212_POE.Models
{
    public class Manager
    {
        public int ManagerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Academic Manager";
    }
}