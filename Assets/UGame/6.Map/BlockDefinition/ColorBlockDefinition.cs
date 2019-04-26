using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    [System.Serializable]
    public class ColorBlockDefinition : BlockDefinition
    {
        /// <summary>
        /// 颜色
        /// </summary>
        public Color color;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="setID">childIndex</param>
        /// <param name="setName">名称</param>
        /// <param name="setColor">颜色</param>
        public ColorBlockDefinition(byte setID, string setName, Color setColor) : base(setID, setName)
        {
            this.color = setColor;
        }
    }
}