
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using Newtonsoft.Json;
using System.IO;

namespace FsCms.Service
{
    public class SysTool
    {
        public static DateTime StrToDate(string strdate)
        {
            try
            {
                DateTime dts = DateTime.Parse(strdate);

                return dts;
            }
            catch
            {
                return DateTime.Parse("1900-01-01");
            }
        }
        public static Int32 StrToInt(string strint)
        {
            try
            {
                Int32 dts = Int32.Parse(strint);

                return dts;
            }
            catch
            {
                return 0;
            }
        }

        public static Int32 ObjToInt(object strint)
        {
            try
            {
                Int32 dts = Int32.Parse(strint.ToString());

                return dts;
            }
            catch
            {
                return 0;
            }
        }
        public static int Obj16ToInt(object strint)
        {
            try
            {
                int dts = int.Parse(strint.ToString());

                return dts;
            }
            catch
            {
                return 0;
            }
        }
        public static decimal Objtodec(object strint)
        {
            try
            {
                decimal dts = decimal.Parse(strint.ToString());

                return dts;
            }
            catch
            {
                return 0;
            }
        }
        public static double ObjToDouble(object strint)
        {
            try
            {
                double dts = double.Parse(strint.ToString());

                return dts;
            }
            catch
            {
                return 0;
            }
        }
        public static double StrToDouble(string strint)
        {
            try
            {
                double dts = double.Parse(strint);

                return dts;
            }
            catch
            {
                return 0;
            }
        }
        public static string ObjToStr(object obj)
        {
            try
            {
                string str = obj.ToString();

                return str;
            }
            catch
            {
                return "";
            }
        }
        public static long StrtoLong(string str)
        {
            try
            {
                long longs = long.Parse(str);

                return longs;
            }
            catch
            {
                return 0;
            }
        }
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }
            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                dt.Columns.Add(entityProperties[i].Name);
            }
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }

        public static string[] GetPropertyNameArray<T>()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string[] array = properties.Select(t => t.Name).ToArray();
            return array;
        }
        public static List<T> JSONStringToList<T>(string json)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }
        public static T JsonToModel<T>(string json) where T : class
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(json);
                object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));


                T t = o as T;
                return t;
            }
            catch (Exception ex)
            {


                return null;
            }


        }

        public static int InvoiceType(string FPZL)
        {
            int InvoiceType = 0;
            switch (FPZL)
            {
                case "s":
                    InvoiceType = 1;
                    break;
                case "c":
                    InvoiceType = 2;
                    break;
                case "j":
                    InvoiceType = 3;
                    break;
                case "p":
                    InvoiceType = 4;
                    break;
                default:
                    InvoiceType = 0;
                    break;
            }
            return InvoiceType;

        }
    }
}
