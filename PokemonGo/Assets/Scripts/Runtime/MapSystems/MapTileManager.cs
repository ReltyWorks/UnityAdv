using PokemonGo.Runtime.Services;
using PokemonGo.Runtime.Services.GoogleMap;
using PokemonGo.Runtime.Services.GPS;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using VContainer;
using static PokemonGo.Runtime.Services.GoogleMap.GoogleMapUtils;

namespace PokemonGo.Runtime.MapSystems
{
    public class MapTileManager : MonoBehaviour
    {
        [Inject] InternetConnectionService _internetConnectionService;
        [Inject] GPSService _gpsService;
        [Inject] GoogleStaticMapService _googleStaticMapService;
        [SerializeField] MapTile _mapTilePrefab;
        [SerializeField] Transform _target;

        Vector3 _zeroOffset;
        Vector3 _center;
        MapLocation _locationOrigin;
        int _column; // maptile 그리드 인덱스
        int _row; // maptile 그리드 인덱스

        MapTile[,] _mapTiles = new MapTile[3, 3];
        MapTile[] _rollingBuffer = new MapTile[3];
        int[] _tilePatterns = { -1, 0, 1 };
        int _zoom = 16;
        MaterialPropertyBlock _materialPropertyBlock;
        static readonly int s_baseMapId = Shader.PropertyToID("_BaseMap");


        private void Awake()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }
        IEnumerator Start()
        {
            yield return new WaitUntil(() => _internetConnectionService.isConnected);
            yield return new WaitUntil(() => _gpsService.isReady);
            yield return C_SpawnTiles();
            yield return C_RefreshTileLoop();
        }

        IEnumerator C_SpawnTiles()
        {
            double lat = _gpsService.latitude;
            double lon = _gpsService.longitude;
            _locationOrigin = new MapLocation(lat, lon);
            float z = LatToUnityY(lat, 0, _zoom);
            float x = LonToUnityX(lon, 0, _zoom);

            _zeroOffset = new Vector3(x, 0, z);
            _center = _zeroOffset;

            for (int i = 0; i < _tilePatterns.Length; i++)
            {
                for (int j = 0; j < _tilePatterns.Length; j++)
                {
                    MapTile mapTile = Instantiate(_mapTilePrefab, transform);
                    mapTile.name = $"({j},{i})";
                    mapTile.transform.localPosition = new Vector3(x + (float)(UNITY_UNIT * _tilePatterns[i]), 
                                                                  0,
                                                                  z + (float)(UNITY_UNIT * _tilePatterns[j]))
                                                      - _zeroOffset;
                    mapTile.transform.localScale = Vector3.one * (float)(UNITY_UNIT / 10.0);
                    yield return _googleStaticMapService.C_LoadMap(
                        UnityYToLat(z + (j - 1) * (float)UNITY_UNIT, 0, _zoom),
                        UnityXToLon(x + (i - 1) * (float)UNITY_UNIT, 0, _zoom),
                        _zoom,
                        (int)GoogleMapUtils.PIXELS * Vector2Int.one,
                        texture => OnTextureLoaded(mapTile.GetComponent<Renderer>(), texture));

                    _mapTiles[j, i] = mapTile;
                }
            }
        }

