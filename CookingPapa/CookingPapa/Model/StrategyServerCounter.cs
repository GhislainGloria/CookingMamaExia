﻿using System;
using System.Collections.Generic;

namespace Model
{
	public class StrategyServerCounter : Strategy
    {
		protected static StrategyServerCounter Instance = new StrategyServerCounter();
		public static StrategyServerCounter GetInstance() { return Instance; }
		protected StrategyServerCounter() { }

		public override void Behavior(AbstractActor self, List<AbstractActor> all)
		{
			Random random = new Random();
			if(random.Next(0, 2) == 1)
			{
				Table table = new Table(10, 1);
                List<DishModel> dish = new List<DishModel>
                {
                    DishModelList.GetAvailableDishes()[random.Next(0, DishModelList.GetAvailableDishes().Count)]
                };
                Order order = new Order(table, dish);
                self.TriggerEvent("order received", order);
			}
		}

		public override void ReactToEvent(AbstractActor self, MyEventArgs args)
		{
			throw new NotImplementedException();
		}
	}
}
