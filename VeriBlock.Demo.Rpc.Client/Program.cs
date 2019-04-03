// VeriBlock NodeCore
// Copyright 2017-2019 VeriBlock, Inc.
// All rights reserved.
// https://www.veriblock.org
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.

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
