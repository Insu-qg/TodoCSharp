using System.Collections.Generic;
using TodoApp.Persistence;

namespace TodoApp.Tests
{
    public class InMemoryRepository<T> : IRepository<T>
    {
        private readonly List<T> _items = new();

        public IEnumerable<T> Load() => _items;

        public void Save(IEnumerable<T> items)
        {
            _items.Clear();
            _items.AddRange(items);
        }
    }
}
