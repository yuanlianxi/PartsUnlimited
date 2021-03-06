﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching.Memory;
using PartsUnlimited.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "Announcement")]
    public class AnnouncementComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IMemoryCache _cache;

        public AnnouncementComponent(IPartsUnlimitedContext context, IMemoryCache memoryCache)
        {
            _db = context;
            _cache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var announcementProduct = await _cache.GetOrSet("announcementProduct", async context =>
            {
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return await GetLatestProduct();
            });

            return View(announcementProduct);
        }

        private Task<Product> GetLatestProduct()
        {
            var latestProduct = _db.Products.OrderByDescending(a => a.Created).FirstOrDefault();
            if ((latestProduct != null) && ((latestProduct.Created - DateTime.UtcNow).TotalDays <= 2))
            {
                return Task.FromResult(latestProduct);
            }
            else
            {
                return Task.FromResult<Product>(null);
            }
        }
    }
}
