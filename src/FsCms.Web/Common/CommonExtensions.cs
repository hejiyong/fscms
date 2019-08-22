using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FsCms.Entity.Enum;
using FsCms.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using FsCms.Service.DAL;

namespace FsCms.Web
{

    public static class LoadToolBarActionLimit
    {

        //判断是否存在Controller和Action，并且进行返回Controller和Action的值
        public static bool IsRouteDataKeys(RouteData routeData, ref string area, ref string controller, ref string action)
        {
            bool flag = false;
            if (routeData == null) return flag;
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
        public static void load(IHtmlHelper htmlHelper, ClaimsPrincipal User)
        {
            var _user = Newtonsoft.Json.JsonConvert.DeserializeObject<SysUserView>(User.Identities.First(u => u.IsAuthenticated).FindFirst(ClaimTypes.UserData).Value);
            if (_user.UserType == UserType.SuperUser)
            {
                htmlHelper.ViewContext.FormContext.FormData["IsAdmin"] = true;
            }
            else
            {
                var _role = _user.userrole.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt64(s)).ToList();
                if (htmlHelper.ViewContext.RouteData.Values.ContainsKey("controller") && htmlHelper.ViewContext.RouteData.Values["controller"] != null && htmlHelper.ViewContext.RouteData.Values["controller"].ToString() == "CompanyObject")
                {
                    htmlHelper.ViewContext.FormContext.FormData["button_Import"] = false;
                }
                //判断是否存在控制器和方法
                string controller = "";
                string action = "";
                string area = "";
                if (IsRouteDataKeys(htmlHelper.ViewContext.RouteData, ref area, ref controller, ref action))
                {
                    //q =>this.UserID, "/" + controller + "/", RoleIDs);
                    var rolebtn = new SysRoleButtonDAL().Query((s) => _role.Contains(s.RoleId)).list.Select(s => s.ButtonId).ToList();
                    var showbtns = new SysMenuButtonDAL().Query(s =>
                        s.Menu.MenuUrl.IndexOf(area + "/" + controller + "/" + action) != -1 &&
                        rolebtn.Contains(s.Id));

                    foreach (SysMenuButton item in showbtns.list)
                    {
                        if (item.Category == (int)SysModuleButtonCategory.ToolbarButton)
                        {
                            SetPurviewViewData(htmlHelper, item.ButtonCode, "button_");
                        }
                        else if (item.Category == (int)SysModuleButtonCategory.ListButton)
                        {
                            SetPurviewViewData(htmlHelper, item.ButtonCode, "list_");
                        }
                        else if (item.Category == (int)SysModuleButtonCategory.GeneralButton)
                        {
                            SetPurviewViewData(htmlHelper, item.ButtonCode, "button_");
                            SetPurviewViewData(htmlHelper, item.ButtonCode, "list_");
                        }
                    }
                }
            }
        }

        //往ViewData中写入数据
        private static void SetPurviewViewData(IHtmlHelper htmlHelper, string ButtonCode, string Prefix)
        {
            if (ButtonCode.IndexOf("button_") != -1)
                htmlHelper.ViewContext.FormContext.FormData[ButtonCode.ToLower()] = true;
            else
                htmlHelper.ViewContext.FormContext.FormData[ButtonCode.ToLower()] = true; //Prefix + 
        }
    }

    public static class CommonExtensions
    {
        //static string ToolBarActionButtonTemplate = "<li class=\"{0}\" ><a href=\"javascript:void(0);\" {1}>{2}</a></li>";
        static string ToolBarActionButtonTemplate = " <button class=\"layui-btn layui-btn-normal layui-btn-sm\" type=\"button\" data-type=\"{0}\" {1}><i class=\"layui-icon\">&#xe642;</i>{2}</button>";
        //static string ListActionButtonTemplate = "<button type=\"button\" class=\"{0}\" {1} title=\"{2}\" {3}></button>";
        static string ListActionButtonTemplate = " <a class=\"layui-btn layui-btn-xs {0}\" lay-event=\"{0}\" {1} {3}>{2}</a>";


        //[[工具栏]]根据传入的参数构建一组工具条按钮
        public static HtmlString ListToolBarButtons(this IHtmlHelper htmlHelper, ClaimsPrincipal User, List<ToolBarActionButton> Buttons, bool isfullcreate = false)
        {
            //初始化工具条按钮的权限
            IToolBarActionButtonRight toolBarRight = htmlHelper.ViewContext as IToolBarActionButtonRight;
            if (toolBarRight != null)
            {
                toolBarRight.LoadToolBarActionButtonRight();
            }
            else
            {
                LoadToolBarActionLimit.load(htmlHelper, User);
            }
            StringBuilder toolBarBuilder = new StringBuilder();
            //输公共按钮前的按钮
            //ToolBarActionButton [] tempButtons = null;
            if (Buttons != null && Buttons.Count > 0)
            {
                foreach (ToolBarActionButton button in Buttons)
                {
                    if (!IsButtonRight(htmlHelper.ViewContext.FormContext, button.Name))
                    {
                        continue;
                    }
                    toolBarBuilder.AppendLine(BuilderToolBarActionButton(button, htmlHelper.ViewContext.FormContext));
                }
            }

            string resultHtml = "";
            if (isfullcreate && toolBarBuilder.Length != 0)
            {
                resultHtml = toolBarBuilder.ToString();
            }
            else resultHtml = toolBarBuilder.ToString();
            return new HtmlString(resultHtml);
        }


