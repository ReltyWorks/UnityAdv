using System;
using UnityEngine;
using UnityEngine.UI;

namespace PokemonGo.Runtime.BattleFieldSystems
{
    public class PlayerInBattleFieldView : MonoBehaviour
    {
        [Header("Canvas - Screen")]
        [SerializeField] Button _leave;

        public event Action onLeaveButtonClicked;


        private void Awake()
        {
            _leave.onClick.AddListener(() => onLeaveButtonClicked?.Invoke());
        }
    }
}