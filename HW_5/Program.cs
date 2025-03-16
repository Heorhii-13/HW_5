using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        var doctorService = new DoctorService();
        var patientService = new PatientService();
        var appointmentService = new AppointmentService();

        while (true)
        {
            Console.WriteLine("1. Manage Doctors");
            Console.WriteLine("2. Manage Patients");
            Console.WriteLine("3. Manage Appointments");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");

            if (!Enum.TryParse(Console.ReadLine(), out MenuOption option))
            {
                Console.WriteLine("Invalid option. Try again.");
                continue;
            }

            switch (option)
            {
                case MenuOption.Doctors:
                    doctorService.Menu();
                    break;
                case MenuOption.Patients:
                    patientService.Menu();
                    break;
                case MenuOption.Appointments:
                    appointmentService.Menu();
                    break;
                case MenuOption.Exit:
                    return;
            }
        }
    }
}

enum MenuOption
{
    Doctors = 1,
    Patients,
    Appointments,
    Exit
}

class Doctor { public int Id; public string Name; }
class Patient { public int Id; public string Name; }
class Appointment { public int Id; public int DoctorId; public int PatientId; public DateTime Date; }

abstract class BaseService<T> where T : class, new()
{
    protected List<T> items = new List<T>();
    protected int idCounter = 1;

    public void Add(T item)
    {
        typeof(T).GetProperty("Id")?.SetValue(item, idCounter++);
        items.Add(item);
        Console.WriteLine("Added successfully.");
    }
    public void Remove(int id)
    {
        var item = items.FirstOrDefault(i => (int)typeof(T).GetProperty("Id").GetValue(i) == id);
        if (item != null)
        {
            items.Remove(item);
            Console.WriteLine("Removed successfully.");
        }
        else Console.WriteLine("Item not found.");
    }
    public void List()
    {
        if (items.Count == 0)
        {
            Console.WriteLine("No items available.");
            return;
        }
        
        foreach (var item in items)
        {
            foreach (var prop in item.GetType().GetProperties())
            {
                Console.Write($"{prop.Name}: {prop.GetValue(item)} \t");
            }
            Console.WriteLine();
        }
    }
    public abstract void Menu();
}

class DoctorService : BaseService<Doctor>
{
    public override void Menu()
    {
        while (true)
        {
            Console.WriteLine("1. Add Doctor\n2. Remove Doctor\n3. List Doctors\n4. Back");
            var input = Console.ReadLine();
            if (input == "4") break;
            if (input == "1")
            {
                Console.Write("Enter name: ");
                Add(new Doctor { Name = Console.ReadLine() });
            }
            else if (input == "2")
            {
                Console.Write("Enter ID: ");
                Remove(int.Parse(Console.ReadLine()));
            }
            else if (input == "3")
            {
                List();
            }
        }
    }
}

class PatientService : BaseService<Patient>
{
    public override void Menu()
    {
        while (true)
        {
            Console.WriteLine("1. Add Patient\n2. Remove Patient\n3. List Patients\n4. Back");
            var input = Console.ReadLine();
            if (input == "4") break;
            if (input == "1")
            {
                Console.Write("Enter name: ");
                Add(new Patient { Name = Console.ReadLine() });
            }
            else if (input == "2")
            {
                Console.Write("Enter ID: ");
                Remove(int.Parse(Console.ReadLine()));
            }
            else if (input == "3")
            {
                List();
            }
        }
    }
}

class AppointmentService : BaseService<Appointment>
{
    public override void Menu()
    {
        while (true)
        {
            Console.WriteLine("1. Add Appointment\n2. Remove Appointment\n3. List Appointments\n4. Back");
            var input = Console.ReadLine();
            if (input == "4") break;
            if (input == "1")
            {
                Console.Write("Enter Doctor ID: ");
                int doctorId = int.Parse(Console.ReadLine());
                Console.Write("Enter Patient ID: ");
                int patientId = int.Parse(Console.ReadLine());
                Add(new Appointment { DoctorId = doctorId, PatientId = patientId, Date = DateTime.Now });
            }
            else if (input == "2")
            {
                Console.Write("Enter ID: ");
                Remove(int.Parse(Console.ReadLine()));
            }
            else if (input == "3")
            {
                List();
            }
        }
    }
}