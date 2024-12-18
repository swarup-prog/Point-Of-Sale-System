using System.Text.Json;

namespace Bislerium.Data.Services
{
    public static class CoffeeService
    {
        // Save Coffee to JSON
        private static void SaveAll(List<CoffeeItem> coffees)
        {
            string appDataDirectoryPath = Utils.GetAppDirectoryPath();
            string coffeesFilePath = Utils.GetCoffeesFilePath();

            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }

            var json = JsonSerializer.Serialize(coffees);
            File.WriteAllText(coffeesFilePath, json);
        }

        // Get Coffee Items
        public static List<CoffeeItem> GetAll()
        {
            string coffeesFilePath = Utils.GetCoffeesFilePath();
            if (!File.Exists(coffeesFilePath))
            {
                return new List<CoffeeItem>();
            }

            var json = File.ReadAllText(coffeesFilePath);

            return JsonSerializer.Deserialize<List<CoffeeItem>>(json);
        }

        // Get Coffee Items by ID
        public static CoffeeItem GetById(Guid id)
        {
            List<CoffeeItem> coffees = GetAll();
            return coffees.FirstOrDefault(x => x.Id == id);
        }

        // Coffee Items by ID
        public static List<CoffeeItem> Create(string type, int price)
        {
            if (price < 1)
            {
                throw new Exception("Price must be at least $ 1");
            }

            List<CoffeeItem> coffees = GetAll();
            coffees.Add(new CoffeeItem
            {
                CoffeeType = type,
                Price = price
            });
            SaveAll(coffees);
            return coffees;
        }

        // Delete Coffee
        public static List<CoffeeItem> Delete(Guid id)
        {
            List<CoffeeItem> coffees = GetAll();
            CoffeeItem coffee = coffees.FirstOrDefault(x => x.Id == id);

            if (coffee == null)
            {
                throw new Exception("Coffee not found.");
            }

            coffees.Remove(coffee);
            SaveAll(coffees);
            return coffees;
        }

        // Update Coffee
        public static List<CoffeeItem> Update(Guid id, string type, int price)
        {
            List<CoffeeItem> coffees = GetAll();
            try
            {
                CoffeeItem coffeeToUpdate = coffees.FirstOrDefault(x => x.Id == id);
                coffeeToUpdate.CoffeeType = type;
                coffeeToUpdate.Price = price;
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("Coffee not found.");
            }

            SaveAll(coffees);
            return coffees;
        }
    }
}
