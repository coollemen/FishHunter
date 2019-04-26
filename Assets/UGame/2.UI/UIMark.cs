using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace UGame
{
    public enum UIMarkType
    {
        Default,
        Element,
        Component
    }

    public class UIMark : MonoBehaviour
    {
        [EnumToggleButtons]
        public UIMarkType markType = UIMarkType.Default;
        public string customComponentName;
    }
}