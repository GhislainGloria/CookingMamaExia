﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model
{
    public class Dish : ACarriableItem
    {      
		public List<Step> Steps { get; set; }
		public int CompletedSteps { get; set; }
		protected Order Order;

		public Dish(DishModel model, Order order) : base("dish", 2)
		{
			Steps = new List<Step>();
			CompletedSteps = 0;
			Order = order;

			foreach(StepModel sm in model.ModelSteps)
			{
				Step step = new Step(sm, this);
				Steps.Add(step);
			}
		}

        public void MarkStepCompleted()
		{
			CompletedSteps++;
			if (CompletedSteps >= Steps.Count)
			{
				Order.MarkDishCompleted();
			}
		}
    }
}
