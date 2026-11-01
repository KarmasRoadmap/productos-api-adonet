using ProductosApi.Models;
using ProductosApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/productos", (IProductoRepository repository) =>
{
    return Results.Ok(repository.ObtenerTodos());
});

app.MapGet("/productos/{id:int}", (int id, IProductoRepository repository) =>
{
    var producto = repository.ObtenerPorId(id);
    if (producto == null) return Results.NotFound(new { mensaje = "Producto no encontrado" });
    return Results.Ok(producto);
});

app.MapPost("/productos", (Producto producto, IProductoRepository repository) =>
{
    if (!ProductoValido(producto))
        return Results.BadRequest(new { mensaje = "Escribe un nombre de hasta 100 caracteres y valores válidos para precio y existencia." });

    producto.Nombre = producto.Nombre.Trim();
    producto.Id = repository.Insertar(producto);
    return Results.Created($"/productos/{producto.Id}", producto);
});

app.MapPut("/productos/{id:int}", (int id, Producto producto, IProductoRepository repository) =>
{
    if (!ProductoValido(producto))
        return Results.BadRequest(new { mensaje = "Escribe un nombre de hasta 100 caracteres y valores válidos para precio y existencia." });

    producto.Id = id;
    producto.Nombre = producto.Nombre.Trim();
    if (!repository.Actualizar(producto)) return Results.NotFound(new { mensaje = "Producto no encontrado" });
    return Results.NoContent();
});

app.MapDelete("/productos/{id:int}", (int id, IProductoRepository repository) =>
{
    if (!repository.Eliminar(id)) return Results.NotFound(new { mensaje = "Producto no encontrado" });
    return Results.NoContent();
});

app.Run();

bool ProductoValido(Producto producto)
{
    return !string.IsNullOrWhiteSpace(producto.Nombre)
        && producto.Nombre.Trim().Length <= 100
        && producto.Precio >= 0 && producto.Precio <= 99999999.99m
        && decimal.Round(producto.Precio, 2) == producto.Precio
        && producto.Existencia >= 0;
}
