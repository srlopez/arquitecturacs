/*
PROGRAMACION CON ABSTRACIONES - REPRESENTACION DE OBJETOS;
Calcula la media del array

Decide cómo representar los valores de nota
cuando sabemos que cada nota la ha obtenifo un alumno.
Luis, Marta, Marcos, Aroa, Nerea, Kike, Juan
7.5M, 4,     6,      5,    4,     6.5M, 7.5M 

Estudiar los distintos tipos de datos que nos ofrece c# para abstracciones de objetos del mundo real
Tuplas, https://docs.microsoft.com/es-es/dotnet/csharp/language-reference/builtin-types/value-tuples
estructuras: https://docs.microsoft.com/es-es/dotnet/csharp/language-reference/builtin-types/struct
registros, https://docs.microsoft.com/es-es/dotnet/csharp/language-reference/builtin-types/record
clases https://docs.microsoft.com/es-es/dotnet/csharp/fundamentals/types/classes
y Dictionary<string, object> https://docs.microsoft.com/es-es/dotnet/api/system.collections.generic.dictionary-2?view=net-5.0

*/

using System;
using System.Collections.Generic;


Console.WriteLine("Empezamos");
//var notas = new[] { 7.5M, 4, 6, 5, 4, 6.5M, 7.5M };

// Tuplas
var tLuis = (Nombre: "Luis", Nota: 7.5M);
var tMarta = (Nombre: "Marta", Nota: 4M);
var tNotas = new[] { tLuis, tMarta };
Tuple<string, int>[] tNotas0 = new Tuple<string, int>[] { }; //<-- inicializado a 0 elementos
Console.WriteLine($"La primera nota del array de tuplas es {tNotas[0].Nota:0.00}");

// Estructuras
var sLuis = new SCalificacion("Luis", 7.5M);
var sNotas = new[] { sLuis, /*...*/};
SCalificacion[] sNotas0 = new SCalificacion[] { };//<-- inicializado a 0 elementos
Console.WriteLine($"La primera nota del array de estructuras es {sNotas[0].Nota:0.00}");


// Records
var rluis = new ReCalificacion("Luis", 7.5M);
var rNotas = new[] { rluis, /*..*/};
ReCalificacion[] rNotas0 = new ReCalificacion[8]; //<- inicializado longitud 8 y cero elementos
Console.WriteLine($"La primera nota del array de records es {rNotas[0].Nota:0.00}");

// Clases
var cLuis = new ClCalificacion("Luis", 7.5M);
var cNotas = new[] { cLuis, /*...*/};
ClCalificacion[] cNotas0 = new ClCalificacion[] { };//<-- inicializado a 0 elementos
Console.WriteLine($"La primera nota del array de clases es {cNotas[0].Nota:0.00}");

// Diccionario
var dLuis =  new Dictionary<string, Object>();
dLuis["nombre"] = "Luis";
dLuis["nota"] = 7.5M;
var dNotas = new[] { dLuis, /*...*/};
var dNotas0 = new Dictionary<string, Object>[7] ;
Console.WriteLine($"La primera nota del array de diccionarios es {dNotas[0]["nota"]:0.00}");





// TopLevel instrucciones obliga a las declaraciones abajo
public struct SCalificacion
{
    public SCalificacion(string nombre, decimal nota)
    {
        Nombre = nombre;
        Nota = nota;
    }

    public string Nombre { get; }
    public decimal Nota { get; }

    public override string ToString() => $"({Nombre}, {Nota})";
}

// Registros
public record ReCalificacion(string Nombre, decimal Nota);

public class ClCalificacion
{
    public string Nombre;
    public decimal Nota;
    public ClCalificacion(string nombre, decimal nota)
    {
        Nombre = nombre;
        Nota = nota;
    }
    public override string ToString() => $"({Nombre}, {Nota})";
}
/*
Elige la opción más adecuada según tu criterio
para representar la Calificación
*/


/*
var suma = CalculoDeLaSuma(notas);
var media = CalculoDeLaMedia(suma, notas.Length);
Console.WriteLine($"La media es {media:0.00}");
Console.WriteLine("Fin");

decimal CalculoDeLaSuma(decimal[] datos)
{
    var suma = 0M;
    var index = 0;
    do
    {
        suma += datos[index];
        index++;
    }
    while (index < datos.Length);
    return suma;
}

decimal CalculoDeLaMedia(decimal total, int nElementos) => total / nElementos;


 */

