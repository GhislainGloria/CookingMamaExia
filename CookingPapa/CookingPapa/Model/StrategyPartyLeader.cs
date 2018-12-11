﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
	class StrategyPartyLeader : Strategy
	{
		private static StrategyPartyLeader Instance = new StrategyPartyLeader();
        public static StrategyPartyLeader GetInstance()
        {
            return Instance;
        }      
        private StrategyPartyLeader() {} 

		private bool HasEverythingNeededToCook(AbstractActor self, Step step, List<AbstractActor> all)
		{
			AbstractActor nearest = self.FindClosest(step.Model.Workboard, all);
			bool hasUtensil = self.Items.Where(i => i.Name == step.Model.Utensil).ToList().Count > 0;
			bool hasIngredient = self.Items.Where(i => i.Name == step.Model.Ingredient).ToList().Count > 0;

			return self.EvaluateDistanceTo(nearest) <= 2 && hasUtensil && hasIngredient;
		}

		public override void Behavior(AbstractActor self, List<AbstractActor> all)
		{         
			// In the stack, there is the current step to execute
			if(self.Stack.Count > 0)
			{
				if(self.Stack.Count == 1)
				{
					// At this point, we know self just received the order, but didn't ask anything
					// Stack[0] -> the step to finish
					// Stack[1] -> PL has ordered the utensil
					// Stack[2] -> PL has ordered the ingredient

					self.Stack.Add(false); // Stack[1]
					self.Stack.Add(false); // Stack[2]
				}

				self.Busy = true;
				Step step = (Step)self.Stack[0];
                
				// Check if we:
                //  Don't have ordered an utensil
                //  Don't have the utensil in the inventory
                // If this applies, then order the utensil
				if(!(bool)self.Stack[1] && self.Items.Where(i => i.Name == step.Model.Utensil).ToList().Count == 0)
				{
					List<AbstractActor> kc = all.Where(a => a.Name == "kitchenclerk").ToList();
					foreach(AbstractActor a in kc)
					{
						if(!a.Busy)
						{                     
							AbstractActor target = a.FindNearestCarriableItem(step.Model.Utensil, all);
							if(target != null)
							{
								Console.WriteLine("Party Leader: I asked a clerk to fetch me a " + step.Model.Utensil);

								a.CommandList.Add(new CommandSetTarget(a, target));
								a.CommandList.Add(new CommandMove(a));
								a.CommandList.Add(new CommandGetItem(a, target, step.Model.Utensil));
								a.CommandList.Add(new CommandSetTarget(a, self));
								a.CommandList.Add(new CommandMove(a));
								a.CommandList.Add(new CommandGiveItem(a, self, step.Model.Utensil));

                                // We want to know if the clerk failed
								a.EventGeneric += self.StrategyCallback;
								self.Stack[1] = true;
							}
						}
					}
				}

				// Check if we:
                //  Don't have ordered the ingredient
                //  Don't have the ingredient in the inventory
                // If this applies, then order the ingredient
				if (!(bool)self.Stack[2] && self.Items.Where(i => i.Name == step.Model.Ingredient).ToList().Count == 0)
                {
                    List<AbstractActor> kc = all.Where(a => a.Name == "kitchenclerk").ToList();
                    foreach (AbstractActor a in kc)
                    {
                        if (!a.Busy)
                        {
							AbstractActor target = a.FindNearestCarriableItem(step.Model.Ingredient, all);
                            if (target != null)
                            {
								Console.WriteLine("Party Leader: I asked a clerk to fetch me a " + step.Model.Ingredient);

                                a.CommandList.Add(new CommandSetTarget(a, target));
                                a.CommandList.Add(new CommandMove(a));
                                a.CommandList.Add(new CommandGetItem(a, target, step.Model.Utensil));
                                a.CommandList.Add(new CommandSetTarget(a, self));
                                a.CommandList.Add(new CommandMove(a));
                                a.CommandList.Add(new CommandGiveItem(a, self, step.Model.Utensil));

                                // We want to know if the clerk failed
                                a.EventGeneric += self.StrategyCallback;
								self.Stack[2] = true;
                            }
                        }
                    }
                }

                // Get to the workbench
				if(self.BusyWalking)
				{
					if(self.CommandList.Count > 0)
					{
						self.CommandList[0].Execute();
						if(self.CommandList[0].IsCompleted)
						{
							self.CommandList.RemoveAt(0);
						}
					}
					else
					{
						Console.WriteLine("PL walking but no cmd? :thinking:");
					}
				}
				else
				{
					AbstractActor target = self.FindClosest(step.Model.Workboard, all);

					// If there's no closest, then fuck don't move, nothing can be done
					if(target == null)
					{
						return;
					}

					if(self.EvaluateDistanceTo(target) > 2)
					{
						self.Target = target;
						self.CommandList.Add(new CommandMove(self));
						return;
					}
				}

                // If we have everything needed to cook, then let's cook
				if(HasEverythingNeededToCook(self, step, all) && step.TimeSpentSoFar++ >= step.Model.Duration)
				{
					step.Complete();
					Console.WriteLine("Party Leader: I completed a step");
					self.Stack.RemoveAt(0);
					self.Busy = false;
				}
			}
			else
			{
				// The stack is empty, no more steps to process
				self.Busy = false;
			}
		}

		public override void ReactToEvent(AbstractActor self, MyEventArgs args)
		{
			switch (args.EventName)
			{
				case "CommandQueueFailed":
					string missingItem = (string)args.Arg2;

					self.BusyWaiting = false;

					AbstractActor failureMan = ((AbstractActor)args.Arg);
					failureMan.EventGeneric -= self.StrategyCallback;

					Console.WriteLine(self.Name + ": Fuck, my " + failureMan.Name + " failed to get me a " + missingItem);

					if (self.Stack.Count == 3)
					{
						if (missingItem == ((Step)self.Stack[0]).Model.Ingredient)
						{
							Console.WriteLine("Same " + self.Name + ": Nevermind, I'll ask for another one of this ingredient");
							self.Stack[2] = false;
						}
						else if (missingItem == ((Step)self.Stack[0]).Model.Utensil)
						{
							Console.WriteLine("Same " + self.Name + ": Nevermind, I'll ask for another one of this utensil");
                            self.Stack[1] = false;
						}
						else
						{
							Console.WriteLine("Same " + self.Name + ": But WTF, I don't need that anyway?? Pls fix or think");                     
						}
					}
					else
					{
						Console.WriteLine("Same " + self.Name + ": But WTF I'm not initialized??");                  
					}

					break;
			}
		}
	}
}
