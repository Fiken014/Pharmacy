using System.Web;
using System.Web.Mvc;

namespace PIV_PF_KendallBonillaMiranda
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
