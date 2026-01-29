namespace API.Repository.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly List<T> _data;

        public GenericRepository()
        {
            _data = new List<T>();
        }

        public virtual Task<T> GetByIdAsync(string id)
        {
            throw new NotImplementedException("Override in derived class");
        }

        public virtual Task<List<T>> GetAllAsync()
        {
            return Task.FromResult(_data);
        }

        public virtual Task<T> AddAsync(T entity)
        {
            _data.Add(entity);
            return Task.FromResult(entity);
        }

        public virtual Task<bool> UpdateAsync(T entity)
        {
            throw new NotImplementedException("Override in derived class");
        }

        public virtual Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException("Override in derived class");
        }

        public virtual Task<bool> ExistsAsync(string id)
        {
            throw new NotImplementedException("Override in derived class");
        }

        protected List<T> GetData()
        {
            return _data;
        }
    }
}
