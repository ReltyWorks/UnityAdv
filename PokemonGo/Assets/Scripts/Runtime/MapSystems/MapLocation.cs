using UnityEngine;

namespace PokemonGo.Runtime.MapSystems
{
    public struct MapLocation
    {
        public MapLocation(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public double latitude; // 위도
        public double longitude; // 경도
    }
}
