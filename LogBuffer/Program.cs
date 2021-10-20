using System;

namespace LogBuffer
{
    class Program
    {
        private const string FilePath = "data.txt";
        static void Main()
        {
            using (var logBuffer = new LogBuffer(FilePath, Console.WriteLine))
            {
                Console.Write($"File path: {logBuffer.PathToFile}\nInput message: ");
                var str = Console.ReadLine();
                while (str != ".")
                {
                    logBuffer.Add(str);
                    Console.Write("Input message: ");
                    str = Console.ReadLine();
                }
            }
        }
    }
}
