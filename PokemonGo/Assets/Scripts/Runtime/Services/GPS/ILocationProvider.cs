using System;
using UnityEngine;

namespace PokemonGo.Runtime.Services.GPS
{
    public interface ILocationProvider
    {
        bool isRunning { get; }
        double latitude { get; }
        double longitude { get; }
        double altitude { get; }
        (double latitude, double longitude) origin { get; }
        /// <summary>
        /// latitude, longitude, altitude, horizontalAccuracy, timestamp
        /// </summary>
        event Action<double, double, double, float, double> onLocationUpdated;

        void StartService();
        void StopService();
    }
}