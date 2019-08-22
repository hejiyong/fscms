using FsCms.Entity;
using FsCms.Service.DAL;
using FsCms.Service.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FsCms.Web.Startups
{
    public class SeedData
    {
        public void Initialize()
        {
            var sysUserDAL = new SysUserDAL();
            var sysMenuDAL = new SysMenuDAL();
            var sysDictionary = new SysDictionaryDAL();
            var siteInfoDAL = new SiteInfoDAL();

            if (sysUserDAL.Count() == 0)
            {
                //初始化用户
                sysUserDAL.Insert(new Entity.SysUser
                {
                    UserName = "admin",
                    Password = "admin",
                    UserType = Entity.Enum.UserType.SuperUser,
                    CreateBy = "admin",
                    CreateDt = DateTime.Now,
                    Email = "admin@freesql.net"
                });
            }

            if (sysMenuDAL.Count() == 0)
            {
                var menuList = new List<SysMenu>();
                var menuid1 = sysMenuDAL.Insert(new SysMenu { MenuName = "权限管理", ParentID = 0, Description = "", MenuUrl = "#", IconUrl = "#", Sort = 1, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                var menuList1 = new List<SysMenu>();
                menuList1.Add(new SysMenu { MenuName = "用户管理", ParentID = menuid1, Description = "", MenuUrl = "/Admin/SysUser/Index", IconUrl = "#", Sort = 2, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                menuList1.Add(new SysMenu { MenuName = "角色管理", ParentID = menuid1, Description = "", MenuUrl = "/Admin/SysRole/Index", IconUrl = "#", Sort = 3, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                menuList1.Add(new SysMenu { MenuName = "菜单管理", ParentID = menuid1, Description = "", MenuUrl = "/Admin/SysMenu/Index", IconUrl = "#", Sort = 4, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                sysMenuDAL.BatchInsert(menuList1);

                var menuList2 = new List<SysMenu>();
                var menuid2 = sysMenuDAL.Insert(new SysMenu { MenuName = "文章管理", ParentID = 0, Description = "", MenuUrl = "#", IconUrl = "#", Sort = 1, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                menuList2.Add(new SysMenu { MenuName = "文章列表", ParentID = menuid2, Description = "", MenuUrl = "/Admin/Article/Index", IconUrl = "#", Sort = 6, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                menuList2.Add(new SysMenu { MenuName = "文章分类", ParentID = menuid2, Description = "", MenuUrl = "/Admin/ArticleType/Index", IconUrl = "#", Sort = 7, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                sysMenuDAL.BatchInsert(menuList2);

                var menuList3 = new List<SysMenu>();
                var menuid3 = sysMenuDAL.Insert(new SysMenu { MenuName = "系统设置", ParentID = 0, Description = "", MenuUrl = "#", IconUrl = "#", Sort = 99, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                menuList3.Add(new SysMenu { MenuName = "字典管理", ParentID = menuid3, Description = "", MenuUrl = "/Admin/SysDictionary/Index", IconUrl = "#", Sort = 9, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                menuList3.Add(new SysMenu { MenuName = "站点信息", ParentID = menuid3, Description = "", MenuUrl = "/Admin/SiteInfo/Index", IconUrl = "#", Sort = 9, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                menuList3.Add(new SysMenu { MenuName = "更新日志", ParentID = menuid3, Description = "", MenuUrl = "/Admin/SysUpdateLog/Index", IconUrl = "#", Sort = 9, Status = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                sysMenuDAL.BatchInsert(menuList3);
            }

            if (sysDictionary.Count() == 0)
            {
                var dic1 = (new SysDictionary { DictName = "权限按钮", DictNo = "PermissionButton", ParentID = 0, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                long id1 = sysDictionary.Insert(dic1);
                var dicList = new List<SysDictionary>();
                dicList.Add(new SysDictionary { DictName = "新增", DictNo = "add", ParentID = id1, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList.Add(new SysDictionary { DictName = "修改", DictNo = "update", ParentID = id1, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList.Add(new SysDictionary { DictName = "删除", DictNo = "delete", ParentID = id1, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList.Add(new SysDictionary { DictName = "导出", DictNo = "export", ParentID = id1, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList.Add(new SysDictionary { DictName = "导入", DictNo = "import", ParentID = id1, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList.Add(new SysDictionary { DictName = "打印", DictNo = "Print", ParentID = id1, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList.Add(new SysDictionary { DictName = "审核", DictNo = "Auth", ParentID = id1, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList.Add(new SysDictionary { DictName = "查看", DictNo = "show", ParentID = id1, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                sysDictionary.BatchInsert(dicList);

                var dic2 = (new SysDictionary { DictName = "站点设置", DictNo = "SiteSetting", ParentID = 0, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                long id2 = sysDictionary.Insert(dic2);

                var dic3 = (new SysDictionary { DictName = "简语描述", DictNo = "Index_Sketch", ParentID = id2, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                long id3 = sysDictionary.Insert(dic3);

                var dicList2 = new List<SysDictionary>();
                dicList2.Add(new SysDictionary { DictName = "字典配置1", DictNo = "01", ParentID = id3, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList2.Add(new SysDictionary { DictName = "字典配置2", DictNo = "02", ParentID = id3, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                dicList2.Add(new SysDictionary { DictName = "字典配置3", DictNo = "03", ParentID = id3, Sort = 1, CreateDt = DateTime.Now, CreateBy = "admin" });
                sysDictionary.BatchInsert(dicList2);
            }

            if (siteInfoDAL.Count() == 0)
            {
                var siteinfo = (new SiteInfo
                {
                    SiteName = "FreeSql",
                    Status = 1,
                    Headline = ".NETCore最方便的ORM",
                    Abstract = "FreeSql 是一个功能强大的 .NETStandard 库，用于对象关系映射程序(O/RM)，支持 .NETCore 2.1+ 或 .NETFramework 4.6.1+",
                    CreateDt = DateTime.Now,
                    CreateBy = "admin"
                });
                siteInfoDAL.Insert(siteinfo);
            }
        }
    }
}
