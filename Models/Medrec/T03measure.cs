using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medrec.Models.medrec
{
    [Table("t03measure", Schema = "public")]
    public partial class T03measure
    {
        [Key]
        [Column("serial")]
        [Required]
        public int Serial { get; set; }

        [Column("mednum")]
        [Required]
        public int Mednum { get; set; }

        [Column("eventtype")]
        public short? Eventtype { get; set; }

        [Column("eventcateg")]
        public int? Eventcateg { get; set; }

        [Column("eventnum")]
        [MaxLength(15)]
        public string Eventnum { get; set; }

        [Column("yeargen")]
        public int? Yeargen { get; set; }

        [Column("yeargenseq")]
        public int? Yeargenseq { get; set; }

        [Column("dat")]
        public DateTime? Dat { get; set; }

        [Column("tim")]
        public short? Tim { get; set; }

        [Column("height")]
        public double? Height { get; set; }

        [Column("weight")]
        public double? Weight { get; set; }

        [Column("weightgr")]
        public int? Weightgr { get; set; }

        [Column("surface")]
        public double? Surface { get; set; }

        [Column("bmi")]
        public double? Bmi { get; set; }

        [Column("systpress")]
        public int? Systpress { get; set; }

        [Column("diastpress")]
        public int? Diastpress { get; set; }

        [Column("heartrate")]
        public int? Heartrate { get; set; }

        [Column("respfreq")]
        public int? Respfreq { get; set; }

        [Column("temp")]
        public double? Temp { get; set; }

        [Column("departm")]
        public int? Departm { get; set; }

        [Column("who")]
        public int? Who { get; set; }

        [Column("datcheck")]
        public DateTime? Datcheck { get; set; }

        [Column("whocheck")]
        public int? Whocheck { get; set; }

        [Column("datlock")]
        public DateTime? Datlock { get; set; }

        [Column("wholock")]
        public int? Wholock { get; set; }

        [Column("ownersource")]
        public int? Ownersource { get; set; }

        [Column("ownertype")]
        public int? Ownertype { get; set; }

        [Column("ownerserial")]
        public int? Ownerserial { get; set; }

        [Column("ownertag")]
        [MaxLength(40)]
        public string Ownertag { get; set; }

        [Column("userid")]
        [MaxLength(20)]
        public string Userid { get; set; }

        [Column("indiclink")]
        [MaxLength(200)]
        public string Indiclink { get; set; }

        [Column("indicproc")]
        public short? Indicproc { get; set; }

        [Column("extid")]
        [MaxLength(40)]
        public string Extid { get; set; }

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

        [Column("eventcode")]
        public int? Eventcode { get; set; }

        [Column("flinsum")]
        public int? Flinsum { get; set; }

        [Column("floutsum")]
        public int? Floutsum { get; set; }
    }
}