        void OnTextureLoaded(Renderer renderer, Texture2D texture)
        {
            renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetTexture(s_baseMapId, texture);
            renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        IEnumerator C_RefreshTileLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                yield return C_CheckLocation();
            }
        }

        IEnumerator C_CheckLocation()
        {
            double lat = _gpsService.latitude;
            double lon = _gpsService.longitude;

            float z = LatToUnityY(lat, 0, _zoom);
            float x = LonToUnityX(lon, 0, _zoom);

            float dz = z - _center.z;
            float dx = x - _center.x;

            float dzAbs = Mathf.Abs(dz);

            if (dzAbs >= UNITY_UNIT)
            {
                // 어뷰징이라는 합리적인 의심이든다...
                if (dzAbs >= UNITY_UNIT * 2)
                {
#if UNITY_EDITOR
                    EditorApplication.ExitPlaymode();
#else
                    Application.Quit();
#endif
                    yield break;
                }

                yield return C_RollRow((int)Mathf.Sign(dz));
                yield break;
            }

            float dxAbs = Mathf.Abs(dx);

            if (dxAbs >= UNITY_UNIT)
            {
                // 어뷰징이라는 합리적인 의심이든다...
                if (dxAbs >= UNITY_UNIT * 2)
                {
#if UNITY_EDITOR
                    EditorApplication.ExitPlaymode();
#else
                    Application.Quit();
#endif
                    yield break;
                }

                yield return C_RollColumn((int)Mathf.Sign(dx));
                yield break;
            }
        }

        IEnumerator C_RollColumn(int directionX)
        {
            int length = _mapTiles.GetLength(1);
            int recycleIdx = directionX == +1 ? 0 : length - 1;
            int insertIdx = (length - 1) - recycleIdx;

            for (int j = 0; j < _mapTiles.GetLength(0); j++)
            {
                _rollingBuffer[j] = _mapTiles[j, recycleIdx];
            }

            if (directionX > 0)
            {
                for (int i = 1; i < length; i++)
                {
                    for (int j = 0; j < _mapTiles.GetLength(0); j++)
                    {
                        _mapTiles[j, i - 1] = _mapTiles[j, i];
                    }
                }
            }
            else
            {
                for (int i = length - 2; i >= 0; i--)
                {
                    for (int j = 0; j < _mapTiles.GetLength(0); j++)
                    {
                        _mapTiles[j, i + 1] = _mapTiles[j, i];
                    }
                }
            }

            for (int j = 0; j < _mapTiles.GetLength(0); j++)
            {
                _mapTiles[j, insertIdx] = _rollingBuffer[j];
            }

            _column += directionX;
            _center.x = (float)UNITY_UNIT * _column + _zeroOffset.x;

            double lat = _gpsService.latitude;
            double lon = _gpsService.longitude;
            float z = LatToUnityY(lat, 0, _zoom);
            float x = LonToUnityX(lon, 0, _zoom);

            for (int i = 0; i < _mapTiles.GetLength(1); i++)
            {
                for (int j = 0; j < _mapTiles.GetLength(0); j++)
                {
                    MapTile mapTile = _mapTiles[j, i];
                    mapTile.transform.localPosition = new Vector3(x + (float)(UNITY_UNIT * _tilePatterns[i]),
                                                                  0,
                                                                  z + (float)(UNITY_UNIT * _tilePatterns[j]))
                                                      - _zeroOffset;
                    mapTile.transform.localScale = Vector3.one * (float)(UNITY_UNIT / 10.0);

                    if (i == insertIdx)
                    {
                        yield return _googleStaticMapService.C_LoadMap(
                            UnityYToLat(z + (j - 1) * (float)UNITY_UNIT, 0, _zoom),
                            UnityXToLon(x + (i - 1) * (float)UNITY_UNIT, 0, _zoom),
                            _zoom,
                            (int)GoogleMapUtils.PIXELS * Vector2Int.one,
                            texture => OnTextureLoaded(mapTile.GetComponent<Renderer>(), texture));
                    }
                }
            }
        }

        IEnumerator C_RollRow(int directionZ)
        {
            int length = _mapTiles.GetLength(0);
            int recycleIdx = directionZ == +1 ? 0 : length - 1;
            int insertIdx = (length - 1) - recycleIdx;

            for (int i = 0; i < _mapTiles.GetLength(1); i++)
            {
                _rollingBuffer[i] = _mapTiles[recycleIdx, i];
            }

            if (directionZ > 0)
            {
                for (int j = 1; j < length; j++)
                {
                    for (int i = 0; i < _mapTiles.GetLength(1); i++)
                    {
                        _mapTiles[j - 1, i] = _mapTiles[j, i];
                    }
                }
            }
            else
            {
                for (int j = length - 2; j >= 0; j--)
                {
                    for (int i = 0; i < _mapTiles.GetLength(1); i++)
                    {
                        _mapTiles[j + 1, i] = _mapTiles[j, i];
                    }
                }
            }

            for (int i = 0; i < _mapTiles.GetLength(0); i++)
            {
                _mapTiles[insertIdx, i] = _rollingBuffer[i];
            }

            _row += directionZ;
            _center.z = (float)UNITY_UNIT * _row + _zeroOffset.z;

            double lat = _gpsService.latitude;
            double lon = _gpsService.longitude;
            float z = LatToUnityY(lat, 0, _zoom);
            float x = LonToUnityX(lon, 0, _zoom);

            for (int i = 0; i < _mapTiles.GetLength(1); i++)
            {
                for (int j = 0; j < _mapTiles.GetLength(0); j++)
                {
                    MapTile mapTile = _mapTiles[j, i];
                    mapTile.transform.localPosition = new Vector3(x + (float)(UNITY_UNIT * _tilePatterns[i]),
                                                                  0,
                                                                  z + (float)(UNITY_UNIT * _tilePatterns[j]))
                                                      - _zeroOffset;
                    mapTile.transform.localScale = Vector3.one * (float)(UNITY_UNIT / 10.0);

                    if (j == insertIdx)
                    {
                        yield return _googleStaticMapService.C_LoadMap(
                            UnityYToLat(z + (j - 1) * (float)UNITY_UNIT, 0, _zoom),
                            UnityXToLon(x + (i - 1) * (float)UNITY_UNIT, 0, _zoom),
                            _zoom,
                            (int)GoogleMapUtils.PIXELS * Vector2Int.one,
                            texture => OnTextureLoaded(mapTile.GetComponent<Renderer>(), texture));
                    }
                }
            }
        }
    }
}