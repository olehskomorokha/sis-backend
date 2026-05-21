using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IProductQueryAiService
{
    Task<ProductCriteriaModel> ParseProductDescriptionAsync(string productDescription);
}
