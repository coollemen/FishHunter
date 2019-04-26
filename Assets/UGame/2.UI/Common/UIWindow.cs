using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UGame
{
    /// <summary>
    /// 这里的窗口是狭义的窗口，
    /// </summary>
    public class UIWindow : UIView
    {
        public Text titleLabel;
        public Button closeButton;
        public override string TypeID => "UIWindow";

    }
}