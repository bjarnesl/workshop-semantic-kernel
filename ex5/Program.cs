using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


Console.WriteLine("Hello, Memory World!");
const string memoryCollectionName = "aboutus";
var config = new { modelId = "gpt-3.5-turbo", apiKey = "****" };

var memoryBuilder = new MemoryBuilder();
memoryBuilder.WithOpenAITextEmbeddingGeneration("text-embedding-ada-002", config.apiKey);
memoryBuilder.WithMemoryStore(new VolatileMemoryStore());
var memory = memoryBuilder.Build();

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(config.modelId, config.apiKey);
var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};
var kernel = kernelBuilder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

await memory.SaveInformationAsync(
    collection: memoryCollectionName,
    description: "Premier league table",
    id: "PremierLeagueTable",
    text: "PTS GP  W  D  L GF GA  GD   HOME   AWAY\r\n1: ARS 64 28 20  4  4 70 24 +46 11-2-1  9-2-3\r\n2: LIV 64 28 19  7  2 65 26 +39 11-3-0  8-4-2\r\n3: MCI 63 28 19  6  3 63 28 +35 10-4-0  9-2-3\r\n4: AVL 56 29 17  5  7 60 42 +18 10-1-3  7-4-4\r\n---------------------------------------------\r\n5: TOT 53 28 16  5  7 59 42 +17 10-0-4  6-5-3\r\n---------------------------------------------\r\n6: MUN 47 28 15  2 11 39 39   0  8-1-5  7-1-6\r\n7: WHU 44 29 12  8  9 46 50  -4  6-6-3  6-2-6\r\n8: BHA 42 28 11  9  8 50 44  +6  7-6-1  4-3-7\r\n9: WOL 41 28 12  5 11 42 44  -2  7-3-4  5-2-7\r\n10:NEW 40 28 12  4 12 59 48 +11  9-2-3  3-2-9\r\n11:CHE 39 27 11  6 10 47 45  +2  6-3-4  5-3-6\r\n12:FUL 38 29 11  5 13 43 44  -1  9-1-5  2-4-8\r\n13:BOU 35 28  9  8 11 41 52 -11  4-5-5  5-3-6\r\n14:CRY 29 28  7  8 13 33 48 -15  4-4-6  3-4-7\r\n15:BRE 26 29  7  5 17 41 54 -13  4-4-6 3-1-11\r\n16:EVE 25 28  8  7 13 29 39 -10  3-4-7  5-3-6\r\n17:NFO 25 29  6  7 16 35 51 -16  4-3-7  2-4-9\r\n=============================================\r\n18:LUT 22 29  5  7 17 42 60 -18  3-3-9  2-4-8\r\n19:BUR 17 29  4  5 20 29 63 -34 2-2-11  2-3-9\r\n20:SHU 14 28  3  5 20 24 74 -50 2-2-10 1-3-10"
    );

ChatHistory history = [];

while (true)
{
    Console.Write("Request > ");
    var userRequest = Console.ReadLine();
    history.AddUserMessage(userRequest!);

    var memories = memory.SearchAsync(memoryCollectionName, userRequest!, limit: 5, minRelevanceScore: 0.77);
    await foreach (var mem in memories)
    {
        history.AddSystemMessage(mem.Metadata.Text);
    }
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

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.



