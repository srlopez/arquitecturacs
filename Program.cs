/*
PROGRAMACION IMPERATIVA
con las siguientes instrucciones exclusivamente IF, GOTO y LABEL
calcula la media del array

Restricciones
    - todo variables globales
    - ninguna función
    - no usar llaves {} para bloques de instruciones
    - if sin else.
*/
    using System;

    var notas = new []{ 7.5M, 4, 6, 5, 4, 6.5M, 7.5M };
    //var notas = new []{ 1M,1,1,1,1,1 };
    var suma = 0M;
    var media = 0M;
    var index = 0;

//  EMPEZAMOS
    Console.WriteLine("Empiezo");

//  CICLO
sumar:
    suma = suma + notas[index];
    index = index +1;
    if(index<notas.Length) goto sumar;

//  FIN CICLO
    Console.WriteLine("fin ciclo");
    media = suma / notas.Length;

//  FINAL
    Console.WriteLine($"La media es {media:0.00}");
