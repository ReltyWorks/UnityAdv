using System;
using UnityEngine;

namespace PokemonGo.Runtime.Services
{
    public class InternetConnectionService
    {
        public bool isConnected
        {
            get
            {
                if ((DateTime.UtcNow.Second - _lastUpdated.Second) < _interval)
                    return _isConnected;

                _isConnected = Application.internetReachability != NetworkReachability.NotReachable;
                _lastUpdated = DateTime.UtcNow;
                return _isConnected;
            }
        }

        private bool _isConnected;
        private float _interval = 2f;
        private DateTime _lastUpdated = DateTime.MinValue;
    }
}