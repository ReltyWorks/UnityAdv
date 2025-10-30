using BeatSaber.Runtime.Infrastructure.DI.ComponentsBinding;
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
    public class SongSelectionView : ComponentsBindingBehaviour
    {
        [Header("Canvas - Main")]
        [Bind] VideoPlayer _videoPlayer;
        [Bind] TextMeshProUGUI _songTitle;
        [Bind] Button _next;
        [Bind] Button _prev;
        [Bind] Button _play;

        public event Action nextRequested;
        public event Action prevRequested;
        public event Action playRequested;


        protected override void Awake()
        {
            base.Awake();
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
