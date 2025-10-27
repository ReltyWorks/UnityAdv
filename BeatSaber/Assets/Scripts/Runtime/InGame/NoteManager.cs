using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static BeatSaber.Runtime.InGame.InGameConstants;

namespace BeatSaber.Runtime.InGame
{
    public class NoteManager : MonoBehaviour
    {
        [SerializeField] GameObject _notePrefab;
        [SerializeField] Vector2 _spawnRange = new Vector2(1f, 1f);
        [SerializeField] Vector2 _spawnOffset = new Vector2(0f, 1f);
        [SerializeField] float _deadlineZ = -1f;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] InGameWorkflow _workflow;
        List<float> _peaks;
        Dictionary<Transform, float> _noteTable;
        List<Transform> _deadNotes;

        private void Start()
        {
            _peaks = ApplicationScope.selected.peaks;
            _workflow.onStateChanged += OnWorkflowStateChanged;
        }

        private void OnWorkflowStateChanged(InGameWorkflow.State newState)
        {
            switch (newState)
            {
                case InGameWorkflow.State.Start:
                    {
                        Play();
                    }
                    break;
                default:
                    break;
            }
        }

        private void Play()
        {
            StartCoroutine(C_SpawnNotes());
            StartCoroutine(C_MoveNotes());
        }

        IEnumerator C_SpawnNotes()
        {
            int index = 0;
            // Reserving : �ڷᱸ���� ����Ҷ� �ڷḦ ����� ������� �˼��ִ»�Ȳ������
            // Capacity �����Ҷ� Ȯ���س��� ���ʿ��� �����Ҵ�/�������÷���/ O(N) �˰��� ���� �������� �������ִ�.
            _noteTable = new Dictionary<Transform, float>(_peaks.Count);
            _deadNotes = new List<Transform>(20); // Dissolve ���� ��Ʈ�� �ִ� ��������

            while (index < _peaks.Count)
            {
                while (index < _peaks.Count)
                {
                    if (_audioSource.time >= _peaks[index])
                    {
                        GameObject note = Instantiate(_notePrefab, transform);
                        float x = Random.Range(-_spawnRange.x / 2f, +_spawnRange.x / 2f) + _spawnOffset.x;
                        float y = Random.Range(-_spawnRange.y / 2f, +_spawnRange.y / 2f) + _spawnOffset.y;
                        note.transform.position = new Vector3(x, y, (_peaks[index] + NOTE_SPAWN_DELAY) * _workflow.playSpeed);
                        _noteTable.Add(note.transform, _peaks[index]);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                yield return null;
            }
        }

        IEnumerator C_MoveNotes()
        {
            while (true)
            {
                _deadNotes.Clear();

                foreach (KeyValuePair<Transform, float> notePair in _noteTable)
                {
                    Vector3 position = notePair.Key.transform.localPosition;
                    position.z = (notePair.Value + NOTE_SPAWN_DELAY - _audioSource.time) * _workflow.playSpeed;
                    notePair.Key.localPosition = position;

                    if (position.z < _deadlineZ)
                    {
                        _deadNotes.Add(notePair.Key);
                    }
                }

                for (int i = 0; i < _deadNotes.Count; i++)
                {
                    _noteTable.Remove(_deadNotes[i]);
                }

                yield return null;
            }
        }
    }
}