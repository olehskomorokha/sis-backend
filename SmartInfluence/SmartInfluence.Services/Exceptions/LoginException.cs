namespace SmartInfluence.Services.Exceptions;

public class LoginException : SystemException
{
    public LoginException(string code, string message)
        : base(code, message)
    {
        
    }
}