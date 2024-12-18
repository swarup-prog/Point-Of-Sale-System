namespace Bislerium.Data
{
    public class CoffeeItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CoffeeType { get; set; }
        public int Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
