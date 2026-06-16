namespace SmartInfluence.Services.Exceptions;

public class InvalidProductDescriptionException : SystemException
{
    public InvalidProductDescriptionException(string message)
        : base("invalid_product_description", message)
    {
    }
}
