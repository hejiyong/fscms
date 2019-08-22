using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FsCms.Entity;
using FsCms.Entity.Enum;
using FsCms.Service.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FsCms.Web
{
    public class AdminBaseController : Controller, IToolBarActionButtonRight
    {
        public SysRoleMenuDAL base_SysRoleMenuDAL { get; set; }

        public SysRoleButtonDAL base_SysRoleButtonDAL { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public long UserID
        {
            get
            {
                return Int32.Parse(User.Identities.First(u => u.IsAuthenticated).FindFirst(ClaimTypes.Sid).Value);
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return User.Identities.First(u => u.IsAuthenticated).FindFirst(ClaimTypes.Name).Value;
            }
        }

        /// <summary>
        /// 所属角色
        /// </summary>
        public List<long> Role
        {
            get
            {
                return CurrentUser.userrole.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt64(s)).ToList();
            }
        }

        public SysUserView CurrentUser
        {
            get
            {

                return Newtonsoft.Json.JsonConvert.DeserializeObject<SysUserView>(User.Identities.First(u => u.IsAuthenticated).FindFirst(ClaimTypes.UserData).Value);
            }
        }



        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType
        {
            get
            {

                return CurrentUser.UserType;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (UserType == UserType.SuperUser)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                string controller = "";
                string action = "";
                string area = "";
                if (!IsRouteDataKeys(context.RouteData, ref area, ref controller, ref action))
                {
                    return;
                }
                //
                if (IsExcludeController(controller, action))
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    var rolemenus = base_SysRoleMenuDAL.Query((s) => this.Role.Contains(s.RoleId) && s.Menu.MenuUrl.IndexOf("/Admin/SysUser/") != 1);
                    if (rolemenus.list.Count() > 0)
                        base.OnActionExecuting(context);
                    else
                    {
                        context.Result = new ContentResult()
                        {
                            Content = "您无访问该页面的权限"
                        };
                        //filterContext.HttpContext.Response.Redirect("/Error");// ("<br />&nbsp;&nbsp;您无访问该页面的权限！");
                        this.ViewData["NoPurview"] = true; // Response.Redirect("/Service/Error");
                    }
                }
            }
        }

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    string controller = "";
        //    string action = "";
        //    var usertype = User.Identities.First(u => u.IsAuthenticated).FindFirst(ClaimTypes.UserData).Value;

        //    if (usertype == UserType.SuperUser.GetHashCode().ToString())
        //    {
        //        base.OnActionExecuting(filterContext);
        //    }
        //    else
        //    {
        //        if (!IsRouteDataKeys(filterContext.RouteData, ref controller, ref action))
        //        {
        //            return;
        //        }
        //        //
        //        if (IsExcludeController(controller, action))
        //        {
        //            base.OnActionExecuting(filterContext);
        //        }
        //        else
        //        {

        //            int result = 1;// BLLFactory.SysUserRoleBLL.IsControllerRightByUserID(this.UserID, controller, action);
        //            if (result == 1)
        //                base.OnActionExecuting(filterContext);
        //            else
        //            {
        //                filterContext.HttpContext.Response.Redirect("/Error");// ("<br />&nbsp;&nbsp;您无访问该页面的权限！");
        //                this.ViewData["NoPurview"] = true; // Response.Redirect("/Service/Error");
        //            }
        //        }
        //    }
        //}

        //判断是否存在Controller和Action，并且进行返回Controller和Action的值
        public bool IsRouteDataKeys(RouteData routeData, ref string area, ref string controller, ref string action)
        {
            bool flag = false;
            if (this.RouteData == null) return flag;
            //
            if (routeData.Values.ContainsKey("controller") && routeData.Values["controller"] != null &&
                  routeData.Values.ContainsKey("action") && routeData.Values["action"] != null &&
                  routeData.Values.ContainsKey("area") && routeData.Values["area"] != null)
            {
                controller = routeData.Values["controller"].ToString();
                action = routeData.Values["action"].ToString();
                area = routeData.Values["area"].ToString();
                if (!string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action))
                    flag = true;
            }
            return flag;
        }

        //排除的控制器
        private bool IsExcludeController(string controller, string action)
        {
            return controller == "Login" || controller == "Service" || controller == "Home";

        }

        public void LoadToolBarActionButtonRight()
        {
            if (this.UserType == UserType.SuperUser)
            {
                this.ViewData["IsAdmin"] = true;
            }
            else
            {
                if (this.RouteData.Values.ContainsKey("controller") && this.RouteData.Values["controller"] != null && this.RouteData.Values["controller"].ToString() == "CompanyObject")
                {
                    this.ViewData["button_Import"] = false;
                }
                //判断是否存在控制器和方法
                string controller = "";
                string action = "";
                string area = "";
                if (IsRouteDataKeys(this.RouteData, ref area, ref controller, ref action))
                {
                    //q =>this.UserID, "/" + controller + "/", RoleIDs);
                    var rolebtn = base_SysRoleButtonDAL.Query((s) => this.Role.Contains(s.RoleId)).list.Select(s => s.ButtonId).ToList();
                    var showbtns = new SysMenuButtonDAL().Query(s =>
                        s.Menu.MenuUrl.indexOf(area + "/" + controller + "/" + action) != -1 &&
                        rolebtn.Contains(s.Id));

                    foreach (SysMenuButton item in showbtns.list)
                    {
                        if (item.Category == (int)SysModuleButtonCategory.ToolbarButton)
                        {
                            SetPurviewViewData(item.ButtonCode, "button_");
                        }
                        else if (item.Category == (int)SysModuleButtonCategory.ListButton)
                        {
                            SetPurviewViewData(item.ButtonCode, "list_");
                        }
                        else if (item.Category == (int)SysModuleButtonCategory.GeneralButton)
                        {
                            SetPurviewViewData(item.ButtonCode, "button_");
                            SetPurviewViewData(item.ButtonCode, "list_");
                        }
                    }
                }
            }
        }

        //往ViewData中写入数据
        private void SetPurviewViewData(string ButtonCode, string Prefix)
        {
            if (ButtonCode.IndexOf("button_") != -1)
                this.ViewData[ButtonCode.ToLower()] = true;
            else
                this.ViewData[ButtonCode.ToLower()] = true; //Prefix + 
        }

    }
}
