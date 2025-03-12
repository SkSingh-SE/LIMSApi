using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LIMSApi.Models
{
    public class CountryMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public required string Name { get; set; }
        public string? Code { get; set; }

    }
}
