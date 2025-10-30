using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatSaber.Runtime.Infrastructure.DI.Scoping
{
    public class ApplicationScope : Scope
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            GameObject go = new GameObject(nameof(ApplicationScope));
            s_instance = go.AddComponent<ApplicationScope>();
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                foreach (var root in scene.GetRootGameObjects())
                {
                    s_instance.InjectAll(root);
                }
            };
            DontDestroyOnLoad(go);
        }

        private static ApplicationScope s_instance;


        protected override void BuildContainer()
        {
            base.BuildContainer();

            container.Register<GameStateBlackboard>(Lifetime.Singleton);
        }
    }
}