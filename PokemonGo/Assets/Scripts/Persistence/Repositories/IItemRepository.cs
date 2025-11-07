using PokemonGo.Persistence.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PokemonGo.Persistence.Repositories
{
    public interface IItemRepository
    {
        event Action<IEnumerable<Item>> OnSaved;

        Item GetItemById(int id);
        IEnumerable<Item> GetItems();
        void InsertItem(Item item);
        void DeleteItem(int id);
        void UpdateItem(Item item);
        void Save();
    }
}