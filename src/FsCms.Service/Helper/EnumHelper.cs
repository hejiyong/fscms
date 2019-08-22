using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FsCms.Service.Helper
{
    public static class EnumHelper
    {
        /// <summary>
        /// 枚举类型转换为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string EnumConvertToString<T>(T en)
        {
            //方法一
            //return color.ToString();

            //方法二
            return Enum.GetName(en.GetType(), en);
        }
        public static T StringConvertToEnum<T>(string str)
        {
            T result = default(T);
            try
            {
                result = (T)Enum.Parse(typeof(T), str);
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        #region 根据枚举生成下拉列表数据源
        /// <summary>
        /// 根据枚举生成下拉列表的数据源
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="firstText">第一行文本(一般用于查询。例如：全部/请选择)</param>
        /// <param name="firstValue">第一行值(一般用于查询。例如：全部/请选择的值)</param>
        /// <returns></returns>
        public static IList<SelectListItem> ToSelectList(Type enumType
            , string firstText = "请选择"
            , string firstValue = "-1")
        {
            IList<SelectListItem> listItem = new List<SelectListItem>();

            if (enumType.IsEnum)
            {
                AddFirst(listItem, firstText, firstValue);

                Array values = Enum.GetValues(enumType);
                if (null != values && values.Length > 0)
                {
                    foreach (int item in values)
                    {
                        listItem.Add(new SelectListItem { Value = item.ToString(), Text = Enum.GetName(enumType, item) });
                    }
                }
            }
            else
            {
                throw new ArgumentException("请传入正确的枚举！");
            }
            return listItem;
        }

        static void AddFirst(IList<SelectListItem> listItem, string firstText, string firstValue)
        {
            if (!string.IsNullOrWhiteSpace(firstText))
            {
                if (string.IsNullOrWhiteSpace(firstValue))
                    firstValue = "-1";
                listItem.Add(new SelectListItem { Text = firstText, Value = firstValue });
            }
        }

        /// <summary>
        /// 根据枚举的描述生成下拉列表的数据源
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IList<SelectListItem> ToSelectListByDesc(
            Type enumType
            , string firstText = "请选择"
            , string firstValue = "-1"
            )
        {
            IList<SelectListItem> listItem = new List<SelectListItem>();

            if (enumType.IsEnum)
            {
                AddFirst(listItem, firstText, firstValue);
                string[] names = Enum.GetNames(enumType);
                names.ToList().ForEach(item =>
                {
                    string description = string.Empty;
                    var field = enumType.GetField(item);
                    object[] arr = field.GetCustomAttributes(typeof(DescriptionAttribute), true); //获取属性字段数组  
                    description = arr != null && arr.Length > 0 ? ((DescriptionAttribute)arr[0]).Description : item;   //属性描述  

                    listItem.Add(new SelectListItem() { Value = ((int)Enum.Parse(enumType, item)).ToString(), Text = description });
                });
            }
            else
            {
                throw new ArgumentException("请传入正确的枚举！");
            }
            return listItem;
        }
        #endregion

        #region 获取枚举的描述

        /// <summary>
        /// 获取枚举的描述信息
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>描述</returns>
        public static string GetDescription(this Enum enumValue)
        {
            string value = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return value;
            System.ComponentModel.DescriptionAttribute attr = (System.ComponentModel.DescriptionAttribute)objs[0];
            return attr.Description;
        }

        #endregion
    }
}
