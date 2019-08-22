using System;
using System.Collections.Generic;
using System.Text;

namespace FsCms.Entity.Enum
{
    public enum SysModuleButtonCategory
    {
        [EnumDescription("公共")]
        GeneralButton = 0,
        [EnumDescription("工具条按钮")]
        ToolbarButton = 1,
        [EnumDescription("列表按钮")]
        ListButton = 2
    }
}
