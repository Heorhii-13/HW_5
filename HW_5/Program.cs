using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class Patient
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime Date { get; set; }
}

interface IDataStorage<T>
{
    void Save(List<T> data);
    List<T> Load();
}

class JsonDataStorage<T> : IDataStorage<T>
{
    private readonly string _filePath;
    public JsonDataStorage(string filePath) => _filePath = filePath;
    public void Save(List<T> data) => File.WriteAllText(_filePath, JsonSerializer.Serialize(data));
    public List<T> Load() => File.Exists(_filePath) ? JsonSerializer.Deserialize<List<T>>(File.ReadAllText(_filePath)) ?? new List<T>() : new List<T>();
}

class XmlDataStorage<T> : IDataStorage<T>
{
    private readonly string _filePath;
    public XmlDataStorage(string filePath) => _filePath = filePath;
    public void Save(List<T> data)
    {
        var serializer = new XmlSerializer(typeof(List<T>));
        using var writer = new StreamWriter(_filePath);
        serializer.Serialize(writer, data);
    }
    public List<T> Load()
    {
        if (!File.Exists(_filePath)) return new List<T>();
        var serializer = new XmlSerializer(typeof(List<T>));
        using var reader = new StreamReader(_filePath);
        return (List<T>)serializer.Deserialize(reader) ?? new List<T>();
    }
}

abstract class BaseService<T> where T : class, new()
{
    protected List<T> items;
    protected int idCounter = 1;
    private readonly IDataStorage<T> _storage;

    protected BaseService(IDataStorage<T> storage)
    {
        _storage = storage;
        items = _storage.Load();
        if (items.Any()) idCounter = items.Max(i => (int)typeof(T).GetProperty("Id").GetValue(i)) + 1;
    }

    public void Add(T item)
    {
        typeof(T).GetProperty("Id")?.SetValue(item, idCounter++);
        items.Add(item);
        _storage.Save(items);
        Console.WriteLine("Added successfully.");
    }
    
    public void Remove(int id)
    {
        var item = items.FirstOrDefault(i => (int)typeof(T).GetProperty("Id").GetValue(i) == id);
        if (item != null)
        {
            items.Remove(item);
            _storage.Save(items);
            Console.WriteLine("Removed successfully.");
        }
        else Console.WriteLine("Item not found.");
    }
    
    public void List()
    {
        if (!items.Any()) { Console.WriteLine("No items available."); return; }
        foreach (var item in items)
        {
            foreach (var prop in item.GetType().GetProperties())
                Console.Write($"{prop.Name}: {prop.GetValue(item)} \t");
            Console.WriteLine();
        }
    }
    
    public abstract void Menu();
}

class DoctorService : BaseService<Doctor>
{
    public DoctorService(IDataStorage<Doctor> storage) : base(storage) {}
    public override void Menu()
    {
        while (true)
        {
            Console.WriteLine("1. Add Doctor\n2. Remove Doctor\n3. List Doctors\n4. Back");
            var input = Console.ReadLine();
            if (input == "4") break;
            if (input == "1") { Console.Write("Enter name: "); Add(new Doctor { Name = Console.ReadLine() }); }
            else if (input == "2") { Console.Write("Enter ID: "); Remove(int.Parse(Console.ReadLine())); }
            else if (input == "3") { List(); }
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Choose storage type: 1. JSON 2. XML");
        var choice = Console.ReadLine();
        IDataStorage<Doctor> doctorStorage = choice == "2" ? new XmlDataStorage<Doctor>("doctors.xml") : new JsonDataStorage<Doctor>("doctors.json");
        var doctorService = new DoctorService(doctorStorage);
        
        while (true)
        {
            Console.WriteLine("1. Manage Doctors\n2. Exit");
            if (Console.ReadLine() == "1") doctorService.Menu();
            else break;
        }
    }
}
