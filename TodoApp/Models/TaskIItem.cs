using System;

namespace TodoApp.Models
{
    public enum Status { Todo, InProgress, Done }
    public enum Priority { Low, Medium, High }

    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Status Status { get; set; } = Status.Todo;
        public Priority Priority { get; set; } = Priority.Medium;

        public override string ToString()
            => $"{Title} [{Status}] (Priorité: {Priority}) - Échéance: {(DueDate?.ToString("u") ?? "Aucune")}";
    }
}
