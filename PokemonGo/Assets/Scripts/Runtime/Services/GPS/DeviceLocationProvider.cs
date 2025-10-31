using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

namespace PokemonGo.Runtime.Services.GPS
{
    public class DeviceLocationProvider : MonoBehaviour, ILocationProvider
    {
        public bool isRunning { get; private set; }

        public double latitude { get; private set; }

        public double longitude { get; private set; }

        public double altitude { get; private set; }

        public (double latitude, double longitude) origin { get; private set; }

        public event Action<double, double, double, float, double> onLocationUpdated;

        private float _resendTime = 1.0f;
        private float _desireAccuracyInMeters = 5f;
        private float _updateDistanceInMeters = 5f;
        private float _elapsedWaitTime = 0f;
        private float _maxWaitTime = 10f;

        public void StartService()
        {
            StartCoroutine(C_RefreshGPSData());
        }

        public void StopService()
        {
            isRunning = false;
            StopAllCoroutines();
        }

        IEnumerator C_RefreshGPSData()
        {
            // GPS 정보 접근 권한 확인 및 요청
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation) == false)
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.FineLocation));
            }

            // 사용자가 GPS 켜놨는지
            if (Input.location.isEnabledByUser == false)
            {
                Debug.LogError("GPS 장치 꺼짐");
                Application.Quit(); // 유저한테 GPS 켜달라고 요청하는 팝업 띄워줘야함 
                yield break;
            }

            Input.location.Start(_desireAccuracyInMeters, _updateDistanceInMeters);

            // 초기화 될때까지 기다림
            while (Input.location.status == LocationServiceStatus.Initializing &&
                   _elapsedWaitTime < _maxWaitTime)
            {
                yield return new WaitForSeconds(1.0f);
                _elapsedWaitTime += 1.0f;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                // TODO : 실패시 예외처리
                throw new Exception("LocationServiceStatus == Failed");
            }

            if (_elapsedWaitTime >= _maxWaitTime)
            {
                // TODO : 타임아웃 에러 예외처리
                throw new Exception("LocationService request timeout");
            }

            LocationInfo locationInfo = Input.location.lastData;
            origin = (locationInfo.latitude, locationInfo.longitude);
            isRunning = true;

            while (true)
            {
                locationInfo = Input.location.lastData;
                latitude = locationInfo.latitude;
                longitude = locationInfo.longitude;
                altitude = locationInfo.altitude;
                onLocationUpdated?.Invoke(latitude, longitude, altitude, locationInfo.horizontalAccuracy, locationInfo.timestamp);
                yield return new WaitForSeconds(_resendTime);
            }
        }
    }
}
