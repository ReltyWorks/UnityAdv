using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PokemonGo.Runtime.BattleFieldSystems
{
    public class InventoryItemSlot : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] TMP_Text _itemNum;


        public void Refresh(Sprite sprite, int itemNum)
        {
            _icon.sprite = sprite;
            _itemNum.text = itemNum == 0 ? string.Empty : itemNum.ToString();
        }
    }
}
