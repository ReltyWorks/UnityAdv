using PokemonGo.Runtime.Services;
using PokemonGo.Runtime.Services.GoogleMap;
using PokemonGo.Runtime.Services.GPS;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PokemonGo.Runtime.Lifetimes
{
    public class MapScope : LifetimeScope
    {
        [SerializeField] MockLocationProvider _mockLocationProviderPrefab;
        [SerializeField] DeviceLocationProvider _deviceLocationProviderPrefab;
        [SerializeField] GPSService _gpsService;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

#if UNITY_EDITOR
            ILocationProvider locationProvider = Instantiate(_mockLocationProviderPrefab);
            builder.RegisterInstance<ILocationProvider>(locationProvider);
#else
            ILocationProvider locationProvider = Instantiate(_deviceLocationProviderPrefab);
            builder.RegisterInstance<ILocationProvider>(locationProvider);
#endif
            builder.RegisterComponent(_gpsService);
            builder.RegisterComponentOnNewGameObject<GoogleStaticMapService>(Lifetime.Scoped);
            builder.RegisterComponentOnNewGameObject<GooglePlacesService>(Lifetime.Scoped);
        }
    }
}