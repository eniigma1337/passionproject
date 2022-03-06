using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using System.Web.Script.Serialization;

namespace WebApplication3.Controllers
{
    public class WeaponController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();


        static WeaponController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:44311/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted: " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Weapon/List

        public ActionResult List()
        {
            //Objective: Communicate with weapon data api to retrieve a list of weapons
            //curl https://localhost:44311/api/weapondata/listweapons

            string url = "weapondata/listweapons";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response is");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<WeaponDto> weapons = response.Content.ReadAsAsync<IEnumerable<WeaponDto>>().Result;
            //Debug.WriteLine("Number of weapons received");
            //Debug.WriteLine(weapons.Count());

            return View(weapons);
        }

        // GET: Weapon/Details/5
        public ActionResult Details(int id)
        {
            //Objective: Communicate with weapon data api to retrieve one weapon
            //curl https://localhost:44311/api/weapondata/findweapon/{id}

            DetailsWeapon ViewModel = new DetailsWeapon();

            string url = "weapondata/findweapon/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response is");
            //Debug.WriteLine(response.StatusCode);

            WeaponDto SelectedWeapon = response.Content.ReadAsAsync<WeaponDto>().Result;
            //Debug.WriteLine("weapon received: ");
            //Debug.WriteLine(SelectedWeapon.WeaponName);
            ViewModel.SelectedWeapon = SelectedWeapon;

            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Weapon/New
        public ActionResult New()
        {
            //Information about all collections in the system.
            //Get api/collectionsdata/listcollections
            string url = "collectionsdata/listcollections";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CollectionsDto> CollectionsOptions = response.Content.ReadAsAsync<IEnumerable<CollectionsDto>>().Result;

            return View(CollectionsOptions);
        }

        // POST: Weapon/Create
        [HttpPost]
        public ActionResult Create(Weapon weapon)
        {
            Debug.WriteLine("jsonpayload:");
            Debug.WriteLine(weapon.WeaponName);
            //Objective: Add a new weapon into the system using the API 
            //curl -H "Content-Type:application/json" -d @weapon.json https://localhost:44311/api/weapondata/addweapon
            string url = "weapondata/addweapon";

            string jsonpayload = jss.Serialize(weapon);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";


            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Weapon/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateWeapon ViewModel = new UpdateWeapon();

            //the existing Weapon information
            string url = "weapondata/findweapon/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            WeaponDto SelectedWeapon = response.Content.ReadAsAsync<WeaponDto>().Result;
            ViewModel.SelectedWeapon = SelectedWeapon;

            // all collections to choose from when updating this animal
            //the existing weapon information
            url = "collectionsdata/listcollections/";
            response = client.GetAsync(url).Result;
            IEnumerable<CollectionsDto> CollectionsOptions = response.Content.ReadAsAsync<IEnumerable<CollectionsDto>>().Result;

            ViewModel.CollectionsOptions = CollectionsOptions;

            return View(ViewModel);
        }

        // POST: Weapon/Update/5
        [HttpPost]
        public ActionResult Update(int id, Weapon weapon, HttpPostedFileBase WeaponPic)
        {
            GetApplicationCookie();
            string url = "weapondata/updateweapon/" + id;
            string jsonpayload = jss.Serialize(weapon);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Debug.WriteLine(content);
            if (response.IsSuccessStatusCode && WeaponPic != null)
            {
                Debug.WriteLine("Calling Update Image method.");
                url = "WeaponData/UploadWeaponPIc/" + id;
                Debug.WriteLine("Received Weapon Picture " + WeaponPic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(WeaponPic.InputStream);
                requestcontent.Add(imagecontent, "WeaponPic", WeaponPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Weapon/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "weapondata/findweapon/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            WeaponDto selectedweapon = response.Content.ReadAsAsync<WeaponDto>().Result;
            return View(selectedweapon);
        }

        // GET: Weapon/Delete/5
        public ActionResult Delete(int id)
        {
            string url = "weapondata/deleteweapon/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
