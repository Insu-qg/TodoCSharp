using System;
using System.Collections.Generic;
using System.Linq;
using TodoApp.Models;
using TodoApp.Persistence;

namespace TodoApp.Services
{
    public class TaskManager
    {
        private readonly IRepository<TaskItem> _repo;
        private readonly List<TaskItem> _tasks;

        public TaskManager(IRepository<TaskItem> repo)
        {
            _repo = repo;
            _tasks = _repo.Load().ToList();
        }

        public IEnumerable<TaskItem> GetAll() => _tasks.OrderBy(t => t.DueDate);

        public TaskItem? GetById(Guid id) => _tasks.FirstOrDefault(t => t.Id == id);

        public void Add(TaskItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new ArgumentException("Le titre est requis.");
            _tasks.Add(item);
            Save();
        }

        public void Update(TaskItem item)
        {
            var existing = GetById(item.Id);
            if (existing == null) throw new InvalidOperationException("Tâche non trouvée.");
            existing.Title = item.Title;
            existing.Description = item.Description;
            existing.DueDate = item.DueDate;
            existing.Priority = item.Priority;
            existing.Status = item.Status;
            Save();
        }

        public bool Remove(Guid id)
        {
            var item = GetById(id);
            if (item == null) return false;
            _tasks.Remove(item);
            Save();
            return true;
        }

        public IEnumerable<TaskItem> Filter(Status? status = null, Priority? priority = null)
        {
            var q = _tasks.AsEnumerable();
            if (status.HasValue) q = q.Where(t => t.Status == status.Value);
            if (priority.HasValue) q = q.Where(t => t.Priority == priority.Value);
            return q.OrderBy(t => t.DueDate);
        }

        private void Save() => _repo.Save(_tasks);
    }
}
