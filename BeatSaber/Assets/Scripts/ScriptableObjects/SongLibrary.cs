using UnityEngine;

namespace BeatSaber.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SongLibrary", menuName = "Scriptable Objects/SongLibrary")]
    public class SongLibrary : ScriptableObject
    {
        [field: SerializeField] public SongSpec[] songSpecs { get; private set; }
    }
}