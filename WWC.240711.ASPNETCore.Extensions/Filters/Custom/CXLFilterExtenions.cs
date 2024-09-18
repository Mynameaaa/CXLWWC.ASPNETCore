using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Filters.Custom
{
    public static class CXLFilterExtenions
    {
        public static List<Type> GetCXLFilter()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(p => typeof(IFilterMetadata).IsAssignableFrom(p)).ToList();
        }
    }
}
