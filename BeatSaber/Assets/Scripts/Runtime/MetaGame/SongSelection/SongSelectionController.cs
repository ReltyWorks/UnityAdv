using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BeatSaber.ScriptableObjects;
using UnityEngine.SceneManagement;
using BeatSaber.Runtime.Infrastructure.DI.Scoping;

namespace BeatSaber.Runtime.MetaGame.SongSelection
{
    public class SongSelectionController : MonoBehaviour
    {
        [SerializeField] SongSelectionView _view;
        [SerializeField] SongLibrary _songLibrary;
        int _selectedIndex;
        [Inject] GameStateBlackboard _gameStateBlackboard;

        private void Start()
        {
            _view.nextRequested += Next;
            _view.prevRequested += Prev;
            _view.playRequested += Play;
            UpdateView();
        }

        private void Next()
        {
            if (_selectedIndex < _songLibrary.songSpecs.Length - 1)
                _selectedIndex++;
            else
                _selectedIndex = 0;

            UpdateView();
        }

        private void Prev()
        {
            if (_selectedIndex > 0)
                _selectedIndex--;
            else
                _selectedIndex = _songLibrary.songSpecs.Length - 1;

            UpdateView();
        }

        private void Play()
        {
            _gameStateBlackboard.selected = _songLibrary.songSpecs[_selectedIndex];
            SceneManager.LoadScene("InGame");
        }

        private void UpdateView()
        {
            SongSpec songSpec = _songLibrary.songSpecs[_selectedIndex];
            _view.RefreshCard(songSpec.title, songSpec.videoClip);
        }
    }
}
