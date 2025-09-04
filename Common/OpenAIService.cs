using OpenAI;
using OpenAI.Chat;
using System.Threading.Tasks;

public class OpenAIService
{
    private readonly OpenAIClient _client;

    public OpenAIService()
    {
        // Thay YOUR_API_KEY bằng key của bạn
        _client = new OpenAIClient("YOUR_API_KEY");
    }

    public async Task<string> GetChatResponseAsync(string userMessage)
    {
        var chatRequest = new ChatRequest(
            new[]
            {
                new Message(Role.System, "Bạn là một trợ lý ảo chuyên tư vấn mỹ phẩm và phòng khám da liễu."),
                new Message(Role.User, userMessage)
            },
            model: "gpt-4o-mini" // Hoặc gpt-4o, gpt-4.1...
        );

        var response = await _client.ChatEndpoint.GetCompletionAsync(chatRequest);

        return response.FirstChoice;
    }
}
