namespace ScpmBe.Services.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string msgId, string message) : base(msgId, message)
        {
        }
    }
}
