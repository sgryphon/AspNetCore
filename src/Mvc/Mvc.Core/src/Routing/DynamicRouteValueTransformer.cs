// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.Routing
{
    public abstract class DynamicRouteValueTransformer
    {
        public abstract Task<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values);
    }
}
