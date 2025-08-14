using System;
using System.Collections.Generic;

namespace Assignment3
{
    // ===========================
    // (a) Marker Interface
    // ===========================
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // ===========================
    // (b) ElectronicItem
    // ===========================
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString()
        {
            return $"ElectronicItem(Id: {Id}, Name: {Name}, Qty: {Quantity}, Brand: {Brand}, Warranty: {WarrantyMonths}m)";
        }
    }

    // ===========================
    // (c) GroceryItem
    // ===========================
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString()
        {
            return $"GroceryItem(Id: {Id}, Name: {Name}, Qty: {Quantity}, Expiry: {ExpiryDate:yyyy-MM-dd})";
        }
    }

    // ===========================
    // (e) Custom Exceptions
    // ===========================
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // ===========================
    // (d) Generic Inventory Repository<T>
    // ===========================
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items.Add(item.Id, item);
        }

        public T GetItemById(int id)
        {
            if (_items.TryGetValue(id, out T? item))
                return item;
            throw new ItemNotFoundException($"Item with ID {id} was not found.");
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Cannot remove: item with ID {id} was not found.");
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            T item = GetItemById(id); // throws if not found
            item.Quantity = newQuantity;
        }
    }

    // ===========================
    // (f) WareHouseManager
    // ===========================
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private readonly InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        // Expose repositories (handy for demo/tests)
        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;

        // Seed 2–3 items of each type
        public void SeedData()
        {
            // Electronics
            _electronics.AddItem(new ElectronicItem(201, "Smartphone", 15, "Tecno", 24));
            _electronics.AddItem(new ElectronicItem(202, "Laptop", 8, "Dell", 12));
            _electronics.AddItem(new ElectronicItem(203, "Bluetooth Speaker", 25, "JBL", 18));

            // Groceries
            _groceries.AddItem(new GroceryItem(301, "Rice (5kg)", 40, DateTime.Now.AddMonths(8)));
            _groceries.AddItem(new GroceryItem(302, "Milk (1L)", 60, DateTime.Now.AddDays(30)));
            _groceries.AddItem(new GroceryItem(303, "Bread", 20, DateTime.Now.AddDays(3)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            List<T> items = repo.GetAllItems();
            if (items.Count == 0)
            {
                Console.WriteLine("  (no items)");
                return;
            }

            foreach (var item in items)
            {
                Console.WriteLine("  " + item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id); // may throw ItemNotFoundException
                int newQty = item.Quantity + quantity;
                repo.UpdateQuantity(id, newQty); // may throw InvalidQuantityException
                Console.WriteLine($"✔ Stock increased: ID {id} ('{item.Name}') → {newQty}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✖ Error increasing stock: {ex.Message}");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"✖ Error increasing stock: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✖ Unexpected error increasing stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id); // may throw ItemNotFoundException
                Console.WriteLine($"✔ Removed item with ID {id}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✖ Error removing item: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✖ Unexpected error removing item: {ex.Message}");
            }
        }
    }

    // ===========================
    // Runner for Question 3
    // ===========================
    public static class WarehouseSystem
    {
        public static void Run()
        {
            Console.WriteLine("========== QUESTION 3: Warehouse Inventory System ==========\n");

            var manager = new WareHouseManager();

            // i. Seed data
            manager.SeedData();

            // iii. Print all grocery items
            Console.WriteLine("Groceries:");
            manager.PrintAllItems(manager.Groceries);
            Console.WriteLine();

            // iv. Print all electronic items
            Console.WriteLine("Electronics:");
            manager.PrintAllItems(manager.Electronics);
            Console.WriteLine();

            // v.a Try to add a duplicate item
            Console.WriteLine("— Test: Add duplicate electronic item (ID 201) —");
            try
            {
                manager.Electronics.AddItem(new ElectronicItem(201, "Duplicate Phone", 5, "Tecno", 24));
                Console.WriteLine("Should not see this: duplicate added.");
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"✖ Duplicate add blocked: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✖ Unexpected error adding duplicate: {ex.Message}");
            }
            Console.WriteLine();

            // v.b Try to remove a non-existent item
            Console.WriteLine("— Test: Remove non-existent grocery item (ID 999) —");
            manager.RemoveItemById(manager.Groceries, 999);
            Console.WriteLine();

            // v.c Try to update with invalid quantity (negative)
            Console.WriteLine("— Test: Set invalid quantity (negative) on Electronic ID 202 —");
            try
            {
                manager.Electronics.UpdateQuantity(202, -10);
                Console.WriteLine("Should not see this: invalid quantity accepted.");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"✖ Invalid quantity blocked: {ex.Message}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✖ Item not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✖ Unexpected error setting quantity: {ex.Message}");
            }
            Console.WriteLine();

            // Bonus: a normal successful operation (increase stock)
            Console.WriteLine("— Increase stock normally (Electronic ID 203 by +7) —");
            manager.IncreaseStock(manager.Electronics, 203, 7);
            Console.WriteLine();

            Console.WriteLine("Done.");
        }
    }
}
