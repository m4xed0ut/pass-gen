using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PassGen
{
    class Program
    {
        // Ensure the key is 16, 24, or 32 characters long
        static string encryptionKey = "YourSecretKey123"; // 16 characters long for AES-128
        static string lastGeneratedPassword = string.Empty; // Store the last generated password
        static string lastWebsiteOrApp = string.Empty; // Store the last website/app name
        static string lastUsername = string.Empty; // Store the last username

        static void Main(string[] args)
        {
            string filePath = "Passwords.enc";
            Console.WriteLine("Press Enter to generate a 16-char password made of random characters \n(upper and lower case letters, digits and various symbols).");
            Console.WriteLine("Press S to store the password, username, and website/app name in an encrypted file (Passwords.enc) and close the program.");
            Console.WriteLine("Press D to decrypt and view the passwords from the file.");
            Console.CursorVisible = false;

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    lastGeneratedPassword = GeneratePassword();
                    Console.WriteLine($"Generated password: {lastGeneratedPassword}");
                    Console.WriteLine("Enter the website or app name where this password will be used:");
                    lastWebsiteOrApp = Console.ReadLine();
                    Console.WriteLine("Enter the username associated with this password:");
                    lastUsername = Console.ReadLine();
                    Console.WriteLine();
                }
                else if (key.Key == ConsoleKey.S)
                {
                    if (!string.IsNullOrEmpty(lastGeneratedPassword) && !string.IsNullOrEmpty(lastWebsiteOrApp) && !string.IsNullOrEmpty(lastUsername))
                    {
                        SavePassword(lastGeneratedPassword, lastWebsiteOrApp, lastUsername, filePath);
                        Console.WriteLine("Password, username, and website/app name saved and encrypted.");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("No password, username, or website/app name to save.");
                    }
                }
                else if (key.Key == ConsoleKey.D)
                {
                    string decryptedContent = DecryptPasswords(filePath);
                    Console.WriteLine("Decrypted passwords:");
                    Console.WriteLine(decryptedContent);
                }
            }
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

        static void SavePassword(string password, string websiteOrApp, string username, string filePath)
        {
            string dataToEncrypt = $"Website/App: {websiteOrApp} - Username: {username} - Password: {password}";
            string encryptedData = Encrypt(dataToEncrypt, encryptionKey);

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
                aes.Key = Encoding.UTF8.GetBytes(key);
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

        static string DecryptPasswords(string filePath)
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
                    decryptedContent.AppendLine(Decrypt(encryptedEntry, encryptionKey));
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
                aes.Key = Encoding.UTF8.GetBytes(key);
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
    }
}

