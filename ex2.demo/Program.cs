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
var intentFunction = kernel.CreateFunctionFromPrompt(
    new PromptTemplateConfig()
    {
        Name = "Intent",
        Description = "Gets the intent of the user",
        Template = @"
            What is the intent of this request? Answer with one word. If you are unsure, dont guess, answer Other.
            You can choose between EndConversation, Other.
            Request: {{$userRequest}}
            Intent: ",
        TemplateFormat = "semantic-kernel",
        InputVariables = [new() {
            Name = "userRequest",
            Description = "This is the request from the user" }]
    });


while (true)
{
    Console.Write("Request > ");
    var userRequest = Console.ReadLine();
    history.AddUserMessage(userRequest!);

    var response = await chatService.GetChatMessageContentAsync(history);
    history.Add(response);
    Console.WriteLine(response);

    var intent = await kernel.InvokeAsync(intentFunction, new() { { "userRequest", userRequest } });
    if (string.Equals(intent.ToString(), "EndConversation")) break;
}
Console.WriteLine("Press any key to exit the application....");
Console.ReadKey();




