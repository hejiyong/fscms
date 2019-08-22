using FsCms.Entity;
using FsCms.Service.DAL;
using FsCms.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FsCms.Web.Common.Help
{
    public class ReadWikiHepler
    {
        ArticleTypeDAL _ArticleTypeDAL = null;
        ArticleContentDAL _ArticleContentDAL = null;

        bool IsClearDataSync = true;

        Dictionary<int, long> level = new Dictionary<int, long> { { 0, 0 } };
        Dictionary<int, long> article = new Dictionary<int, long> { { 0, 0 } };

        List<ArticleType> ArticleTypeList = null;
        List<ArticleContent> ArticleContentList = null;

        public ReadWikiHepler()
        {
            _ArticleTypeDAL = new ArticleTypeDAL();
            _ArticleContentDAL = new ArticleContentDAL();
            IsClearDataSync = AppSettingsManager.Get("PathConfig:IsClearDataSync").ToLower() == "true" ? true : false;

            if (!IsClearDataSync)
            {
                ArticleTypeList = _ArticleTypeDAL.Query(s => s.Id > 0).list;
                ArticleContentList = _ArticleContentDAL.Query(s => s.Id > 0).list;
            }
        }

        private void ClearData()
        {
            if (IsClearDataSync)
            {
                _ArticleTypeDAL.Delete(s => true);
                _ArticleContentDAL.Delete(s => true);
            }
        }

        private long CreateOrUpdateType(dynamic item, int sortno)
        {
            long currTypeID = 0;
            if (IsClearDataSync || ArticleTypeList.Count(c => c.TypeName == item.title && c.Status == 1) == 0)
            {
                currTypeID = _ArticleTypeDAL.Insert(new ArticleType
                {
                    UpID = level[item.level - 1],
                    SortNum = sortno,
                    CreateBy = "system",
                    CreateDt = DateTime.Now,
                    Status = 1,
                    Tag = "",
                    TypeName = item.title,
                });
            }
            else
            {
                var updateItem = ArticleTypeList.Where(c => c.TypeName == item.title && c.Status == 1).FirstOrDefault();
                updateItem.UpID = level[item.level - 1];
                updateItem.SortNum = sortno;
                updateItem.UpdateBy = "system";
                updateItem.UpdateDt = DateTime.Now;
                updateItem.Status = 1;
                updateItem.Tag = "";
                updateItem.TypeName = item.title;
                _ArticleTypeDAL.Update(updateItem);
                currTypeID = updateItem.Id;
            }
            return currTypeID;
        }

        private long CreateOrUpdateContent(dynamic item, long currTypeID, int sortno)
        {
            long id = 0;
            if (IsClearDataSync || ArticleContentList.Count(c => c.Title == item.title && c.Status == 1) == 0)
            {
                id = _ArticleContentDAL.Insert(new ArticleContent
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
                    LevelNum = item.level,
                    ParentArticleID = article[item.level - 1],
                    StarCount = 0,
                    WatchCount = 0,
                    Title = item.title,
                });
            }
            else
            {
                var updateItem = ArticleContentList.Where(c => c.Title == item.title && c.Status == 1).FirstOrDefault();
                updateItem.TypeID = currTypeID;
                updateItem.SortNum = sortno;
                updateItem.UpdateBy = "system";
                updateItem.UpdateDt = DateTime.Now;
                updateItem.Abstract = item.row;
                updateItem.EditorMode = 0;
                updateItem.OriginType = 1;
                updateItem.OriginUrl = item.url;
                updateItem.LevelNum = item.level;
                updateItem.ParentArticleID = article[item.level - 1];
                updateItem.Title = item.title;
                id = ArticleContentList.Where(c => c.Title == item.title && c.Status == 1).FirstOrDefault().Id;
            }

            return id;
        }

        public void WikiToArticle()
        {
            ClearData();

            //初始化Freesql.wiki中的数据到article
            string docpath = AppSettingsManager.Get($"PathConfig:InitDocFiles"); // "E:\\GitHub\\FreeSqlCms\\src\\FsCms.Web\\wwwroot\\file\\FreeSql.wiki";

            List<dynamic> rows = new List<dynamic>();
            //判断文件夹是否存在
            if (System.IO.Directory.Exists(docpath))
            {
                string[] alllines = System.IO.File.ReadAllLines(docpath + "/_Sidebar.md");
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
            else throw new Exception("配置文件不存在" + docpath);

            long currTypeID = 0;
            int sortno = 0;
            foreach (var item in rows)
            {
                sortno++;
                if (level.ContainsKey(item.level) == false) level[item.level] = 0;
                if (item.datatype == 1) //分类
                {
                    currTypeID = level[item.level] = CreateOrUpdateType(item, sortno);
                }
                else
                {
                    article[item.level] = CreateOrUpdateContent(item, currTypeID, sortno);
                }
            }
        }
    }
}
