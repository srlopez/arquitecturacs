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

namespace Aplicacion
{
    using Services;
    using Negocio;
    using Ui.Console;
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Componemos la aplición");

            var repositorio = new RepositorioCSV();
            var sistema = new Sistema(repositorio);
            var vista = new Vista();
            var controlador = new Controlador(sistema, vista);
            controlador.Run();

            Console.WriteLine("Fin");
        }
    }

    namespace Ui.Console
    {
        using System;
        using Negocio.Modelos;

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
                    catch (InvalidOperationException)
                    {
                        throw;
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
       "Informe Suspensos",
       "Informe Aprobados H",
       "Informe Todos",
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
                        case 3:
                            InformeSuspensos();
                            break;
                        case 4:
                            InformeAprobadosH();
                            break;
                        case 5:
                            InformeTodos();
                            break;

                        case int.MinValue:
                            // Salimos 
                            return;
                    }
                    Console.WriteLine("Pulsa Return para continuar");
                    Console.ReadLine();
                }
            }


            // CASOS DE USO
            public void obtenerLaMedia() =>
                Console.WriteLine($"La media de la notas es: {sistema.CalculoDeLaMedia():0.00}");

            public delegate bool MiPredicado<in T>(T obj);
            // https://zetcode.com/csharp/predicate/
            private static bool AprobadosH(Calificacion c) => c.Sexo == "H" && c.Nota >= 5;
            private static bool Suspensos(Calificacion c) => c.Nota < 5;
            public void InformeSuspensos() =>
                InformeGenerico("Informe Aprobados Hombres", sistema.LasNotas(), Suspensos);

            public void InformeAprobadosH() =>
                InformeGenerico("Informe Aprobados Hombres", sistema.LasNotas(), AprobadosH);

            public void InformeTodos() =>
                vista.mostrarObjetos("Informe Suspensos", sistema.LasNotas());

            private void InformeGenerico(string titulo, List<Calificacion> lista, MiPredicado<Calificacion> esValido)
            {
                List<Calificacion> selecion = new List<Calificacion>();
                foreach (Calificacion cal in lista)
                {
                    if (esValido(cal)) selecion.Add(cal);
                };
                vista.mostrarObjetos(titulo, selecion);
            }
        }
    }
    namespace Negocio
    {
        namespace Modelos
        {
            public class Calificacion
            {
                public string Nombre;
                public string Sexo;
                public decimal Nota;

                // public Calificacion(string sexo, string nombre, decimal nota)
                // {
                //     Nombre = nombre;
                //     Sexo = sexo;
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
                        Nota = decimal.Parse(columns[2])
                    };

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