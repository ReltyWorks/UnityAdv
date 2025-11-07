using UnityEngine;

namespace PokemonGo
{
    [CreateAssetMenu(fileName = "MonsterSpec", menuName = "Scriptable Objects/MonsterSpec")]
    public class MonsterSpec : ScriptableObject
    {
        [field: SerializeField] public int id { get; private set; }

        [Header("Stats")]
        [field : SerializeField] public int hpMax { get; private set; }

        [Header("GameObject")]
        [field : SerializeField] public GameObject prefab { get; private set; }
    }
}