using System;
using System.Collections.Generic;
using System.IO;
using Assignment3;

public class Program
{
    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
        Assignment3.HealthSystem.Run();
        Assignment3.WarehouseSystem.Run();

        Console.WriteLine("\n=== Running Question 4: Grading System ===");
        RunGradingSystem();

        Console.WriteLine("\n=== Running Question 5: Inventory System ===");
        RunInventorySystem();
    }

    public static void RunGradingSystem()
    {
        string baseDir = Directory.GetCurrentDirectory();
        string inputFile = Path.Combine(baseDir, "students.txt");
        string outputFile = Path.Combine(baseDir, "report.txt");

        StudentResultProcessor processor = new StudentResultProcessor();

        try
        {
            List<Student> students = processor.ReadStudentsFromFile(inputFile);
            processor.WriteReportToFile(students, outputFile);

            Console.WriteLine("Grading report generated successfully.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("GradingSystem Error: Input file not found.");
            Console.WriteLine(ex.Message);
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine("GradingSystem Error: Invalid score format.");
            Console.WriteLine(ex.Message);
        }
        catch (Assignment3.MissingFieldException ex)
        {
            Console.WriteLine("GradingSystem Error: Missing required fields.");
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("GradingSystem Error: An unexpected error occurred.");
            Console.WriteLine(ex.Message);
        }
    }

    public static void RunInventorySystem()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "inventory.json");

        var inventoryApp = new InventoryApp(filePath);

        // Seed and save data
        inventoryApp.SeedSampleData();
        inventoryApp.SaveData();

        // Simulate clearing memory and reloading
        inventoryApp = new InventoryApp(filePath);
        inventoryApp.LoadData();
        inventoryApp.PrintAllItems();
    }
}
