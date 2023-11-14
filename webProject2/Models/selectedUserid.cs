using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace webProject2.Models
{
    public class selectedUserid
    {   


        public int Id { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime buyDate { get; set; }
        public int userid {  get; set; }
        public int itemid { get; set; }
        public int quantity { get; set; }
    }
}
