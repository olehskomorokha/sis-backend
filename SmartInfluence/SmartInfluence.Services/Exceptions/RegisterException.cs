namespace SmartInfluence.Services.Exceptions;

public class RegisterException : SystemException
{
    public RegisterException(string code, string message)
        : base(code, message)
    {
        
    }
}