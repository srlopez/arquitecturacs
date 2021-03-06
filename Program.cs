#define CIDx
// CID Directiva para compilar utilizando nuestro Contenedor de Inyección de Dependencias
// DI para compilar con el Contenedor de Servicios de Microsoft
// {OTROVALOR} y el programa es una típica arquitectura de Tres Capas
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using static System.Console;

namespace Aplicacion
{
    using Data;
    using Negocio;
    using UI.Console;
    using Core;
    /*
     Entrypoint
     Raiz de la composición de servicios/objetos.
    */
    class Program
    {
        static public string DataPath = "data/";
        static void Main(string[] args)
        {
#if DI
            // DI Utilizando ServiceCollection como contenedor de servicios
            var services = new ServiceCollection();
            services.AddSingleton<IRepositorio, RepoCalificacionSQLite>();
            services.AddSingleton<Sistema>();
            services.AddSingleton<Vista>();
            services.AddSingleton<Controlador>();
            var _serviceProvider = services.BuildServiceProvider(true);
            IServiceScope scope = _serviceProvider.CreateScope();
            var controlador = scope.ServiceProvider.GetRequiredService<Controlador>();
#elif CID
            // Utilizando Core.CID como contenedor de servicios
            CID.Register<IRepositorio, RepoCalificacionJSON>();
            CID.Register<Sistema>();
            CID.Register<Vista>();
            CID.Register<Controlador>();
            var controlador = CID.Create<Controlador>();
#else
            // Arquitectura en Tres Capas
            var repositorio = new RepoCalificacionCSV();
            var sistema = new Sistema(repositorio);
            var vista = new Vista();
            var controlador = new Controlador(sistema, vista);
#endif
            // Arrancamos la aplicación
            // Independientemente de cómo la hayamos compuesto.
            controlador.Run();
            // Y finaliza el ciclo

#if DI
            _serviceProvider.Dispose();
#endif
        }
    }

    /*
    Capa de Presentación/UserInterface
    Separamos la Vista y el Controlador de la Capa de Negocio
    */
    namespace UI.Console

    {
        using Negocio.Modelos;

