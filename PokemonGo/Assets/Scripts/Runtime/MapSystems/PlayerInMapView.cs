using System;
using UnityEngine;
using UnityEngine.UI;

namespace PokemonGo.Runtime.MapSystems
{
    public class PlayerInMapView : MonoBehaviour
    {
        [Header("Canvas - BattleInfo")]
        [SerializeField] Canvas _battleInfo;
        [SerializeField] Button _fight;

        public event Action onFightButtonClicked;


        private void Awake()
        {
            _fight.onClick.AddListener(() => onFightButtonClicked?.Invoke());
        }

        public void ShowBattleInfo(Vector3 position, Quaternion rotation)
        {
            _battleInfo.transform.position = position;
            _battleInfo.transform.rotation = rotation;
            _battleInfo.enabled = true;
        }

        public void HideBattleInfo()
        {
            _battleInfo.enabled = false;
        }
    }
}