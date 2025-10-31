using TMPro;
using UnityEngine;

namespace PokemonGo.Runtime.MapSystems
{
    public class PlaceMarker : MonoBehaviour
    {
        TMP_Text _name;

        private void Awake()
        {
            _name = GetComponentInChildren<TMP_Text>();
        }

        public void ChangeName(string name)
        {
            _name.text = name;
        }
    }
}