using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ex3.Plugins.MessageProvider
{
    internal class MessageProvider
    {

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
    }

    internal record Message(int MessageId, string Content, string Sender);
}
