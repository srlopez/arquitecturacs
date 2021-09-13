/*
PROGRAMACION CON ABSTRACIONES - REPRESENTACION DE OBJETOS;
Calcula la media del array

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

    private decimal CalculoDeLaSuma(decimal[] datos) => datos.Sum();
    public decimal CalculoDeLaMedia()
    {
        var notas = Notas.Select(calificacion => calificacion.Nota).ToArray();
        return CalculoDeLaSuma(notas) / Notas.Length;
    }
}

