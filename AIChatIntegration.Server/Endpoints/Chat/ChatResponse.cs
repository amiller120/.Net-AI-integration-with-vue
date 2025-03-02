using Microsoft.Extensions.AI;

namespace AIChatIntegration.Server.Endpoints.Chat
{
    public class ChatResponse
    {
        public ChatMessage Message { get; set; } = new();
    }
}