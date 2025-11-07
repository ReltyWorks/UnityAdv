using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using PokemonGo.Persistence.Entities;
using System;

namespace PokemonGo.Persistence.Repositories
{
    public class MockItemRepository : IItemRepository
    {
        public MockItemRepository()
        {
            _context = new Context();
        }

        public class Context
        {
            public Context()
            {
                _path = Application.persistentDataPath + "/Item.json";
                Load();
            }


            public List<Item> Items { get; private set; }

            readonly string _path;
            

            public void Save()
            {
                string json = JsonConvert.SerializeObject(Items);
                File.WriteAllText(_path, json);
                Debug.Log("Saved item data.");
            }

            public void Load()
            {
                if (File.Exists(_path))
                {
                    string json = File.ReadAllText(_path);
                    Items = JsonConvert.DeserializeObject<List<Item>>(json);
                    Debug.Log("Loaded item data.");
                }
                else
                {
                    Items = new List<Item>();
                    Save();
                }
            }
        }

        Context _context;

        public event Action<IEnumerable<Item>> OnSaved;

        public void DeleteItem(int id)
        {
            int index = _context.Items.FindIndex(item => id == item.Id);

            if (index < 0)
                throw new System.Exception($"존재하지 않는 아이템을 지우려고 함. id :{id}");

            _context.Items.RemoveAt(index);
        }

        public Item GetItemById(int id)
        {
            return _context.Items.Find(x => id == x.Id);
        }

        public IEnumerable<Item> GetItems()
        {
            return _context.Items;
        }

        public void InsertItem(Item item)
        {
            int index = _context.Items.FindIndex(x => item.Id == x.Id);

            if (index < 0)
            {
                _context.Items.Add(item);
            }
            else
            {
                _context.Items[index] = new Item(item.Id, item.Num + _context.Items[index].Num);
            }
        }

        public void Save()
        {
            try
            {
                _context.Save();
                OnSaved?.Invoke(_context.Items);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to save data. {ex.Message}");
            }
        }

        public void UpdateItem(Item item)
        {
            int index = _context.Items.FindIndex(x => item.Id == x.Id);

            if (index < 0)
            {
                _context.Items.Add(item);
            }
            else
            {
                if (item.Num == 0)
                    _context.Items.RemoveAt(index);
                else if (item.Num > 0)
                    _context.Items[index] = item;
                else
                    throw new System.Exception($"아이템 갯수는 반드시 0 이상이어야합니다.");
            }
        }
    }
}