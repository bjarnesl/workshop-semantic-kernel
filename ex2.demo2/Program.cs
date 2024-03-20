using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

Console.WriteLine("Hello, Quitter World!");
var kernelBuilder = Kernel.CreateBuilder();
var config = new
{
    modelID = "gpt-4",
    azureEndpoint = "",
    apiKey = ""
};
kernelBuilder.AddAzureOpenAIChatCompletion(config.modelID, config.azureEndpoint, config.apiKey);
var kernel = kernelBuilder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory history = [];

//Load prompts
var prompts = kernel.CreatePluginFromPromptDirectory("./../../../Prompts");
while (true)
{
    Console.Write("Request > ");
    var userRequest = Console.ReadLine();
    history.AddUserMessage(userRequest!);

    var response = await chatService.GetChatMessageContentAsync(history);
    history.Add(response);
    Console.WriteLine(response);

    var intent = await kernel.InvokeAsync(prompts["Intent"], new() { { "userRequest", userRequest } });
    if (string.Equals(intent.ToString(), "EndConversation")) break;
}
Console.WriteLine("Press any key to exit the application....");
Console.ReadKey();




