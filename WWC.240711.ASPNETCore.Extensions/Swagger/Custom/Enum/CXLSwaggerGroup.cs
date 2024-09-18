using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions
{
    public enum CXLSwaggerGroup
    {
        [EnumDescription("默认分组")]
        Default = 0,

        [EnumDescription("用户分组")]
        User = 1,

        [EnumDescription("库存分组")]
        Stock = 2,

        [EnumDescription("订单分组")]
        Order = 3,

        [EnumDescription("管理员分组")]
        Admin = 4,

        [EnumDescription("后台分组")]
        Back = 5,
    }
}
