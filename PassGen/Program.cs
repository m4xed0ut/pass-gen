using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PassGen
{
    class Program
    {
        static string masterPassword; // Master password for encrypting and decrypting the file
        static string filePath = "Passwords.enc"; // File path for storing encrypted passwords

        static void Main(string[] args)
        {
            // Change the foreground color (text color) if needed, e.g., to white
            Console.ForegroundColor = ConsoleColor.Green;
            
            // Check if the encrypted file already exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Welcome! You need to set up a master password for the encrypted file.");
                masterPassword = ReadPassword("Enter a master password: ");
                string confirmPassword = ReadPassword("Re-enter the master password to confirm: ");

                if (masterPassword != confirmPassword)
                {
                    Console.WriteLine("Passwords do not match. Exiting...");
                    return;
                }

                Console.WriteLine("Master password set. You can now use the application.");
            }
            else
            {
                masterPassword = ReadPassword("Enter your master password to access the application: ");
            }

            ShowMenu();
        }

        static void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Password Manager");
                Console.WriteLine("1. Generate a new password");
                Console.WriteLine("2. Store an existing login and password");
                Console.WriteLine("3. Decrypt and view stored passwords");
                Console.WriteLine("4. Exit");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GenerateAndStorePassword();
                        break;
                    case "2":
                        StoreExistingLogin();
                        break;
                    case "3":
                        ViewStoredPasswords();
                        break;
                    case "4":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
        }

        static void GenerateAndStorePassword()
        {
            string password = GeneratePassword();
            Console.WriteLine($"Generated password: {password}");
            Console.WriteLine("Enter the website or app name where this password will be used:");
            string websiteOrApp = Console.ReadLine();
            Console.WriteLine("Enter the username associated with this password:");
            string username = Console.ReadLine();
            Console.WriteLine();

            SavePassword(username, password, websiteOrApp);
            Console.WriteLine("Password, username, and website/app name saved and encrypted.");
            Console.ReadLine(); // Pause to allow the user to see the confirmation
        }

        static void StoreExistingLogin()
        {
            Console.WriteLine("Enter the username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter the password:");
            string password = ReadPassword("Password: ");
            Console.WriteLine("Enter the website or app name where this login will be used:");
            string websiteOrApp = Console.ReadLine();

            SavePassword(username, password, websiteOrApp);
            Console.WriteLine("Existing login information saved and encrypted.");
            Console.ReadLine(); // Pause to allow the user to see the confirmation
        }

        static void ViewStoredPasswords()
        {
            string decryptedContent = DecryptPasswords();
            Console.WriteLine("Decrypted passwords:");
            Console.WriteLine(decryptedContent);
            Console.ReadLine(); // Pause to allow the user to read the output
        }

        static string GeneratePassword()
        {
            Random rand = new Random();
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_+".ToCharArray();
            char[] password = new char[16];

            for (int i = 0; i < 16; i++)
            {
                password[i] = chars[rand.Next(chars.Length)];
            }

            return new string(password);
        }

        static void SavePassword(string username, string password, string websiteOrApp)
        {
            string dataToEncrypt = $"Website/App: {websiteOrApp} - Username: {username} - Password: {password}";
            string encryptedData = Encrypt(dataToEncrypt, masterPassword);

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(encryptedData);
            }
        }

        static string Encrypt(string plainText, string key)
        {
            byte[] iv = new byte[16]; // AES block size is 128 bits, or 16 bytes
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = GenerateKeyFromPassword(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        array = ms.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        static string DecryptPasswords()
        {
            if (!File.Exists(filePath))
            {
                return "No encrypted password file found.";
            }

            string encryptedContent = File.ReadAllText(filePath);
            string[] encryptedEntries = encryptedContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            StringBuilder decryptedContent = new StringBuilder();

            foreach (string encryptedEntry in encryptedEntries)
            {
                if (!string.IsNullOrEmpty(encryptedEntry))
                {
                    decryptedContent.AppendLine(Decrypt(encryptedEntry, masterPassword));
                }
            }

            return decryptedContent.ToString();
        }

        static string Decrypt(string cipherText, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = GenerateKeyFromPassword(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        static byte[] GenerateKeyFromPassword(string password)
        {
            // Use a SHA-256 hash of the password to create a 256-bit key
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedKey = sha256.ComputeHash(keyBytes);

                // Use the first 16 bytes (128 bits) for AES encryption
                byte[] aesKey = new byte[16];
                Array.Copy(hashedKey, aesKey, aesKey.Length);
                return aesKey;
            }
        }

        static string ReadPassword(string prompt)
        {
            Console.Write(prompt);
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                // Handle backspace
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b"); // Erase the last character from console
                    }
                }
                // Handle enter
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                // Handle other keys
                else if (key.KeyChar >= 32 && key.KeyChar <= 126) // Printable ASCII characters
                {
                    password.Append(key.KeyChar);
                    Console.Write("*"); // Print asterisks to mask the password
                }

            } while (true);

            return password.ToString();
        }
    }
}
