using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PokemonGo.Runtime.Services.GoogleMap;
using static PokemonGo.Runtime.Services.GoogleMap.GoogleMapUtils;

namespace PokemonGo.Runtime.Services.GPS
{
    public class MockLocationProvider : MonoBehaviour, ILocationProvider
    {
        public bool isRunning { get; private set; }
        public double latitude { get; set; } = 37.4946;
        public double longitude { get; set; } = 127.0276056;
        public double altitude { get; set; }

        public (double latitude, double longitude) origin => (37.4946, 127.0276056);

        Transform _mockPlayer;
        float _mockPlayerMoveSpeed = 100f;
        public event Action<double, double, double, float, double> onLocationUpdated;


        private void Update()
        {
            if (isRunning == false)
                return;

            MoveMockPlayer();
            UpdateLocation();
        }

        public void StartService()
        {
            isRunning = true;
            SpawnMockPlayer();
        }

        public void StopService()
        {
            isRunning = false;
        }

        private void SpawnMockPlayer()
        {
            _mockPlayer = new GameObject("MockPlayer").transform;
        }

        private void MoveMockPlayer()
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            Vector3 velocity = direction * _mockPlayerMoveSpeed;
            _mockPlayer.position += velocity * Time.deltaTime;
        }

        private void UpdateLocation()
        {
            latitude = origin.latitude + UnityYToLat(_mockPlayer.position.z, 0, 16);
            longitude = origin.longitude + UnityXToLon(_mockPlayer.position.x, 0, 16);
            onLocationUpdated?.Invoke(latitude, longitude, 0, 0, 0);
            Debug.Log($"Updated location {latitude}, {longitude}");
        }
    }
}
