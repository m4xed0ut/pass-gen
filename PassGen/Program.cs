using System;
using System.IO;

namespace PassGen
{
    class Program
    {

        static void Main(string[] args)
        {

            FileStream pass = new FileStream("Pass.txt", FileMode.Create);

            Console.WriteLine("Pressing Enter generates a 16-char password made of random characters \n(upper and lower case letters, digits and various symbols)." );
            Console.WriteLine();
            Console.WriteLine("Pressing S stores the password in a text file (Pass.txt) and closes the program.");


            StreamWriter portal = new StreamWriter(pass);
            Console.SetOut(portal);

            Console.CursorVisible = false;


            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                Random rand = new Random();

                char[] chars = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            ,'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            , '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
            ,'!', '@', '#', '$', '%', '^', '&', '*', '(',')','_','-','+' };

                int index1 = rand.Next(chars.Length);
                int index2 = rand.Next(chars.Length);
                int index3 = rand.Next(chars.Length);
                int index4 = rand.Next(chars.Length);
                int index5 = rand.Next(chars.Length);
                int index6 = rand.Next(chars.Length);
                int index7 = rand.Next(chars.Length);
                int index8 = rand.Next(chars.Length);
                int index9 = rand.Next(chars.Length);
                int index10 = rand.Next(chars.Length);
                int index11 = rand.Next(chars.Length);
                int index12 = rand.Next(chars.Length);
                int index13 = rand.Next(chars.Length);
                int index14 = rand.Next(chars.Length);
                int index15 = rand.Next(chars.Length);
                int index16 = rand.Next(chars.Length);



                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(
                        $"{chars[index1]}" +
                        $"{chars[index2]}" +
                        $"{chars[index3]}" +
                        $"{chars[index4]}" +
                        $"{chars[index5]}" +
                        $"{chars[index6]}" +
                        $"{chars[index7]}" +
                        $"{chars[index8]}" +
                        $"{chars[index9]}" +
                        $"{chars[index10]}" +
                        $"{chars[index11]}" +
                        $"{chars[index12]}" +
                        $"{chars[index13]}" +
                        $"{chars[index14]}" +
                        $"{chars[index15]}" +
                        $"{chars[index16]}");
                    Console.WriteLine();
                }

                if (key.Key == ConsoleKey.S)
                {
                    portal.Close();
                    Environment.Exit(0);
                }
            }
        }
    }
}
