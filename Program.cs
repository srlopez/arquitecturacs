/*
PROGRAMACION CON ARQUITECTURA 

Sistema
Vista
Controlador

*/

using System;
using System.Linq;

Console.WriteLine("Empezamos");

Calificacion[] notas = new[] {
        //Luis, Marta, Marcos, Aroa, Nerea, Kike, Juan
        //7.5M, 4,     6,      5,    4,     6.5M, 7.5M 
        new Calificacion("Luis", 7.5M),
        new Calificacion("Marta", 4),
        new Calificacion("Marcos", 6),
        new Calificacion("Aroa", 5),
        new Calificacion("Nerea", 4),
        new Calificacion("Kike", 6.5M),
        new Calificacion("Juan", 7.5M)
    };

var sistema = new Sistema(notas);
var vista = new Vista();
var controlador = new Controlador(sistema, vista);
controlador.Run();
Console.WriteLine("Fin");

public class Vista
{

    public int obtenerEntero(string prompt)
    {
        int entero = int.MinValue;
        string input = "";
        bool entradaIncorrecta = true;
        while (entradaIncorrecta)
        {
            try
            {
                Console.Write($"   {prompt.Trim()}: ");
                input = Console.ReadLine();
                if (input != "fin")
                {
                    entero = int.Parse(input);
                    entradaIncorrecta = false;
                }
                else
                {
                    entero = int.MinValue;
                    entradaIncorrecta = false;
                }
            }
            catch (FormatException)
            {
                ;
            }
        }
        return entero;
    }
    public int obtenerOpcion(string titulo, Object[] opciones, string prompt)
    {
        Console.WriteLine($"   === {titulo} ===");
        Console.WriteLine();
        for (int i = 0; i < opciones.Length; i++)
        {
            Console.WriteLine($"   {i + 1:##}.- {opciones[i]}");
        }
        Console.WriteLine();
        return obtenerEntero(prompt);
    }
}

public class Controlador
{
    string[] menu = new[]{
       "Obtener la media de las notas",
       "Obtener la mejor nota"
       };

    private Sistema sistema;
    private Vista vista;

    public Controlador(Sistema sistema, Vista vista)
    {
        this.sistema = sistema;
        this.vista = vista;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            var opcion = vista.obtenerOpcion("Menu de Opciones", menu, "Seleciona una opción");
            switch (opcion)
            {
                case 1:
                    obtenerLaMedia();
                    break;
                case 2:
                    Console.WriteLine($"No implementado");
                    break;
                case int.MinValue:
                    // Salimos 
                    return;
            }
            Console.WriteLine("\n\nPulsa Return para continuar");
            Console.ReadLine();
        }
    }

    public void obtenerLaMedia()
    {
        Console.WriteLine($"La media de la notas es: {sistema.CalculoDeLaMedia():0.00}");
    }


}

public class Calificacion
{
    public string Nombre;
    public decimal Nota;
    public Calificacion(string nombre, decimal nota)
    {
        Nombre = nombre;
        Nota = nota;
    }
    public override string ToString() => $"({Nombre}, {Nota})";
}

public class Sistema
{
    Calificacion[] Notas;

    public Sistema(Calificacion[] notas)
    {
        Notas = notas;
    }

    private decimal CalculoDeLaSuma(decimal[] datos) => datos.Sum();
    public decimal CalculoDeLaMedia()
    {
        var notas = Notas.Select(calificacion => calificacion.Nota).ToArray();
        return CalculoDeLaSuma(notas) / Notas.Length;
    }
}

