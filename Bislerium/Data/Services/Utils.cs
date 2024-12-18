using System.Security.Cryptography;

namespace Bislerium.Data.Services
{
    public static class Utils
    {
        private const char _segmentDelimiter = ':';

        // Generate Hash Password
        public static string HashSecret(string input)
        {
            var saltSize = 16;
            var iterations = 100_000;
            var keySize = 32;
            HashAlgorithmName algorithm = HashAlgorithmName.SHA256;
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

            return string.Join(
                _segmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                iterations,
                algorithm
            );
        }
        // Compare password and hashed password
        public static bool VerifyHash(string input, string hashString)
        {
            string[] segments = hashString.Split(_segmentDelimiter);
            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            HashAlgorithmName algorithm = new(segments[3]);
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                iterations,
                algorithm,
                hash.Length
            );

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }

        // Get App Directory
        public static string GetAppDirectoryPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Bislerium"
            );
        }

        // Get File Paths
        public static string GetAppUsersFilePath()
        {
            return Path.Combine(GetAppDirectoryPath(), "users.json");
        }

        public static string GetCoffeesFilePath()
        {
            return Path.Combine(GetAppDirectoryPath(), "coffees.json");
        }

        public static string GetAddInsFilePath()
        {
            return Path.Combine(GetAppDirectoryPath(), "addins.json");
        }
        public static string GetMembersFilePath()
        {
            return Path.Combine(GetAppDirectoryPath(), "members.json");
        }

        public static string GetOrdersFilePath()
        {
            return Path.Combine(GetAppDirectoryPath(), "orders.json");
        }
        
        public static string GetReportsFilePath()
        {
            string reportsPath = Path.Combine(GetAppDirectoryPath(), "Reports");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(reportsPath))
            {
                Directory.CreateDirectory(reportsPath);
            }

            return reportsPath + "\\";
        }
    }
}
