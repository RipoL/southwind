﻿using Signum.Engine;
using Signum.Entities;
using Signum.Entities.Reflection;
using Signum.React.Filters;
using Southwind.Entities;
using Southwind.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Southwind.React.ApiControllers
{
    public class CatalogController : ApiController
    {
        [Route("api/catalog"), HttpGet, AllowAnonymous]
        public List<CategoryWithProducts> Catalog()
        {
            return ProductLogic.ActiveProducts.Value.Select(a => new CategoryWithProducts
            {
                category = a.Key,
                products = a.Value
            }).ToList();
        }

        public class CategoryWithProducts
        {
            public CategoryEntity category;
            public List<ProductEntity> products;
        }
    }
}