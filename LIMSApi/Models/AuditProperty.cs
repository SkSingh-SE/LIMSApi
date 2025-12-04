using System.ComponentModel.DataAnnotations.Schema;
using LIMSApi.Helpers;

namespace LIMSApi.Models
{
    public class AuditProperty
    {
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } 
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CompanyCode { get; set; } = "LIMS";
        public bool IsActive { get; set; } = true;

        [NotMapped]
        private LoggedInUserDTO loggedInUser;
        public AuditProperty()
        {
            loggedInUser = LoggedInUserProvider.CurrentUser;
            if (loggedInUser != null)
            {
                this.CreatedBy = loggedInUser.EmployeeID;
                this.ModifiedBy = loggedInUser.EmployeeID;
                this.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";
            }
            else
            {
                this.CreatedBy = 0; // Or throw/log warning as needed
                this.ModifiedBy = 0;
                this.CompanyCode = "LIMS";
            }

            this.CreatedOn = DateTime.UtcNow;
            this.ModifiedOn = DateTime.UtcNow;

        }
    }

}
