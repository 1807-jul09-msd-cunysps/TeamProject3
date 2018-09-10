namespace WebAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CreditHistory")]
    public partial class CreditHistory
    {
        [Key]
        [StringLength(9)]
        public string SSN { get; set; }

        public int? RiskScore { get; set; }
    }
}
