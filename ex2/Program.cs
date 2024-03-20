using Microsoft.SemanticKernel;

Console.WriteLine("Hello, Itent World!");
var kernelBuilder = Kernel.CreateBuilder();
var config = new
{
    modelID = "gpt-4",
    azureEndpoint = "",
    apiKey = ""
};
kernelBuilder.AddAzureOpenAIChatCompletion(config.modelID, config.azureEndpoint, config.apiKey);
var kernel = kernelBuilder.Build();

while (true)
{
    Console.Write("Request > ");
    var userRequest = Console.ReadLine();

    #region v1
    //var prompt = $@"What is the intent of this request? {userRequest}";
    #endregion

    #region v2
    //var prompt = $@"
    //    What is the intent of this request? Keep it short, less than 15 words.
    //    {userRequest}";
    #endregion

    #region v3
    //var prompt = $@"
    //    What is the intent of this request? Keep it short, less than 15 words.
    //    You can choose between SendEmail, CreateDocument or Other.
    //    Request: {userRequest}
    //    Intent: ";
    #endregion

    #region v4
    var prompt = @$"Provide the intent of the request using the following json format:
{{
    ""intent"": {{intent}}
}}
 
You can choose between the following intents:
[""SendEmail"", ""SendMessage"", ""CompleteTask"", ""CreateDocument""]

The user input is: {userRequest}";
 
    #endregion

    var response = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine(response);
}




