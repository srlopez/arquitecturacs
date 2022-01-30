#define IoC // Directiva para compilar utilizando el Contenedor de Dependencias
            // Cambiandola por NoIoC onvertimos el programa en una tiípica arquitectura de Tres Capas

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using static System.Console;

namespace Aplicacion
{
    using Servicios;
    using Negocio;
    using UI.Console;
    using Core;
    /*
     Entrypoint
    */
    class Program
    {
        static void Main(string[] args)
        {
#if IoC
            // Utilizando el IoC, como arquitectura de servicios
            IoC.Register<IRepositorio, RepositorioJSON>();
            IoC.Register<Sistema>();
            IoC.Register<Vista>();
            IoC.Register<Controlador>();
            var controlador = IoC.Create<Controlador>();
#else
            // Arquitectura en Tres Capas
            var repositorio = new RepositorioCSV();
            var sistema = new Sistema(repositorio);
            var vista = new Vista();
            var controlador = new Controlador(sistema, vista);
#endif
            // Arrancamos la aplicación
            controlador.Run();
        }
    }

    /*
    Capa de Presentación/UserInterface
    Separamos la Vista del Controlador
    */
    namespace UI.Console

    {
        using Negocio.Modelos;

        public class Vista
        {
            public void LimpiarPantalla() => Clear();
            public void MostrarLineaCR(Object msg)
            {
                Write(msg.ToString());
                ReadKey();
            }
            public void MostrarLinea(Object msg) => WriteLine(msg.ToString());
            // c# Generics
            public T ObtenerInput<T>(string peticion)
            {
                while (true)
                {
                    Write($"   {peticion.Trim()}: ");
                    var input = ReadLine();
                    // Generamos una Excepción
                    if (input.ToLower().Trim() == "fin") throw new Exception("Entrada cancelada por el usuario");
                    try
                    {
                        // c# Reflexion
                        var valor = TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
                        return (T)valor;
                    }
                    // Captura y control de la excepción si falla el conversor
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message}");
                    }
                }
            }
            public void MostrarLista<T>(string titulo, List<T> opciones)
            {
                WriteLine($"   [ {titulo} ]");
                WriteLine();
                for (int i = 0; i < opciones.Count; i++)
                {
                    WriteLine($"   {i + 1:##}.- {opciones[i].ToString()}");
                }
                WriteLine();
            }
            public T ObtenerOpcion<T>(string titulo, List<T> datos, string prompt)
            {
                MostrarLista(titulo, datos);
                int input = 0;
                while (input < 1 || input > datos.Count)
                    try
                    {
                        input = ObtenerInput<int>(prompt);
                    }
                    catch { throw; }; //Relanzamos la excepción
                return datos.ElementAt(input - 1);
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
                // c# Dictionary Colección generica
                this.casosDeUso = new Dictionary<string, Action>(){
                    { "Añadir Calificación", AñadirCalificacion },
                    { "Obtener la media de las notas", obtenerLaMedia },
                    { "Obtener la mejor nota",()=>vista.MostrarLinea($"Caso de uso no implementado") },
                    { "Informe Suspensos", InformeSuspensos },
                    { "Informe Aprobados por Sexo", InformeAprobadosXSexo },
                    { "Informe Todos", InformeTodos },
                    { "Informe por Alumno", InformesXAlumno },
                    { "Porcentaje por Sexo de Alumnos", PorcentajeXSexo}
                };
            }
            // CICLO DE APLICACIÓN
            public void Run()
            {
                vista.LimpiarPantalla();
                // Acceso a las Claves del diccionario
                var menu = casosDeUso.Keys.ToList<String>();
                while (true)
                    try
                    {
                        var opcion = vista.ObtenerOpcion("Gestor de Notas", menu, "Seleciona una opción");
                        casosDeUso[opcion].Invoke();
                        vista.MostrarLineaCR("Pulsa Return para continuar");
                    }
                    catch { break; }
                sistema.CerrarSistema();
            }
            // CASOS DE USO
            public void obtenerLaMedia() =>
                 vista.MostrarLinea($"La media de la notas es: {sistema.CalculoDeLaMedia():0.00}");
            private static bool AprobadosXSexo(Calificacion c, string sexo) => c.Sexo == sexo && c.Nota >= 5;
            private static bool Suspensos(Calificacion c) => c.Nota < 5;
            public void InformeSuspensos() =>
                InformeGenerico("Informe Suspensos", sistema.Notas, Suspensos);
            public void InformeTodos() =>
                // C# Lambda como parametros: programacion funcional
                InformeGenerico($"Informe general", sistema.Notas, (Calificacion cal) => true);
            public void InformeAprobadosXSexo()
            {
                var sexo = vista.ObtenerInput<String>("Sexo");
                // C# Lambda reconstruido
                InformeGenerico($"Informe Aprobados {sexo}", sistema.Notas, (Calificacion cal) => AprobadosXSexo(cal, sexo));
            }
            private void InformeGenerico(string titulo, List<Calificacion> lista, Func<Calificacion, bool> esValido)
            {
                List<Calificacion> calSeleccionadas = new List<Calificacion>();
                foreach (Calificacion cal in lista)
                {
                    if (esValido.Invoke(cal)) calSeleccionadas.Add(cal);
                };
                vista.MostrarLista(titulo, calSeleccionadas);
                // Un poco más funcional, en una linea sería
                // vista.MostrarObjetos(titulo, lista.Where(esValido).ToList());
            }
            private void InformesXAlumno()
            {
                while (true) try
                    {
                        var nombre = vista.ObtenerInput<String>("Nombre del alumno");
                        // Lambda
                        var calificación = sistema.Notas.FirstOrDefault(c => c.Nombre == nombre);
                        // pattern matching
                        var msg = calificación switch
                        {
                            null => "Alumno no encontrado",
                            _ => calificación.ToString()
                        };
                        vista.MostrarLinea(msg);
                    }
                    catch (Exception e)
                    {
                        vista.MostrarLinea(e.Message);
                        return;
                    }
            }
            private void AñadirCalificacion()
            {
                try
                {
                    var nombre = vista.ObtenerInput<String>("Nombre del alumno");
                    var nota = vista.ObtenerInput<Decimal>("Nota");
                    var sexo = vista.ObtenerInput<String>("Sexo");

                    sistema.AñadirNota(new Calificacion
                    {
                        Nombre = nombre,
                        Nota = nota,
                        Sexo = sexo
                    });
                }
                catch (Exception e)
                {
                    vista.MostrarLinea(e.Message);
                    return;
                }
            }
            private void PorcentajeXSexo()
            {
                var h = sistema.PorcentajeXSexo("H");
                var m = sistema.PorcentajeXSexo("M");
                vista.MostrarLinea($"H:  {h}");
                vista.MostrarLinea($"M:  {m}");
            }
            /*  
            Con predicado   
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
    /*
    Capa de Negocio/Bussness Logic
    Cubre la lógica principal de la aplicación.
    */
    namespace Negocio
    {
        using Modelos;
        namespace Modelos
        {
            public class Calificacion
            {
                public string Nombre { get; set; }
                public string Sexo { get; set; }
                public decimal Nota { get; set; }

