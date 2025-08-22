using Blockio_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace Blockio_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly Services.DatabaseServices _db;
        public NotesController(Services.DatabaseServices db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetNotes()
        {
            var notes = new List<Note>();

            using var conn = _db.GetConnection();
            conn.Open();

            string query = "SELECT Id, Title, Content, Status FROM notes";
            using var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                notes.Add(new Note
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Content = reader.GetString("Content"),
                    Status = reader.GetString("Status")
                });
            }
            return Ok(notes);
        }

        [HttpPost]
        public IActionResult AddNote([FromBody]Note note)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = "INSERT INTO Notes (Title, Content, Status, UserId) VALUES (@Title, @Content, @Status, @UserId)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", note.Title);
            cmd.Parameters.AddWithValue("@Content", note.Content);
            cmd.Parameters.AddWithValue("@Status", note.Status);
            cmd.Parameters.AddWithValue("@UserId", note.UserId);

            cmd.ExecuteNonQuery();

            return Ok("Note was Added successfully");
        }

       [HttpPut("{id}")]
       public IActionResult UpdateNote(int id, [FromBody] Note note)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = "UPDATE Notes SET Title = @Title, Content = @Content, Status = @Status WHERE Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Title", note.Title);
            cmd.Parameters.AddWithValue("@Content", note.Content);
            cmd.Parameters.AddWithValue("@Status", note.Status);

            var rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Note was updated successfully");
            }
            else
            {
                return NotFound("Note not found");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNote(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = "DELETE FROM notes WHERE Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            var rowsAffected = cmd.ExecuteNonQuery();

           if (rowsAffected > 0)
            {
                return Ok("Note was deleted successfully");
            }
            else
            {
                return NotFound("Note not found");
            }
        }

        [HttpGet("status/{status}")]
        public IActionResult GetNoteById(string status)
        {
            var notes = new List<Note>();

            using var conn = _db.GetConnection();
            conn.Open();

            string query = "Select Id, Title, Content, UserId From Notes Where Status=@status";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", status.ToLower());

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                notes.Add(new Note
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Content = reader.GetString("Content"),
                    Status = status,
                    UserId = reader.GetInt32("UserId")
                });
            }
            return Ok(notes);
        }

    }
}