using MySqlConnector;
using ProductosApi.Models;

namespace ProductosApi.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly string conexion;

    public ProductoRepository(IConfiguration configuracion)
    {
        conexion = configuracion.GetConnectionString("Productos")
            ?? throw new InvalidOperationException("Falta la conexión de Productos.");
    }

    public List<Producto> ObtenerTodos()
    {
        var productos = new List<Producto>();
        using var cn = new MySqlConnection(conexion);
        cn.Open();
        using var cmd = new MySqlCommand("SELECT id, nombre, precio, existencia FROM productos ORDER BY id", cn);
        using var lector = cmd.ExecuteReader();

        while (lector.Read())
        {
            productos.Add(new Producto
            {
                Id = lector.GetInt32("id"),
                Nombre = lector.GetString("nombre"),
                Precio = lector.GetDecimal("precio"),
                Existencia = lector.GetInt32("existencia")
            });
        }
        return productos;
    }

    public Producto? ObtenerPorId(int id)
    {
        using var cn = new MySqlConnection(conexion);
        cn.Open();
        using var cmd = new MySqlCommand("SELECT id, nombre, precio, existencia FROM productos WHERE id = @id", cn);
        cmd.Parameters.AddWithValue("@id", id);
        using var lector = cmd.ExecuteReader();

        if (!lector.Read()) return null;

        return new Producto
        {
            Id = lector.GetInt32("id"),
            Nombre = lector.GetString("nombre"),
            Precio = lector.GetDecimal("precio"),
            Existencia = lector.GetInt32("existencia")
        };
    }

    public int Insertar(Producto producto)
    {
        using var cn = new MySqlConnection(conexion);
        cn.Open();
        using var cmd = new MySqlCommand(
            "INSERT INTO productos (nombre, precio, existencia) VALUES (@nombre, @precio, @existencia)", cn);
        cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
        cmd.Parameters.AddWithValue("@precio", producto.Precio);
        cmd.Parameters.AddWithValue("@existencia", producto.Existencia);
        cmd.ExecuteNonQuery();
        return checked((int)cmd.LastInsertedId);
    }

    public bool Actualizar(Producto producto)
    {
        using var cn = new MySqlConnection(conexion);
        cn.Open();
        using var cmd = new MySqlCommand(
            "UPDATE productos SET nombre = @nombre, precio = @precio, existencia = @existencia WHERE id = @id", cn);
        cmd.Parameters.AddWithValue("@id", producto.Id);
        cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
        cmd.Parameters.AddWithValue("@precio", producto.Precio);
        cmd.Parameters.AddWithValue("@existencia", producto.Existencia);
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var cn = new MySqlConnection(conexion);
        cn.Open();
        using var cmd = new MySqlCommand("DELETE FROM productos WHERE id = @id", cn);
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }
}
