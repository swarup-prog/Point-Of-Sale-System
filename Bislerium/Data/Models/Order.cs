using Bislerium.Data.Enums;

namespace Bislerium.Data
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid Coffee { get; set; }
        public Guid AddIn { get; set; }
        public string MemberPhone { get; set; }
        public double Total { get; set; }
        public OrderType OrderType { get; set; } = OrderType.Normal;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }

    }
}
