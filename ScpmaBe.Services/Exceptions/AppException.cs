namespace ScpmBe.Services.Exceptions
{
    public abstract class AppException : Exception
    {
        public string MessageId { get; set; }

        protected AppException(string msgId, string message) : base(message) {
            MessageId = msgId;
        }
    }
}
