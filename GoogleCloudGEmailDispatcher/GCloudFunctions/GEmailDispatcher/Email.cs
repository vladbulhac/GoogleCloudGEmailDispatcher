namespace EmailDispatcher
{
    public class Email
    {
        public string From { get; set; }

        public string To { get; set; }

        public string ReplyTo { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
    }
}