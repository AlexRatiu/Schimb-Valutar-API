using System.ComponentModel.DataAnnotations.Schema;

namespace Schimb_Valutar_API.DAL.DBO
{
    public class Currency
    {
        public int id { get; set; }
        public string abreviation { get; set; }

        [Column(TypeName = "decimal(18, 4)")] 
        public decimal value { get; set; }
    }
}
