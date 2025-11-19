using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medrec.Models.medrec
{
    [Table("t90tab1", Schema = "public")]
    public partial class T90tab1
    {
        [Key]
        [Column("serial")]
        [Required]
        public int Serial { get; set; }

        [Column("tab")]
        [Required]
        public short Tab { get; set; }

        [Column("keycode")]
        [Required]
        public int Keycode { get; set; }

        [Column("abbrev")]
        [MaxLength(20)]
        public string Abbrev { get; set; }

        [Column("descr")]
        [MaxLength(180)]
        public string Descr { get; set; }

        [Column("descr1")]
        [MaxLength(200)]
        public string Descr1 { get; set; }

        [Column("par01")]
        [MaxLength(100)]
        public string Par01 { get; set; }

        [Column("par02")]
        [MaxLength(100)]
        public string Par02 { get; set; }

        [Column("par03")]
        [MaxLength(100)]
        public string Par03 { get; set; }

        [Column("color")]
        public int? Color { get; set; }

        [Column("datfr")]
        public DateTime? Datfr { get; set; }

        [Column("datto")]
        public DateTime? Datto { get; set; }

        [Column("extid")]
        [MaxLength(40)]
        public string Extid { get; set; }

        [Column("data1x")]
        [MaxLength(32000)]
        public string Data1x { get; set; }

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

        [Column("data1o")]
        public byte[] Data1o { get; set; }
    }
}