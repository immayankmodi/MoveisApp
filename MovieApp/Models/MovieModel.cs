using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MovieApp.DataModel;
using System.Data;

namespace MovieApp.Models
{
    public class MovieModel
    {

        MoviesEntities db = new MoviesEntities();

        public long MovieID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Name: ")]
        public string MovieName { get; set; }

        [Required]
        [Display(Name = "Year: ")]
        public long Year { get; set; }

        [Required]
        [Display(Name = "Producer: ")]
        public long Producer { get; set; }
        public IEnumerable<SelectListItem> Producers { get; set; }

        public string ProducerName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Genre: ")]
        public string Genre { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Title: ")]
        public string Title { get; set; }

        public string PageCount { get; set; }

        public string PageNumber { get; set; }

        public tblMovy Movie { get; set; }

        public List<tblMovy> Movies { get; set; }

        public List<MovieModel> MovieWithProducer { get; set; }

        /// <summary>
        /// Get all movies list from database
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="sortingDir"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public MovieModel GetAllMovies(MovieModel model, string sortingOrder, string sortingDir, string searchText)
        {

            var result = from m in db.tblMovies select new MovieModel { MovieName = m.MovieName };

            if (!string.IsNullOrEmpty(searchText))
            {
                result = from m in db.tblMovies
                         join p in db.tblProducers on m.Producer equals p.ProducerID
                         where m.MovieName.ToLower().Contains(searchText.ToLower()) ||
                         p.FirstName.ToLower().Contains(searchText.ToLower()) ||
                         p.LastName.ToLower().Contains(searchText.ToLower()) ||
                         m.genre.ToLower().Contains(searchText.ToLower()) ||
                         m.title.ToLower().Contains(searchText.ToLower())
                         select new MovieModel
                         {
                             MovieName = m.MovieName,
                             Year = (long)m.Year,
                             Producer = (long)m.Producer,
                             ProducerName = p.FirstName + " " + p.LastName,
                             Genre = m.genre,
                             Title = m.title,
                             MovieID = m.MovieID
                         };
            }
            else
            {
                result = from m in db.tblMovies
                         join p in db.tblProducers on m.Producer equals p.ProducerID
                         select new MovieModel
                         {
                             MovieName = m.MovieName,
                             Year = (long)m.Year,
                             Producer = (long)m.Producer,
                             ProducerName = p.FirstName + " " + p.LastName,
                             Genre = m.genre,
                             Title = m.title,
                             MovieID = m.MovieID
                         };
            }

            model.MovieWithProducer = result.ToList();

            switch (sortingOrder)
            {
                case "MovieName":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.MovieWithProducer = model.MovieWithProducer.OrderBy(u => u.MovieName).ToList();
                    else
                        model.MovieWithProducer = model.MovieWithProducer.OrderByDescending(u => u.MovieName).ToList();
                    break;
                case "Year":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.MovieWithProducer = model.MovieWithProducer.OrderBy(u => u.Year).ToList();
                    else
                        model.MovieWithProducer = model.MovieWithProducer.OrderByDescending(u => u.Year).ToList();
                    break;
                case "ProducerName":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.MovieWithProducer = model.MovieWithProducer.OrderBy(u => u.ProducerName).ToList();
                    else
                        model.MovieWithProducer = model.MovieWithProducer.OrderByDescending(u => u.ProducerName).ToList();
                    break;
                case "Genre":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.MovieWithProducer = model.MovieWithProducer.OrderBy(u => u.Genre).ToList();
                    else
                        model.MovieWithProducer = model.MovieWithProducer.OrderByDescending(u => u.Genre).ToList();
                    break;
                case "title":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.MovieWithProducer = model.MovieWithProducer.OrderBy(u => u.Title).ToList();
                    else
                        model.MovieWithProducer = model.MovieWithProducer.OrderByDescending(u => u.Title).ToList();
                    break;
                default:
                    model.MovieWithProducer = model.MovieWithProducer.OrderByDescending(u => u.MovieID).ToList();
                    break;
            }




            return model;
        }

        /// <summary>
        /// Get all available producers for dropdown selection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetProducers()
        {
            var var_Producer = new tblProducer();
            var roles = db.tblProducers
                        .ToList()
                        .Select(x =>
                                new SelectListItem
                                {
                                    Value = x.ProducerID.ToString(),
                                    Text = x.FirstName.ToString() + " " + x.LastName.ToString()
                                });

            return new SelectList(roles, "Value", "Text");
        }

        /// <summary>
        /// Add new movie
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long AddMovie(MovieModel model)
        {
            long movieId = 0;
            tblMovy mvi = new tblMovy();
            mvi.MovieName = model.MovieName;
            mvi.Year = model.Year;
            mvi.genre = model.Genre;
            mvi.title = model.Title;
            mvi.Producer = model.Producer;
            db.tblMovies.Add(mvi);
            db.SaveChanges();
            movieId = mvi.MovieID;
            return movieId;
        }

        /// <summary>
        /// Get existing movie based on movie id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public MovieModel GetMovie(MovieModel model, long movieId)
        {
            model.Movie = db.tblMovies.Where(u => u.MovieID == movieId).FirstOrDefault();
            if (model.Movie != null && model.Movie.MovieID > 0)
            {
                model.MovieID = model.Movie.MovieID;
                model.MovieName = model.Movie.MovieName;
                model.Year = (long)model.Movie.Year;
                model.Producer = (long)model.Movie.Producer;
                model.Genre = model.Movie.genre;
                model.Title = model.Movie.title;
            }
            return model;
        }

        /// <summary>
        /// Edit existing movie
        /// </summary>
        /// <param name="model"></param>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public MovieModel EditMovie(MovieModel model, long movieId)
        {
            model.Movie = db.tblMovies.Where(u => u.MovieID == movieId).FirstOrDefault();
            if (model.Movie != null && model.Movie.MovieID > 0)
            {
                model.Movie.MovieName = model.MovieName;
                model.Movie.Year = (long)model.Year;
                model.Movie.Producer = (long)model.Producer;
                model.Movie.genre = model.Genre;
                model.Movie.title = model.Title;
                db.SaveChanges();
            }
            return model;
        }

        /// <summary>
        /// Delete movie from the database
        /// </summary>
        /// <param name="movieId"></param>
        public void DeleteMovieByMovieId(long movieId)
        {
            tblMovy movieToDelete;

            using (var ctx = new MoviesEntities())
            {
                movieToDelete = ctx.tblMovies.Where(s => s.MovieID == movieId).FirstOrDefault<tblMovy>();
            }

            using (var newContext = new MoviesEntities())
            {
                newContext.Entry(movieToDelete).State = EntityState.Deleted;

                newContext.SaveChanges();
            }

        }
    }
}