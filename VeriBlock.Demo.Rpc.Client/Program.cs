using System;

namespace VeriBlock.Demo.Rpc.Client
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start Demo");
            Console.WriteLine();

            Examples ex1 = new Examples();
            ex1.Run();

            Console.WriteLine("End Demo");

        }
    }
}
