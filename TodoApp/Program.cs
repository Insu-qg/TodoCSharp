using System;
using System.Globalization;
using TodoApp.Models;
using TodoApp.Persistence;
using TodoApp.Services;

class Program
{
    static void Main()
    {
        var repo = new JsonFileRepository<TaskItem>("tasks.json");
        var manager = new TaskManager(repo);

        while (true)
        {
            Console.WriteLine("\n--- Gestionnaire de tâches ---");
            Console.WriteLine("1. Lister les tâches");
            Console.WriteLine("2. Ajouter une tâche");
            Console.WriteLine("3. Modifier une tâche");
            Console.WriteLine("4. Supprimer une tâche");
            Console.WriteLine("5. Filtrer par statut/priorité");
            Console.WriteLine("6. Quitter");
            Console.Write("Choix: ");
            var input = Console.ReadLine();

            try
            {
                switch (input)
                {
                    case "1": ListTasks(manager); break;
                    case "2": AddTask(manager); break;
                    case "3": EditTask(manager); break;
                    case "4": DeleteTask(manager); break;
                    case "5": FilterTasks(manager); break;
                    case "6": return;
                    default: Console.WriteLine("Choix invalide."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
    }

    static void ListTasks(TaskManager m)
    {
        var tasks = m.GetAll();
        if (!tasks.Any()) { Console.WriteLine("Aucune tâche."); return; }
        int i = 1;
        foreach (var t in tasks)
        {
            Console.WriteLine($"{i++}. {t.Title} | {t.Status} | {t.Priority} | {t.DueDate?.ToString("yyyy-MM-dd") ?? "—"} | id:{t.Id}");
        }
    }

    static void AddTask(TaskManager m)
    {
        Console.Write("Titre: "); var title = Console.ReadLine() ?? "";
        Console.Write("Description (optionnel): "); var desc = Console.ReadLine();
        Console.Write("Date échéance (yyyy-MM-dd) ou vide: "); var d = Console.ReadLine();
        DateTime? due = null;
        if (!string.IsNullOrWhiteSpace(d) && DateTime.TryParse(d, out var parsed)) due = parsed;
        Console.Write("Priorité (Low, Medium, High) [Medium]: "); var p = Console.ReadLine();
        Enum.TryParse<Priority>(p, true, out var prio);

        var task = new TaskItem
        {
            Title = title,
            Description = desc,
            DueDate = due,
            Priority = prio
        };

        m.Add(task);
        Console.WriteLine("Tâche ajoutée.");
    }

    static void EditTask(TaskManager m)
    {
        Console.Write("Id de la tâche à modifier: ");
        if (!Guid.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id invalide."); return; }
        var t = m.GetById(id);
        if (t == null) { Console.WriteLine("Tâche non trouvée."); return; }

        Console.Write($"Titre ({t.Title}): "); var title = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(title)) t.Title = title;
        Console.Write($"Description ({t.Description}): "); var desc = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(desc)) t.Description = desc;
        Console.Write($"Date échéance ({t.DueDate?.ToString("yyyy-MM-dd") ?? "—"}): "); var d = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(d) && DateTime.TryParse(d, out var parsed)) t.DueDate = parsed;
        Console.Write($"Statut ({t.Status}): "); var s = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(s) && Enum.TryParse<Status>(s,true,out var st)) t.Status = st;
        Console.Write($"Priorité ({t.Priority}): "); var p = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(p) && Enum.TryParse<Priority>(p,true,out var pr)) t.Priority = pr;

        m.Update(t);
        Console.WriteLine("Tâche mise à jour.");
    }

    static void DeleteTask(TaskManager m)
    {
        Console.Write("Id de la tâche à supprimer: ");
        if (!Guid.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id invalide."); return; }
        Console.Write("Confirmer suppression? (o/N): ");
        var ok = Console.ReadLine();
        if (ok?.ToLower() == "o" && m.Remove(id)) Console.WriteLine("Supprimée.");
        else Console.WriteLine("Annulé ou introuvable.");
    }

    static void FilterTasks(TaskManager m)
    {
        Console.Write("Filtrer par statut (Todo, InProgress, Done) ou Enter: ");
        var s = Console.ReadLine();
        Status? st = null;
        if (!string.IsNullOrWhiteSpace(s) && Enum.TryParse<Status>(s, true, out var parsed)) st = parsed;

        Console.Write("Filtrer par priorité (Low, Medium, High) ou Enter: ");
        var p = Console.ReadLine();
        Priority? pr = null;
        if (!string.IsNullOrWhiteSpace(p) && Enum.TryParse<Priority>(p, true, out var prParsed)) pr = prParsed;

        var list = m.Filter(st, pr);
        foreach (var t in list) Console.WriteLine($"{t.Title} | {t.Status} | {t.Priority} | {t.DueDate?.ToString("yyyy-MM-dd") ?? "—"} | id:{t.Id}");
    }
}
