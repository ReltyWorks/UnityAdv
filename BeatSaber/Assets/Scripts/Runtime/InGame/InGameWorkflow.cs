using System;
using System.Collections;
using UnityEngine;
using static BeatSaber.Runtime.InGame.InGameConstants;

namespace BeatSaber.Runtime.InGame
{
    public class InGameWorkflow : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Standby,
            Start,
            WaitForFinished
        }

        public State current
        {
            get => _current;
            private set
            {
                if (_current == value)
                    return;

                _current = value;
                onStateChanged?.Invoke(_current);
            }
        }

        public int countDown { get; private set; } = 3;
        public float playSpeed { get; set; } = 3f;
        

        State _current;
        [SerializeField] AudioSource _audioSource;
        WaitForSeconds _waitFor1Seconds = new WaitForSeconds(1);

        public event Action<State> onStateChanged;

        IEnumerator Start()
        {
            _audioSource.clip = ApplicationScope.selected.audioClip;
            current = State.Standby;
            
            while (countDown > 0)
            {
                yield return _waitFor1Seconds;
                countDown--;
            }

            current = State.Start;
            yield return new WaitForSeconds(NOTE_SPAWN_DELAY);
            _audioSource.Play();

            current = State.WaitForFinished;
            // TODO : 노트 다 사라질때까지 대기
        }
    }
}