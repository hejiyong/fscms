using System;
using System.Web;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using FsCms.Entity;
using FsCms.Service;
using FsCms.Service.DAL;
using System.Collections.Generic;

namespace TestConsole
{
    class Program
    {
        public static IFreeSql mysql = new FreeSql.FreeSqlBuilder()
          .UseConnectionString(FreeSql.DataType.Sqlite, "Data Source=|DataDirectory|\\document.db;Pooling=true;Max Pool Size=10")
          .UseAutoSyncStructure(true)
          .UseMonitorCommand(
              cmd =>
              {
                  Trace.WriteLine(cmd.CommandText);
              }, //监听SQL命令对象，在执行前
              (cmd, traceLog) =>
              {
                  Console.WriteLine(traceLog);
              }) //监听SQL命令对象，在执行后
          .UseLazyLoading(true)
          .Build();

        static void Main(string[] args)
        {
            //var list = mysql.Select<ArticleContent>().Where(w => w.Id > 0).ToList();
            //UserMenuTest();
            //UserRoleTest();

            //初始化Freesql.wiki中的数据到article
            //读取文件
            string docpath = "E:\\GitHub\\FreeSqlCms\\src\\FsCms.Web\\wwwroot\\file\\FreeSql.wiki";

            List<dynamic> rows = new List<dynamic>();
            //判断文件夹是否存在
            if (System.IO.Directory.Exists(docpath))
            {
                string[] alllines = System.IO.File.ReadAllLines(docpath + "\\_Sidebar.md");
                for (int i = 0; i < alllines.Length; i++)
                {
                    var row = alllines[i];
                    //判断row的类型  文章还是分类
                    if (row.indexOf("##") != -1)
                    {
                        var typename = row.Replace("##", "").Trim();
                        rows.Add(new
                        {
                            title = typename,
                            datatype = 1,
                            level = 1,
                            url = "",
                            row = row
                        });
                    }
                    else if (row.indexOf("* ") != -1)
                    {
                        var trimCount = alllines[i].Length - alllines[i].TrimStart().Length;
                        Regex rg = new Regex(@"(?i)(?<=\[)(.*)(?=\])");
                        var filename = rg.Match(alllines[i]).Value;
                        var url = "";

                        if (string.IsNullOrEmpty(filename))
                        {
                            //filename = alllines[i].Trim('#').Trim().Trim('*').Trim();
                            filename = row.TrimStart('*').Trim();
                        }
                        else
                        {
                            url = Regex.Replace(alllines[i], @"(.*\()(.*)(\).*)", "$2");
                            int startIndex = url.indexOf("/FreeSql/wiki/");
                            if (startIndex != -1)
                                url = url.Substring(startIndex, url.Length - startIndex).Replace("/FreeSql/wiki/", "/FreeSql.wiki/");
                        }
                        if (url == "")
                        {
                            rows.Add(new { title = filename, datatype = 2, level = Convert.ToInt32(trimCount / 4) + 1, url = "", row = row });
                        }
                        else
                        {
                            rows.Add(new { title = filename, datatype = 3, level = Convert.ToInt32(trimCount / 4) + 1, url = url, row = row });
                        }
                    }
                }
            }
            Console.WriteLine("获取的记录条数：");

            Dictionary<int, long> level = new Dictionary<int, long> { { 0, 0 } };
            Dictionary<int, long> article = new Dictionary<int, long> { { 0, 0 } };
            long currTypeID = 0;
            int sortno = 0;
            foreach (var item in rows)
            {
                sortno++;
                if (level.ContainsKey(item.level) == false) level[item.level] = 0;
                Console.WriteLine($"title = {item.title}, datatype = {item.datatype}, level ={item.level}, url = {item.url}");
                if (item.datatype == 1) //分类
                {
                    currTypeID = level[item.level] = mysql.Insert<ArticleType>(new ArticleType
                    {
                        UpID = level[item.level - 1],
                        SortNum = sortno,
                        CreateBy = "system",
                        CreateDt = DateTime.Now,
                        Status = 1,
                        Tag = "",
                        TypeName = item.title,
                    }).ExecuteIdentity();
                }
                else 
                {
                    article[item.level] = mysql.Insert< ArticleContent>(new ArticleContent
                    {
                        TypeID = currTypeID,
                        SortNum = sortno,
                        CreateBy = "system",
                        CreateDt = DateTime.Now,
                        Status = 1,
                        Abstract = item.row,
                        DocContent = "",
                        EditorMode = 0,
                        OriginType = 1,
                        OriginUrl = item.url,
                        ParentArticleID = article[item.level - 1],
                        StarCount = 0,
                        WatchCount = 0,
                        Title = item.title,
                    }).ExecuteIdentity();
                }
            }
            Console.ReadKey();
        }

        static void UserMenuTest()
        {
            var select = mysql.Select<SysRoleMenu>();

            var list = select.LeftJoin<SysUserRole>((a, b) => a.RoleId == b.RoleId)
                .LeftJoin<SysMenu>((a, c) => a.MenuId == c.Id)
                .Where<SysUserRole>((rm, ur) => ur.UserId == 1);

            var sql = list.ToSql("a.Id,c.Id as MenuId, c.MenuName,c.MenuUrl,c.ParentID");
            Console.WriteLine(sql);
            var resultList = list.ToList();

            foreach (var item in resultList)
            {
                Console.WriteLine($"{item.Menu.Id}-{item.Menu.MenuName}-{item.Menu.MenuUrl}-{item.Menu.ParentID}");
            }

            var sql2 = list.ToSql();
        }

        static void UserRoleTest()
        {

            var select = mysql.Select<SysDictionary>();
            Console.WriteLine(select.OrderBy(s => s.CreateBy).OrderByDescending(s => s.CreateDt).ToSql());


            //var list = select.LeftJoin<SysUser>((a, b) => a.UserId == b.Id)
            //    .LeftJoin<SysRole>((a, c) => a.RoleId == c.Id)
            //    .Where(w => w.Status == 1);

            //var sql = list.ToSql("a.Id,b.UserName,c.RoleName");
            //Console.WriteLine(sql);
            //var resultList = list.ToList();

            //foreach (var item in resultList)
            //{
            //    Console.WriteLine($"{item.Id}-{item.Role.RoleName}-{item.User.UserName}");
            //}
            var a = "";
            //var sql2 = list.ToSql();
        }
    }
}
