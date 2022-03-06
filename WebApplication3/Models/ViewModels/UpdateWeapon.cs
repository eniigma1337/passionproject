using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models.ViewModels
{
    public class UpdateWeapon
    {
        //This viewmodel is a class which stores information that we need to present to /Weapon/Update/{}

        //the existing weapon information

        public WeaponDto SelectedWeapon { get; set; }

        // all collections to choose from when updating this weapon

        public IEnumerable<CollectionsDto> CollectionsOptions { get; set; }
    }
}