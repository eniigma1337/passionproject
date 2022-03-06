using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplication3.Models
{
    public class Weapon
    {
        [Key]
        public int WeaponID { get; set; }
        public string WeaponName { get; set; }
        public int WeaponCreationYear { get; set; }
        public string WeaponDescription { get; set; }
        public bool WeaponHasPic { get; set; }
        public string PicExtension { get; set; }

        //A weapon belongs to one collections
        //A collections can have many weapons
        [ForeignKey("Collections")]
        public int CollectionsID { get; set; }

        public virtual Collections Collections { get; set; }
    }
    public class WeaponDto
    {
        public int WeaponID { get; set; }
        public string WeaponName { get; set; }
        public int WeaponCreationYear { get; set; }
        public string WeaponDescription { get; set; }
        public int CollectionsID { get; set; }
        public string CollectionsName { get; set; }
        public string CollectionsYear { get; set; }
        public bool WeaponHasPic { get; set; }
        public string PicExtension { get; set; }




    }
}