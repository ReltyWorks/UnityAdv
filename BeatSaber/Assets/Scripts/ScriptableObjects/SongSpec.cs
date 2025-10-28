using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Video;

/* Attribute 사용예시
public class EngineEditor
{
    GameObject _selected;

    public void RefreshInspector()
    {
        if (_selected == null)
            return;

        var components = _selected.GetComponents<Component>();

        foreach (var component in components)
        {
            var publicFields = component.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            // TODO : publicFields 순회하면서 Inspector 에 그림 
            foreach (var field in publicFields)
            {
                Debug.Log(field.Name);
            }

            var nonPublicFields = component.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            foreach(var field in nonPublicFields)
            {
                var serializeFieldAttribute = field.GetCustomAttribute<SerializeField>();

                if (serializeFieldAttribute != null)
                    Debug.Log(field.Name);
            }
        }
    }
}
*/

namespace BeatSaber.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SongSpec", menuName = "Scriptable Objects/SongSpec")]
    public class SongSpec : ScriptableObject
    {
        [field: SerializeField] public string title { get; private set; }
        [field: SerializeField] public string description { get; private set; }
        [field: SerializeField] public AudioClip audioClip { get; private set; }
        [field: SerializeField] public VideoClip videoClip { get; private set; }
        [field: SerializeField] public float bpm { get; private set; }
        [field: SerializeField] public List<float> peaks { get; private set; }
    }
}