using UnityEngine;
using UnityEngine.UI;

namespace PokemonGo.Runtime.BattleFieldSystems
{
    public class MonsterInBattleFieldView : MonoBehaviour
    {
        [Header("Canvas - Status")]
        [SerializeField] Canvas _status;
        [SerializeField] Slider _hpBar;
        

        public void ShowStatus(Vector3 position, Quaternion rotation)
        {
            _status.transform.position = position;
            _status.transform.rotation = rotation;
            _status.enabled = true;
        }

        public void HideStatus()
        {
            _status.enabled = false;
        }

        public void InitializeHpBar(float maxHp)
        {
            _hpBar.minValue = 0;
            _hpBar.maxValue = maxHp;
        }

        public void UpdateHpBar(float hp)
        {
            _hpBar.value = hp;
        }
    }
}