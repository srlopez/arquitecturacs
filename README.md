# arquitecturacs

## Construcción de una Mini Arquitectura de Aplicación en C#

Para mostrar:
- La abstración, encapsulación y la separación de responsabilidades en las clases.
- En UI, la independencia de la Vista y de su Controlador
- La agrupación de la Lógica de Negocio en una clase.
- La separación de la capa de Datos mediante un servicio de Repositorio.
- La abstracción y el desacople de clases mediante las interfaces.
- La inyección de Predicado.
- La inyección de Dependencia de Servicios.
- La evolución de la arquitectura de tres capas a una arquitectura de servicios con IoC.
- La organización del código en namespaces.

Y todo en el Program.cs 🧐🤔🤦

## La aplicación  
Nos piden solucionar el problema de un profesor de Arte (_suponemos que la informática le queda lejos_) que desea obtener información sobre un determinado exámen, como la media, la mejor y peor nota, porcentajes de suspensos, etc. Y eso mismo segmentado entre chicos y chicas.   

Nos muestra una lista de las notas:  
``` 
Luis, Marta, Marcos, Aroa, Nerea, Kike, Juan
H,    M,     H,      M,    M,     H,    H
7.5,  4,     6,      5,    4,     6.5,  7.5   
```
Y además nos dice que no tiene la nota de algunos alumnos, y que luego las tendrá que añadir.

### Requitos

La aplicación debería ser coherente con la arquitectura estudiada.
Tendrá un CRUD de Notas, y una serie de informes.


### Pruebas Unitarias

Pues eso... un proyecto xUnit que verifique...

Echándole imaginación, y añadiendo Examen/Curso podríamos hacer otras cositas, como los ejemplos de la pag 130 y 132 del documento PDF `PruebasDeSoftware.pdf` adjunto al proyecto

### Mas
Suponemos un entorno multiusuario. Entonces...
Los métodos del sistema deberían ser asíncronos, y las estructuras de datos deberían ser capaces de ser accedidas concurrentemente.
Revisa del Sitema
- AñadirNota
- PorcentajeXSexo
Y mira cómo deben ser invocadas desde el controlador.

Arrancamos la aplicación en la Terminal con `.vscode/launch.json` con este valor:   
            `"console": "integratedTerminal",`

# Utilizando el DI Contenedor de Microsoft
`dotnet add package Microsoft.Extensions.DependencyInjection --version 6.0.0`  
