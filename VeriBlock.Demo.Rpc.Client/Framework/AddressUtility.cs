using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeriBlock.Demo.Rpc.Client.Framework
{
    public class AddressUtility
    {
        private const char STARTING_CHAR = 'V';
        private const char ENDING_CHAR_MULTISIG = '0';

        private const int ADDRESS_LENGTH = 30;
        private const int MULTISIG_ADDRESS_LENGTH = 30;

        private const int ADDRESS_DATA_START = 0;
        private const int ADDRESS_DATA_END = 24;
        private const int MULTISIG_ADDRESS_DATA_START = 0;
        private const int MULTISIG_ADDRESS_DATA_END = 24;

        private const int ADDRESS_CHECKSUM_START = 25;
        private const int ADDRESS_CHECKSUM_END = 29;
        private const int ADDRESS_CHECKSUM_LENGTH = ADDRESS_CHECKSUM_END - ADDRESS_CHECKSUM_START;
        private const int MULTISIG_ADDRESS_CHECKSUM_START = 25;
        private const int MULTISIG_ADDRESS_CHECKSUM_END = 28;
        private const int MULTISIG_ADDRESS_CHECKSUM_LENGTH = MULTISIG_ADDRESS_CHECKSUM_END - MULTISIG_ADDRESS_CHECKSUM_START;

        private const int MULTISIG_ADDRESS_M_VALUE = 1;
        private const int MULTISIG_ADDRESS_N_VALUE = 2;

        private const int MULTISIG_ADDRESS_MIN_M_VALUE = 1;
        private const int MULTISIG_ADDRESS_MIN_N_VALUE = 2;
        private const int MULTISIG_ADDRESS_MAX_M_VALUE = 58;
        private const int MULTISIG_ADDRESS_MAX_N_VALUE = 58;

        private const int MULTISIG_ADDRESS_SIGNING_GROUP_START = 3;
        private const int MULTISIG_ADDRESS_SIGNING_GROUP_END = 24;
        private const int MULTISIG_ADDRESS_SIGNING_GROUP_LENGTH = MULTISIG_ADDRESS_SIGNING_GROUP_END - MULTISIG_ADDRESS_SIGNING_GROUP_START;

        private const int MULTISIG_ADDRESS_IDENTIFIER_INDEX = 30;

        /**
         * Attempts to automatically parse the appropriate (standard or multisig) address type automatically from a protobuf
         * ByteString. If the provided ByteString is not a valid standard or multisig address, then this will throw an exception.
         *
         * @param toParse ByteString to parse into a Standard or Multisig address
         * @return A valid Standard or Multisig address parsed from the provided ByteString
         */
        public static string ParseProperAddressTypeAutomatically(ByteString toParse)
        {
            if (toParse == null)
            {
                throw new ArgumentNullException("toParse");
            }

            string address = null;
            if (TryParseBase58Address(toParse, out address))
            {
                return address;
            }
            else if (TryParseBase59Address(toParse, out address))
            {
                return address;
            }
            else
            {
                throw new ArgumentException("ParseProperAddressTypeAutomatically cannot be called with an invalid ByteString!");
            }
        }

        /**
         * Determines whether the provided protobuf ByteString represents a valid standard address.
         *
         * @param toTest ByteString which may or may not represent a valid standard address
         * @return Whether the provided ByteString represents a valid standard address
         */
        public static bool TryParseBase58Address(ByteString toTest, out string result)
        {
            if (toTest == null)
            {
                throw new ArgumentNullException("toTest");
            }

            string potentialAddress = Base58.Encode(toTest.ToByteArray());

            var testResult = IsValidAddress(potentialAddress);
            if (testResult)
                result = potentialAddress;
            else
                result = null;

            return testResult;
        }

        /**
         * Determines whether the provided protobuf ByteString represents a valid multisig address.
         *
         * @param toTest ByteString which may or may not represent a valid multisig address
         * @return Whether the provided ByteString represents a valid multisig address
         */
        public static bool TryParseBase59Address(ByteString toTest, out string result)
        {
            if (toTest == null)
            {
                throw new ArgumentNullException("toTest");
            }

            string potentialMultisigAddress = Base59.Encode(toTest.ToByteArray());

            var testResult = IsValidMultisigAddress(potentialMultisigAddress);
            if (testResult)
                result = potentialMultisigAddress;
            else
                result = null;

            return testResult;
        }

        /**
         * Check whether the provided String is a plausible standard address, meaning it is:
         * --> 30 characters long
         * --> Encoded in Base58
         * --> Has a valid checksum
         * --> Starts with the correct starting character
         * <p>
         * There is no way to determine whether an address has a corresponding public/private keypair based on
         * the address alone, it is possible that an address was simply generated in another manner made to fit
         * the requirements of VeriBlock network addresses.
         *
         * @param toTest String to test for being an address
         * @return Whether or not the provided String is a valid address
         */
        public static bool IsValidAddress(String toTest)
        {
            if (toTest == null)
                return false;

            /* All addresses are exactly 30 characters */
            if (toTest.Length != ADDRESS_LENGTH)
                return false;

            if (toTest[0] != STARTING_CHAR)
                return false;

            /* All standard addresses are Base58 */
            if (!Base58.IsBase58String(toTest))
                return false;

            /* Take the non-checksum part, recalculate the checksum */
            String checksum = CryptoUtility.ComputeSHA256HashAsBase58(GetDataPortionFromAddress(toTest));

            /* If the checksums match, the address is valid. Otherwise, invalid. */
            return ChopChecksumStandard(checksum).Equals(GetChecksumPortionFromAddress(toTest));
        }

        /**
     * Check whether the provided String is a plausible multi-sig address, meaning it is:
     * --> 30 characters long
     * --> Encoded in Base58 (Excluding ending '0')
     * --> Has a valid checksum
     * --> Starts with the correct starting character
     * --> Contains an m value greater than 0 and less than 59
     * --> Contains an n value greater than 1 and less than 59
     */
        public static bool IsValidMultisigAddress(String toTest)
        {
            if (toTest == null)
                return false;

            /* All addresses are exactly 30 characters */
            if (toTest.Length != MULTISIG_ADDRESS_LENGTH)
            {
                return false;
            }

            if (toTest[(toTest.Length - 1)] != ENDING_CHAR_MULTISIG)
            {
                return false;
            }

            /* To make the addresses 'human-readable' we add 1 to the decoded value (1 in Base58 is 0,
             * but we want an address with a '1' in the m slot to represent m=1, for example).
             * this allows addresses with m and n both <= 9 to be easily recognized. Additionally,
             * an m or n value of 0 makes no sense, so this allows multisig to range from 1 to 58,
             * rather than what would have otherwise been 0 to 57. */
            int m = Base58.Decode("" + toTest[MULTISIG_ADDRESS_M_VALUE])[0] + 1;
            int n = Base58.Decode("" + toTest[MULTISIG_ADDRESS_N_VALUE])[0] + 1;

            /* Need at least two addresses for it to be 'multisig' */
            if (n < MULTISIG_ADDRESS_MIN_N_VALUE)
            {
                return false;
            }

            /* Can't require more signatures than addresses */
            if (m > n)
            {
                return false;
            }

            /* Impossible */
            if (n > MULTISIG_ADDRESS_MAX_N_VALUE || m > MULTISIG_ADDRESS_MAX_M_VALUE)
            {
                return false;
            }

            /* Rest of address will be Base58 */
            if (!Base58.IsBase58String(toTest.Substring(MULTISIG_ADDRESS_DATA_START, MULTISIG_ADDRESS_CHECKSUM_END + 1)))
                return false;

            /* Take the non-checksum part, recalculate the checksum */
            String checksum = CryptoUtility.ComputeSHA256HashAsBase58(
                toTest.Substring(MULTISIG_ADDRESS_DATA_START, MULTISIG_ADDRESS_DATA_END + 1));

            /* If the checksums match, the address is valid. Otherwise, invalid. */
            return ChopChecksumMultisig(checksum).Equals(GetChecksumPortionFromMultisigAddress(toTest));
        }



        /**
         * Returns the "data" (starting character plus public key hash) section of a standard address
         *
         * @param address Standard address to extract the data section from
         * @return The data portion from the provided address
         */
        private static String GetDataPortionFromAddress(String address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address.Length != ADDRESS_LENGTH)
            {
                throw new ArgumentException("getDataPortionFromAddress cannot be called with an address " +
                        "(" + address + ") which is not exactly " + ADDRESS_LENGTH + " characters long!");
            }
            return address.Substring(ADDRESS_DATA_START, ADDRESS_DATA_END + 1);
        }

        /**
         * Returns the checksum portion from the standard address
         *
         * @param address Standard address to extract the checksum section from
         * @return The checksum portion from the provided address
         */
        private static String GetChecksumPortionFromAddress(String address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address.Length != ADDRESS_LENGTH)
            {
                throw new ArgumentException("getChecksumPortionFromAddress cannot be called with an address " +
                        "(" + address + ") which is not exactly " + ADDRESS_LENGTH + " characters long!");
            }

            return address.Substring(ADDRESS_CHECKSUM_START);
        }


        /**
         * Returns the checksum portion from the multisig address
         *
         * @param address Multisig address to extract the checksum section from
         * @return The checksum portion from the provided multisig address
         */
        private static String GetChecksumPortionFromMultisigAddress(String address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address.Length != ADDRESS_LENGTH)
            {
                throw new ArgumentException("getChecksumPortionFromAddress cannot be called with an address " +
                        "(" + address + ") which is not exactly " + ADDRESS_LENGTH + " characters long!");
            }

            //Want to get 4 char snippet from say 25 to 28+1... 
            return address.Substring(MULTISIG_ADDRESS_CHECKSUM_START, MULTISIG_ADDRESS_CHECKSUM_END + 1
                - MULTISIG_ADDRESS_CHECKSUM_START);
        }

        /**
         * Chops a checksum to the appropriate length for use in creating a standard address
         * @param checksum The full checksum to chop
         * @return The chopped checksum appropriate for use in creating a standard address
         */
        private static String ChopChecksumStandard(String checksum)
        {

            if (checksum == null)
            {
                throw new ArgumentNullException("checksum");
            }
            if (checksum.Length < ADDRESS_CHECKSUM_LENGTH)
            {
                throw new ArgumentException("getChecksumPortionFromAddress cannot be called with an checksum " +
                        "(" + checksum + ") which is not at least " + ADDRESS_CHECKSUM_LENGTH + " characters long!");
            };
            return checksum.Substring(0, ADDRESS_CHECKSUM_LENGTH + 1);
        }

        /**
         * Chops a checksum to the appropriate length for use in creating a multisig address
         * @param checksum The full checksum to chop
         * @return The chopped checksum appropriate for use in creating a multisig address
         */
        private static String ChopChecksumMultisig(String checksum)
        {

            if (checksum == null)
            {
                throw new ArgumentNullException("checksum");
            }
            if (checksum.Length < ADDRESS_CHECKSUM_LENGTH)
            {
                throw new ArgumentException("getChecksumPortionFromAddress cannot be called with an checksum " +
                        "(" + checksum + ") which is not at least " + ADDRESS_CHECKSUM_LENGTH + " characters long!");
            }

            return checksum.Substring(0, MULTISIG_ADDRESS_CHECKSUM_LENGTH + 1);
        }

    }
}
