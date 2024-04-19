using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductoController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Producto>>> GetAllProductos()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Producto> productos = await SelectAllProductos(connection);
            return Ok(productos);
        }

        [HttpGet("{productoId}")]
        public async Task<ActionResult<Producto>> GetProducto(int productoId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var producto = await connection.QueryFirstAsync<Producto>("select * from Producto where IdProducto = @Id",
                new { Id = productoId });
            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<List<Producto>>> CreateProducto(Producto producto)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("INSERT INTO Producto(Nombre, Marca, Distribuidor, Color) values (@Nombre, @Marca, @Distribuidor, @Color)", producto);
            return Ok(await SelectAllProductos(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Producto>>> UpdateProducto(Producto producto)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Producto set nombre = @Nombre, marca = @Marca, distribuidor = @Distribuidor, color = @Color where idproducto = @IdProducto", producto);
            return Ok(await SelectAllProductos(connection));
        }

        [HttpDelete("{productoId}")]
        public async Task<ActionResult<List<Producto>>> DeleteProducto(int productoId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from Producto where IdProducto = @Id", new { Id = productoId});
            return Ok(await SelectAllProductos(connection));
        }

        private static async Task<IEnumerable<Producto>> SelectAllProductos(SqlConnection connection)
        {
            return await connection.QueryAsync<Producto>("select * from Producto");
        }
    }
}
