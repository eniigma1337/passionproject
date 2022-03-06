using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models.ViewModels
{
    public class DetailsCollections
    {
        //The collections itself that we want to display
        public CollectionsDto SelectedCollections { get; set; }

        //All of the related weapons to that particular species
        public IEnumerable<WeaponDto> RelatedWeapons { get; set; }
    }
}
