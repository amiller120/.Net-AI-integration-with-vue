
using Microsoft.Extensions.AI;

namespace AIChatIntegration.Server.Endpoints.Chat
{
    public class Chat : FastEndpoints.Endpoint<ChatRequest, ChatResponse>
    {
        public override void Configure()
        {
            Post("/chat");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ChatRequest req, CancellationToken ct)
        {
            var chatClient = Resolve<IChatClient>();

            if (chatClient == null)
            {
                AddError("Chat client not found");
                ThrowIfAnyErrors();
            }

            // if you need a system message
        //    var systemMessage = new ChatMessage(
        //    ChatRole.System,
        //    "You are a trust bank officer, you provide helpful expert information about banking. Do not offer to take their information, only help give answers when you are able."
        //);

            var userMessage = new ChatMessage(ChatRole.User, req.Message);

            var response = await chatClient.GetResponseAsync(userMessage, cancellationToken: ct);

            await SendAsync(new ChatResponse
            {
                Message = response.Message
            }, cancellation: ct);
        }
    }
}
