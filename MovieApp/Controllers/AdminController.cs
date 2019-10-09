using System;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using MovieApp.Models;

namespace MovieApp.Controllers
{
    public class AdminController : Controller
    {
        /// <summary>
        /// Login view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login function to authenticate user.
        /// If user is an admin, then provide admin access else access to normal user area
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(AdminModel model)
        {
            try
            {
                if (!model.Email.ToLower().Equals("admin"))
                {
                    return RedirectToAction("Index", "Home");
                }

                model.CheckLogin(model);
                if (model.User != null && model.User.IsAdmin == true && model.User.UserId > 0)
                {
                    FormsAuthentication.SetAuthCookie(model.User.Name.ToString(), false);
                    return RedirectToAction("UserList", "Admin", new { id = model.User.UserId });
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login details");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error while login");
            }

            return View();
        }

        #region "Users"

        /// <summary>
        /// Authorize attribute on top of the method will authorize that user is logged in or not
        /// It'll retrieve list of users from the database
        /// Parameters are used to support sorting, searching and paging users in grid
        /// </summary>
        /// <param name="sortingOrder"></param>
        /// <param name="sortingDir"></param>
        /// <param name="searchText"></param>
        /// <param name="filterValue"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult UserList(string sortingOrder, string sortingDir, string searchText, string filterValue, int? pageNo)
        {
            UserModel model = new UserModel();
            try
            {
                ViewBag.CurrentSortOrder = sortingOrder;
                if (string.IsNullOrEmpty(sortingDir))
                    sortingDir = "ASC";
                if (pageNo == null)
                    pageNo = 1;
                ViewBag.sortingDir = sortingDir;
                if (searchText != null)
                {
                    pageNo = 1;
                }
                else
                {
                    searchText = filterValue;
                }
                ViewBag.FilterValue = searchText;
                model.GetAllUsers(model, sortingOrder, sortingDir, searchText);

                int pageSize = 5;
                int pageNumber = (pageNo ?? 1);

                return View(model.Users.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex) { }

            return View(model.Users.ToPagedList(1, 3));
        }

        /// <summary>
        /// View for creating user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult CreateUser()
        {
            UserModel model = new UserModel();
            return View(model);
        }

        /// <summary>
        /// It'll register new user in Users table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult CreateUser(UserModel model)
        {
            try
            {
                long userId = model.RegisterUser(model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error while updating profile.";
                ModelState.AddModelError("", "Error while updating details");
            }

            return RedirectToAction("UserList", new { sortingOrder = "Name", filterValue = "", pageNo = 1 });
        }

        /// <summary>
        /// It'll retrieve existing user from Users table based on passed users id for edit user page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult EditUser(long? id)
        {
            UserModel model = new UserModel();
            try
            {
                model.GetUserProfile(model, (long)id);
            }
            catch { }
            return View(model);
        }

        /// <summary>
        /// It'll modify existing user in Users table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult EditUser(UserModel model)
        {
            model.EmailId = model.Email;
            model.EditProfile(model, model.UserId);

            return RedirectToAction("UserList", new { sortingOrder = "Name", filterValue = "", pageNo = 1 });
        }

        /// <summary>
        /// It'll retrieve existing user from Users table based on passed users id and Remove it from database users table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult DeleteUser(long id)
        {
            UserModel model = new UserModel();
            try
            {
                model.DeleteUserByUserId(id);
            }
            catch { }
            return RedirectToAction("UserList", new { sortingOrder = "Name", filterValue = "", pageNo = 1 });
        }

        #endregion

        #region "Movies"

        /// <summary>
        /// Authorize attribute on top of the method will authorize that user is logged in or not
        /// It'll retrieve list of movies from the database
        /// Parameters are used to support sorting, searching and paging movies in grid
        /// </summary>
        /// <param name="sortingOrder"></param>
        /// <param name="sortingDir"></param>
        /// <param name="searchText"></param>
        /// <param name="filterValue"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult MovieList(string sortingOrder, string sortingDir, string searchText, string filterValue, int? pageNo)
        {
            MovieModel model = new MovieModel();
            try
            {
                ViewBag.CurrentSortOrder = sortingOrder;

                if (string.IsNullOrEmpty(sortingDir))
                    sortingDir = "ASC";
                if (pageNo == null)
                    pageNo = 1;

                ViewBag.sortingDir = sortingDir;
                if (searchText != null)
                {
                    pageNo = 1;
                }
                else
                {
                    searchText = filterValue;
                }
                ViewBag.FilterValue = searchText;
                model.GetAllMovies(model, sortingOrder, sortingDir, searchText);

                int pageSize = 5; 
                int pageNumber = (pageNo ?? 1); 

                return View(model.MovieWithProducer.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex) { }

            return View(model.MovieWithProducer.ToPagedList(1, 3));
        }

        /// <summary>
        /// View for creating movie with the list of existing producers for dropdown selection
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult CreateMovie()
        {
            MovieModel model = new MovieModel();
            model.Producers = model.GetProducers();
            return View(model);
        }

        /// <summary>
        /// It'll create a new movie in the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult CreateMovie(MovieModel model)
        {
            try
            {             
                long movieId = model.AddMovie(model);
            }
            catch (Exception ex) { }

            return RedirectToAction("MovieList", new { sortingOrder = "MovieName", filterValue = "" });
        }

        /// <summary>
        /// It'll retrieve movie based on id for edit page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult EditMovie(long? id)
        {
            MovieModel model = new MovieModel();
            try
            {
                model.GetMovie(model, (long)id);
                model.Producers = model.GetProducers();
            }
            catch { }
            return View(model);
        }

        /// <summary>
        /// It'll modify existing movie details in database movie table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult EditMovie(MovieModel model)
        {
            model.EditMovie(model, model.MovieID);

            return RedirectToAction("MovieList", new { sortingOrder = "MovieName", filterValue = "" });
        }

        /// <summary>
        /// It'll remove movie based on passed id from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult DeleteMovie(long id)
        {
            MovieModel model = new MovieModel();
            try
            {
                model.DeleteMovieByMovieId(id);

            }
            catch { }
            return RedirectToAction("MovieList", new { sortingOrder = "MovieName", filterValue = "" });
        }

        #endregion

        #region "Producers"

        /// <summary>
        /// Authorize attribute on top of the method will authorize that user is logged in or not
        /// It'll retrieve list of producers from the database
        /// Parameters are used to support sorting, searching and paging producers in grid
        /// </summary>
        /// <param name="sortingOrder"></param>
        /// <param name="sortingDir"></param>
        /// <param name="searchText"></param>
        /// <param name="filterValue"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult ProducerList(string sortingOrder, string sortingDir, string searchText, string filterValue, int? pageNo)
        {
            ProducerModel model = new ProducerModel();
            try
            {
                ViewBag.CurrentSortOrder = sortingOrder;

                if (string.IsNullOrEmpty(sortingDir))
                    sortingDir = "ASC";
                if (pageNo == null)
                    pageNo = 1;

                ViewBag.sortingDir = sortingDir;
                if (searchText != null)
                {
                    pageNo = 1;
                }
                else
                {
                    searchText = filterValue;
                }
                ViewBag.FilterValue = searchText;
                model.GetAllProducers(model, sortingOrder, sortingDir, searchText);

                int pageSize = 3; 
                int pageNumber = (pageNo ?? 1); 

                return View(model.Producers.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex) { }

            return View(model.Producers.ToPagedList(1, 3));
        }

        /// <summary>
        /// View for creating producer
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult CreateProducer()
        {
            ProducerModel model = new ProducerModel();
            return View(model);
        }

        /// <summary>
        /// It'll add new producer in producer table into database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult CreateProducer(ProducerModel model)
        {
            try
            {
                long producerId = model.AddProducer(model);
            }
            catch (Exception ex)
            {
            }

            return RedirectToAction("ProducerList", new { sortingOrder = "FirstName", filterValue = "" });
        }

        /// <summary>
        /// It'll retrieve existing producer based on id for edit producer page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult EditProducer(long? id)
        {
            ProducerModel model = new ProducerModel();
            try
            {
                model.GetProducer(model, (long)id);
            }
            catch { }
            return View(model);
        }

        /// <summary>
        /// It'll modify existing producer into producers table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult EditProducer(ProducerModel model)
        {
            model.EditProducer(model, model.ProducerID);

            return RedirectToAction("ProducerList", new { sortingOrder = "FirstName", filterValue = "" });
        }

        /// <summary>
        /// It'll remove producer based on passed id from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult DeleteProducer(long id)
        {
            ProducerModel model = new ProducerModel();
            try
            {
                model.DeleteProducerByProducerId(id);

            }
            catch { }
            return RedirectToAction("ProducerList", new { sortingOrder = "FirstName", filterValue = "" });
        }

        #endregion

        /// <summary>
        /// Restrict direct access to the admin url's
        /// If user tries to access admin pages directly from web browser, then it'll verify that the user is a admin or not
        /// If user is not an admin, user will be redirected to the index/home page
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            if (!actionName.ToLower().Equals("login"))
            {
                if (!System.Web.HttpContext.Current.User.Identity.Name.ToString().ToLower().Equals("admin"))
                {
                    filterContext.HttpContext.Response.Redirect("/Home/Index");
                }
            }
        }
        
    }
}