using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaber.Runtime.Infrastructure.DI.Scoping
{
    public enum Lifetime
    {
        Singleton, // 한번 생성되면 파괴전까지 유지
        Scoped, // 지정된 Scope 에서 벗어나면 (Scope 객체 파괴 등) 파괴
        Transient // 의존성주입이 필요할때마다 새로 만듬
    }

    public class Container : IDisposable
    {
        static Dictionary<Type, object> s_singletons = new Dictionary<Type, object>(); // singleton 객체
        static Dictionary<Type, Component> s_monoSingletons = new Dictionary<Type, Component>(); // singleton 객체
        Dictionary<Type, object> _scopedInstances; // scoped 객체
        Dictionary<Type, Component> _scopedMonoInstances; // scoped 객체
        List<IDisposable> _transientTracked; // transient 추적용

        public static object GetSingleton(Type type)
        {
            if (s_singletons.TryGetValue(type, out object singleton))
                return singleton;

            object instance = Activator.CreateInstance(type);
            s_singletons.Add(type, instance);
            return instance;
        }

        public static Component GetMonoSingleton(Type type)
        {
            if (s_monoSingletons.TryGetValue(type, out Component singleton))
                return singleton;

            GameObject gameObject = new GameObject(type.Name);
            Component instance = gameObject.AddComponent(type);
            s_monoSingletons.Add(type, instance);
            GameObject.DontDestroyOnLoad(gameObject);
            return instance;
        }

        public object GetScoped(Type type)
        {
            if (_scopedInstances.TryGetValue(type, out object singleton))
                return singleton;

            object instance = Activator.CreateInstance(type);
            s_singletons.Add(type, instance);
            return instance;
        }

        public Component GetScopedMono(Type type)
        {
            if (_scopedMonoInstances.TryGetValue(type, out Component singleton))
                return singleton;

            GameObject gameObject = new GameObject(type.Name);
            Component instance = gameObject.AddComponent(type);
            _scopedMonoInstances.Add(type, instance);
            GameObject.DontDestroyOnLoad(gameObject);
            return instance;
        }

        public void Dispose()
        {
            _scopedInstances.Clear();

            foreach (var monobehaviour in _scopedMonoInstances)
                GameObject.Destroy(monobehaviour.Value.gameObject);

            _scopedMonoInstances.Clear();
        }

        /// <summary>
        /// 일반적인 C# 타입을 등록하여
        /// 객체를 생성
        /// </summary>
        /// <typeparam name="T"> 주입이 필요한 타입 </typeparam>
        /// <param name="lifetime"> 현재 타입의 객체의 생명주기를 어떻게 관리할것인지 </param>
        /// <exception cref="Exception"> lifetime 이 singleton/ scoped 일 경우 중복등록 요청 예외 </exception>
        public void Register<T>(Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    {
                        Type type = typeof(T);

                        if (s_singletons.TryAdd(type, Activator.CreateInstance(type)) == false)
                            throw new Exception($"{type} 싱글톤 등록됐는데 또 등록하려고했음");
                    }
                    break;
                case Lifetime.Scoped:
                    {
                        Type type = typeof(T);

                        if (_scopedInstances.TryAdd(type, Activator.CreateInstance(type)) == false)
                            throw new Exception($"{type} 스코프 등록됐는데 또 등록하려고했음");
                    }
                    break;
                case Lifetime.Transient:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 빈 GameObject 를 만들어서 컴포넌트 인스턴스를 붙여서 관리함.
        /// </summary>
        /// <typeparam name="T"> 주입이 필요한 컴포넌트 </typeparam>
        /// <param name="lifetime"> 생명 주기 관리방식 </param>
        /// <exception cref="Exception"> lifetime 이 singleton/ scoped 일 경우 중복등록 요청 예외 </exception>
        public void RegisterComponent<T>(Lifetime lifetime)
            where T : Component
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    {
                        Type type = typeof(T);
                        GameObject gameObject = new GameObject(type.Name);

                        if (s_monoSingletons.TryAdd(type, gameObject.AddComponent<T>()) == false)
                            throw new Exception($"{type} 싱글톤 등록됐는데 또 등록하려고했음");

                        GameObject.DontDestroyOnLoad(gameObject);
                    }
                    break;
                case Lifetime.Scoped:
                    {
                        Type type = typeof(T);
                        GameObject gameObject = new GameObject(type.Name);

                        if (_scopedMonoInstances.TryAdd(type, gameObject.AddComponent<T>()) == false)
                            throw new Exception($"{type} 스코프 등록됐는데 또 등록하려고했음");

                        GameObject.DontDestroyOnLoad(gameObject);
                    }
                    break;
                case Lifetime.Transient:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 이미 존재하는 컴포넌트 등록
        /// </summary>
        /// <param name="component"> Hierarchy 에 존재하거나 외부에서 따로 생성한 컴포넌트 </param>
        /// <param name="lifetime"> 생명 주기 관리 방식 </param>
        /// <exception cref="Exception"> lifetime 이 singleton/ scoped 일 경우 중복등록 요청 예외 </exception>
        public void RegisterComponent(Component component, Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    {
                        Type type = component.GetType();
                        GameObject gameObject = component.gameObject;

                        if (s_monoSingletons.TryAdd(type, component) == false)
                            throw new Exception($"{type} 싱글톤 등록됐는데 또 등록하려고했음");

                        GameObject.DontDestroyOnLoad(gameObject);
                    }
                    break;
                case Lifetime.Scoped:
                    {
                        Type type = component.GetType();
                        GameObject gameObject = component.gameObject;

                        if (_scopedMonoInstances.TryAdd(type, component) == false)
                            throw new Exception($"{type} 스코프 등록됐는데 또 등록하려고했음");

                        GameObject.DontDestroyOnLoad(gameObject);
                    }
                    break;
                case Lifetime.Transient:
                    break;
                default:
                    break;
            }
        }
    }
}
