namespace SmartInfluence.Services.Exceptions;

public class SystemException : Exception
{
    public string Code { get; }

    public SystemException(string code, string message = null, Exception inner = null)
        : base(message ?? code, inner)
    {
        Code = code;
    }
}