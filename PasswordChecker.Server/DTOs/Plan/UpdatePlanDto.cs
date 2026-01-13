namespace PasswordChecker.Server.DTOs.Plan
{
    public class UpdatePlanDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int MaxChecksPerDay { get; set; }
        public bool IsActive { get; set; }
    }
}
