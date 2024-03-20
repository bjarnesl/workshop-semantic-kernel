# Workshop Semantic Kernel
This repoisotory contains exercices for a workhop introducing Semantic Kernel. 
The evolution of the repository is preserved using tags for each step, e.g. step0. So if you would like to follow along, start with checking out step[0].

## Prerequisites
- [ ] dotnet 8 sdk
- [ ] Api key

## Exercise 1 - Chat
We create a simple chat bot, evolving from one acecpting only a single message, to one that will remember your conversatoins.

### A. Single message - together
 - [ ] Create a new solution
 - [ ] Install the ```Microsoft.SemanticKernel``` package
 - [ ] Configure and build the kernel using the Builder-pattern:
      ```csharp
      var kernelBuilder = Kernel.CreateBuilder();
      var config = new { modelID = "gpt-4", azureEndpoint = "", apiKey = "….." };
      kernelBuilder.AddAzureOpenAIChatCompletion(config.modelID, config.azureEndpoint,config.apiKey);
      var kernel = kernelBuilder.Build();
      ```
 - [ ] Invoke the chatservice and print the result:
      ```csharp
      var chatService = kernel.GetRequiredService<IChatCompletionService>();
      var chatResponse = await chatService.GetChatMessageContentAsync("Hi! How are you?");
      Console.WriteLine(chatResponse);
      ```
 - [ ] Read the user input from console instead of hard coding it

 ### B. Back and forth chat
 - [ ] Create a loop, listenting for input
    ```csharp
    while(true) {
       Console.Write("User > ");
       var chatRequest = Console.ReadLine();
       var chatResponse = await chatService.GetChatMessageContentAsync(history);
       Console.WriteLine(chatResponse);
    }
    ```

### C. Remember me!
 - [ ] Preserve, and pass along history
```csharp
    ChatHistory history = [];
    while(true) {
        Console.Write("User > ");
        var chatRequest = Console.ReadLine();
        history.AddUserMessage(chatRequest!);
        var chatResponse = await chatService.GetChatMessageContentAsync(history); 
        history.AddAssistantMessage(chatResponse.Content!);
        Console.WriteLine(chatResponse);    
    }
```

### D. Bonus - straming chat
 If wou would like to get that delayed output feel, you can
 use the GetStreamnChatMessageContentAsync Method:

 ```csharp
     string message = "";
     await foreach (var chunk in chatService.GetStreamingChatMessageContentsAsync(history, executionSettings:executionSettings, kernel:kernel))
     {
         Console.Write(chunk);
         await Task.Delay(10);
         message += chunk;
     }
     Console.WriteLine(Environment.NewLine);
     history.AddAssistantMessage(message);
 ```

 ## Exercise 2 - Prompt engineering
 We create an online prompt, and envolve it to generate more specific results

 Version 1:
```csharp
var prompt = $@"What is the intent of this request? {userRequest}";
var response = await kernel.InvokePromptAsync(prompt);
Console.WriteLine(response);
```

Version 2:
```csharp
var prompt = $@"
What is the intent of this request? Keep it short, less than 15 words.
{userRequest}";
```

Version 3:
```csharp
var prompt = $@"
What is the intent of this request? Keep it short, less than 15 words.
You can choose between SendEmail, CreateDocument or Other.
Request: {userRequest}
Intent: ";
```

