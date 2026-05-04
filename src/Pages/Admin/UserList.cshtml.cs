using Kmums.Areas.Identity.Data;
using Kmums.Models.Amin;
using Kmums.Models.LinkedModels;
using Microsoft.AspNetCore.Mvc;

namespace Kmums.Pages.Admin
{
    public class UserListModel : AdminControlModel
    {
        private readonly DataContext _context;

        public UserListModel(DataContext context)
        {
            _context = context;
        }

        public IList<User_Roles> UserRole { get; set; }
        public ActionResult OnGetAsync()
        {
            IQueryable<User_Roles> user = from users in _context.Users
                                          join userrolesmapping in _context.UserRoles
                                          on users.Id equals userrolesmapping.UserId
                                          join role in _context.Roles
                                          on userrolesmapping.RoleId equals role.Id
                                          select new User_Roles
                                          {
                                              User = users,
                                              Roles = role
                                          };
            if (user == null)
                return NotFound();


            UserRole = user.ToList();
            return Page();

        }
    }
}