        //[列表工具栏]构建一个工具条按钮
        static string BuilderToolBarActionButton(ToolBarActionButton actionButton, FormContext controller)
        {
            if (actionButton == null || string.IsNullOrWhiteSpace(actionButton.Name) || string.IsNullOrWhiteSpace(actionButton.Text))
            {
                return string.Empty;
            }
            string attributeStr = null;
            if (!string.IsNullOrWhiteSpace(actionButton.Name))
            {
                attributeStr = string.Format(" name = \"{0}\" ", actionButton.Name);
            }
            if (actionButton.Attributes != null && actionButton.Attributes.Count > 0)
            {
                attributeStr += string.Join(" ", actionButton.Attributes.Select(attribute => string.Format("{0}=\"{1}\"", attribute.Key, attribute.Value)));
            }
            return string.Format(ToolBarActionButtonTemplate, actionButton.ClassName, attributeStr, actionButton.Text);
        }

        //[列表行按钮]构建一个工具条按钮
        static string BuilderListActionButton(ToolBarActionButton actionButton, FormContext controller)
        {
            if (actionButton == null || string.IsNullOrWhiteSpace(actionButton.Name) || string.IsNullOrWhiteSpace(actionButton.Text))
            {
                return string.Empty;
            }
            string attributeStr = null;
            if (!string.IsNullOrWhiteSpace(actionButton.Name))
            {
                attributeStr = string.Format(" name = \"{0}\" ", actionButton.Name);
            }
            if (actionButton.Attributes != null && actionButton.Attributes.Count > 0)
            {
                attributeStr += string.Join(" ", actionButton.Attributes.Select(attribute => string.Format("{0}=\"{1}\"", attribute.Key, attribute.Value)));
            }
            string style = "";
            if (actionButton.Style != null && actionButton.Attributes.Count > 0)
            {
                style = " style=\"" + actionButton.Style + "\"";
            }
            return string.Format(ListActionButtonTemplate, actionButton.ClassName, attributeStr, actionButton.Text, style);
        }

        //构建一组公共的工具条按钮
        static string BuilderToolBarCommonActionButton(FormContext controller)
        {
            //公共按钮 目前只包含 (增加，修改，删除)
            StringBuilder buttonsBuilder = new StringBuilder();
            List<ToolBarActionButton> buttons = new List<ToolBarActionButton>() {
                new ToolBarActionButton() { Name = "button_Add", ClassName = "li_1", Text = "新增", Attributes = new Dictionary<string, object> { { "onclick", "spf.listToolBarActions.add(this);" } } },
                new ToolBarActionButton() { Name = "button_Update", ClassName = "li_2", Text = "修改", Attributes = new Dictionary<string, object> { { "onclick", "spf.listToolBarActions.update(this);" } } },
                new ToolBarActionButton() { Name = "button_Delete", ClassName = "li_3", Text = "删除", Attributes = new Dictionary<string, object> { { "onclick", "spf.listToolBarActions.delete(this);" } } }
            };
            foreach (ToolBarActionButton button in buttons)
            {
                if (!IsButtonRight(controller, button.Name))
                {
                    continue;
                }
                buttonsBuilder.AppendLine(BuilderToolBarActionButton(button, controller));
            }
            return buttonsBuilder.ToString();
        }

        //[列表]根据传入的参数构建一组工具条按钮
        public static HtmlString ListViewButtons(this IHtmlHelper htmlHelper, ClaimsPrincipal User, List<ToolBarActionButton> Buttons)
        {
            //初始化工具条按钮的权限
            IToolBarActionButtonRight toolBarRight = htmlHelper.ViewContext as IToolBarActionButtonRight;
            if (toolBarRight != null)
            {
                toolBarRight.LoadToolBarActionButtonRight();
            }
            else
            {
                LoadToolBarActionLimit.load(htmlHelper, User);
            }
            StringBuilder toolBarBuilder = new StringBuilder();
            //输公共按钮前的按钮
            if (Buttons != null && Buttons.Count > 0)
            {
                foreach (ToolBarActionButton button in Buttons)
                {
                    if (!IsButtonRight(htmlHelper.ViewContext.FormContext, button.Name))
                    {
                        continue;
                    }
                    toolBarBuilder.AppendLine(BuilderListActionButton(button, htmlHelper.ViewContext.FormContext));
                }
            }
            return new HtmlString(toolBarBuilder.ToString());
        }

        public static string GeneralString(this string value, int n)
        {
            System.Text.StringBuilder a = new System.Text.StringBuilder(n * value.Length);
            for (int i = 0; i < n; i++)
            {
                a.Append(value);
            }
            return a.ToString();
        }

        static bool IsButtonRight(FormContext controller, string name)
        {
            //管理员获取全部全新
            if (controller.FormData.ContainsKey("IsAdmin")) return true;
            //
            string btnName = name.ToLower();
            if (!controller.FormData.ContainsKey(btnName))
            {
                return false;
            }
            bool right;
            bool isOK = bool.TryParse(controller.FormData[btnName].ToString(), out right);
            if (isOK)
            {
                return true;
            }
            return right;
        }
    }
}
