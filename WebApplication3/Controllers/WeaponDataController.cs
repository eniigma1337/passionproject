using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class WeaponDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/WeaponData/ListWeapons
        [HttpGet]
        public IEnumerable<WeaponDto> ListWeapons()
        {
            List<Weapon> Weapons = db.Weapons.ToList();
            List<WeaponDto> WeaponDtos = new List<WeaponDto>();

            Weapons.ForEach(a => WeaponDtos.Add(new WeaponDto()
            {
                WeaponID = a.WeaponID,
                WeaponName = a.WeaponName,
                WeaponCreationYear = a.WeaponCreationYear,
                WeaponDescription = a.WeaponDescription,
                WeaponHasPic = a.WeaponHasPic,
                PicExtension = a.PicExtension,
                CollectionsID = a.Collections.CollectionsID,
                CollectionsName = a.Collections.CollectionsName,
                CollectionsYear = a.Collections.CollectionsYear
            }));
            return WeaponDtos;
        }

        /// <summary>
        /// Gathers information about all weapons related to a particular collections ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all weapons in the database, including their associated collections matched with a particular collections ID
        /// </returns>
        /// <param name="id">Collections ID.</param>
        /// <example>
        /// GET: api/WeaponData/ListWeaponsForCollections/5
        /// </example>
        [HttpGet]
        [ResponseType(typeof(WeaponDto))]
        public IHttpActionResult ListWeaponsForCollections(int id)
        {
            List<Weapon> Weapons = db.Weapons.Where(a => a.CollectionsID == id).ToList();
            List<WeaponDto> WeaponDtos = new List<WeaponDto>();

            Weapons.ForEach(a => WeaponDtos.Add(new WeaponDto()
            {
                WeaponID = a.WeaponID,
                WeaponName = a.WeaponName,
                WeaponCreationYear = a.WeaponCreationYear,
                WeaponDescription = a.WeaponDescription,
                WeaponHasPic = a.WeaponHasPic,
                PicExtension = a.PicExtension,
                CollectionsID = a.Collections.CollectionsID,
                CollectionsName = a.Collections.CollectionsName,
                CollectionsYear = a.Collections.CollectionsYear
            }));

            return Ok(WeaponDtos);
        }

        // GET: api/WeaponData/FindWeapon/5
        [ResponseType(typeof(Weapon))]
        [HttpGet]
        public IHttpActionResult FindWeapon(int id)
        {
            Weapon Weapon = db.Weapons.Find(id);
            WeaponDto WeaponDto = new WeaponDto()
            {
                WeaponID = Weapon.WeaponID,
                WeaponName = Weapon.WeaponName,
                WeaponHasPic = Weapon.WeaponHasPic,
                PicExtension = Weapon.PicExtension,
                WeaponCreationYear = Weapon.WeaponCreationYear,
                WeaponDescription = Weapon.WeaponDescription,
                CollectionsID = Weapon.Collections.CollectionsID,
                CollectionsName = Weapon.Collections.CollectionsName,
                CollectionsYear = Weapon.Collections.CollectionsYear
            };
            if (Weapon == null)
            {
                return NotFound();
            }

            return Ok(WeaponDto);
        }

        // Post: api/WeaponData/UpdateWeapon/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateWeapon(int id, Weapon weapon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != weapon.WeaponID)
            {
                return BadRequest();
            }

            db.Entry(weapon).State = EntityState.Modified;
            db.Entry(weapon).Property(a => a.WeaponHasPic).IsModified = false;
            db.Entry(weapon).Property(a => a.PicExtension).IsModified = false;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeaponExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public IHttpActionResult UploadWeaponPic(int id)
        {
            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var weaponPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (weaponPic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(weaponPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Weapons/"), fn);

                                //save the file
                                weaponPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the animal haspic and picextension fields in the database
                                Weapon SelectedWeapon = db.Weapons.Find(id);
                                SelectedWeapon.WeaponHasPic = haspic;
                                SelectedWeapon.PicExtension = extension;
                                db.Entry(SelectedWeapon).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }
                }
                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();
            }
        }

        // POST: api/WeaponData/AddWeapon
        [HttpPost]
        [ResponseType(typeof(Weapon))]
        public IHttpActionResult AddWeapon(Weapon weapon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Weapons.Add(weapon);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = weapon.WeaponID }, weapon);
        }

        /// <summary>
        /// Deletes an weapon from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the weapon</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/WeaponData/DeleteWeapon/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Weapon))]
        [HttpPost]
        public IHttpActionResult DeleteWeapon(int id)
        {
            Weapon weapon = db.Weapons.Find(id);
            if (weapon == null)
            {
                return NotFound();
            }
            if (weapon.WeaponHasPic && weapon.PicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/Weapons/" + id + "." + weapon.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists...preparing to delete");
                    System.IO.File.Delete(path);
                }
            }
            db.Weapons.Remove(weapon);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WeaponExists(int id)
        {
            return db.Weapons.Count(e => e.WeaponID == id) > 0;
        }
    }
}