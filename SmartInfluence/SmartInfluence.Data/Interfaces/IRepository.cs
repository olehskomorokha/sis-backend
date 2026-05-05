namespace SmartInfluence.Data.Interfaces;

public interface IRepository<T>
{
    public Task<List<T>> GetAllAsync();
}
