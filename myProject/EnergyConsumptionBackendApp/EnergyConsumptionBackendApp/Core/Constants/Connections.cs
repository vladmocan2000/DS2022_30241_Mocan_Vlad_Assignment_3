using Grpc.Core;
using GrpcChat;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace EnergyConsumptionBackendApp.Core.Constants
{
    public static class Connections
    {
        public static ConcurrentDictionary<string, WebSocket> websocketConnections = new();
        public static ConcurrentDictionary<string, IServerStreamWriter<ChatMessage>> grpcChatConnections = new(); 
    }
}