                public override string ToString() => $"({Nombre}, {Nota})";
            }
        }
        public class Sistema
        {
            IRepositorio Repositorio;

            //Caché
            public List<Calificacion> Notas { get; }
            public Sistema(IRepositorio repositorio)
            {
                Repositorio = repositorio;
                Repositorio.Inicializar();
                Notas = Repositorio.CargarCalificaciones();
            }
            public void CerrarSistema()
            {
                Repositorio.GuardarCalificaciones(Notas);
            }
            public decimal CalculoDeLaMedia()
            {
                var aNotas = Notas.Select(calificacion => calificacion.Nota).ToArray();
                return CalculoDeLaSuma(aNotas) / Notas.Count;
                // función interna
                decimal CalculoDeLaSuma(decimal[] datos) => datos.Sum();
            }
            public string PorcentajeXSexo(string sexo)
            {
                double cuenta = Notas.Where(calificacion => calificacion.Sexo == sexo).Count();
                var total = Notas.Count();
                return $"{cuenta}/{total}  {cuenta * 100 / total:#.00}%";
            }
            public void AñadirNota(Calificacion cal) =>
                Notas.Add(cal);
        }
    }
    /*
    Capa de Datos
    Contiene los servicios de acceso a datos
    */
    namespace Servicios
    {
        using Negocio.Modelos;

