﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class CartItemVM
    {
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public float ProductPrice { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Image { get; set; }
        public float SubTotal { get; set; }

        public static float TotalPrice = 0;

    }
}
