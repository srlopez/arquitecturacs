/*
con las siguientes instrucciones IF, GOTO y LABEL
calcula la media del array
- todo variables globales
- ninguna función
*/

    using System;

    var notas = new []{ 7.1M, 4, 6, 5, 4, 6.5M, 7 };
    var suma = 0M;
    var media = 0M;
    var index = 0;

    Console.WriteLine("Empiezo");
sumar:
    suma = suma + notas[index];
    index = index +1;
    if(index<notas.Length) goto sumar;

    Console.WriteLine("fin ciclo");
    media = suma / notas.Length;

    Console.WriteLine($"La media es {media}");
