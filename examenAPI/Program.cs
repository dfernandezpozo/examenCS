using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    static HttpClient client = new HttpClient();
    static List<Digimon> savedDigimons = new List<Digimon>();

    static async Task<JsonDocument?> GetCharacterAsync(string name)
    {
        try
        {
            var response = await client.GetAsync($"https://digi-api.com/api/v1/digimon/{name.ToLower()}");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content);
        }
        catch
        {
            return null;
        }
    }

    static async Task Main(string[] args)
    {
        int opcion = 0;
        while (opcion != 3)
        {
            Console.WriteLine("\n=== Menú de digimon ===");
            Console.WriteLine("1. Buscar digimon por nombre");
            Console.WriteLine("2. Listar digmon guardados");
            Console.WriteLine("3. Salir");

            Console.WriteLine("Ingrese una opción:");
            string opcionInput = Console.ReadLine()!;
            if (!int.TryParse(opcionInput, out opcion))
            {
                Console.WriteLine("Opción no válida. Por favor ingrese un número.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    Console.WriteLine("Dime el nombre del digimon que deseas buscar:");
                    string name = Console.ReadLine()!;

                    var digimon = await GetCharacterAsync(name);

                    if (digimon != null)
                    {
                        var root = digimon.RootElement;
                        var first = root.GetProperty("levels")[0];
                        var second = root.GetProperty("types")[0];

                        Console.WriteLine("\nNombre:" + name);
                        Console.WriteLine($"Nivel: {first.GetProperty("level")}");
                        Console.WriteLine($"Tipo: {second.GetProperty("type")}");


                        
                        Console.WriteLine("Deseas guardar el digimon en la lista? (s/n)");
                        string respuesta = Console.ReadLine()!;
                        if (respuesta.ToLower() == "s")
                        {
                            var digimonObj = new Digimon
                            {
                                Name= name,
                                Level= first.GetProperty("level").GetString()!,
                                Type= second.GetProperty("type").GetString()!,
                                
                            };

                            savedDigimons.Add(digimonObj);
                            Console.WriteLine("Digimon guardado en la lista.");
                        }
                        else
                        {
                            Console.WriteLine("No se guardó el digimon");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Digimon no encontrado");
                    }
                    break;

                case 2:
                    if (savedDigimons.Count == 0)
                    {
                        Console.WriteLine("No hay digimon guardados.");
                    }
                    else
                    {
                        Console.WriteLine("\n=== Mis personajes ===");
                        foreach (var c in savedDigimons)
                        {      
                            Console.WriteLine($"1. {c.Name} ({c.Level}) - {c.Type}");
                        }
                    }
                    break;

                case 3:
                    Console.WriteLine("Saliendo...");
                    break;

                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }
}

class Digimon
{
    public string Name { get; set; } = "";
    public string Level {get;set;}="";

    public string Type {get;set;}="";
}