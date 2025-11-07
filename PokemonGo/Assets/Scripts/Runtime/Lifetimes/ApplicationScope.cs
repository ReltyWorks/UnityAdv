using PokemonGo.Persistence.Repositories;
using PokemonGo.Runtime.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PokemonGo.Runtime.Lifetimes
{
    public class ApplicationScope : LifetimeScope
    {
        [SerializeField] bool _simulateDb = true;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<InternetConnectionService>(Lifetime.Singleton);

            if (_simulateDb)
                builder.Register<IItemRepository, MockItemRepository>(Lifetime.Singleton);
            else
                builder.Register<IItemRepository, ItemRepository>(Lifetime.Singleton);
        }
    }
}