using System;
using System.Collections.Generic;
using System.Text;

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
