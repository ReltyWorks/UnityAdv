using PokemonGo.Runtime.Services;
using PokemonGo.Runtime.Services.GPS;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PokemonGo.Runtime.Lifetimes
{
    public class ApplicationScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<InternetConnectionService>(Lifetime.Singleton);
        }
    }
}