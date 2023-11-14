using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using webProject2.Models;

namespace webProject2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class APIOrdersController : ControllerBase



    {
        [HttpGet("{userid}")]
        public IEnumerable<selectedUserid> Get(int userid) {
            List<selectedUserid> list = new List<selectedUserid>();
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
            string sql = " SELECT * FROM orders where userid= '" + userid + "'";
            SqlCommand comm=new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader=comm.ExecuteReader();

            while (reader.Read()) {

                list.Add(new selectedUserid { itemid = (int)reader["itemid"],

                    userid = (int)reader["userid"],
                    quantity = (int)reader["quantity"]
                });

                
            
            }

            conn.Close();
            reader.Close();
            return (list);
        }

    }
}
