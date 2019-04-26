using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace UGame
{
    [System.Serializable]
    public class CustomEditorTool
    {
        [HideInInspector]
        public string name;

        public CustomEditorTool()
        {
            this.name = "tool";
        }
    }
}