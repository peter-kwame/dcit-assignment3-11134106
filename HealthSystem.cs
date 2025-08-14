using System;
using System.Collections.Generic;

namespace Assignment3
{
    // ===========================
    // Generic Repository<T>
    // ===========================
    public class Repository<T>
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(items); // return a copy
        }

        public T? GetById(Func<T, bool> predicate)
        {
            foreach (T item in items)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default;
        }

        public bool Remove(Func<T, bool> predicate)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (predicate(items[i]))
                {
                    items.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }

    // ===========================
    // Patient & Prescription
    // ===========================
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"Patient(Id: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender})";
        }
    }

    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"Prescription(Id: {Id}, PatientId: {PatientId}, Medication: {MedicationName}, DateIssued: {DateIssued:yyyy-MM-dd})";
        }
    }

    // ===========================
    // HealthSystemApp
    // ===========================
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new Repository<Patient>();
        private readonly Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();

            List<Prescription> allPrescriptions = _prescriptionRepo.GetAll();
            foreach (Prescription p in allPrescriptions)
            {
                if (!_prescriptionMap.ContainsKey(p.PatientId))
                {
                    _prescriptionMap[p.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[p.PatientId].Add(p);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out List<Prescription>? list))
            {
                return new List<Prescription>(list); // return a copy
            }
            return new List<Prescription>();
        }

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Ama Mensah", 29, "Female"));
            _patientRepo.Add(new Patient(2, "Kofi Boateng", 41, "Male"));
            _patientRepo.Add(new Patient(3, "Yaw Owusu", 35, "Male"));

            DateTime today = DateTime.Now;
            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin 500mg", today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Vitamin C 1000mg", today.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Ibuprofen 200mg", today.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(104, 2, "Paracetamol 500mg", today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(105, 3, "Loratadine 10mg", today));
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== All Patients ===");
            List<Patient> patients = _patientRepo.GetAll();
            foreach (Patient p in patients)
            {
                Console.WriteLine(p);
            }
            Console.WriteLine();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            Console.WriteLine($"=== Prescriptions for PatientId: {patientId} ===");
            List<Prescription> prescriptions = GetPrescriptionsByPatientId(patientId);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
            }
            else
            {
                foreach (Prescription pr in prescriptions)
                {
                    Console.WriteLine(pr);
                }
            }
            Console.WriteLine();
        }

        public Repository<Patient> PatientRepo => _patientRepo;
        public Repository<Prescription> PrescriptionRepo => _prescriptionRepo;
    }

    // ===========================
    // HealthSystem Runner
    // ===========================
    public static class HealthSystem
    {
        public static void Run()
        {
            Console.WriteLine("========== QUESTION 2: Healthcare System ==========\n");

            HealthSystemApp app = new HealthSystemApp();

            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            int selectedPatientId = 2;
            app.PrintPrescriptionsForPatient(selectedPatientId);
        }
    }
}
