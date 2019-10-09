using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MovieApp.DataModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace MovieApp.Models
{
    public class ProducerModel
    {

        MoviesEntities db = new MoviesEntities();

        public long ProducerID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name: ")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name: ")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date Of Birth: ")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Display(Name = "No Of Movies: ")]
        public long NumberOfMovies { get; set; }

        public string PageCount { get; set; }

        public string PageNumber { get; set; }

        public tblProducer Producer { get; set; }

        public List<tblProducer> Producers { get; set; }

        /// <summary>
        /// Get all existing producers list from producers database table
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="sortingDir"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public ProducerModel GetAllProducers(ProducerModel model, string sortingOrder, string sortingDir, string searchText)
        {

            if (!string.IsNullOrEmpty(searchText))
                model.Producers = db.tblProducers.Where(u => u.FirstName.ToLower().Contains(searchText.ToLower())
                || u.LastName.ToLower().Contains(searchText.ToLower())
                || u.NumberOfMovies == NumberOfMovies
                ).ToList();
            else
                model.Producers = db.tblProducers.ToList();

            switch (sortingOrder)
            {
                case "FirstName":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.Producers = model.Producers.OrderBy(u => u.FirstName).ToList();
                    else
                        model.Producers = model.Producers.OrderByDescending(u => u.FirstName).ToList();
                    break;
                case "LastName":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.Producers = model.Producers.OrderBy(u => u.LastName).ToList();
                    else
                        model.Producers = model.Producers.OrderByDescending(u => u.LastName).ToList();
                    break;
                case "DateOfBirth":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.Producers = model.Producers.OrderBy(u => u.DateOfBirth).ToList();
                    else
                        model.Producers = model.Producers.OrderByDescending(u => u.DateOfBirth).ToList();
                    break;
                case "NumberOfMovies":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.Producers = model.Producers.OrderBy(u => u.NumberOfMovies).ToList();
                    else
                        model.Producers = model.Producers.OrderByDescending(u => u.NumberOfMovies).ToList();
                    break;
                default:
                    model.Producers = model.Producers.OrderByDescending(u => u.ProducerID).ToList();
                    break;

            }

            return model;
        }

        /// <summary>
        /// Add a new producer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long AddProducer(ProducerModel model)
        {
            long producerId = 0;
            tblProducer prd = new tblProducer();
            prd.FirstName = model.FirstName;
            prd.LastName = model.LastName;
            prd.DateOfBirth = model.DateOfBirth;
            prd.NumberOfMovies = model.NumberOfMovies;
            db.tblProducers.Add(prd);
            db.SaveChanges();
            producerId = prd.ProducerID;
            return producerId;
        }

        /// <summary>
        /// Get producer based on producer id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public ProducerModel GetProducer(ProducerModel model, long producerId)
        {
            model.Producer = db.tblProducers.Where(u => u.ProducerID == producerId).FirstOrDefault();
            if (model.Producer != null && model.Producer.ProducerID > 0)
            {
                model.ProducerID = model.Producer.ProducerID;
                model.FirstName = model.Producer.FirstName;
                model.LastName = model.Producer.LastName;
                model.DateOfBirth = model.Producer.DateOfBirth;
                model.NumberOfMovies = (long)model.Producer.NumberOfMovies;
            }
            return model;
        }

        /// <summary>
        /// Edit producer based on producer id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public ProducerModel EditProducer(ProducerModel model, long producerId)
        {
            model.Producer = db.tblProducers.Where(u => u.ProducerID == producerId).FirstOrDefault();
            if (model.Producer != null && model.Producer.ProducerID > 0)
            {
                model.Producer.FirstName = model.FirstName;
                model.Producer.LastName = model.LastName;
                model.Producer.DateOfBirth = model.DateOfBirth;
                model.Producer.NumberOfMovies = (long)model.NumberOfMovies;
                db.SaveChanges();
            }
            return model;
        }

        /// <summary>
        /// Delete producer based on producer id
        /// </summary>
        /// <param name="producerId"></param>
        public void DeleteProducerByProducerId(long producerId)
        {
            tblProducer producerToDelete;
          
            using (var ctx = new MoviesEntities())
            {
                producerToDelete = ctx.tblProducers.Where(s => s.ProducerID == producerId).FirstOrDefault<tblProducer>();
            }

            using (var newContext = new MoviesEntities())
            {
                newContext.Entry(producerToDelete).State = EntityState.Deleted;

                newContext.SaveChanges();
            }

        }
    }
}