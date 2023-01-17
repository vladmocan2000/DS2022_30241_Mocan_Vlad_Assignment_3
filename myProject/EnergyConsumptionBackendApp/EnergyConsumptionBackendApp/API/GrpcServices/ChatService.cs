using EnergyConsumptionBackendApp.Core.Constants;
using Grpc.Core;
using GrpcChat;
using static GrpcChat.ChatService;

namespace EnergyConsumptionBackendApp.API.GrpcServices
{
    public class ChatService : ChatServiceBase
    {
        private readonly ILogger<ChatService> _logger;
        public ChatService(ILogger<ChatService> logger)
        {
            _logger = logger;
        }

        public override async Task<Empty> sendMsg(ChatMessage chatMessage, ServerCallContext context)
        {
            if (!Connections.grpcChatConnections.ContainsKey(chatMessage.To + "->" + chatMessage.From))
            {
                Console.WriteLine("grpc username " + chatMessage.To + "->" + chatMessage.From + " not found!");
                return new Empty();
            }
            if (!Connections.grpcChatConnections.ContainsKey(chatMessage.From + "->" + chatMessage.To))
            {
                Console.WriteLine("grpc username " + chatMessage.From + "->" + chatMessage.To + " not found!");
                return new Empty();
            }
            Console.WriteLine("Sending message to" + chatMessage.To + "->" + chatMessage.From);
            await Connections.grpcChatConnections[chatMessage.To + "->" + chatMessage.From].WriteAsync(chatMessage);
            Console.WriteLine("Sending message to" + chatMessage.From + "->" + chatMessage.To);
            await Connections.grpcChatConnections[chatMessage.From + "->" + chatMessage.To].WriteAsync(chatMessage);
            return new Empty();
        }

        public override async Task receiveMsg(
            Client client,
            IServerStreamWriter<ChatMessage> responseStream,
            ServerCallContext context)
        {
            Console.WriteLine("Stream opened by " + client.Username);
            if (!Connections.grpcChatConnections.TryAdd(client.Username, responseStream))
            {
                Console.WriteLine("Stream opened by " + client.Username + " could not be added to dictionary!");
            }
            
            while (true)
            {
                
            }
        }
    }
}
