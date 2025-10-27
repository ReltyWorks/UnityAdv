using UnityEngine;

namespace BeatSaber.Runtime.InGame
{
    public class MapTileManager : MonoBehaviour
    {
        [SerializeField] GameObject _tilePrefab;
        [SerializeField] int _initSpawnCount;
        [SerializeField] float _tileScaleZ = 2f;
        [SerializeField] float _spawnOffsetZ = -10f;
        [SerializeField] float _speed = 3f;
        [SerializeField] AudioSource _audioSource;
        Transform[] _tiles;
        int[] _tileIndices;


        private void Start()
        {
            InitSpawn();
        }

        private void Update()
        {
            MoveTiles();
        }

        private void InitSpawn()
        {
            _tiles = new Transform[_initSpawnCount];
            _tileIndices = new int[_initSpawnCount];

            for (int i = 0; i < _initSpawnCount; i++)
            {
                GameObject tile = Instantiate(_tilePrefab, transform);
                _tiles[i] = tile.transform;
                float z = _spawnOffsetZ + i * _tileScaleZ;
                _tiles[i].localPosition = Vector3.forward * z;
                _tileIndices[i] = i;
            }
        }

        private void MoveTiles()
        {
            float scrollZ = _audioSource.time * _speed;
            float recycleThreshold = _spawnOffsetZ - _tileScaleZ; // 충분히 멀어진 타일은 다시 뒤로 보내서 재사용해야함

            for (int i = 0; i < _tiles.Length; i++)
            {
                float z = _spawnOffsetZ + _tileIndices[i] * _tileScaleZ - scrollZ;

                if (z < recycleThreshold)
                {
                    _tileIndices[i] += _initSpawnCount;
                    z = _spawnOffsetZ + _tileIndices[i] * _tileScaleZ - scrollZ;
                }

                _tiles[i].localPosition = Vector3.forward * z;
            }
        }
    }
}