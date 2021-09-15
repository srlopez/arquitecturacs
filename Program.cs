/*
PROGRAMACION CON ARQUITECTURA 
CON REPOSITORIO

Sistema
Vista
Controlador
Repositorio

*/

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Empezamos");

        var repositorio = new RepositorioCSV();
        var sistema = new Sistema(repositorio);
        var vista = new Vista();
        var controlador = new Controlador(sistema, vista);
        controlador.Run();
        Console.WriteLine("Fin");
    }
}
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

    public void mostrarObjetos<T>(string titulo, List<T> opciones)
    {
        Console.WriteLine($"   > {titulo}");
        Console.WriteLine();
        for (int i = 0; i < opciones.Count; i++)
        {
            Console.WriteLine($"   {i + 1:##}.- {opciones[i].ToString()}");
        }
        Console.WriteLine();
    }
    public int obtenerOpcion<T>(string titulo, List<T> datos, string prompt)
    {
        mostrarObjetos(titulo, datos);
        return obtenerEntero(prompt);
    }
}

public class Controlador
{
    List<String> menu = new List<String>{
       "Obtener la media de las notas",
       "Obtener la mejor nota",
       "Informe A",
       "Informe B"
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
            var opcion = vista.obtenerOpcion<String>("Menu de Opciones", menu, "Seleciona una opción");
            switch (opcion)
            {
                case 1:
                    obtenerLaMedia();
                    break;
                case 2:
                    Console.WriteLine($"No implementado");
                    break;
                case 3:
                    InformeA();
                    break;
                case 4:
                    InformeB();
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

    public void InformeA()
    {
        vista.mostrarObjetos("inforem A", sistema.Notas);
    }

        delegate bool Predicate<in T>(T obj);
    private static bool AprobadosH(Calificacion c) =>  c.Sexo=="H" && c.Nota>=5;
    private static bool Suspensos(Calificacion c) => c.Nota<5;
   
    public void InformeB()
    {
        vista.mostrarObjetos("informe B", sistema.Notas);
    }

    private void InformeGenerico(string titulo, List<Calificacion> lista){

    }

/*
public delegate bool Predicate<in T>(T obj);

*/

}

public class Calificacion
{
    public string Nombre;
    public string Sexo;
    public decimal Nota;

    // public Calificacion(string nombre, decimal nota)
    // {
    //     Nombre = nombre;
    //     Nota = nota;
    // }

    public override string ToString() => $"({Nombre}, {Nota})";

    internal static Calificacion ParseRow(string row)
    {
        //Console.WriteLine(row);
        var columns = row.Split(',');
        return new Calificacion()
        {
            Sexo = columns[0],
            Nombre = columns[1],
            Nota = decimal.Parse(columns[1])
        };

    }
}

public class Sistema
{
    IRepositorio Repositorio;

    public List<Calificacion> Notas;

    public Sistema(IRepositorio repositorio)
    {
        Repositorio = repositorio;
        Repositorio.Inicializar();
    }

    private decimal CalculoDeLaSuma(decimal[] datos) => datos.Sum();
    public decimal CalculoDeLaMedia()
    {

        Notas = Repositorio.CargarCalificaciones();

        var notas = Notas.Select(calificacion => calificacion.Nota).ToArray();
        return CalculoDeLaSuma(notas) / Notas.Count;
    }
}

public interface IRepositorio
{
    void Inicializar();
    List<Calificacion> CargarCalificaciones();

}

public class RepositorioCSV : IRepositorio
{
    string datafile;
    void IRepositorio.Inicializar()
    {
        this.datafile = "notas.csv";
    }
    List<Calificacion> IRepositorio.CargarCalificaciones()
    {
        return File.ReadAllLines(datafile)
            .Skip(1)
            .Where(row => row.Length > 0)
            .Select(Calificacion.ParseRow).ToList();
    }


}