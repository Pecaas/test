using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medrec.Models.medrec
{
    [Table("t01patid", Schema = "public")]
    public partial class T01patid
    {
        [Key]
        [Column("serial")]
        [Required]
        public int Serial { get; set; }

        [Column("mednum")]
        [Required]
        public int Mednum { get; set; }

        [Column("idtype")]
        public short? Idtype { get; set; }

        [Column("idnum")]
        [MaxLength(15)]
        public string Idnum { get; set; }

        [Column("yeargen")]
        public int? Yeargen { get; set; }

        [Column("yeargenseq")]
        public int? Yeargenseq { get; set; }

        [Column("delted")]
        public short? Delted { get; set; }

        [Column("datnew")]
        public DateTime? Datnew { get; set; }

        [Column("whonew")]
        public int? Whonew { get; set; }

        [Column("trmnew")]
        public int? Trmnew { get; set; }

        [Column("datmod")]
        public DateTime? Datmod { get; set; }

        [Column("whomod")]
        public int? Whomod { get; set; }

        [Column("trmmod")]
        public int? Trmmod { get; set; }

        [Column("insdat")]
        public DateTime? Insdat { get; set; }

        [Column("insver")]
        public int? Insver { get; set; }

        [Column("sysdat")]
        public DateTime? Sysdat { get; set; }

        [Column("sysver")]
        public int? Sysver { get; set; }
    }
}