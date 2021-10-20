using System;

namespace LogBuffer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var logBuffer = new LogBuffer("data.txt"))
            {
                var str = Console.ReadLine();
                while (str != ".")
                {
                    logBuffer.Add(str);
                    str = Console.ReadLine();
                }
            }
        }
    }
}
