using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WebApplication3.Models
{
    public class Collections
    {
        [Key]
        public int CollectionsID { get; set; }
        public string CollectionsName { get; set; }
        public string CollectionsYear { get; set; }
    }

    public class CollectionsDto
    {
        public int CollectionsID { get; set; }
        public string CollectionsName { get; set; }
        public string CollectionsYear { get; set; }

    }
}