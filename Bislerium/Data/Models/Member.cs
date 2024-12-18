namespace Bislerium.Data
{
    public class Member
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Phone { get; set; }
        public int TotalPurchase { get; set; } = 0;
        public int OrdersTillComplimentary { get; set; } = 10;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }

    }
}
