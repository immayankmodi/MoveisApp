using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using MovieApp.Models;

namespace MovieApp.Controllers
{
    public class UserController : Controller
    {
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
        /// Login view page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Check for user login
        /// If user is a valid user redirect to a page which shows movies list else give error message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(UserModel model)
        {
            try
            {
                if (model.EmailId.ToLower().Equals("admin"))
                {
                    return RedirectToAction("Index", "Home");
                }

                model.CheckLogin(model);
                if (model.User != null && model.User.UserId > 0)
                {
                    FormsAuthentication.SetAuthCookie(model.User.Name.ToString(), false);
                    return RedirectToAction("MovieList", "User", new { id = model.User.UserId });
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

        /// <summary>
        /// Logout user session and also signout from FormsAuthentication
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// This method checks if email is available or take during user create/update
        /// </summary>
        /// <param name="email"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsEmailAvailable(string email, long UserId)
        {
            UserModel model = new UserModel();
            try
            {
                model.User = model.CheckEmail(email, UserId);
            }
            catch { }
            return Json(model.User == null);
        }
    }
}