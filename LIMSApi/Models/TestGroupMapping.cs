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
        [ForeignKey("TestGroup")]
        public long TestGroupID { get; set; }

        public virtual TestGroup? TestGroup { get; set; }

        [Required]
        [ForeignKey("TestMaster")]
        public long TestID { get; set; }

        public virtual TestMaster? TestMaster { get; set; }

        [Required]
        [ForeignKey("TestMethod")]
        public long TestMethodID { get; set; }

        public virtual TestMethodMaster? TestMethod { get; set; }
    }
}
