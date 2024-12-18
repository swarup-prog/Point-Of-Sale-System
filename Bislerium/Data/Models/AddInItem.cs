namespace Bislerium.Data
{
     public class AddInItem
     {
           public Guid Id { get; set; } = Guid.NewGuid();
           public string AddInName { get; set; }
           public int Price { get; set; }
           public DateTime CreatedAt { get; set; } = DateTime.Now;
     }
}