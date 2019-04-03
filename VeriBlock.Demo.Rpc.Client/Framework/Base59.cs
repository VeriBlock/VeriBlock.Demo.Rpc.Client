// VeriBlock NodeCore
// Copyright 2017-2019 VeriBlock, Inc.
// All rights reserved.
// https://www.veriblock.org
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Linq;
using System.Text;

namespace VeriBlock.Demo.Rpc.Client.Framework
{
    public class Base59
    {
        public static readonly char[] ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0".ToCharArray();
        private static readonly int[] INDEXES = new int[128];
        private static readonly int BASE_59 = ALPHABET.Length;
        private const int BASE_256 = 256;

        static Base59()
        {
            INDEXES = Enumerable.Repeat(-1, 128).ToArray();
            for (int i = 0; i < ALPHABET.Length; i++)
            {
                INDEXES[ALPHABET[i]] = i;
            }
        }

        public static bool IsBase58String(String toTest)
        {
            return toTest.All(c => ALPHABET.Contains(c));
        }

        public static String Encode(byte[] input)
        {
            if (input.Length == 0)
            {
                // paying with the same coin
                return "";
            }

            //
            // Make a copy of the input since we are going to modify it.
            //
            byte[] localInput = new byte[input.Length];
            input.CopyTo(localInput, 0);

            //
            // Count leading zeroes
            //
            int zeroCount = 0;
            while (zeroCount < localInput.Length && localInput[zeroCount] == 0)
            {
                ++zeroCount;
            }

            //
            // The actual encoding
            //
            byte[] temp = new byte[localInput.Length * 2];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < localInput.Length)
            {
                byte mod = DivMod59(localInput, startAt);
                if (localInput[startAt] == 0)
                {
                    ++startAt;
                }

                temp[--j] = (byte)ALPHABET[mod];
            }

            //
            // Strip extra '1' if any
            //
            while (j < temp.Length && temp[j] == ALPHABET[0])
            {
                ++j;
            }

            //
            // Add as many leading '1' as there were leading zeros.
            //
            while (--zeroCount >= 0)
            {
                temp[--j] = (byte)ALPHABET[0];
            }

            return Encoding.UTF8.GetString(temp.Skip(j).ToArray());
        }

        private static byte DivMod59(byte[] number, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number.Length; i++)
            {
                int digit256 = (int)number[i] & 0xFF;
                int temp = remainder * BASE_256 + digit256;

                number[i] = (byte)(temp / BASE_59);

                remainder = temp % BASE_59;
            }

            return (byte)remainder;
        }
    }
}
