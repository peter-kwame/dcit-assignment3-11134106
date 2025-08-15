using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Assignment3
{
    // Marker interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // Immutable inventory record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // Generic inventory logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return _log;
        }

        public void SaveToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
                Console.WriteLine("Inventory saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("File not found.");
                    return;
                }

                string json = File.ReadAllText(_filePath);
                _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                Console.WriteLine("Inventory loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    // Integration class
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Mouse", 25, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Keyboard", 15, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Monitor", 8, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Headphones", 12, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
            }
        }
    }
}
