
namespace TIKSN.Pushalot
{
    public class MessageBuilder
    {
        public MessageBuilder()
        {

        }

        public string MessageTitle { get; set; }

        public string MessageBody { get; set; }

        public string MessageLinkTitle { get; set; }

        public string MessageLink { get; set; }

        public bool MessageIsImportant { get; set; }

        public bool MessageIsSilent { get; set; }

        public string MessageImage { get; set; }

        public string MessageSource { get; set; }

        public int? MessageTimeToLive { get; set; }

        public Message Build()
        {
            MessageLink link = null;

            if (!string.IsNullOrEmpty(this.MessageLink))
            {
                link = new MessageLink(this.MessageLinkTitle, new System.Uri(this.MessageLink));
            }

            MessageImage image = null;

            if (!string.IsNullOrEmpty(this.MessageImage))
            {
                image = new MessageImage(new System.Uri(this.MessageImage));
            }

            var message = new Message(this.MessageTitle, this.MessageBody, link, this.MessageIsImportant, this.MessageIsSilent, image, this.MessageSource, this.MessageTimeToLive);

            return message;
        }
    }
}
