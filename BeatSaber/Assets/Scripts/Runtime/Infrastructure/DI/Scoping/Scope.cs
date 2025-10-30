using System;
using System.Reflection;
using UnityEngine;

namespace BeatSaber.Runtime.Infrastructure.DI.Scoping
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute { }


    public abstract class Scope : MonoBehaviour
    {
        protected Container container;
        [SerializeField] Component[] _registerTargets;


        protected virtual void Awake()
        {
            container = new Container();
            BuildContainer();
        }

        protected virtual void OnDestroy()
        {
            container.Dispose();
        }

        protected virtual void BuildContainer()
        {
            if (_registerTargets != null)
                for (int i = 0; i < _registerTargets.Length; i++)
                    container.RegisterComponent(_registerTargets[i], Lifetime.Scoped);
        }

        public void InjectAll(GameObject root)
        {
            var components = root.GetComponentsInChildren<MonoBehaviour>();

            foreach (var component in components)
                Inject(component);
        }

        private void Inject(object target)
        {
            Type type = target.GetType();
            var fieldInfos = type.GetFields(System.Reflection.BindingFlags.Instance  |
                                            System.Reflection.BindingFlags.NonPublic |
                                            System.Reflection.BindingFlags.Public);

            foreach (var fieldInfo in fieldInfos)
            {
                var injectAttribute = fieldInfo.GetCustomAttribute<InjectAttribute>();

                if (injectAttribute == null)
                    continue;

                if (TryGetInstance(fieldInfo.FieldType, out object instance))
                {
                    fieldInfo.SetValue(target, instance);
                    Debug.Log($"[DI.Scoping] Resolve 성공 {type.Name}.{fieldInfo.Name}");
                }
                else
                {
                    Debug.LogError($"[DI.Scoping] Resolve 실패 {type.Name}.{fieldInfo.Name}");
                }
            }
        }

        private bool TryGetInstance(Type type, out object instance)
        {
            var singleton = Container.GetSingleton(type) ?? Container.GetMonoSingleton(type);

            if (singleton != null)
            {
                instance = singleton;
                return true;
            }

            var scoped = container.GetScoped(type) ?? container.GetScopedMono(type);

            if (scoped != null)
            {
                instance = scoped;
                return true;
            }

            instance = null;
            return false;
        }
    }
}