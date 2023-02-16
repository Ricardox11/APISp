using ConsultarSP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> _logger;
    private readonly string _connectionString = "Data Source=<ServerName>;Initial Catalog=<DatabaseName>;Integrated Security=True";

    public CustomerController(ILogger<CustomerController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> Get(int id)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetCustomerById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = id;


                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        var customer = new Customer
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Email = (string)reader["Email"]
                        };

                        return customer;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el cliente.");
            return StatusCode(500);
        }
    }
}
