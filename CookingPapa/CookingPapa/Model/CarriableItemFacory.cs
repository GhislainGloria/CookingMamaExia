﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    class CarriableItemFacory : ICarriableItem
    {
        
        public ICarriableItem CreateCarriableItem()
        {
            return null;
        }

        public CarriableItemFacory()
        {
        }
    }
}