        public class Vista
        {
            public string cancelInput = "fin";
            public void LimpiarPantalla() => Clear();
            public void MostrarLineaCR(Object msg)
            {
                Write(msg);
                ReadKey();
            }
            public void MostrarLinea(Object msg) => WriteLine(msg);
            // c# Generics
            public T ObtenerInput<T>(string prompt)
            {
                while (true)
                {
                    Write($"   {prompt.Trim()}: ");
                    var input = ReadLine();
                    // Generamos una Excepción
                    if (input.ToLower().Trim() == cancelInput)
                        throw new Exception("Entrada cancelada por el usuario");
                    try
                    {
                        // c# Reflexion + Librería
                        var valor = System.ComponentModel.TypeDescriptor
                                    .GetConverter(typeof(T))
                                    .ConvertFromString(input);
                        return (T)valor;
                    }
                    // Captura y control de la excepción si falla el conversor
                    catch (Exception e)
                    {
                        WriteLine($"Error: {e.Message}");
                    }
                }
            }
            public void MostrarLista<T>(string titulo, List<T> datos)
            {
                WriteLine($"   [ {titulo} ]");
                WriteLine();
                var i = 0;
                datos.ForEach(item => WriteLine($"   {++i:##}.- {item}"));
                WriteLine();
            }
            public T ObtenerElemento<T>(string titulo, List<T> datos, string prompt)
            {
                MostrarLista(titulo, datos);
                int input = 0;
                while (input < 1 || input > datos.Count)
                    try
                    {
                        input = ObtenerInput<int>(prompt);
                    }
                    catch { throw; }; //Relanzamos la excepción cancelInput
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
                // c# Dictionary Colección genérica
                this.casosDeUso = new Dictionary<string, Action>(){
                    { "Añadir Calificación", AñadirCalificacion },
                    { "Obtener la media de las notas", obtenerLaMedia },
                    { "Obtener la mejor nota",()=>vista.MostrarLinea($"Caso de uso no implementado") },
                    { "Informe Suspensos", InformeSuspensos },
                    { "Informe Aprobados por Sexo", InformeAprobadosXSexo },
                    { "Informe Todos", InformeTodos },
                    { "Informe por Alumno", InformesXAlumno },
                    { "Porcentaje por Sexo de Alumnos", async()=>await  PorcentajeXSexo()}
                    //{ "Porcentaje por Sexo de Alumnos", PorcentajeXSexo}
                };
            }
            // CICLO DE APLICACIÓN
            public void Run()
            {
                vista.LimpiarPantalla();
                vista.MostrarLinea($"Modo persitencia: \"{sistema.RepoInfo}\".");
                vista.MostrarLinea($"Aviso: para finalizar escribir \"{vista.cancelInput}\".\n");
                // Acceso a las Claves del diccionario
                var menu = casosDeUso.Keys.ToList<String>();
                while (true)
                    try
                    {
                        var opcion = vista.ObtenerElemento("Gestor de Notas", menu, "Seleciona una opción");
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
            private async void AñadirCalificacion()
            {
                try
                {
                    var nombre = vista.ObtenerInput<String>("Nombre del alumno");
                    var nota = vista.ObtenerInput<Decimal>("Nota");
                    var sexo = vista.ObtenerInput<String>("Sexo");

                    // Esto evita warnings, y permite que el método sea invocado
                    // asíncronamente. Lo hago por no complicar el ejemplo, pero debería ser 
                    // así await sistema.AñadirNota(). Y mostrar otras posibilidades
                    _ = sistema.AñadirNota(new Calificacion
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
            // private void PorcentajeXSexo() //<- Evitando poner async Task
            // {
            // var h = sistema.PorcentajeXSexo("H").GetAwaiter().GetResult();
            // var m = sistema.PorcentajeXSexo("M").GetAwaiter().GetResult();
            private async Task PorcentajeXSexo()
            {
                var h = await sistema.PorcentajeXSexo("H");
                var m = await sistema.PorcentajeXSexo("M");

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
    Capa de Negocio/Business Logic
    Cubre la lógica principal de la aplicación.
    */
    namespace Negocio
    {
        using Modelos;

        namespace Modelos
        {

            public interface IParserCSV
            // Una interfaz para implementar en cada modelo que sea susceptible de guardarse en CSV
            {
                Calificacion FromCSVRow(string row);
                string ToCSVRow();
                string ToCSVHeader();
            }
            public class Calificacion : IParserCSV
            {
                public string Nombre { get; set; }
                public string Sexo { get; set; }
                public decimal Nota { get; set; }
                // Muy importante. ToString es nuestro ModelView/DTO para vista
                public override string ToString() => $"({Nombre}, {Nota})";

                /* 
                Interfaz de objeto a CSV ------------------------------------------
                Nota1: Esta aproximación es un poco forzada.
                Desde un punto de vista,los modelos deben ser más 'POCO',
                y estos métodos deberían pertenecer al repositrio específico,
                y ser pasados como parámetros a las funciones de la clase abstracta.
                Nota2: Presentamos de esta manera los métodos de forma pedagógica,
                para entender otras formas de diseñar el servicio de repositorio.
                */
                Calificacion IParserCSV.FromCSVRow(string row) => Calificacion.FromCSVRow(row);
                public static Calificacion FromCSVRow(string row)
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
                string IParserCSV.ToCSVHeader() => Calificacion.ToCSVHeader();
                public static string ToCSVHeader() => "SX,NOMBRE,NOTA";
                string IParserCSV.ToCSVRow() => $"{Sexo},{Nombre},{Nota}";

            }
        }
        public class Sistema
        {
            private readonly object bloqueo = new object();

            IRepositorio Repositorio;
            //Caché
            public List<Calificacion> Notas { get; }
            public Sistema(IRepositorio repositorio)
            {
                Repositorio = repositorio;
                Notas = Repositorio.CargarCalificaciones();
            }
            public string RepoInfo { get => $"{Repositorio.GetType().ToString().Split(".").Last()}"; }
            public void CerrarSistema()
            {
                Repositorio.GuardarCalificaciones(Notas);
            }
            public decimal CalculoDeLaMedia()
            {
                //Notas.Average(calificacion => calificacion.Nota);
                //Notas.Select(calificacion => calificacion.Nota).Average();

                var aNotas = Notas.Select(calificacion => calificacion.Nota).ToArray();
                return CalculoDeLaSuma(aNotas) / Notas.Count;
                // ejemplo de función interna a una función
                decimal CalculoDeLaSuma(decimal[] datos) => datos.Sum();
            }
            public async Task AñadirNota(Calificacion cal)
            {
                // Bloqueo (En caso de multiusuario )
                lock (bloqueo)
                {
                    Notas.Add(cal);
                }
            }

            // Ejemplo en modo asincrono. Para trabajar TODOS los métodos
            public async Task<string> PorcentajeXSexo(string sexo)
            {
                double cuenta = Notas.Where(calificacion => calificacion.Sexo == sexo).Count();
                var total = Notas.Count();
                //return Task.FromResult($"{cuenta}/{total}  {cuenta * 100 / total:#.00}%");
                // await Task.Delay(0);
                return $"{cuenta}/{total}  {cuenta * 100 / total:#.00}%";
            }

        }
    }
    /*
    Capa de Almacen de Datos/Repositorio
    Contiene los servicios de acceso a datos CSV, JSON, SQLite
    */
    namespace Data
    {
        using Negocio.Modelos;

        public interface IRepositorio
        {
            List<Calificacion> CargarCalificaciones();
            void GuardarCalificaciones(List<Calificacion> notas);
        }
        public abstract class RepositorioCSV //<T> where T : IParserCSV
        // Repositorio Genérico CSV
        {
            private string _datafile;
            protected RepositorioCSV(string file)
            {
                _datafile = Program.DataPath + file;
            }

            public void Guardar<T>(List<T> data) where T : IParserCSV
            {
                // metodo invocado desde static
                string header = (string)typeof(T).GetMethod("ToCSVHeader").Invoke(null, new object[] { });
                var lines = new List<string> { header };
                lines.AddRange(data.Select(i => i.ToCSVRow()));//<- Funciona desde instancia
                File.WriteAllLines(_datafile, lines);
            }

            public List<T> Cargar<T>() where T : IParserCSV =>
                File.ReadAllLines(_datafile)
                    .Skip(1)
                    .Where(row => row.Length > 0)
                    //.Select(T.FromCSVRow) <- Invocar al método stático desde <T> No funciona
                    .Select(row => (T)typeof(T)
                        .GetMethod("FromCSVRow")// BindingFlags.Public | BindingFlags.Static)
                        .Invoke(null, new object[] { row }))
                    .ToList();
        }
        public class RepoCalificacionCSV : RepositorioCSV, IRepositorio
        {
            private List<Calificacion> _notas;

            public RepoCalificacionCSV() : base("notas.csv")
            {
                _notas = Cargar<Calificacion>();
            }

            List<Calificacion> IRepositorio.CargarCalificaciones() => _notas;
            void IRepositorio.GuardarCalificaciones(List<Calificacion> data) => Guardar<Calificacion>(data);

        }
        public class RepoCalificacionJSON : IRepositorio
        {
            private string datafile;
            private List<Calificacion> _notas;

            public RepoCalificacionJSON()
            {
                datafile = Program.DataPath + "notas.json";
                _notas = CargarCalificaciones();
            }
            List<Calificacion> IRepositorio.CargarCalificaciones() => _notas;
            void IRepositorio.GuardarCalificaciones(List<Calificacion> data) => GuardarCalificaciones(data);

            //Private
            private List<Calificacion> CargarCalificaciones()
            {
                var txtJson = File.ReadAllText(datafile);
                return JsonSerializer.Deserialize<List<Calificacion>>(txtJson);
            }
            private void GuardarCalificaciones(List<Calificacion> data)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(datafile, json);
            }

        }
        public class RepoCalificacionSQLite : IRepositorio
        {
            private string _datafile;
            private string _connString;
            private List<Calificacion> _notas;

            public RepoCalificacionSQLite()
            {
                _datafile = Program.DataPath + "notas.db";
                _connString = "Data Source=" + _datafile + ";";
                if (!File.Exists(_datafile)) CrearTabla();
                _notas = CargarCalificaciones();
            }
            List<Calificacion> IRepositorio.CargarCalificaciones() => _notas;
            void IRepositorio.GuardarCalificaciones(List<Calificacion> data) => GuardarCalificaciones(data);

            // Privados
            private void CrearTabla() => ExecuteCmd(
                "CREATE TABLE IF NOT EXISTS NOTAS (" +
                    "Nombre NVARCHAR(30) PRIMARY KEY, " +
                    "Sexo CHAR(1), " +
                    "Nota REAL" +
                ")");
            private List<Calificacion> CargarCalificaciones() =>
                ExecuteReader("SELECT * FROM NOTAS", ParseCalificacion);
            private void GuardarCalificaciones(List<Calificacion> data)
            {
                // DELETE
                ExecuteCmd("DELETE FROM NOTAS");
                // INSERTAMOS TODO
                var insert = "INSERT INTO NOTAS (SEXO,NOMBRE,NOTA) VALUES ";
                var values = String.Join(",",
                    data.Select(cal => "('" + cal.Sexo + "','" + cal.Nombre + "'," + cal.Nota + ")")
                    );
                insert += values;
                ExecuteCmd(insert);
            }
            // Privadas de SQLLite
            private void ExecuteCmd(string sql)
            {
                using (SqliteConnection db = new SqliteConnection(_connString))
                {
                    db.Open();
                    SqliteCommand cmd = new SqliteCommand(sql, db);
                    cmd.ExecuteNonQuery();
                    db.Close();
                }
            }
            private List<T> ExecuteReader<T>(string sql, Func<SqliteDataReader, T> parseT)
            {
                List<T> data = new();
                using (SqliteConnection db = new SqliteConnection(_connString))
                {
                    db.Open();
                    SqliteCommand cmd = new SqliteCommand(sql, db);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            data.Add(parseT(reader));
                    db.Close();
                }
                return data;
            }
            private Calificacion ParseCalificacion(SqliteDataReader reader) =>
                new Calificacion
                {
                    Sexo = reader.GetString(1),
                    Nombre = reader.GetString(0),
                    Nota = reader.GetDecimal(2)
                };
        }
    }
    /*
    Core. Utilidades típicas de una aplicación
    Logger, Configuración, etc..
    CID
    */
    namespace Core
    {
        // Static para referenciarlo desde todos los componentes
        public static class CID
        {
            private static readonly Dictionary<Type, (Type, Object)> services =
                new Dictionary<Type, (Type, Object)>();

            static CID() { }

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