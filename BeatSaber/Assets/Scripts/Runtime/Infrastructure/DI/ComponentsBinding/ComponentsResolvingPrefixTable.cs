using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaber.Runtime.Infrastructure.DI.ComponentsBinding
{
    public static class ComponentsResolvingPrefixTable
    {
        public static bool TryGetPrefix(Type type, out string prefix)
        {
            if (s_prefixTable.TryGetValue(type, out prefix))
                return true;

            return false;
        }

        private static Dictionary<Type, string> s_prefixTable = new Dictionary<Type, string>()
        {
            { typeof(GameObject), "" },
            { typeof(Transform), "" },
            { typeof(RectTransform), "" },
            { typeof(Canvas), "Canvas - " },
            { typeof(TMP_Text), "Text (TMP) - " },
            { typeof(TextMeshPro), "Text (TMP) - " },
            { typeof(TextMeshProUGUI), "Text (TMP) - " },
            { typeof(TMP_InputField), "InputField (TMP) - " },
            { typeof(Button), "Button - " },
            { typeof(Image), "Image - " },
            { typeof(RawImage), "RawImage - " },
            { typeof(Toggle), "Toggle - " },
            { typeof(Slider), "Slider - " },
        };
    }
}