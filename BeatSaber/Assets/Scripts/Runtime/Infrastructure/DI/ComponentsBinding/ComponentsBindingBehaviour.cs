using System;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaber.Runtime.Infrastructure.DI.ComponentsBinding
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindAttribute : Attribute { }

    /// <summary>
    /// 이 클래스를 상속받은 클래스 필드가 Component 를 주입할게 있다면
    /// 종속된 모든 GameObject 들을 탐색하여 적절한 Component 를 주입함.
    /// </summary>
    public abstract class ComponentsBindingBehaviour : MonoBehaviour
    {
        StringBuilder _stringBuilder; // GameObject 이름을 연산하기위한 버퍼

        protected virtual void Awake()
        {
            _stringBuilder = new StringBuilder(35); // 대충 제일긴 prefix 글자수 + 변수이름 넉넉하게 잡은 수
            ResolveFields();
        }

        /// <summary>
        /// BindAttribute 가 붙은 필드들을 모두 찾고 의존성을 해결함.
        /// </summary>
        private void ResolveFields()
        {
            Type type = GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                var bindAttribute = fieldInfo.GetCustomAttribute<BindAttribute>();

                if (bindAttribute == null)
                    continue;

                ResolveField(fieldInfo);
            }
        }

        /// <summary>
        /// 종속된 하위 GameObject 들 모두 탐색하여 필드의 의존성을 주입.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <exception cref="Exception"></exception>
        private void ResolveField(FieldInfo fieldInfo)
        {
            Type fieldType = fieldInfo.FieldType;
            string fieldName = fieldInfo.Name;
            string gameObjectName = ConvertFieldNameToGameObjectName(fieldType, fieldName);
            GameObject found = FindGameObjectByName(transform, gameObjectName);

            if (fieldType == typeof(GameObject))
            {
                if (found != null)
                {
                    fieldInfo.SetValue(this, found);
                    return;
                }
            }
            else if (typeof(Component).IsAssignableFrom(fieldType))
            {
                if (found != null)
                {
                    Component component = found.GetComponent(fieldType);

                    if (component != null)
                    {
                        fieldInfo.SetValue(this, component);
                        return;
                    }
                }
            }

            throw new Exception($"[{nameof(ComponentsBindingBehaviour)}] Check names .. {fieldName} expected to bind {gameObjectName}");
        }

        /// <summary>
        /// 필드 이름과 타입으로 패턴을 조합하여 주입할 Component 를 갖는 GameObject 이름으로 변환
        /// </summary>
        /// <returns> 주입할 Component를 가지는 GameObject 의 이름 </returns>
        /// <exception cref="Exception"> 멤버 필드의 이름이 잘못되었음 </exception>
        private string ConvertFieldNameToGameObjectName(Type fieldType, string fieldName)
        {
            _stringBuilder.Clear();

            // 접두어 가져오기
            if (ComponentsResolvingPrefixTable.TryGetPrefix(fieldType, out string prefix) == false)
                prefix = string.Empty;

            int start = fieldName[0] == '_' ? 1 : 0;

            if (fieldName.Length < start + 1)
                throw new Exception($"[{nameof(ComponentsBindingBehaviour)}] : Check name of the field {fieldName}");

            _stringBuilder.Append(prefix);
            _stringBuilder.Append(char.ToUpper(fieldName[start]));

            for (int i = start + 1; i < fieldName.Length; i++)
                _stringBuilder.Append(fieldName[i]);

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Root 및 하위 모든 자식 GameObject 탐색
        /// </summary>
        private GameObject FindGameObjectByName(Transform root, string name)
        {
            if (root.name == name)
                return root.gameObject;

            Transform found = root.Find(name);

            if (found != null)
                return found.gameObject;

            return FindGameObjectRecursively(root, name);
        }

        /// <summary>
        /// 재귀적으로 하위 모든자식 탐색
        /// </summary>
        private GameObject FindGameObjectRecursively(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child.gameObject;

                GameObject found = FindGameObjectRecursively(child, name);

                if (found != null)
                    return found.gameObject;
            }

            return null;
        }
    }
}