        public interface IRepositorio
        {
            void Inicializar();
            List<Calificacion> CargarCalificaciones();
            void GuardarCalificaciones(List<Calificacion> data);

        }
        public class RepositorioCSV : IRepositorio
        {
            string datafile;
            void IRepositorio.Inicializar()
            {
                datafile = "notas.csv";
            }
            List<Calificacion> IRepositorio.CargarCalificaciones()
            {
                return File.ReadAllLines(datafile)
                    .Skip(1)
                    .Where(row => row.Length > 0)
                    .Select(ParseRow).ToList();
                // DTO=>Modelo
                Calificacion ParseRow(string row)
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

                    var columns = row.Split(',');
                    return new Calificacion
                    {
                        Sexo = columns[0],
                        Nombre = columns[1],
                        Nota = decimal.Parse(columns[2])
                    };
                }
            }
            void IRepositorio.GuardarCalificaciones(List<Calificacion> data)
            {
                var lines = new List<string> { "SX,NOMBRE,NOTA" };
                lines.AddRange(data.Select(ToDTO));
                File.WriteAllLines(datafile, lines);
                // Modelo=>DTO
                string ToDTO(Calificacion item) => $"{item.Sexo},{item.Nombre},{item.Nota}";
            }

        }
        public class RepositorioJSON : IRepositorio
        {
            string datafile;
            void IRepositorio.Inicializar()
            {
                datafile = "notas.json";
            }
            List<Calificacion> IRepositorio.CargarCalificaciones()
            {
                var txtJson = File.ReadAllText(datafile);
                return JsonSerializer.Deserialize<List<Calificacion>>(txtJson);
            }
            void IRepositorio.GuardarCalificaciones(List<Calificacion> data)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(datafile, json);
            }
        }
    }
    /*
    Core. Utilidades típicas de una aplicación
    Logger, etc..
    IoC
    */
    namespace Core
    {
        // Static para referenciarlo desde todos los componentes
        public static class IoC
        {
            private static readonly Dictionary<Type, (Type, Object)> services =
                new Dictionary<Type, (Type, Object)>();

            static IoC() { }

            #region Register
            public static void Register<TImplementation>() =>
                Register<TImplementation, TImplementation>();
            public static void Register<TInterface, TImplementation>() where TImplementation : TInterface =>
                services[typeof(TInterface)] = (typeof(TImplementation), null);
            #endregion
            #region Create
            public static TInterface Create<TInterface>(Object[] parameters = null) =>
                    (TInterface)Create(typeof(TInterface), parameters);
            public static object Create(Type type, Object[] concreteParams)
            {
                // Verificamos si la instancia ya está creada
                var (concreteType, concreteInstance) = services[type];
                if (concreteInstance is not null) return concreteInstance;

                // Obtenemos el primer constructor
                var defaultConstructor = concreteType.GetConstructors()[0];
                // Obtenemos los parámetros
                var defaultParams = defaultConstructor.GetParameters();
                // Instanciamos los parámetros con recursión
                // Los parámetros de los servicios sólo pueden ser otros servicios
                var parameters = concreteParams ?? defaultParams.Select(param => Create(param.ParameterType, null)).ToArray();
                // Construimos la instancia
                concreteInstance = defaultConstructor.Invoke(parameters);
                // Actualizamos el registro
                services[type] = (concreteType, concreteInstance);

                // Devolvemos la instancia
                return concreteInstance;
            }
            #endregion
            // public static string ToString()
            // {
            //     var sb = new StringBuilder();
            //     foreach (var (key, (type, instance)) in services)
            //         sb.Append($"{l(key)}: {l(type)} {instance! is null}\n");
            //     string l(object s) => s.ToString().Split(".").Last();
            //     return sb.ToString();
            // }
        }

    }
}