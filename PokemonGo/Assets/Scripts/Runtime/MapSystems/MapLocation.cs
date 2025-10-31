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

        public double latitude; // ����
        public double longitude; // �浵
    }
}
