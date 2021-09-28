/*
PROGRAMACION CON ARQUITECTURA 
CON REPOSITORIO E INYECCION DE PREDICADO
https://zetcode.com/csharp/predicate/

Clases con namespaces
Aplicacion.Negocio.Sistema
Aplicacion.Negocio.Modelos.Calificacion
Aplicacion.UI.Console.Vista
Aplicacion.UI.Console.Controlador
Aplicacion.Services.Repositorio
*/

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Aplicacion
{
    using Services;
    using Negocio;
    using UI.Console;
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("> Componemos la aplición");
            var repositorio = new RepositorioCSV();
            var sistema = new Sistema(repositorio);
            var vista = new Vista();
            var controlador = new Controlador(sistema, vista);

            Console.WriteLine("> Lanzamos la aplición");
            controlador.Run();

            Console.WriteLine("> Aplicación finalizada");
        }
    }

    namespace UI.Console
    {
        using System;
        using Negocio.Modelos;

        public class Vista
        {
            public void LimpiarPantalla() => Console.Clear();
            public void MuestraLineCR(Object msg)
            {
                Console.WriteLine(msg.ToString());
                Console.ReadLine();
            }
            public void MuestraLine(Object msg) => Console.WriteLine(msg.ToString());

            // c# Generics
            public T ObtenerInput<T>(string prompt)
            {
                while (true)
                {
                    Console.Write($"   {prompt.Trim()}: ");
                    var input = Console.ReadLine();
                    if (input.ToLower().Trim() == "fin") input = int.MinValue.ToString();
                    try
                    {
                        // c# Reflexion
                        var valor = TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
                        return (T)valor;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
            }
            public void MostrarObjetos<T>(string titulo, List<T> opciones)
            {
                Console.WriteLine($"   [ {titulo} ]");
                Console.WriteLine();
                for (int i = 0; i < opciones.Count; i++)
                {
                    Console.WriteLine($"   {i + 1:##}.- {opciones[i].ToString()}");
                }
                Console.WriteLine();
            }
            public (int, T) ObtenerOpcion<T>(string titulo, List<T> datos, string prompt)
            {
                MostrarObjetos(titulo, datos);
                int input = int.MaxValue;
                while (input < 1 || input > datos.Count)
                {
                    input = ObtenerInput<int>(prompt);
                    if (input == int.MinValue) break;
                };
                // c# pattern matching de programación funcional
                return input switch
                {
                    int.MinValue => (input, default(T)),
                    _ => (input - 1, datos.ElementAt(input - 1))
                };
            }
        }
        public class Controlador
        {
            private Sistema sistema;
            private Vista vista;
            private Dictionary<string, Action> casosDeUso;

            public Controlador(Sistema sistema, Vista vista)
            {
                this.sistema = sistema;
                this.vista = vista;
                // c# Action vs Func: programación funcional
                // c# Dictionary Colecion generica
                this.casosDeUso = new Dictionary<string, Action>(){
                    { "Obtener la media de las notas", obtenerLaMedia },
                    { "Obtener la mejor nota",()=>vista.MuestraLine($"Caso de uso no implementado") },
                    { "Informe Suspensos", InformeSuspensos },
                    { "Informe Aprobados H", InformeAprobadosH },
                    { "Informe Todos", InformeTodos },
                    { "Pruebas genericas de inut", Pruebas },
                };
            }

            public void Run()
            {
                vista.LimpiarPantalla();
                // Acceso a las Claves dels diccionario
                var menu = casosDeUso.Keys.ToList<String>();
                while (true)
                {
                    // c# Deconstrucción de tuplas para obtener una opcion
                    (var opcion, var key) = vista.ObtenerOpcion("Menu de Usuario", menu, "Seleciona una opción");
                    if (opcion == int.MinValue) return;
                    // Ejecución de la opción escogida
                    casosDeUso[key].Invoke();
                    vista.MuestraLineCR("Pulsa Return para continuar");
                }
            }

            // CASOS DE USO
            public void obtenerLaMedia() =>
                 vista.MuestraLine($"La media de la notas es: {sistema.CalculoDeLaMedia():0.00}");

            private static bool AprobadosH(Calificacion c) => c.Sexo == "H" && c.Nota >= 5;
            private static bool Suspensos(Calificacion c) => c.Nota < 5;
            public void InformeSuspensos() =>
                //InformeGenerico("Informe Aprobados Hombres", sistema.LasNotas(), Suspensos);
                // C# Lambda como parametros: programacion funcional
                InformeGenerico("Informe Aprobados Hombres", sistema.LasNotas(), (Calificacion c) => c.Nota < 5);

            public void InformeAprobadosH() =>
                InformeGenerico("Informe Aprobados Hombres", sistema.LasNotas(), AprobadosH);
            public void InformeTodos() =>
                vista.MostrarObjetos("Informe Suspensos", sistema.LasNotas());
            private void InformeGenerico(string titulo, List<Calificacion> lista, Func<Calificacion, bool> esValido)
            {
                List<Calificacion> calSeleccionadas = new List<Calificacion>();
                foreach (Calificacion cal in lista)
                {
                    if (esValido.Invoke(cal)) calSeleccionadas.Add(cal);
                };
                vista.MostrarObjetos(titulo, calSeleccionadas);
            }

            public void Pruebas()
            {
                var s = vista.ObtenerInput<string>("un string");
                Console.WriteLine($"{s} {s == int.MinValue.ToString()}");
                var d = vista.ObtenerInput<decimal>("un decimal");
                Console.WriteLine($"{d} {d == int.MinValue}");
                var f = vista.ObtenerInput<float>("un float");
                Console.WriteLine($"{f} {f == int.MinValue}");
                var i = vista.ObtenerInput<int>("un int");
                Console.WriteLine($"{i} {i == int.MinValue}");
            }

            /*     
            // https://zetcode.com/csharp/predicate/
            // https://stackoverflow.com/questions/8099631/how-to-return-value-from-action

            private delegate bool MiPredicado<in T>(T obj);

            private void InformeGenericoD(string titulo, List<Calificacion> lista, MiPredicado<Calificacion> esValido)
            {
                List<Calificacion> calSeleccionadas = new List<Calificacion>();
                foreach (Calificacion cal in lista)
                {
                    if (esValido(cal)) calSeleccionadas.Add(cal);
                };
                vista.MostrarObjetos(titulo, calSeleccionadas);
            }
            */

        }
    }
    namespace Negocio
    {
        namespace Modelos
        {
            public class Calificacion
            {
                public string Nombre { get; set; }
                public string Sexo { get; set; }
                public decimal Nota { get; set; }
                public Calificacion(string sexo, string nombre, decimal nota)
                {
                    Nombre = nombre;
                    Sexo = sexo;
                    Nota = nota;
                }

                public override string ToString() => $"({Nombre}, {Nota})";
                internal static Calificacion ParseRow(string row)
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

                    var columns = row.Split(',');
                    // return new Calificacion
                    // {
                    //     Sexo = columns[0],
                    //     Nombre = columns[1],
                    //     Nota = decimal.Parse(columns[2])
                    // };
                    return new Calificacion(
                        nombre: columns[1],
                        sexo: columns[0],
                        nota: decimal.Parse(columns[2], nfi)
                    );

                }
            }
        }
        public class Sistema
        {
            IRepositorio Repositorio;

            //List<Calificacion> Notas;

            public Sistema(IRepositorio repositorio)
            {
                Repositorio = repositorio;
                Repositorio.Inicializar();
            }

            private decimal CalculoDeLaSuma(decimal[] datos) => datos.Sum();
            public decimal CalculoDeLaMedia()
            {
                var Notas = Repositorio.CargarCalificaciones();
                var aNotas = Notas.Select(calificacion => calificacion.Nota).ToArray();
                return CalculoDeLaSuma(aNotas) / Notas.Count;
            }

            public List<Modelos.Calificacion> LasNotas() =>
                 Repositorio.CargarCalificaciones();
        }

    }
    namespace Services
    {
        using Negocio.Modelos;

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
    }
}