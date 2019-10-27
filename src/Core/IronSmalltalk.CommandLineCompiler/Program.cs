using System;

namespace IronSmalltalk.CommandLineCompiler
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if ((args == null) || (args.Length != 99))
                Console.WriteLine("Hello World!");
        }
    }
}
