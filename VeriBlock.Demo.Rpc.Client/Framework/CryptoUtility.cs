// VeriBlock NodeCore
// Copyright 2017-2020 Xenios SEZC.
// All rights reserved.
// https://www.veriblock.org
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.

using System.Security.Cryptography;
using System.Text;

namespace VeriBlock.Demo.Rpc.Client.Framework
{
    public class CryptoUtility
    {
        public static string ComputeSHA256HashAsBase58(string input)
        {
            var b = UTF8Encoding.UTF8.GetBytes(input);

            var algorithm = SHA256.Create();

            var hash = algorithm.ComputeHash(b);

            return Base58.Encode(hash);
        }
    }
}
