using PokemonGo.Persistence.Entities;
using System;
using System.Collections.Generic;

namespace PokemonGo.Persistence.Repositories
{
    public class ItemRepository : IItemRepository
    {
        public event Action<IEnumerable<Item>> OnSaved;

        public void DeleteItem(int id)
        {
            throw new System.NotImplementedException();
        }

        public Item GetItemById(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Item> GetItems()
        {
            throw new System.NotImplementedException();
        }

        public void InsertItem(Item item)
        {
            throw new System.NotImplementedException();
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateItem(Item item)
        {
            throw new System.NotImplementedException();
        }
    }
}