Version 4:
```csharp
var prompt = @$"Provide the intent of the request using the following json format:
{{
    ""intent"": {{intent}}
}}
 
You can choose between the following intents:
[""SendEmail"", ""SendMessage"", ""CompleteTask"", ""CreateDocument""]

The user input is: {userRequest}";
```
See the documentation for describing the template syntax, at [Micrisoft.Learn](https://learn.microsoft.com/en-us/semantic-kernel/prompts/prompt-template-syntax)
See suggestions on how to improve your prompts at [Microsof.Learn](https://learn.microsoft.com/en-us/semantic-kernel/prompts/your-first-prompt?tabs=Csharp#improving-the-prompt-with-prompt-engineering)


## Exercise 2 - Demo 1 - From prompt to function
We create a function from a prompt
```csharp
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
        TemplateFormat ="semantic-kernel",
        InputVariables = [new() { 
            Name = "userRequest", 
            Description = "This is the request from the user" }]
    });

```

## Exercise 2 - Demo 2 -  Move prompt to file
- [ ] Create a new folder called Prompts, with a subfolder called Intent.
- [ ] Create two new files
  - skprompt.txt
  - config.json

- [ ] Simply copy-paste the prompt into the skprompt file.  
- [ ] In the config.json, using the following template
```json
{
    "schema": 1,
    "type": "",
    "description": "",
    "input_variables": [
        {
            "Name": "",
            "Description": "",
            "required": true
        }
    ]
}
```

## Exercise 3 - Native function/plugin
- [ ] Create a new folder call Plugin
- [ ] Add you plugin as a class with a static method, providing the required annotations
```csharp
     private static readonly List<Message> _messages = [
         new Message(1,"I am feeling good now, although I previously suffered from amnesia", "bjarne"),
         new Message(2, "this is not important. I had a cough in the past", "per"),
         new Message(3, "this is not important, I do have a cold though.", "karianne"),
         new Message(4, "this is not important", "turid")];

 [KernelFunction, Description("Retrieves a list of all messages")]
 public static string GetAllMessages()
 {
     return System.Text.Json.JsonSerializer.Serialize(_messages);
 }
```
- [ ] Register your function
```csharp
    kernelBuilder.Plugins.AddFromType<MesassageProvider>();
```

- [ ] Either invoke the function directly
```csharp
    bool isPrio = await kernel.InvokeAsync<bool>("MessageProvider", "GetAllMessages", new() { {}});
``` 
- [ ] Or configure your Kernel to automatically invoke plugins. 
Remember to pass the kernel when invoking the chat service, as it is used to identify avilable plugins.
```csharp
var executionSettings = new OpenAIPromptExecutionSettings 
{ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions};
....
await foreach (var chunk in chatService.GetStreamingChatMessageContentsAsync(history, executionSettings:executionSettings, kernel:kernel))
```

- [ ] Add a PriorityCheckPLugin in the similar way. Ask the AI to get all prioritized messages

## Exercise 4 - Built in plugins
- [ ] Install the Plugin Nuget Package ```Microsoft.SemanticKernel.Plugins.Core``` (Alpha)
- [ ] Add one or more of the following plugins:
  -  ConversationSummaryPlugin
  -  FileIOPlugin
  -  HttpPlugin
  -  MathPlugin
  -  TextMemoryPlugin
  -  TextPlugin
  -  TimePlugin
  -  WaitPlugin
   ```
    kernelBuilder.Plugins.AddFromType<TimePlugin>();
   ```

## Exercise 5 - Memories
- [ ] Ensure that you are using an API that support embeddings
- [ ] Install nuget ```Microsoft.SemanticKernel.Plugins.Memory```
- [ ] Create and build a semantictextmemory
```csharp
    var memoryBuilder = new MemoryBuilder();
    memoryBuilder.WithOpenAITextEmbeddingGeneration("text-embedding-ada-002", config.apiKey);
    memoryBuilder.WithMemoryStore(new VolatileMemoryStore());
    var memory = memoryBuilder.Build();
```
- [ ] Add a memory to the store (source: [plaintextsports.com](https://plaintextsports.com/premier-league/2023-2024/table))
```
    await memory.SaveInformationAsync(
    collection: memoryCollectionName,
    description: "Premier league table",
    id: "PremierLeagueTable",
    text: "PTS GP  W  D  L GF GA  GD   HOME   AWAY\r\n1: ARS 64 28 20  4  4 70 24 +46 11-2-1  9-2-3\r\n2: LIV 64 28 19  7  2 65 26 +39 11-3-0  8-4-2\r\n3: MCI 63 28 19  6  3 63 28 +35 10-4-0  9-2-3\r\n4: AVL 56 29 17  5  7 60 42 +18 10-1-3  7-4-4\r\n---------------------------------------------\r\n5: TOT 53 28 16  5  7 59 42 +17 10-0-4  6-5-3\r\n---------------------------------------------\r\n6: MUN 47 28 15  2 11 39 39   0  8-1-5  7-1-6\r\n7: WHU 44 29 12  8  9 46 50  -4  6-6-3  6-2-6\r\n8: BHA 42 28 11  9  8 50 44  +6  7-6-1  4-3-7\r\n9: WOL 41 28 12  5 11 42 44  -2  7-3-4  5-2-7\r\n10:NEW 40 28 12  4 12 59 48 +11  9-2-3  3-2-9\r\n11:CHE 39 27 11  6 10 47 45  +2  6-3-4  5-3-6\r\n12:FUL 38 29 11  5 13 43 44  -1  9-1-5  2-4-8\r\n13:BOU 35 28  9  8 11 41 52 -11  4-5-5  5-3-6\r\n14:CRY 29 28  7  8 13 33 48 -15  4-4-6  3-4-7\r\n15:BRE 26 29  7  5 17 41 54 -13  4-4-6 3-1-11\r\n16:EVE 25 28  8  7 13 29 39 -10  3-4-7  5-3-6\r\n17:NFO 25 29  6  7 16 35 51 -16  4-3-7  2-4-9\r\n=============================================\r\n18:LUT 22 29  5  7 17 42 60 -18  3-3-9  2-4-8\r\n19:BUR 17 29  4  5 20 29 63 -34 2-2-11  2-3-9\r\n20:SHU 14 28  3  5 20 24 74 -50 2-2-10 1-3-10"
    );
```
- [ ] Search the store for relevant memories compared to the request from the user, and add them to the history
```csharp
    var memories = memory.SearchAsync(memoryCollectionName, userRequest!, limit: 5, minRelevanceScore: 0.77);
    await foreach (var mem in memories)
    {
        history.AddSystemMessage(mem.Metadata.Text);
    }
```
- [ ] The LLM will now be able to answer questions related to the memory.