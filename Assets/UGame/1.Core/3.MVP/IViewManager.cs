using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    public interface IViewManager
    {
        void RegisterView(ViewBase view);
        void UnregisterView(ViewBase view);
        bool HasView(ViewBase view);
        T LoadView<T>(string path,ContextBase context) where T:ViewBase;
        ViewBase[] LoadVIews(string[] paths,ContextBase[] contexts);
        void UnloadView(string viewID);
        void UnloadAllViews();
    }
}