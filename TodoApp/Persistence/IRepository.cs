using System.Collections.Generic;

namespace TodoApp.Persistence
{
    public interface IRepository<T>
    {
        IEnumerable<T> Load();
        void Save(IEnumerable<T> items);
    }
}
