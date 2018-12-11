using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class IngredientFactory
    {
        public static Ingredient CreateIngredient(string ingredient)
        {
            switch (ingredient)
            {
                case "ingredient":
                    return new Ingredient(ingredient, 1, 1);

				case "carrot":
					return new Ingredient(ingredient, 1, 100000);

                default:
                    Console.WriteLine("Cet ingrédient n'existe pas");
                    return null;
            }
        }
    }
}
