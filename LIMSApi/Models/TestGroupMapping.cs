using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestGroupMapping
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestGroupID { get; set; }


        [Required]
        public long TestID { get; set; }

        [Required]
        public long TestMethodID { get; set; }

        [ForeignKey("TestID")]
        public virtual TestMaster? TestMaster { get; set; }
        [ForeignKey("TestGroupID")]
        public virtual TestGroup? TestGroup { get; set; }


        [ForeignKey("TestMethodID")]
        public virtual TestMethodMaster? TestMethod { get; set; }
    }
}
