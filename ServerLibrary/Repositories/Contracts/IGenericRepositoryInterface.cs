using BaseLibrary.Responses;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IGenericRepositoryInterface<T>
    {
        // Task will wait for a process to be done before it gets returned
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task<GeneralResponse> Insert(T item);
        Task<GeneralResponse> Update(T item);
        Task<GeneralResponse> DeleteById(int id);
    }
}
