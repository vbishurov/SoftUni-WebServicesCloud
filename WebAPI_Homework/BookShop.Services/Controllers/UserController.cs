namespace BookShop.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Data;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Models.BindingModels;

    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private readonly BookShopContext context = BookShopContext.Create();

        [Route("{username}/purchases")]
        public IHttpActionResult GetUserPurchases(string username)
        {
            var user = this.context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return this.BadRequest("User " + username + " does not exist.");
            }

            var userPurchases = this.context.Purchases.Where(p => p.User.Id == user.Id).Select(p => new
            {
                p.Date,
                p.User.UserName,
                p.Book.Title,
                p.Price,
                p.IsRecalled
            }).OrderBy(p => p.Date);

            return this.Ok(userPurchases);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{username}/roles")]
        public IHttpActionResult AddRoleToUser([FromUri]string username, [FromBody]RoleBindingModel roleBindingModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = this.context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return this.BadRequest("User " + username + " does not exist");
            }

            var role = this.context.Roles.FirstOrDefault(r => r.Name == roleBindingModel.Name);

            if (role == null)
            {
                this.context.Roles.Add(new IdentityRole()
                {
                    Name = roleBindingModel.Name
                });

                this.context.SaveChanges();

                role = this.context.Roles.FirstOrDefault(r => r.Name == roleBindingModel.Name);
            }


            user.Roles.Add(new IdentityUserRole()
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            try
            {
                this.context.SaveChanges();

                return this.Ok(string.Format("Role {0} added to user {1}", roleBindingModel.Name, username));
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{username}/roles")]
        public IHttpActionResult RemoveUserFromRole([FromUri]string username, [FromBody]RoleBindingModel roleBindingModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = this.context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return this.BadRequest("User " + username + " does not exist");
            }

            var role = this.context.Roles.FirstOrDefault(r => r.Name == roleBindingModel.Name);

            if (role == null)
            {
                return this.BadRequest("Role " + roleBindingModel.Name + " does not exist");
            }

            var userRole = user.Roles.FirstOrDefault(r => r.RoleId == role.Id);

            if (userRole == null)
            {
                return this.BadRequest(string.Format("User {0} does not have role {1}", username, role.Name));
            }

            user.Roles.Remove(userRole);

            try
            {
                this.context.SaveChanges();

                return this.Ok(string.Format("User {0} removed from role {1}", username, roleBindingModel.Name));
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }
    }
}