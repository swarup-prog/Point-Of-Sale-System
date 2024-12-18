using System.Text.Json;

namespace Bislerium.Data.Services
{
    public static class MemberService
    {
        // Save member to JSON
        private static void SaveAll(List<Member> members)
        {
            string appDataDirectoryPath = Utils.GetAppDirectoryPath();
            string membersFilePath = Utils.GetMembersFilePath();

            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }

            var json = JsonSerializer.Serialize(members);
            File.WriteAllText(membersFilePath, json);
        }

        // Get all members from JSON
        public static List<Member> GetAll()
        {
            string membersFilePath = Utils.GetMembersFilePath();
            if (!File.Exists(membersFilePath))
            {
                return new List<Member>();
            }

            var json = File.ReadAllText(membersFilePath);

            return JsonSerializer.Deserialize<List<Member>>(json);
        }

        // Update total purchases and ordersTillComplimentary  after member order
        public static List<Member> UpdateOnOrder(Guid id) {
            List<Member> members = GetAll();

            try
            {
                Member memberToUpdate = members.FirstOrDefault(x => x.Id == id);
                memberToUpdate.OrdersTillComplimentary = memberToUpdate.OrdersTillComplimentary - 1;
                memberToUpdate.TotalPurchase = memberToUpdate.TotalPurchase + 1;
            }
            catch (ArgumentNullException)
            {
                throw new Exception("Member not found.");
            }
            SaveAll(members);
            return members;
        }
        
        // Reset order till complimentary value after the complimentary is redeemed
        public static List<Member> ResetOrderTillComplimentary(Guid id) {
            List<Member> members = GetAll();

            try
            {
                Member memberToReset = members.FirstOrDefault(x => x.Id == id);
                memberToReset.OrdersTillComplimentary = 10;
            }
            catch (ArgumentNullException)
            {
                throw new Exception("Member not found.");
            }
            SaveAll(members);
            return members;
        }

        
        // Get member by ID
        public static Member GetById(Guid id)
        {
            List<Member> members = GetAll();
            return members.FirstOrDefault(x => x.Id == id);
        }
        public static Member GetByPhone(string phone) 
        {
            List<Member> members = GetAll();
            if (!CheckExistence(phone))
            {
                throw new Exception("Member not found.");
            }
            
            return members.FirstOrDefault(x => x.Phone == phone);
            
        }

        // Create member 
        public static List<Member> Create(string name, string phone, Guid userId)
        {

            List<Member> members = GetAll();
            
            if (CheckExistence(phone))
            {
                throw new Exception("Member already exists.");
            }

            members.Add(
                new Member
                {
                    Name = name,
                    Phone = phone,
                    CreatedBy = userId
                }
            );
            SaveAll(members);
            return members;
        }

        // Update member
        public static List<Member> Update(Guid id, string name, string phone)
        {
            List<Member> members = GetAll();


            try
            {
                Member memberToUpdate = members.FirstOrDefault(x => x.Id == id);
                memberToUpdate.Name = name;
                memberToUpdate.Phone = phone;
            }
            catch (ArgumentNullException)
            {
                throw new Exception("Member not found.");
            }
            SaveAll(members);
            return members;
        }

        // Check if member exists
        public static bool CheckExistence (string phoneNumber) {
            List<Member> members = GetAll();
            Member member = members.FirstOrDefault(x => x.Phone == phoneNumber);

            if (member == null)
            {
                return false;
            }
            return true;
        }

        // Delete member
        public static List<Member> Delete(Guid id)
        {
            List<Member> members = GetAll();
            Member member = members.FirstOrDefault(x => x.Id == id);

            if (member == null)
            {
                throw new Exception("Member not found.");
            }

            //Order.DeleteByMemberId(id);
            members.Remove(member);
            SaveAll(members);

            return members;
        }
    }
}



