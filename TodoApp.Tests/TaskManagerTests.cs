using System;
using System.Linq;
using TodoApp.Models;
using TodoApp.Services;
using Xunit;

namespace TodoApp.Tests
{
    public class TaskManagerTests
    {
        private TaskManager CreateManager()
        {
            var repo = new InMemoryRepository<TaskItem>();
            return new TaskManager(repo);
        }

        [Fact]
        public void AddTask_ShouldIncreaseCount()
        {
            var manager = CreateManager();
            var task = new TaskItem { Title = "Test task" };

            manager.Add(task);

            var all = manager.GetAll();
            Assert.Single(all);
            Assert.Equal("Test task", all.First().Title);
        }

        [Fact]
        public void RemoveTask_ShouldDecreaseCount()
        {
            var manager = CreateManager();
            var task = new TaskItem { Title = "Task to remove" };
            manager.Add(task);

            bool removed = manager.Remove(task.Id);

            Assert.True(removed);
            Assert.Empty(manager.GetAll());
        }

        [Fact]
        public void UpdateTask_ShouldModifyProperties()
        {
            var manager = CreateManager();
            var task = new TaskItem { Title = "Old title", Priority = Priority.Low };
            manager.Add(task);

            task.Title = "New title";
            task.Priority = Priority.High;
            manager.Update(task);

            var updated = manager.GetById(task.Id);
            Assert.Equal("New title", updated?.Title);
            Assert.Equal(Priority.High, updated?.Priority);
        }

        [Fact]
        public void FilterTask_ShouldReturnCorrectResults()
        {
            var manager = CreateManager();
            manager.Add(new TaskItem { Title = "T1", Status = Status.Todo, Priority = Priority.Low });
            manager.Add(new TaskItem { Title = "T2", Status = Status.Done, Priority = Priority.High });
            manager.Add(new TaskItem { Title = "T3", Status = Status.Todo, Priority = Priority.High });

            var filtered = manager.Filter(status: Status.Todo, priority: Priority.High).ToList();

            Assert.Single(filtered);
            Assert.Equal("T3", filtered.First().Title);
        }

        [Fact]
        public void AddTask_WithEmptyTitle_ShouldThrow()
        {
            var manager = CreateManager();
            var task = new TaskItem { Title = "" };

            Assert.Throws<ArgumentException>(() => manager.Add(task));
        }
    }
}
