/*
PROGRAMACION CON FUNCIONES PARÁMETORS * RETURN;
Calcula la media del array

Debes usar funciones con retorno y variables locales;

Restricciones:
    - todo variables globales
*/

using System;


Console.WriteLine("Empezamos");
var notas = new[] { 7.5M, 4, 6, 5, 4, 6.5M, 7.5M };
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




