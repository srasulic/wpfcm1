
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
    public class MessageXls : IButtonMessage { }
    public class MessageArchive : IButtonMessage { }
    public class MessagePickCert : IButtonMessage { }
    public class MessageShowWeb : IButtonMessage { }
    public class MessageArchiveSelected : IButtonMessage { }
    public class MessageGetPibNames : IButtonMessage { }

}
