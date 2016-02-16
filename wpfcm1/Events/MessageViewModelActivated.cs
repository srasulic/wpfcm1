
namespace wpfcm1.Events
{
    public class MessageViewModelActivated
    {
        public MessageViewModelActivated(string typeName)
        {
            Name = typeName;
        }

        public string Name { get; set; }
    }
}
