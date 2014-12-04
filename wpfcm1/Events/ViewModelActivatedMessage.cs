
namespace wpfcm1.Events
{
    public class ViewModelActivatedMessage
    {
        public ViewModelActivatedMessage(string typeName)
        {
            Name = typeName;
        }

        public string Name { get; set; }
    }
}
