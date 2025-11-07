using PokemonGo.Runtime.Services;
using PokemonGo.Runtime.Services.GPS;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;
using static PokemonGo.Runtime.Services.GoogleMap.GoogleMapUtils;
using static PokemonGo.Runtime.MapSystems.BlackboardInMap;
using UnityEngine.SceneManagement;
using System;

namespace PokemonGo.Runtime.MapSystems
{
    public class PlayerInMapController : MonoBehaviour
    {
        [Inject] InternetConnectionService _internetConnectionService;
        [Inject] GPSService _gpsService;
        [SerializeField] PlayerInMapView _view;
        LayerMask _battleMakerMask = 1 << 10; // BattleTargetInMap
        Vector3 _zeroOffset;
        bool _zeroed;
        Transform _camera;


        IEnumerator Start()
        {
            _camera = Camera.main.transform;
            yield return new WaitUntil(() => _internetConnectionService.isConnected);
            yield return new WaitUntil(() => _gpsService.isReady);
            yield return C_Zeroing();
            _view.onFightButtonClicked += OnFightButtonClicked;
        }

        IEnumerator C_Zeroing()
        {
            double lat = _gpsService.latitude;
            double lon = _gpsService.longitude;
            float z = LatToUnityY(lat, 0, zoom);
            float x = LonToUnityX(lon, 0, zoom);
            _zeroOffset = new Vector3(x, 0, z);
            _zeroed = true;
            yield break;
        }


        private void Update()
        {
            if (_zeroed == false)
                return;

            Move();
            SearchBattleTarget();
        }

        void Move()
        {
            double lat = _gpsService.latitude;
            double lon = _gpsService.longitude;

            float z = LatToUnityY(lat, 0, zoom);
            float x = LonToUnityX(lon, 0, zoom);

            transform.position = new Vector3(x, 0, z) - _zeroOffset;
            Debug.Log($"Moving... {lat}, {lon}");
        }

        void SearchBattleTarget()
        {
            Collider[] overlapped = Physics.OverlapSphere(transform.position, 5f, _battleMakerMask);

            if (overlapped.Length > 0)
            {
                Collider closest = overlapped[0];
                float sqrMin = (closest.transform.position - transform.position).sqrMagnitude;

                for (int i = 1; i < overlapped.Length; i++)
                {
                    float sqr = (overlapped[i].transform.position - transform.position).sqrMagnitude;

                    if (sqr < sqrMin)
                    {
                        sqrMin = sqr;
                        closest = overlapped[i];
                    }
                }

                _view.ShowBattleInfo(closest.transform.position + Vector3.up * 1.5f, 
                                     Quaternion.LookRotation(_camera.forward, 
                                                             _camera.up));
            }
            else
            {
                _view.HideBattleInfo();
            }
        }

        private void OnFightButtonClicked()
        {
            SceneManager.LoadScene("BattleField");
        }
    }
}