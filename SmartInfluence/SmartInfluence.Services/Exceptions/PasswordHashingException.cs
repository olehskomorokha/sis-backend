namespace SmartInfluence.Services.Exceptions;

public class PasswordHashingException : SystemException
{
    public PasswordHashingException(string code, string message) 
        : base(code, message)
    {
        
    }
}