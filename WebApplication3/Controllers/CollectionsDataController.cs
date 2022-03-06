using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication3.Models;
using System.Diagnostics;

namespace WebApplication3.Controllers
{
    public class CollectionsDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Collections in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all collections in the database
        /// </returns>
        /// <example>
        /// GET: api/CollectionsData/ListCollections
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CollectionsDto))]
        public IHttpActionResult ListCollections()
        {
            List<Collections> Collections = db.Collections.ToList();
            List<CollectionsDto> CollectionsDtos = new List<CollectionsDto>();

            Collections.ForEach(s => CollectionsDtos.Add(new CollectionsDto()
            {
                CollectionsID = s.CollectionsID,
                CollectionsName = s.CollectionsName,
                CollectionsYear = s.CollectionsYear
            }));

            return Ok(CollectionsDtos);
        }

        /// <summary>
        /// Returns all Collections in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A Collection in the system matching up to the Collections ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Collection</param>
        /// <example>
        /// GET: api/CollectionsData/FindCollections/5
        /// </example>
        [ResponseType(typeof(CollectionsDto))]
        [HttpGet]
        public IHttpActionResult FindCollections(int id)
        {
            Collections Collections = db.Collections.Find(id);
            CollectionsDto CollectionsDto = new CollectionsDto()
            {
                CollectionsID = Collections.CollectionsID,
                CollectionsName = Collections.CollectionsName,
                CollectionsYear = Collections.CollectionsYear
            };
            if (Collections == null)
            {
                return NotFound();
            }

            return Ok(CollectionsDto);
        }

        /// <summary>
        /// Updates a Collections in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Collections ID primary key</param>
        /// <param name="Collections">JSON FORM DATA of a Collections</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/CollectionsData/UpdateCollections/2
        /// FORM DATA: Collections JSON Object
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateCollections(int id, Collections Collections)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Collections.CollectionsID)
            {

                return BadRequest();
            }

            db.Entry(Collections).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionsExists(id))
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

        /// <summary>
        /// Adds an Collections to the system
        /// </summary>
        /// <param name="Collections">JSON FORM DATA of a Collections</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Collections ID, Collections Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/CollectionsData/Collections
        /// FORM DATA: Collections JSON Object
        /// </example>
        [ResponseType(typeof(Collections))]
        [HttpPost]
        public IHttpActionResult AddCollections(Collections Collections)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Collections.Add(Collections);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Collections.CollectionsID }, Collections);
        }

        /// <summary>
        /// Deletes a Collections from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Collections</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/CollectionsData/DeleteCollections/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Collections))]
        [HttpPost]
        public IHttpActionResult DeleteCollections(int id)
        {
            Collections Collections = db.Collections.Find(id);
            if (Collections == null)
            {
                return NotFound();
            }

            db.Collections.Remove(Collections);
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

        private bool CollectionsExists(int id)
        {
            return db.Collections.Count(e => e.CollectionsID == id) > 0;
        }
    }
}