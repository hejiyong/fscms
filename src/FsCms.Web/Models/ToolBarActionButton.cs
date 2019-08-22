using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FsCms.Web
{
    public enum Dirtect
    {

        //在公共按钮后添加
        Behind,
        //在公共按钮前添加
        Front
    }

    public class ToolBarActionButton
    {
        /// <summary>
        /// 必须用用于控件权限
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 按钮文本
        /// </summary>
        public string Text { set; get; }
        /// <summary>
        /// 自写样式
        /// </summary>
        public string Style { get; set; }
        /// <summary>
        /// 按钮的样式
        /// </summary>
        public string ClassName { set; get; }
        //public Dirtect Direct { set; get; }
        /// <summary>
        /// 按钮的扩展属性
        /// </summary>
        public Dictionary<string, object> Attributes { set; get; }
    }

    public interface IToolBarActionButtonRight
    {
        void LoadToolBarActionButtonRight();
    }
}
