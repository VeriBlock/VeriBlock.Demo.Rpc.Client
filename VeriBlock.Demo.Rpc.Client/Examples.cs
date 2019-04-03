// VeriBlock NodeCore
// Copyright 2017-2019 VeriBlock, Inc.
// All rights reserved.
// https://www.veriblock.org
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using System.Text;
using Core;
using VeriBlock.Demo.Rpc.Client.Framework;

namespace VeriBlock.Demo.Rpc.Client
{
    public class Examples
    {

        public void Run()
        {
            String strAddress = "127.0.0.1:10500";

            using (NodeCoreAdminClient adminClient = new NodeCoreAdminClient(strAddress))
            {
                //For simplicity, force sync:

                GetInfo(adminClient);
                ValidateAddress(adminClient);
                GetBalance(adminClient);
                GetTransactionById(adminClient);
                GetNewAddress(adminClient);
                GetBlockByIndex(adminClient);
                GetBlockByHash(adminClient);
                SendTransaction(adminClient);
            }

        }

        public void GetNewAddress(NodeCoreAdminClient adminClient)
        {
            Console.WriteLine("GetNewAddress");
            Console.WriteLine("NC_CLI command: getnewaddress");

            //Generate address
            GetNewAddressRequest requestAddress = new GetNewAddressRequest();
            GetNewAddressReply replyAddress = adminClient.AdminClient.GetNewAddress(requestAddress);
            if (replyAddress.Success)
            {
                String address = Base58.ToBase58(replyAddress.Address);
                Console.WriteLine("New Address: {0}", address);
            }

            Console.WriteLine("--------------------");
            Console.WriteLine();
        }

        public void SendTransaction(NodeCoreAdminClient adminClient)
        {
            Console.WriteLine("SendTransaction");
            Console.WriteLine("NC_CLI command: send <amount> <destinationAddress> [sourceAddress]");

            //Sending implies also signing

            String sourceAddress = "V4m6JbAs8VaEa3wwemJPgsPRY8ETdk";
            String targetAddress = "VDPNEDihaDAuKqcH35YJ1DNHaFbj2R";
            double amount = 10.5;

            SendCoinsRequest request = new SendCoinsRequest();
            request.SourceAddress = Utils.ConvertAddressToByteString(sourceAddress);
            Output o = new Output();
            o.Address = Utils.ConvertAddressToByteString(targetAddress);
            o.Amount = Utils.ConvertVbkToAtomicUnits(amount);
            request.Amounts.Add(o);

            SendCoinsReply reply = adminClient.AdminClient.SendCoins(request);

            if (reply.Success)
            {
                //Note - could create multiple Tx, pick just the first one for demo:
                Console.WriteLine("Created Transaction: {0}", reply.TxIds[0].ToHexString());
            }

            Console.WriteLine("--------------------");
            Console.WriteLine();
        }

        public void GetBlockByIndex(NodeCoreAdminClient adminClient)
        {
            Console.WriteLine("GetBlockByIndex");
            Console.WriteLine("NC_CLI command: getblockfromindex <blockIndex>");

            int iBlockIndex = 100;

            BlockFilter filter = new BlockFilter();
            filter.Index = iBlockIndex;
            GetBlocksRequest request = new GetBlocksRequest();
            request.Filters.Add(filter);
            request.SearchLength = 2000;

            GetBlocksReply reply = adminClient.AdminClient.GetBlocks(request);
            if (reply.Success)
            {
                if (reply.Blocks.Count > 0)
                {
                    //Display info
                    String blockHash = reply.Blocks[0].Hash.ToHexString();
                    Console.WriteLine("BlockHash={0}", blockHash);
                }
            }

            Console.WriteLine("--------------------");
            Console.WriteLine();
        }

        public void GetBlockByHash(NodeCoreAdminClient adminClient)
        {
            Console.WriteLine("GetBlockByHash");
            Console.WriteLine("NC_CLI command: getblockfromhash <blockHash>");

            String blockHash = "000000F09852F90A2263595FAD6CF8D0187B16DA0EA7F7F4";


            BlockFilter filter = new BlockFilter();
            filter.Hash = blockHash.ToByteString();
            GetBlocksRequest request = new GetBlocksRequest();
            request.Filters.Add(filter);
            request.SearchLength = 2000;

            GetBlocksReply reply = adminClient.AdminClient.GetBlocks(request);
            if (reply.Success)
            {
                if (reply.Blocks.Count > 0)
                {
                    //Display info
                    int iBlockIndex = reply.Blocks[0].Number;
                    Console.WriteLine("BlockIndex={0}", iBlockIndex);
                }
            }

            Console.WriteLine("--------------------");
            Console.WriteLine();
        }

