using PokemonGo.Runtime.Services;
using PokemonGo.Runtime.Services.GoogleMap;
using PokemonGo.Runtime.Services.GPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using VContainer;
using static PokemonGo.Runtime.Services.GoogleMap.GoogleMapUtils;

namespace PokemonGo.Runtime.MapSystems
{
    public class PlaceMarkerManager : MonoBehaviour
    {
        [Inject] InternetConnectionService _internetConnectionService;
        [Inject] GPSService _gpsService;
        [Inject] GooglePlacesService _googlePlacesService;
        [SerializeField] PlaceMarker _markerPrefab;
        List<PlaceMarker> _markerPool = new List<PlaceMarker>(20);
        List<PlaceMarker> _markersSpawned = new List<PlaceMarker>(20);


        IEnumerator Start()
        {
            yield return new WaitUntil(() => _internetConnectionService.isConnected);
            yield return new WaitUntil(() => _gpsService.isReady);
            PrepareMarkers();
            RefreshMarkers();
        }

        void PrepareMarkers()
        {
            for (int i = 0; i < 20; i++)
            {
                PlaceMarker marker = Instantiate(_markerPrefab, transform);
                marker.gameObject.SetActive(false);
                _markerPool.Add(marker);
            }
        }

        void PushToPool(PlaceMarker marker)
        {
            marker.gameObject.SetActive(false);
            _markerPool.Add(marker);
        }

        PlaceMarker PopFromPool()
        {
            int last = _markerPool.Count - 1;
            PlaceMarker marker = _markerPool[last];
            _markerPool.RemoveAt(last);
            marker.gameObject.SetActive(true);
            return marker;
        }

        void RefreshMarkers()
        {
            _googlePlacesService.SearchNearbyRequest(
                    _gpsService.latitude,
                    _gpsService.longitude,
                    200,
                    new List<string> { "restaurant" },
                    null,
                    null,
                    null,
                    20,
                    "POPULARITY",
                    "ko",
                    "KR",
                    GooglePlacesService.PlacesFields.DisplayName |
                    GooglePlacesService.PlacesFields.Types |
                    GooglePlacesService.PlacesFields.FormattedAddress |
                    GooglePlacesService.PlacesFields.Location,
                    PlaceMarkers
                );
        }

        void PlaceMarkers(IEnumerable<(string name, double latitude, double longitude)> places)
        {
            Debug.Log("Placing markers...");
            for (int i = 0; i < _markersSpawned.Count; i++)
            {
                PushToPool(_markersSpawned[i]);
            }

            _markersSpawned.Clear();

            foreach (var place in places)
            {
                PlaceMarker marker = PopFromPool();
                _markersSpawned.Add(marker);
                marker.ChangeName(place.name);
                marker.transform.localPosition =
                    new Vector3(LonToUnityX(place.longitude, _gpsService.origin.longitude, 16),
                                0f,
                                LatToUnityY(place.latitude, _gpsService.origin.latitude, 16));
            }
        }
    }
}