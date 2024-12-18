using System.Text.Json;

namespace Bislerium.Data.Services
{
    public static class AddInService
    {
        // Save Add-Ins to JSON
        private static void SaveAll(List<AddInItem> addIns)
        {
            string appDataDirectoryPath = Utils.GetAppDirectoryPath();
            string addInsFilePath = Utils.GetAddInsFilePath();

            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }

            var json = JsonSerializer.Serialize(addIns);
            File.WriteAllText(addInsFilePath, json);
        }

        // Get Add-Ins from JSON
        public static List<AddInItem> GetAll()
        {
            string addInsFilePath = Utils.GetAddInsFilePath();
            if (!File.Exists(addInsFilePath))
            {
                return new List<AddInItem>();
            }

            var json = File.ReadAllText(addInsFilePath);

            return JsonSerializer.Deserialize<List<AddInItem>>(json);
        }

        // Get Add_in by Id
        public static AddInItem GetById(Guid id)
        {
            List<AddInItem> addIns = GetAll();
            return addIns.FirstOrDefault(x => x.Id == id);
        }

        // Create Add-In
        public static List<AddInItem> Create(string name, int price)
        {
            if (price < 1)
            {
                throw new Exception("Price must be at least $ 1");
            }

            List<AddInItem> AddIns = GetAll();
            AddIns.Add(new AddInItem
            {
                AddInName = name,
                Price = price
            });
            SaveAll(AddIns);
            return AddIns;
        }

        // Delete Add-In
        public static List<AddInItem> Delete(Guid id)
        {
            List<AddInItem> AddIns = GetAll();
            AddInItem AddIn = AddIns.FirstOrDefault(x => x.Id == id);

            if (AddIn == null)
            {
                throw new Exception("addIn not found.");
            }

            AddIns.Remove(AddIn);
            SaveAll(AddIns);
            return AddIns;
        }

        //Update Add-In
        public static List<AddInItem> Update(Guid id, string name, int price)
        {
            List<AddInItem> AddIns = GetAll();
            AddInItem AddInToUpdate = AddIns.FirstOrDefault(x => x.Id == id);

            if (AddInToUpdate == null)
            {
                throw new Exception("AddIn not found.");
            }

            AddInToUpdate.AddInName = name;
            AddInToUpdate.Price = price;
            SaveAll(AddIns);
            return AddIns;
        }
    }
}
