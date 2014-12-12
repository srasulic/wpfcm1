
namespace wpfcm1.Events
{
    public interface IButtonMessage { };
    public class MessageShowHome : IButtonMessage { }
    public class MessageSync : IButtonMessage { }
    public class MessageTogglePreview :IButtonMessage { }
    public class MessageSign :IButtonMessage { }
    public class MessageExtractData :IButtonMessage { }
    public class MessageReject :IButtonMessage { }
    public class MessageValidate :IButtonMessage { }
    public class MessageAck :IButtonMessage { }
}
