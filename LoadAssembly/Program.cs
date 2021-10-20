using System;
using System.Linq;

namespace LoadAssembly
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var assemblyLoader = new AssemblyLoader(args[0]);

                var classInfo = assemblyLoader.GetPublicTypes();
                classInfo = classInfo
                    .OrderBy(x => x.Namespace)
                    .ToArray();

                foreach (var info in classInfo)
                {
                    Console.WriteLine(info.ToString(true));
                }
            }

            Console.ReadLine();
        }
    }
}
