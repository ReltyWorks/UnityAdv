using System.Collections.Generic;
using UnityEngine;

namespace PokemonGo
{
    [CreateAssetMenu(fileName = "MonsterSpecLibrary", menuName = "Scriptable Objects/MonsterSpecLibrary")]
    public class MonsterSpecLibrary : ScriptableObject
    {
        [SerializeField] MonsterSpec[] _monsterSpecs;

        public MonsterSpec GetRandom() => _monsterSpecs[Random.Range(0, _monsterSpecs.Length)];

        #region Runtime
        Dictionary<int, MonsterSpec> _table;

        public MonsterSpec GetById(int monsterId)
        {
            return _table[monsterId];
        }

        private void OnEnable()
        {
            BuildTable();
        }

        void BuildTable()
        {
            if (_monsterSpecs == null || _monsterSpecs.Length == 0)
                return;

            _table = new Dictionary<int, MonsterSpec>(_monsterSpecs.Length);

            for (int i = 0; i < _monsterSpecs.Length; i++)
            {
                if (_table.TryAdd(_monsterSpecs[i].id, _monsterSpecs[i]) == false)
                    throw new System.Exception($"id {_monsterSpecs[i].id} is wrong.");
            }

            Debug.Log("Built MonsterSpecLibrary table.");
        }
        #endregion
    }
}