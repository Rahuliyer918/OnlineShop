﻿using System;
using System.Collections.Generic;

namespace OnlineShop.Models.Db;

public partial class ProductsGallery
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? ImageName { get; set; }
}
