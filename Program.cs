/*
PROGRAMACION ESTRUCTURADA POR BLOQUES
Calcula la media del array

Puedes usar FOR, WHILE, DO y llaves {}

Restricciones:
    - todo variables globales
    - ninguna función
*/

using System;

var notas = new[] { 7.5M, 4, 6, 5, 4, 6.5M, 7.5M };
var suma = 0M;
var index = 0;

//  EMPEZAMOS
Console.WriteLine("Empiezo");

//  CICLO
for (suma = 0; index < notas.Length; suma+=notas[index], index++)
//{
//   suma = suma + notas[index];
//}

//  FINAL

Console.WriteLine("fin ciclo");
var media = suma / notas.Length;
Console.WriteLine($"La media es {media:0.00}");
