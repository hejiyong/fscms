using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FsCms.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using FsCms.Entity;
using FsCms.Service.DAL;
using FsCms.Entity.Enum;

namespace FsCms.Web.Areas.Admin.Controllers
{
    [Area(AreasName.Admin)]
    public class LoginController : Controller
    {
        public SysUserDAL SysUserDAL { get; set; }

        public SysRoleDAL SysRoleDAL { get; set; }

        public SysUserRoleDAL SysUserRoleDAL { get; set; }

        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Index(string returnUrl = "")
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost("LoginIn")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            var user = await SysUserDAL.GetByOneAsync(q => q.UserName == model.userName && q.Password == model.password);
            if (user != null)
            {
                var isSuperUser = user.UserType == UserType.SuperUser;
                //获取用户的权限列表
                var userRoles = await SysUserRoleDAL.QueryAsync(r => r.UserId == user.Id);
                if (userRoles.list != null && userRoles.list.Count() > 0 || isSuperUser)
                {
                    var roleCodes = "Admin";// 
                    var roleids = isSuperUser ? "0" : string.Join(",", userRoles.list.Select(s => s.RoleId).ToArray());

                    //用户标识
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.UserData, Newtonsoft.Json.JsonConvert.SerializeObject(new SysUserView{
                        Id = user.Id,
                        UserName = user.UserName,
                        RealName = user.RealName,
                        UserType = user.UserType,
                        userrole = roleids,
                    })));
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleCodes));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    return Json(new { status = 1 });
                }
                else
                {
                    return Json(new { status = 2, errorMessage = "账号未设置角色" });
                }
            }
            else
            {
                return Json(new { status = 2, errorMessage = "用户不存在或密码错误" });
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
