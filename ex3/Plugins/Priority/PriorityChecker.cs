using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ex3.Plugins.Priority
{
    public class PriorityChecker
    {
        [KernelFunction, Description("Checks wheter a specific message is prioritized or not, based on the sender of a message")]
        public static bool IsPrioritized([Description("The sender of the message to be checked")] string sender)
        {
            if (string.Equals(sender, "bjarne", StringComparison.InvariantCultureIgnoreCase)) return true;
            return false;
        }
    }
}
