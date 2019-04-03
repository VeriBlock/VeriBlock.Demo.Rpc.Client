// VeriBlock NodeCore
// Copyright 2017-2019 VeriBlock, Inc.
// All rights reserved.
// https://www.veriblock.org
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.

using Google.Protobuf;
using System;
using System.Linq;

namespace VeriBlock.Demo.Rpc.Client.Framework
{
    public static class Base58
    {
        public static readonly char[] ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();
        private static readonly char ENCODED_ZERO = ALPHABET[0];
        private static readonly int[] INDEXES = new int[128];

        static Base58()
        {
            INDEXES = Enumerable.Repeat(-1, 128).ToArray();
            for (int i = 0; i < ALPHABET.Length; i++)
            {
                INDEXES[ALPHABET[i]] = i;
            }
        }

        public static string ToBase58(this ByteString byteString)
        {
            return ToBase58(byteString.ToByteArray());
        }

        public static string ToBase58(this byte[] bytes)
        {
            return Encode(bytes);
        }

        public static bool IsBase58String(String toTest)
        {
            return toTest.All(c => ALPHABET.Contains(c));
        }

        public static String Encode(byte[] input)
        {
            if (input.Length == 0)
            {
                return "";
            }
            // Count leading zeros.
            int zeros = 0;
            while (zeros < input.Length && input[zeros] == 0)
            {
                ++zeros;
            }

            // Convert base-256 digits to base-58 digits (plus conversion to ASCII characters)
            byte[] localInput = new byte[input.Length];
            input.CopyTo(localInput, 0);

            char[] encoded = new char[localInput.Length * 2]; // upper bound
            int outputStart = encoded.Length;
            for (int inputStart = zeros; inputStart < localInput.Length;)
            {
                encoded[--outputStart] = ALPHABET[DivMod(localInput, inputStart, 256, 58)];
                if (localInput[inputStart] == 0)
                {
                    ++inputStart; // optimization - skip leading zeros
                }
            }
            // Preserve exactly as many leading encoded zeros in output as there were leading zeros in input.
            while (outputStart < encoded.Length && encoded[outputStart] == ENCODED_ZERO)
            {
                ++outputStart;
            }
            while (--zeros >= 0)
            {
                encoded[--outputStart] = ENCODED_ZERO;
            }
            // Return encoded string (including encoded leading zeros).
            return new String(encoded, outputStart, encoded.Length - outputStart);
        }

        public static byte[] Decode(string input)
        {
            if (input.Length == 0)
            {
                return new byte[0];
            }

            // Convert the base58-encoded ASCII chars to a base58 byte sequence (base58 digits).
            byte[] input58 = new byte[input.Length];
            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];
                int digit = c < 128 ? INDEXES[c] : -1;
                if (digit < 0)
                {
                    throw new Exception("Illegal character " + c + " at position " + i);
                }
                input58[i] = (byte)digit;
            }
            // Count leading zeros.
            int zeros = 0;
            while (zeros < input58.Length && input58[zeros] == 0)
            {
                ++zeros;
            }
            // Convert base-58 digits to base-256 digits.
            byte[] decoded = new byte[input.Length];
            int outputStart = decoded.Length;
            for (int inputStart = zeros; inputStart < input58.Length;)
            {
                decoded[--outputStart] = DivMod(input58, inputStart, 58, 256);
                if (input58[inputStart] == 0)
                {
                    ++inputStart; // optimization - skip leading zeros
                }
            }
            // Ignore extra leading zeroes that were added during the calculation.
            while (outputStart < decoded.Length && decoded[outputStart] == 0)
            {
                ++outputStart;
            }

            // Return decoded data (including original number of leading zeros).
            return decoded.Skip(outputStart - zeros).ToArray();
        }

        private static byte DivMod(byte[] number, int firstDigit, int b, int divisor)
        {
            // this is just long division which accounts for the base of the input digits
            int remainder = 0;
            for (int i = firstDigit; i < number.Length; i++)
            {
                int digit = (int)number[i] & 0xFF;
                int temp = remainder * b + digit;
                number[i] = (byte)(temp / divisor);
                remainder = temp % divisor;
            }
            return (byte)remainder;
        }
    }
}
