using Microsoft.AspNetCore.Mvc;
using WWC._240711.ASPNETCore.Extensions.Swagger.Custom;

namespace WWC._240711.ASPNETCore.Extensions
{
    [Route("cxlApp")]
    [CXLApiExplorerSettings("App", nameof(SwaggerGroup.Order))]
    public class CXLApp : ICXLController
    {

        [HttpGet]
        [CXLRoute("myGet", ApiName = "Get方法名字")]
        public string Get()
        {
            return "Get";
        }

        [HttpPost]
        public string Post(string first, string last)
        {
            return first + "--" + last;
        }

    }
}