        public void GetBalance(NodeCoreAdminClient adminClient)
        {
            Console.WriteLine("GetBalance");
            Console.WriteLine("NC_CLI command: getbalance [address]");

            String address = "V4m6JbAs8VaEa3wwemJPgsPRY8ETdk";

            GetBalanceRequest request = new GetBalanceRequest();
            request.Addresses.Add(Utils.ConvertAddressToByteString(address));
            GetBalanceReply reply = adminClient.AdminClient.GetBalance(request);

            if (reply.Success)
            {
                Console.WriteLine("Confirmed={0}", Utils.ConvertAtomicToVbkUnits(reply.Confirmed[0].Amount));
                Console.WriteLine("Pending={0}", Utils.ConvertAtomicToVbkUnits(reply.Unconfirmed[0].Amount));
            }

            Console.WriteLine("--------------------");
            Console.WriteLine();
        }

        public void GetInfo(NodeCoreAdminClient adminClient)
        {
            Console.WriteLine("GetBalance");
            Console.WriteLine("NC_CLI command: getinfo");
            Console.WriteLine("NC_CLI command: getstateinfo");

            GetInfoRequest requestInfo = new GetInfoRequest();
            GetInfoReply replyInfo = adminClient.AdminClient.GetInfo(requestInfo);

            GetStateInfoRequest requestState= new GetStateInfoRequest();
            GetStateInfoReply replyState = adminClient.AdminClient.GetStateInfo(requestState);

            Console.WriteLine("NetworkHeight={0}", replyState.NetworkHeight);
            Console.WriteLine("LocalBlockchainHeight={0}", replyState.LocalBlockchainHeight);
            Console.WriteLine("ConnectedPeerCount={0}", replyState.ConnectedPeerCount);
            Console.WriteLine("LastBlock.Hash={0}", replyInfo.LastBlock.Hash.ToHexString());

            Console.WriteLine("--------------------");
            Console.WriteLine();
        }

        public void GetTransactionById(NodeCoreAdminClient adminClient)
        {
            Console.WriteLine("GetTransactionById");
            Console.WriteLine("NC_CLI command: gettransaction <txId> [searchLength]");

            String txId = "DED0DB5035D90A756B6F5333157F13D55103C5F76378FD732809CDE27E6B57F3";

            GetTransactionsRequest request = new GetTransactionsRequest();
            request.Ids.Add(txId.ToByteString());
            request.SearchLength = 2000;

            GetTransactionsReply reply = adminClient.AdminClient.GetTransactions(request);
            if (reply.Success)
            {
                if (reply.Transactions.Count > 0)
                {
                    //Display info
                    int iBlockIndex = reply.Transactions[0].BlockNumber;
                    double sourceAmount = Utils.ConvertAtomicToVbkUnits(reply.Transactions[0].Transaction.SourceAmount);
                    Console.WriteLine("BlockIndex={0}, sourceAmount={1}", iBlockIndex, sourceAmount);
                }
            }

            Console.WriteLine("--------------------");
            Console.WriteLine();
        }

        public void ValidateAddress(NodeCoreAdminClient adminClient)
        {
            Console.WriteLine("GetBalance");
            Console.WriteLine("NC_CLI command: validateaddress <address>");

            String address = "V4m6JbAs8VaEa3wwemJPgsPRY8ETdk";

            ValidateAddressRequest request = new ValidateAddressRequest();
            request.Address = Utils.ConvertAddressToByteString(address);

            ValidateAddressReply reply = adminClient.AdminClient.ValidateAddress(request);
            if (reply.Success)
            {
                Console.WriteLine("IsRemote: {0}", reply.IsRemote);
                Console.WriteLine("PublicKey: {0}", reply.PublicKey.ToHexString());
            }
            else
            {
                Console.WriteLine("Could not validate address: '{0}'", address);
            }

            Console.WriteLine("--------------------");
            Console.WriteLine();
        }

    }
}
