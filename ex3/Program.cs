using ex3.Plugins.MessageProvider;
using ex3.Plugins.Priority;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

Console.WriteLine("Hello, Quitter World!");
var kernelBuilder = Kernel.CreateBuilder();
var config = new
{
    modelID = "gpt-4",
    azureEndpoint = "",
    apiKey = ""
};
kernelBuilder.AddAzureOpenAIChatCompletion(config.modelID, config.azureEndpoint, config.apiKey);


//Register Functions
kernelBuilder.Plugins.AddFromType<PriorityChecker>();
kernelBuilder.Plugins.AddFromType<MessageProvider>();

var executionSettings = new OpenAIPromptExecutionSettings 
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};
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
    string message = "";
    await foreach (var chunk in chatService.GetStreamingChatMessageContentsAsync(history, executionSettings:executionSettings, kernel:kernel))
    {
        Console.Write(chunk);
        await Task.Delay(10);
        message += chunk;
    }
    Console.WriteLine(Environment.NewLine);
    history.AddAssistantMessage(message);

    var intent = await kernel.InvokeAsync(prompts["Intent"], new() { { "userRequest", userRequest } });
    if (string.Equals(intent.ToString(), "EndConversation")) break;
}
Console.WriteLine("Press any key to exit the application....");
Console.ReadKey();




