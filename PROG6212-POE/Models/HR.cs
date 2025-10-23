namespace PROG6212_POE.Models
{
    public class HR
    {
        public int HRId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = "Human Resources";
    }
}