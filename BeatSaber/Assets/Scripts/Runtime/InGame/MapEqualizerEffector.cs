using BeatSaber.Utils;
using UnityEngine;

namespace BeatSaber.Runtime.InGame
{
    public class MapEqualizerEffector : MonoBehaviour
    {
        [SerializeField] GameObject _unitPrefab;
        [SerializeField] float _unitLengthZ = 1f;
        [SerializeField] Vector3 _spawningOffset;
        [SerializeField, Range(64, 512)] int _spawnCount = 128;
        [SerializeField] float _scaleYMax = 5f;
        [SerializeField] float _gamma = 2f;
        [SerializeField] bool _doShuffle = true;
        [SerializeField] AudioSource _audioSource;

        Transform[] _spawnedUnits;
        float[] _spectrumData;
        

        private void Awake()
        {
            _spawnedUnits = new Transform[_spawnCount];
            _spectrumData = new float[_spawnCount];

            for (int i = 0; i < _spawnCount; i++)
            {
                Transform spawned = Instantiate(_unitPrefab, transform).transform;
                spawned.position = _spawningOffset + Vector3.forward * i * _unitLengthZ;
                _spawnedUnits[i] = spawned;
            }
        }

        private void Update()
        {
            _audioSource.GetSpectrumData(_spectrumData, 0, FFTWindow.Hamming);

            if (_doShuffle)
                _spectrumData.Shuffle();

            _spectrumData[0] = _spectrumData[_spawnCount - 1] = 0f;
            _spawnedUnits[0].localScale = _spawnedUnits[_spawnCount - 1].localScale = new Vector3(1f, 0.01f, 1f);

            for (int i = 1; i < _spawnCount - 1; i++)
            {
                _spectrumData[i] = Mathf.Log(1f + _spectrumData[i], 2) * _scaleYMax;
                _spectrumData[i] = Mathf.Pow(_spectrumData[i], _gamma); // 스펙트럼 증폭 상수
                _spectrumData[i] = (_spectrumData[i - 1] + _spectrumData[i] + _spectrumData[i + 1]) / 3f; // 평균값 보간
                float prevScaleY = _spawnedUnits[i].localScale.y;
                // 이전 프레임과 차이가 너무나면 정신없으니까 이전프레임과 보간
                _spawnedUnits[i].localScale = new Vector3(1, Mathf.Lerp(prevScaleY, _spectrumData[i], Time.deltaTime * 30f), 1);
            }
        }
    }
}