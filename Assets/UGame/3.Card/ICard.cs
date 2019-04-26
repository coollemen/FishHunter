using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    public interface ICard
    {
        int Level { get; set; }
        int RareLevel { get; set; }
        string Name { get; set; }
        int Cost { get; set; }
        int Price { get; set; }
        string Description { get; set; }
        void LevelUp();
        void RareLevelUp();
    }
}