using Blockio_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;   
using System.Data;

namespace Blockio_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Services.DatabaseServices _db;
        public UserController(Services.DatabaseServices db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new List<User>();

            using var conn = _db.GetConnection();
            conn.Open();

            string query = "SELECT Id, Email, DisplayName FROM users";
            using var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    DisplayName = reader.GetString("DisplayName")
                });
            }
            return Ok(users);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string query = "INSERT INTO Users (Email, DisplayName,PasswordHash) VALUES (@Email, @DisplayName,@PasswordHash)";

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@DisplayName", user.DisplayName);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            var rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                return Ok("User was added successfully");
            }
            else
            {
                return BadRequest("Failed to add user");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string query = "SELECT Id, Email, DisplayName FROM Users WHERE Id = @Id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var user = new User
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    DisplayName = reader.GetString("DisplayName")
                };
                return Ok(user);
            }
            return NotFound("User not found");
        }

        [HttpGet("email/{email}/password/{password}")]
        public IActionResult GetUserByEmailAndPassword(string email, string password)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string query = "SELECT Id, Email, DisplayName FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@PasswordHash", password);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var user = new User
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    DisplayName = reader.GetString("DisplayName")
                };
                return Ok(user);
            }
            return NotFound("User not found");
        }
    }
}
