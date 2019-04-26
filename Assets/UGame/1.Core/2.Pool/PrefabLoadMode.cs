using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    public enum PrefabLoadMode 
    {
        /// <summary>
        /// 立即
        /// </summary>
        OnInit,
        /// <summary>
        /// 当使用时
        /// </summary>
        OnUsed,
        /// <summary>
        /// 预载
        /// </summary>
        Prelaod
    }
}