using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UGame
{
      public partial class WindowView
      {
            public Text headerTitle;
            public Button closeButton;
            public void GetUIComponents()
            {
                  headerTitle=transform.GetChild(3).GetComponent<Text>();
                  closeButton=transform.GetChild(4).GetComponent<Button>();
            }
      }
}
