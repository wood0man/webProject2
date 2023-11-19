using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using webProject2.Models;

namespace webProject2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class APIUsersController : ControllerBase
    {
        [HttpGet("{role}")]
        public IEnumerable<selectedRole> Get(int Id)
        {
            List<selectedRole> list = new List<selectedRole>();
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
            string sql = " SELECT * FROM users where Id= '" + Id + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {

                list.Add(new selectedRole
                {
                    name = (string)reader["name"],

                    password = (string)reader["password"],
                    Id = (int)reader["Id"],
                    role = (string)reader["role"],
                    registerDate = (DateTime)reader["registerDate"]
                });



            }

            conn.Close();
            reader.Close();
            return (list);
        }

    }
}
