using InspurOA.DAL;
using InspurOA.Identity.EntityFramework;
using InspurOA.Models;
using System;
using System.Data.Entity;

namespace InspurOA.Web.Models
{
    public class InspurDropCreateDatabaseIfModelChanges: DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        //如果Model变化，则重新执行该Seed方法，调用基类的Seed方法将保存当前的修改
        protected override void Seed(ApplicationDbContext context)
        {
            var user = new InspurUser { Id = Guid.NewGuid().ToString(), UserName = "Administrator", Email = "2016@InspurWorld.com", PasswordHash= "AAXop29zECFZ/hRQrti3Aa1oMVaPuPrzKbBYBOesFcdM/5AJunDRixQ12tmo/b+d3w==" };//Password=A123456789
            var role = new InspurIdentityRole { RoleId = Guid.NewGuid().ToString(), RoleCode = "Admin", RoleName = "管理员", RoleDescription = "管理系统的基本授权" };
            var userRole = new InspurIdentityUserRole { UserId = user.Id, RoleId = role.RoleId };

            context.Users.Add(user);
            context.Roles.Add(role);
            context.UserRoles.Add(userRole);

            base.Seed(context);
        }
    }
}