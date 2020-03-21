// VeriBlock NodeCore
// Copyright 2017-2020 Xenios SEZC.
// All rights reserved.
// https://www.veriblock.org
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.

using System;
using Grpc.Core;
using static Core.Admin;

namespace VeriBlock.Demo.Rpc.Client.Framework
{
    public class NodeCoreAdminClient : IDisposable
    {
        public Channel Channel { get; private set; }
        public AdminClient AdminClient { get; private set; }

        public NodeCoreAdminClient(string address)
        {
            Channel = new Channel(address, ChannelCredentials.Insecure, new[] { new ChannelOption("grpc.max_receive_message_length", -1) });
            AdminClient = new AdminClient(Channel);
        }

        public void Connect()
        {
            Channel.ConnectAsync();
        }

        public void Dispose()
        {
            AdminClient = null;
            Channel.ShutdownAsync().Wait();
        }
    }
}
