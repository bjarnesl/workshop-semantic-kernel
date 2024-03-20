//namespace ex3.Plugins
//{
//    public class PriorityChecker
//    {
//        private static readonly List<Message> _messages = [
//            new Message(1,"this is not important", "bjarne"),
//            new Message(2, "this is not important", "bjarne"),
//            new Message(3, "this is not important", "bjarne"),
//            new Message(4, "this is not important", "bjarne")];

//        public static bool IsPrioritized(int messageId)
//        {

//            var currentMessage = _messages.Single(m => m.MessageId == messageId);
//            if (string.Equals(currentMessage.Sender, "bjarne", StringComparison.InvariantCultureIgnoreCase)) return true;
//            return false;
//        }

//    }
//    internal record Message(int MessageId, string Content, string Sender);
//}
