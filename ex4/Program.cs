using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;

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
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
kernelBuilder.Plugins.AddFromType<TimePlugin>();
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};
var kernel = kernelBuilder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory history = [];


while (true)
{
    Console.Write("Request > ");
    var userRequest = Console.ReadLine();
    history.AddUserMessage(userRequest!);
    string message = "";
    await foreach (var chunk in chatService.GetStreamingChatMessageContentsAsync(history, executionSettings: executionSettings, kernel: kernel))
    {
        Console.Write(chunk);
        await Task.Delay(10);
        message += chunk;
    }
    Console.WriteLine(Environment.NewLine);
    history.AddAssistantMessage(message);
}




