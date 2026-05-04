using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Kmums.Areas.Identity.Data;
using Kmums.Models.User;
using Kmums.Models.LinkedModels;
using Kmums.Models.Amin;
using System.Diagnostics;

namespace Kmums.Pages.Admin
{
    public class UserListModel : AdminControlModel
    {
        private readonly DataContext _context;

        public UserListModel(DataContext context)
        {
            _context = context;
        }

        public IList<User_Roles> UserRole { get;set; }
        public ActionResult OnGetAsync()
        {
                var user = from users in _context.Users
                           join userrolesmapping in _context.UserRoles
                           on users.Id equals userrolesmapping.UserId
                           join role in _context.Roles
                           on userrolesmapping.RoleId equals role.Id
                           select new User_Roles
                           {
                               User = users,
                               Roles = role
                           };
            if(user == null)
                return NotFound();


            UserRole = user.ToList() ;
            return Page();
            
        }
    }
}
