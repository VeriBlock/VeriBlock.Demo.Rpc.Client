using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VeriBlock.Demo.Rpc.Client.Framework;

namespace VeriBlock.Demo.Framework.Tests
{
    [TestClass]
    public class AddressTests
    {
        [TestMethod]
        public void IsValidAddress_MultiSig()
        {
            var testAddress = "V2357pxG7ohXgqcVEdFrp5c1TGKfW0";

            bool bNormal = AddressUtility.IsValidAddress(testAddress);
            bool bMulti = AddressUtility.IsValidMultisigAddress(testAddress);

            Assert.AreEqual(false, bNormal);
            Assert.AreEqual(true, bMulti);

        }


        [TestMethod]
        public void IsValidAddress_Normal()
        {
            var testAddress = "VGKBNdQwed4PRnYCvnhDTaVJi2vmRs";

            bool bNormal = AddressUtility.IsValidAddress(testAddress);
            bool bMulti = AddressUtility.IsValidMultisigAddress(testAddress);

            Assert.AreEqual(true, bNormal);
            Assert.AreEqual(false, bMulti);

        }

        [TestMethod]
        public void IsValidAddress_Bad()
        {
            String[] astr = new string[]
            {
                "",
                null,
                "XYZ",
                "01234562-*73738434",
                "V2399990",
                "VGKBNdQwed4PRnYCvnhDTaVJi2vmRs--------",
                "VGKBNdQwed4PRnYCvnhDTaVJi2vmR"
            };

            foreach (String testAddress in astr)
            {
                bool bNormal = AddressUtility.IsValidAddress(testAddress);
                bool bMulti = AddressUtility.IsValidMultisigAddress(testAddress);

                Assert.AreEqual(false, bNormal);
                Assert.AreEqual(false, bMulti);
            }

        }

        [TestMethod]
        public void IsValidAddress_MassTest()
        {
            String[] realAddresses = GetManyAddresses();
            foreach (string address in realAddresses)
            {
                //Test 1: Check that is valid:
                //either normal or multisig
                bool bNormal = AddressUtility.IsValidAddress(address);
                bool bMulti = AddressUtility.IsValidMultisigAddress(address);

                //if ends with 0, then multisig
                bool isMultiSig = address.EndsWith("0");
                if (isMultiSig)
                {
                    //MuliSig
                    Assert.AreEqual(false, bNormal);
                    Assert.AreEqual(true, bMulti);
                }
                else
                {
                    //Normal
                    Assert.AreEqual(true, bNormal);
                    Assert.AreEqual(false, bMulti);
                }
            }

        }

        [TestMethod]
        public void IsValidAddress_MassTest_Invalid()
        {
            int iTestCount = 0;
            String[] realAddresses = GetManyAddresses();
            foreach (string address in realAddresses)
            {
                //Change last 3 char

                for (int i = 0; i < address.Length; i++)
                {
                    char[] ach = address.ToCharArray();
                    ach[i] = Convert.ToChar((int)ach[i]+1); //increment by 1, which will break the checksum

                    //return to string
                    String strMalformedAddress = new string(ach);

                    //No longer valid
                    Assert.AreEqual(false, AddressUtility.IsValidAddress(strMalformedAddress));
                    Assert.AreEqual(false, AddressUtility.IsValidMultisigAddress(strMalformedAddress));
                    iTestCount++;
                }

            }
            Console.WriteLine("Ran {0} combinations", iTestCount);
        }

        private String[] GetManyAddresses()
        {
            String[] astr = new string[]
            {
"V12PyBd7q9WqiarQuRqyWvJBX7gUK6",
"V12VggtAS958tjAQf9EwiVafA43Sx4",
"V1392ytdhodW1aeKRWUdwDzssDmc7g",
"V13XCVZN3iEYnCKHL5SuZYbVsCUudn",
"V1amJUd57PcLWB1aCvUJGGVxQHFHot",
"V1FzU9WUSqiJj5stayEiNDhdeHPKdo",
"VZsyTafkLhBgNMau56AgPwu3B3QUPM",
"VZURJhdqkzCKc6gc2hG1ffSh23VpB9",
"VZVfQ5Dve55MWALeLg8eGy6EN9QSX6"
            };

            return astr;
        }

    }
}
