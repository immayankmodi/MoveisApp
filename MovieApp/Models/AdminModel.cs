using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MovieApp.DataModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class AdminModel
    {

        MoviesEntities db = new MoviesEntities();

        [Required]
        [StringLength(50)]
        [Display(Name = "Email: ")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password: ")]
        public string Password { get; set; }

        public User User { get; set; }

        /// <summary>
        /// Check for user login with email and password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AdminModel CheckLogin(AdminModel model)
        {
            model.User = db.Users.Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefault();
            return model;
        }
        
    }
}