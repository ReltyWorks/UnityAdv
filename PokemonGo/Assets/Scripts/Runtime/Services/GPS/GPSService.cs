using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace PokemonGo.Runtime.Services.GPS
{
    public class GPSService : MonoBehaviour
    {
        public bool isReady
        {
            get
            {
                if (_locationProvider == null)
                    return false;

                return _locationProvider.isRunning;
            }
        }

        public double latitude => _locationProvider.latitude;
        public double longitude => _locationProvider.longitude;
        public double altitude => _locationProvider.altitude;
        public (double latitude, double longitude) origin => _locationProvider.origin;

        [Inject] ILocationProvider _locationProvider;


        private void Start()
        {
            _locationProvider.StartService();
        }
    }
}
