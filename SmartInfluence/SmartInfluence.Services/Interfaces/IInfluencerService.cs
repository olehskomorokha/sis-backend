using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Interfaces;

public interface IInfluencerService
{
    public Task<List<InfluencerResponseModel>> GetAllAsync();
    public Task<InfluencerResponseModel?> GetByIdAsync(int id);
}
