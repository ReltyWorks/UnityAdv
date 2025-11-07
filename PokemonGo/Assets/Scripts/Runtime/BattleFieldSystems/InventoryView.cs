using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PokemonGo.Runtime.BattleFieldSystems
{
    public class InventoryView : MonoBehaviour
    {
        [Header("Canvas - Screen")]
        [SerializeField] Button _openBag;

        [Header("Canvas - Bag")]
        [SerializeField] Canvas _bag;
        [SerializeField] Button _closeBag;
        [SerializeField] Transform _content;

        List<InventoryItemSlot> _slots;
        const int TOTAL_SLOTS = 32;


        private void Awake()
        {
            InstantiateSlots();
        }

        private void Start()
        {
            _openBag.onClick.AddListener(() => _bag.enabled = true);
            _closeBag.onClick.AddListener(() => _bag.enabled = false);
        }

        void InstantiateSlots()
        {
            _slots = new List<InventoryItemSlot>(TOTAL_SLOTS);
            InventoryItemSlot original = _content.GetChild(0).GetComponent<InventoryItemSlot>();

            for (int i = 0; i < TOTAL_SLOTS; i++)
            {
                _slots.Add(Instantiate(original, _content));
            }

            original.gameObject.SetActive(false);
        }

        public void RefreshSlots(IEnumerable<(int id, int num)> items)
        {
            using (IEnumerator<InventoryItemSlot> e1 = _slots.GetEnumerator())
            using (IEnumerator<(int id, int num)> e2 = items.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (e2.MoveNext())
                    {
                        e1.Current.Refresh(null, e2.Current.num);
                    }
                    else
                    {
                        e1.Current.Refresh(null, 0);
                    }
                }
            }
        }
    }
}