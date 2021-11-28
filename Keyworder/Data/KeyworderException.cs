using System.Runtime.Serialization;

namespace Keyworder.Data;

[Serializable]
public class KeyworderException : Exception
{
    public KeyworderException()
    {
    }

    public KeyworderException(string? message) : base(message)
    {
    }

    public KeyworderException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected KeyworderException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}