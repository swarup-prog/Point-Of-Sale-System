using Bislerium.Data.Enums;
using System.Text.Json;

namespace Bislerium.Data.Services
{
    public static class OrderService
    {
        // Save order to JSON
        private static void SaveAll(List<Order> orders)
        {
            string appDataDirectoryPath = Utils.GetAppDirectoryPath();
            string ordersFilePath = Utils.GetOrdersFilePath();

            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }

            var json = JsonSerializer.Serialize(orders);
            File.WriteAllText(ordersFilePath, json);
        }

        // Get all orders from JSON
        public static List<Order> GetAll()
        {
            string ordersFilePath = Utils.GetOrdersFilePath();
            if (!File.Exists(ordersFilePath))
            {
                return new List<Order>();
            }

            var json = File.ReadAllText(ordersFilePath);

            return JsonSerializer.Deserialize<List<Order>>(json);
        }

        //Create Order method 
        public static List<Order> Create(Guid coffee, Guid addIn, string phone, double total, string orderType, Guid userId)
        {
            List<Order> orders = GetAll();
            if (phone != null)
            {
                bool memberExits = MemberService.CheckExistence(phone);
                if (!memberExits)
                {
                    throw new Exception("Member does not exist!");
                } 
                else
                {
                    Member member = MemberService.GetByPhone(phone);
                    MemberService.UpdateOnOrder(member.Id);
                }
            }

            OrderType type;

            if (orderType == "complimentary")
            {
                type = OrderType.Complimentary;
                Member member = MemberService.GetByPhone(phone);
                MemberService.ResetOrderTillComplimentary(member.Id);
            } 
            else if (orderType == "discount")
            {
                type = OrderType.Regualar;
            } 
            else
            {
                type = OrderType.Normal;
            }

            orders.Add(new Order
            {
                Coffee = coffee,
                AddIn = addIn,
                MemberPhone = phone,
                OrderType = type,
                Total = total,
                CreatedBy = userId
            });
            SaveAll(orders);
            return orders;
        }

        // Delete order
        public static List<Order> Delete(Guid id)
        {
            List<Order> orders = GetAll();
            Order order = orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            orders.Remove(order);
            SaveAll(orders);
            return orders;
        }
    }
}

