using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medrec.Models.medrec
{
    [Table("t01pat", Schema = "public")]
    public partial class T01pat
    {
        [Key]
        [Column("mednum")]
        [Required]
        public int Mednum { get; set; }

        [Column("persnum")]
        public int? Persnum { get; set; }

        [Column("persid")]
        [MaxLength(15)]
        public string Persid { get; set; }

        [Column("datbirth")]
        public DateTime? Datbirth { get; set; }

        [Column("sex")]
        public short? Sex { get; set; }

        [Column("lname")]
        [MaxLength(60)]
        public string Lname { get; set; }

        [Column("fname")]
        [MaxLength(40)]
        public string Fname { get; set; }

        [Column("inscomp")]
        public int? Inscomp { get; set; }

        [Column("insid")]
        [MaxLength(15)]
        public string Insid { get; set; }

        [Column("postcode")]
        [MaxLength(20)]
        public string Postcode { get; set; }

        [Column("country")]
        public int? Country { get; set; }

        [Column("countrymember")]
        public int? Countrymember { get; set; }

        [Column("identnum")]
        [MaxLength(15)]
        public string Identnum { get; set; }

        [Column("mpisysver")]
        public int? Mpisysver { get; set; }

        [Column("mpisysdat")]
        public DateTime? Mpisysdat { get; set; }

        [Column("v")]
        [MaxLength(40)]
        public string V { get; set; }

        [Column("patcateg")]
        public int? Patcateg { get; set; }

        [Column("datevidbeg")]
        public DateTime? Datevidbeg { get; set; }

        [Column("datevidend")]
        public DateTime? Datevidend { get; set; }

        [Column("reaevidend")]
        public short? Reaevidend { get; set; }

        [Column("treatwho")]
        public int? Treatwho { get; set; }

        [Column("dg")]
        [MaxLength(10)]
        public string Dg { get; set; }

        [Column("warning")]
        [MaxLength(60)]
        public string Warning { get; set; }

        [Column("medver")]
        public int? Medver { get; set; }

        [Column("userid")]
        [MaxLength(20)]
        public string Userid { get; set; }

        [Column("indiclink")]
        [MaxLength(200)]
        public string Indiclink { get; set; }

        [Column("mednumlink")]
        public int? Mednumlink { get; set; }

        [Column("schred")]
        public int? Schred { get; set; }

        [Column("schrednum")]
        public int? Schrednum { get; set; }

        [Column("indicstate")]
        public short? Indicstate { get; set; }

        [Column("indicstate1")]
        public short? Indicstate1 { get; set; }

        [Column("indicuser")]
        public int? Indicuser { get; set; }

        [Column("data1x")]
        [MaxLength(32000)]
        public string Data1x { get; set; }

        [Column("data1o")]
        public byte[] Data1o { get; set; }

        [Column("metaserial")]
        public int? Metaserial { get; set; }

        [Column("viewserial")]
        public int? Viewserial { get; set; }

        [Column("biserial")]
        public int? Biserial { get; set; }

        [Column("params")]
        [MaxLength(100)]
        public string Params { get; set; }

        [Column("dynitems")]
        [MaxLength(8000)]
        public string Dynitems { get; set; }

        [Column("dynitemsserial")]
        public int? Dynitemsserial { get; set; }

        [Column("iname")]
        [MaxLength(20)]
        public string Iname { get; set; }

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