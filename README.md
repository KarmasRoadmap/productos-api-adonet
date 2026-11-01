# Productos API

Práctica de ADO.NET: una API en .NET 10 que consulta y modifica productos en
MySQL. Usa comandos SQL con parámetros, una interfaz de repositorio y Swagger.
La búsqueda es por ID, con el método `ObtenerPorId` solicitado en la actividad.

## Requisitos

- SDK de .NET 10.
- Docker con Docker Compose en ejecución.

## Ejecutar

En una terminal, dentro de esta carpeta, crea los archivos de configuración local:

```bash
cp .env.example .env
cp appsettings.Development.example.json appsettings.Development.json
```

Edita ambos archivos y reemplaza `ELIGE_UNA_CLAVE` por la misma contraseña
local de MySQL. Esos dos archivos están ignorados por Git. Si ya tenías la base
creada, usa su contraseña actual; cambiar el archivo no cambia la contraseña
que ya existe dentro de MySQL.

Después inicia MySQL:

```bash
docker compose up -d
```

La primera vez se descarga la imagen de MySQL. Espera a que inicie; esto puede
tardar unos momentos. El contenedor crea `productos_db` y ejecuta
`Database/productos.sql` automáticamente **solo cuando el volumen está vacío**.

En la misma carpeta inicia la API:

```bash
dotnet restore
dotnet run
```

Abre [Swagger](http://localhost:5203/swagger) para probar la API. Mantén
`dotnet run` ejecutándose. MySQL usa el puerto local `3307`; la API usa `5203`.
La cadena de conexión local se configura en `appsettings.Development.json`.

## Endpoints y prueba

| Método | Ruta | Operación |
| --- | --- | --- |
| GET | `/productos` | Mostrar todos |
| GET | `/productos/{id}` | Buscar por ID |
| POST | `/productos` | Insertar y obtener el ID creado |
| PUT | `/productos/{id}` | Actualizar por ID |
| DELETE | `/productos/{id}` | Eliminar por ID |

En Swagger, abre una ruta, pulsa **Try it out**, llena los campos y pulsa
**Execute**. Para POST y PUT se puede usar este JSON:

```json
{
  "nombre": "Cuaderno",
  "precio": 45.50,
  "existencia": 10
}
```

Orden sugerido: POST, GET de la lista, GET por ID, PUT con el ID recibido,
GET por ID para comprobar el cambio y DELETE. Un GET o DELETE de un ID que no
existe responde 404. Si se envía `id` en el JSON de POST se ignora: MySQL asigna
el número. En PUT se usa el ID de la ruta. El nombre es obligatorio, máximo
100 caracteres; el precio y la existencia deben ser no negativos. El precio
acepta hasta dos decimales.

Para comprobar la persistencia, detén la API con `Ctrl+C`, vuelve a ejecutar
`dotnet run` y consulta la lista. Los datos de MySQL permanecen en el volumen.
`docker compose stop` detiene MySQL y `docker compose start` vuelve a iniciarlo.

## Archivos principales

```text
ProductosApi/
├── Models/Producto.cs
├── Repositories/IProductoRepository.cs
├── Repositories/ProductoRepository.cs
├── Database/productos.sql
├── Properties/launchSettings.json
├── Program.cs
├── appsettings.json
├── appsettings.Development.example.json
├── .env.example
├── ProductosApi.csproj
└── compose.yaml
```

- `IProductoRepository` conserva los cinco métodos indicados en la actividad.
- `ProductoRepository` implementa la interfaz con `MySqlConnection`,
  `MySqlCommand` y `MySqlDataReader` de ADO.NET. Los valores de las consultas
  se envían mediante parámetros como `@id` y `@nombre`.
- `Program.cs` registra el repositorio y conecta las rutas con sus métodos.
  Swagger solo se habilita en el entorno de desarrollo del perfil `http`.
- `Database/productos.sql` crea la tabla. No agrega registros automáticamente.

La lista y la búsqueda leen la base de datos en cada petición. Los datos no se
guardan en una lista temporal de C#.
