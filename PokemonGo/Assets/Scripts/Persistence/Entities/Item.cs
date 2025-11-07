using UnityEngine;

namespace PokemonGo.Persistence.Entities
{
    public struct Item
    {
        public Item(int id, int num)
        {
            Id = id;
            Num = num;
        }

        public static Item Empty => new Item(0, 0);

        public int Id;
        public int Num;

        public static bool operator==(Item op1, Item op2)
            => (op1.Id == op2.Id) && (op1.Num == op2.Num);

        public static bool operator!=(Item op1, Item op2)
            => !(op1 == op2);
    }
}