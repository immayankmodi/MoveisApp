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
    public class UserModel
    {

        MoviesEntities db = new MoviesEntities();

        public long UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Name: ")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Remote("IsEmailAvailable", "User", HttpMethod = "POST", ErrorMessage = "Email already exists. Please enter a different one.", AdditionalFields = "UserId")]
        [StringLength(50)]
        [Display(Name = "Email: ")]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        [Display(Name = "Email: ")]
        public string EmailId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Address: ")]
        public string Address { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password: ")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password: ")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        public bool IsAdmin { get; set; }

        public string PageCount { get; set; }

        public string PageNumber { get; set; }

        public User User { get; set; }

        public List<User> Users { get; set; }

        /// <summary>
        /// Get all users from the users database table
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="sortingDir"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public UserModel GetAllUsers(UserModel model, string sortingOrder, string sortingDir, string searchText)
        {

            if (!string.IsNullOrEmpty(searchText))
                model.Users = db.Users.Where(u => u.Name.ToLower().Contains(searchText.ToLower())
                || u.Email.ToLower().Contains(searchText.ToLower())
                || u.Address.ToLower().Contains(searchText.ToLower())
                ).Where(u => u.IsAdmin == false).ToList();
            else
                model.Users = db.Users.Where(u => u.IsAdmin == false).ToList();

            switch (sortingOrder)
            {
                case "Name":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.Users = model.Users.OrderBy(u => u.Name).ToList();
                    else
                        model.Users = model.Users.OrderByDescending(u => u.Name).ToList();
                    break;
                case "Email":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.Users = model.Users.OrderBy(u => u.Email).ToList();
                    else
                        model.Users = model.Users.OrderByDescending(u => u.Email).ToList();
                    break;
                case "Address":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.Users = model.Users.OrderBy(u => u.Address).ToList();
                    else
                        model.Users = model.Users.OrderByDescending(u => u.Address).ToList();
                    break;
                case "Password":
                    if (sortingDir.ToLower().Equals("asc"))
                        model.Users = model.Users.OrderBy(u => u.Password).ToList();
                    else
                        model.Users = model.Users.OrderByDescending(u => u.Password).ToList();
                    break;
                default:
                    model.Users = model.Users.OrderByDescending(u => u.UserId).ToList();
                    break;
            }

            return model;
        }

        /// <summary>
        /// It'll register new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long RegisterUser(UserModel model)
        {
            long userId = 0;
            User user = new User();
            user.Name = model.Name;
            user.Email = model.Email;
            user.Address = model.Address;
            user.Password = model.Password;
            user.IsAdmin = model.IsAdmin;
            db.Users.Add(user);
            db.SaveChanges();
            userId = user.UserId;
            return userId;
        }

        /// <summary>
        /// Check for user login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserModel CheckLogin(UserModel model)
        {
            model.User = db.Users.Where(u => u.Email == model.EmailId && u.Password == model.Password).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Get user's profile based on users id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserModel GetUserProfile(UserModel model, long userId)
        {
            model.User = db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            if (model.User != null && model.User.UserId > 0)
            {
                model.UserId = model.User.UserId;
                model.Name = model.User.Name;
                model.Email = model.User.Email;
                model.Password = model.User.Password;
                model.Address = model.User.Address;
            }
            return model;
        }

        /// <summary>
        /// Edit user's profile based on users id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserModel EditProfile(UserModel model, long userId)
        {
            model.User = db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            if (model.User != null && model.User.UserId > 0)
            {
                model.User.Name = model.Name;
                model.User.Password = model.Password;
                model.User.Address = model.Address;
                model.User.Email = model.EmailId;
                db.SaveChanges();
            }
            return model;
        }

        /// <summary>
        /// Check Email for passed user's id
        /// </summary>
        /// <param name="email"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public User CheckEmail(string email, long UserId)
        {
            User user = new User();
            user = db.Users.Where(p => p.Email == email).Where(p => p.UserId != UserId).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get user's profile based on email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetProfileByEmail(string email)
        {
            User user = new User();
            user = db.Users.Where(p => p.Email == email).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Delete user by user's id
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteUserByUserId(long userId)
        {
            User userToDelete;
            
            using (var ctx = new MoviesEntities())
            {
                userToDelete = ctx.Users.Where(s => s.UserId == userId).FirstOrDefault<User>();
            }
            
            using (var newContext = new MoviesEntities())
            {
                newContext.Entry(userToDelete).State = EntityState.Deleted;

                newContext.SaveChanges();
            }

        }
    }
}