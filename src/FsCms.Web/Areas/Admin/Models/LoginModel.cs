using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FsCms.Web.Areas.Admin.Models
{

    public class LoginModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string verifycode { get; set; }

        /// <summary>
        /// 登录成功-跳转页面
        /// </summary>
        public string returnUrl { get; set; }
    }
}
