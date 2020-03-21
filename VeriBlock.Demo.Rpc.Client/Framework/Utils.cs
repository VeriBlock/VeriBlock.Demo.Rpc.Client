// VeriBlock NodeCore
// Copyright 2017-2020 Xenios SEZC.
// All rights reserved.
// https://www.veriblock.org
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace VeriBlock.Demo.Rpc.Client.Framework
{
    public class Utils
    {
        public static Google.Protobuf.ByteString ConvertAddressToByteString(String address)
        {
            Google.Protobuf.ByteString addressBS = Google.Protobuf.ByteString.CopyFrom(Base58.Decode(address));
            return addressBS;
        }

        /// <summary>
        /// VBK internally stored as long. Need to convert to 8 decimal points
        /// </summary>
        public static double ConvertAtomicToVbkUnits(long input)
        {
            return input / 100000000.0;
        }

        public static long ConvertVbkToAtomicUnits(double input)
        {
            return Convert.ToInt64(input * 100000000);
        }
    }
}
