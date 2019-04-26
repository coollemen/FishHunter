using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    /// <summary>
    /// 等级
    /// </summary>
    public int level;
    /// <summary>
    /// 最大等级
    /// </summary>
    public int maxLevel;
    /// <summary>
    /// 珍惜度等级
    /// </summary>
    public int rareLv;
    /// <summary>
    /// 最大珍惜度等级
    /// </summary>
    public int maxRareLv;
    /// <summary>
    /// 描述
    /// </summary>
    public string description;
    /// <summary>
    /// 经验值
    /// </summary>
    public int exp;
    /// <summary>
    /// 最大经验值
    /// </summary>
    public int maxExp;
}
