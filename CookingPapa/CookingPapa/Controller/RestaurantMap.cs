﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Controller
{
    public class RestaurantMap
    {

		public Size MapSize;
		public List<AbstractActor> Actors { get; set; }
		protected Dictionary<string, Point> CachedDictionnary;

		public RestaurantMap()
		{
			Actors = new List<AbstractActor>();
            CachedDictionnary = new Dictionary<string, Point>();

			if (MessageBox.Show(
				"Cliquez sur Oui pour la cuisine, Non pour la salle",
				"Sélection de la salle", 
				MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
				// Load the kitchen map
				MapSize = new Size(15, 15);
                AbstractActor actor2 = null;

				AbstractActor actor = ActorFactory.CreateActor("diver"); // Mobile
				actor.Position = new Point(10, 15);
				Actors.Add(actor);

				actor2 = ActorFactory.CreateActor("server counter");
				actor2.Position = new Point(15, 15);
				Actors.Add(actor2);

				actor = ActorFactory.CreateActor("partyleader");
				actor.Position = new Point(13, 13);
				Actors.Add(actor);

				actor = ActorFactory.CreateActor("kitchenclerk");
				actor.Position = new Point(1, 10);
				Actors.Add(actor);

				actor = ActorFactory.CreateActor("shed");
				actor.Position = new Point(2, 2);
				actor.Items.Add(UtensilFactory.CreateUtensil("fork"));
				Actors.Add(actor);

                actor = ActorFactory.CreateActor("chef"); // Immobile
                actor.Position = new Point(10, 10);
                actor2.EventGeneric += actor.StrategyCallback;
                Actors.Add(actor);
            }
            else
            {
				// Load the room map
				MapSize = new Size(40, 15);
            }
		}

		public Dictionary<string, Point> DisplayableData() {         
			CachedDictionnary.Clear();
			foreach(AbstractActor actor in Actors) {
				CachedDictionnary.Add(actor.Name, actor.Position);
			}

			return CachedDictionnary;
		}

		public void NextActorsTick()
		{
			foreach (AbstractActor actor in Actors)
			{
				Task task = Task.Factory.StartNew(() => actor.NextTick(Actors));
				ThreadPool.AddTask(task);
			}
		}
    }
}