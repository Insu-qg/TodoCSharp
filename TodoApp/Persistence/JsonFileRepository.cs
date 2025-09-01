using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TodoApp.Persistence
{
    public class JsonFileRepository<T> : IRepository<T>
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { WriteIndented = true };

        public JsonFileRepository(string filePath) => _filePath = filePath;

        public IEnumerable<T> Load()
        {
            if (!File.Exists(_filePath)) return new List<T>();
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
            }
            catch (JsonException)
            {
                // Fichier corrompu : on renvoie liste vide
                return new List<T>();
            }
        }

        public void Save(IEnumerable<T> items)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var json = JsonSerializer.Serialize(items, _options);
            File.WriteAllText(_filePath, json);
        }
    }
}
