using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BeatSaber.Runtime.MetaGame.SongSelection
{
    public class SongSelectionView : MonoBehaviour
    {
        [Header("Canvas - Main")]
        [SerializeField] VideoPlayer _videoPlayer;
        [SerializeField] TextMeshProUGUI _songTitle;
        [SerializeField] Button _next;
        [SerializeField] Button _prev;
        [SerializeField] Button _play;

        public event Action nextRequested;
        public event Action prevRequested;
        public event Action playRequested;


        private void Awake()
        {
            _next.onClick.AddListener(() => nextRequested?.Invoke());
            _prev.onClick.AddListener(() => prevRequested?.Invoke());
            _play.onClick.AddListener(() => playRequested?.Invoke());
        }

        public void RefreshCard(string title, VideoClip videoClip)
        {
            _songTitle.text = title;
            _videoPlayer.Stop();
            _videoPlayer.clip = videoClip;
            _videoPlayer.Play();
        }
    }
}
