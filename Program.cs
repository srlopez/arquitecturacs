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

var Sistema = new Sistema(notas);
Console.WriteLine($"La media de la notas es: {Sistema.CalculoDeLaMedia():0.00}");
Console.WriteLine("Fin");


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

    public Sistema( Calificacion[] notas ){
        Notas = notas;
    }

    private decimal CalculoDeLaSuma(decimal[] datos)
    {
        var suma = 0M;
        foreach(decimal val in datos)
        {
            suma += val;
        }
        return suma;
    }

    public decimal CalculoDeLaMedia()
    {

        var notas = new decimal[Notas.Length];
        var i = 0;
        foreach (Calificacion cal in Notas)
        {
            notas[i++] = cal.Nota;
        }
        return CalculoDeLaSuma(notas) / Notas.Length;
    }
}

