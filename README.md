# Console (CSVImporter)

CSVImporter es una aplicacion de consola para importar datos desde un excel (csv) hacia la base de datos. 

A traves de inyeccion de dependencias se importan los servicios a utlizar:

- Metdodos:
 Run: ejecuta el importador
 Logging: hace un registo en tiempo real del consumo de memoria

## Serivces
- ServiceImporter: transforma la informacion almacenada en el archivo excel en un listado de objetos propios a insertar en la db

## Repositories
- RepositoyImporter: limpia la tabla actual, y recibe un listado de objetos listos para insertar en la db nuevamente

## Helpers 

- AutoMapper
- Serilog

Agregado de librerias:
 - FileHelpers: se utiliza para leer de manera eficaz y rapida el contenido dentro del archivo excel
 - Sqlbullcopy: se utliza para realizar el insert masivo de datos importados desde excel