using System;
using System.Collections.Generic;
using System.IO;

namespace Assignment3
{
    // Student class
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty; // Fix for warning CS8618
        public int Score { get; set; }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100)
                return "A";
            else if (Score >= 70 && Score <= 79)
                return "B";
            else if (Score >= 60 && Score <= 69)
                return "C";
            else if (Score >= 50 && Score <= 59)
                return "D";
            else
                return "F";
        }
    }

    // Custom exception: InvalidScoreFormatException
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    // Custom exception: MissingFieldException
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // Processor class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    string[] parts = line.Split(',');

                    if (parts.Length != 3)
                    {
                        throw new MissingFieldException($"Line {lineNumber}: Expected 3 fields, found {parts.Length}.");
                    }

                    try
                    {
                        int id = int.Parse(parts[0].Trim());
                        string fullName = parts[1].Trim();
                        int score;

                        if (!int.TryParse(parts[2].Trim(), out score))
                        {
                            throw new InvalidScoreFormatException($"Line {lineNumber}: Score '{parts[2]}' is not a valid integer.");
                        }

                        Student student = new Student
                        {
                            Id = id,
                            FullName = fullName,
                            Score = score
                        };

                        students.Add(student);
                    }
                    catch (FormatException ex)
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: {ex.Message}");
                    }
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }
}
