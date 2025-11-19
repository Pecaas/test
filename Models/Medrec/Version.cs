using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medrec.Models.medrec
{
    [Table("version", Schema = "public")]
    public partial class Version
    {
        [Column("projekt")]
        public short? Projekt { get; set; }

        [Column("system")]
        public int? System { get; set; }

        [Column("server")]
        public short? Server { get; set; }

        [Key]
        [Column("versionnum")]
        [Required]
        public int Versionnum { get; set; }

        [Column("sysversionnum")]
        public int? Sysversionnum { get; set; }

        [Column("versiondate")]
        public DateTime? Versiondate { get; set; }

        [Column("language")]
        [MaxLength(20)]
        public string Language { get; set; }

        [Column("info")]
        [MaxLength(30)]
        public string Info { get; set; }

        [Column("createdate")]
        public DateTime? Createdate { get; set; }

        [Column("dbclosed")]
        public int? Dbclosed { get; set; }

        [Column("dbdatfr")]
        public int? Dbdatfr { get; set; }

        [Column("dbdatto")]
        public int? Dbdatto { get; set; }

        [Column("dbtype")]
        [MaxLength(20)]
        public string Dbtype { get; set; }

        [Column("dbservertype")]
        public int? Dbservertype { get; set; }
    }
}