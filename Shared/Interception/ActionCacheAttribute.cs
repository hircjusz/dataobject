using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using SoftwareMind.Utils.Extensions;

namespace SoftwareMind.Shared.Interception
{
    public class ActionCacheAttribute : ActionFilterAttribute
    {
        public bool IsEnabled
        {
            get
            {
#if DEBUG
                return true;
#else
                return true;
#endif
            }
        }

        public int Duration { get; set; }

        public ActionCacheAttribute()
        {
            this.Duration = 24 * 3600;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.IsEnabled)
            {
                var cache = filterContext.HttpContext.Cache;
                var cacheKey = this.GetCacheKey(filterContext.RequestContext);
                var cacheResult = cache.Get(cacheKey);

                if (cacheResult != null)
                {
                    filterContext.Result = (ActionResult)cacheResult;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var cache = filterContext.HttpContext.Cache;
            var cacheKey = this.GetCacheKey(filterContext.RequestContext);

            cache.Add(
                cacheKey,
                filterContext.Result,
                null,
                DateTime.Now.AddSeconds(this.Duration),
                Cache.NoSlidingExpiration,
                CacheItemPriority.Default,
                null
            );

            base.OnActionExecuted(filterContext);
        }

        private string GetCacheKey(RequestContext requestContext)
        {
            var formData = requestContext.HttpContext.Request.Form.ToEnumerable();
            var queryData = requestContext.HttpContext.Request.QueryString.ToEnumerable();
            var routeData = requestContext.RouteData;

            IEnumerable<string> data = new[] { new KeyValuePair<string, object>("area", routeData.DataTokens["area"]) }
                .Union(formData)
                .Union(queryData)
                .Union(routeData.Values)
                .OrderBy(x => x.Key)
                .Select(x => x.Key + "=" + x.Value);

            return string.Join("|", data);
        }
    }
}