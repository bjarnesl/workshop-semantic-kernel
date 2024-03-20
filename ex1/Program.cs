using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

Console.WriteLine("Hello, Chat World!");
var kernelBuilder = Kernel.CreateBuilder();
var config = new { 
    modelID = "gpt-4", 
    azureEndpoint = "", 
    apiKey = "" 
};
kernelBuilder.AddAzureOpenAIChatCompletion(config.modelID, config.azureEndpoint, config.apiKey);
var kernel = kernelBuilder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory history = [];
while (true)
{
    Console.Write("User > ");
    var chatRequest = Console.ReadLine();
    history.AddUserMessage(chatRequest!);
    var chatResponse = await chatService.GetChatMessageContentAsync(history);
    history.AddAssistantMessage(chatResponse.Content!);
    Console.WriteLine(chatResponse);
}