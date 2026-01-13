namespace PasswordChecker.Server.DTOs.Plan
{
    public class CreatePlanDto
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int MaxChecksPerDay { get; set; }
    }
}
