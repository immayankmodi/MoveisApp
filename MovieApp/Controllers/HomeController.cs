using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieApp.Models;

namespace MovieApp.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Home page for the website
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            CreateAdminIfNotExist();
            return View();
        }

        /// <summary>
        /// If admin user is not created then "CreateAdminIfNotExist" method will create admin user in database
        /// </summary>
        public void CreateAdminIfNotExist()
        {
            UserModel model = new UserModel();

            model.User = model.CheckEmail("admin", 0);
            if (model.User != null && model.User.UserId > 0)
            {
            }
            else
            {
                model.Name = "admin";
                model.Email = "admin";
                model.Address = "admin";
                model.Password = "admin";
                model.IsAdmin = true;
                long userId = model.RegisterUser(model);
            }
        }

    }
}