using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using System.Web.Script.Serialization;

namespace WebApplication3.Controllers
{
    public class CollectionsController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CollectionsController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:44311/api/");
        }

        // GET: Collections/List
        public ActionResult List()
        {
            //objective: communicate with our Collections data api to retrieve a list of Collections
            //curl https://localhost:44324/api/Collectionsdata/listCollections


            string url = "collectionsdata/listcollections";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<CollectionsDto> Collections = response.Content.ReadAsAsync<IEnumerable<CollectionsDto>>().Result;
            //Debug.WriteLine("Number of Collections received : ");
            //Debug.WriteLine(Collections.Count());


            return View(Collections);
        }


        // GET: Collections/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our Collections data api to retrieve one Collections
            //curl https://localhost:44324/api/Collectionsdata/findcollections/{id}

            DetailsCollections ViewModel = new DetailsCollections();

            string url = "collectionsdata/findcollections/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            CollectionsDto SelectedCollections = response.Content.ReadAsAsync<CollectionsDto>().Result;
            //Debug.WriteLine("Collections received : ");
            //Debug.WriteLine(SelectedCollections.CollectionsName);

            ViewModel.SelectedCollections = SelectedCollections;

            //showcase information about weapons related to this collections
            //send a request to gather information about weapons related to a particular collections ID
            url = "weapondata/listweaponsforcollections/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<WeaponDto> RelatedWeapons = response.Content.ReadAsAsync<IEnumerable<WeaponDto>>().Result;

            ViewModel.RelatedWeapons = RelatedWeapons;


            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Collections/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Species/Create
        [HttpPost]
        public ActionResult Create(Collections Collections)
        {
            //Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Collections.CollectionsName);
            //objective: add a new Collections into our system using the API
            //curl -H "Content-Type:application/json" -d @Collections.json  https://localhost:44324/api/Collectionsdata/addCollections 
            string url = "collectionsdata/addcollections";


            string jsonpayload = jss.Serialize(Collections);
            //Debug.WriteLine(jsonpayload);

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

        // GET: Collections/Edit/2
        public ActionResult Edit(int id)
        {
            string url = "collectionsdata/findcollections/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CollectionsDto selectedCollections = response.Content.ReadAsAsync<CollectionsDto>().Result;
            return View(selectedCollections);
        }

        // POST: Collections/Update/2
        [HttpPost]
        public ActionResult Update(int id, Collections Collections)
        {

            string url = "collectionsdata/updatecollections/" + id;
            string jsonpayload = jss.Serialize(Collections);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            ////Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Collections/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "collectionsdata/findcollections/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CollectionsDto selectedCollections = response.Content.ReadAsAsync<CollectionsDto>().Result;
            return View(selectedCollections);
        }

        // POST: Collections/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "collectionsdata/deletecollections/" + id;
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
