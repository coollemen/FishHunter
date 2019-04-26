using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
namespace UGame
{
    public partial class CustomMenuItems
    {
        public class UIComponentInfo
        {
            public string name;
            public string type;
            public int childIndex;

            public UIComponentInfo()
            {
                
            }
            public UIComponentInfo(string setName, string setType, int setChildIndex)
            {
                this.name = setName;
                this.type = setType;
                this.childIndex = setChildIndex;
            }
        }
        /// <summary>
        /// 创建MVC代码
        /// </summary>
        [MenuItem("GameObject/uGame/Create UI View Script", false, 20)]
        private static void CreateNewMvcScripts()
        {
            //获取选中的object的名字
            var objName = Selection.activeGameObject.name;
            var go = Selection.activeGameObject;
            string path = "Assets/GameDev/UI";
            CreateViewScriptFile(objName, path);
            CreateViewComponentsScriptFile(objName + "View", path,go);
            AssetDatabase.Refresh();
        }


        private static void CreateViewScriptFile(string name, string path)
        {
            var builder = new ScriptBuilder();
            builder.AddUsingLib("System.Collections");
            builder.AddUsingLib("System.Collections.Generic");
            builder.AddUsingLib("UnityEngine");
            builder.AddUsingLib("UnityEngine.UI");
            builder.AddUsingLib("UniRx");

            builder.BeginNamespace("UGame");
            //创建模型类
            builder.BeginClass(name + "Context", "ContextBase");
            builder.EndClass();
            //创建视图类
            builder.BeginClass(name + "View", "UIView", ScriptBuilder.AccessType.Public, false, false, true);
            //fun start()
            builder.BeginFunction("Start",ScriptBuilder.AccessType.Public,"void",false,false,true);
            builder.WriteComments("框架自动创建的代码，用于获取UI组件，请不要删除！");
            builder.WriteLine("GetUIComponents();");
            builder.WriteLine("DataBindings();");
            builder.WriteComments("添加你自己的代码");
            builder.EndFunction();
            //fun BindingUIComponents()
            builder.BeginFunction("DataBindings", ScriptBuilder.AccessType.Public, "void", false, false, true);
            builder.EndFunction();

            builder.EndClass();
            builder.EndNamespace();
            string filePath = path + "/" + name + "View" + ".cs";
            string compontsName =  name + "View"+".Components" + ".cs";
            if (File.Exists(filePath) == false)
            {
                File.WriteAllText(filePath, builder.ToString());
            }
            else
            {
                EditorUtility.DisplayDialog("文件创建", $"\"{compontsName}\"文件创建或更新成功！\"{filePath}\"文件已存在！如果需要重新创建，请先删除该文件！", "确定");
            }

        }

        private static void CreateViewComponentsScriptFile(string name, string path, GameObject go)
        {
            var builder = new ScriptBuilder();
            builder.AddUsingLib("System.Collections");
            builder.AddUsingLib("System.Collections.Generic");
            builder.AddUsingLib("UnityEngine");
            builder.AddUsingLib("UnityEngine.UI");
            builder.BeginNamespace("UGame");

            builder.BeginClass(name, "", ScriptBuilder.AccessType.Public, false, false, true);
            //添加UI组件
            List<UIComponentInfo> infos = new List<UIComponentInfo>();
            var uimarks = go.GetComponentsInChildren<UIMark>();
            for (int i = 0; i < uimarks.Length; i++)
            {
                var markCom = uimarks[i];
                var info = GetUIComponentInfo(markCom);
                infos.Add(info);
            }
            //添加组件字段
            foreach (var info in infos)
            {
                builder.AddField(info.name, info.type);
            }
            //添加GetComponents函数
            builder.BeginFunction("GetUIComponents");
            foreach (var info in infos)
            {    
                var text = $"{info.name}=transform.GetChild({info.childIndex}).GetComponent<{info.type}>();";
                builder.WriteLine(text);
            }
            builder.EndFunction();

            builder.EndClass();
            builder.EndNamespace();
            File.WriteAllText(path + "/" + name + ".Components.cs", builder.ToString());
        }

        private static UIComponentInfo GetUIComponentInfo(UIMark markCom)
        {
            var info = new UIComponentInfo();
            var fieldName = string.IsNullOrEmpty(markCom.customComponentName)
                ? markCom.name
                : markCom.customComponentName;
            fieldName = FirstCharLower(fieldName);
            info.name = fieldName;
            info.childIndex = markCom.transform.GetSiblingIndex();
            var selectableCom = markCom.GetComponent<UnityEngine.UI.Selectable>();
            if (selectableCom != null)
            {
                if (selectableCom is UnityEngine.UI.Button)
                {
                    info.type = "Button";
                }
                else if (selectableCom is UnityEngine.UI.Toggle)
                {
                    info.type = "Toggle";
                }
                else if (selectableCom is UnityEngine.UI.Slider)
                {
                    info.type = "Slider";
                }
                else if (selectableCom is UnityEngine.UI.Scrollbar)
                {
                    info.type = "Scrollbar";
                }
                else if (selectableCom is UnityEngine.UI.Dropdown)
                {
                    info.type = "Dropdown";
                }
                else if (selectableCom is UnityEngine.UI.InputField)
                {
                    info.type = "InputField";
                }
            } //end if
            else
            {
                var graphicCom = markCom.GetComponent<UnityEngine.UI.Graphic>();
                if (graphicCom != null)
                {
                    if (graphicCom is UnityEngine.UI.Image)
                    {
                        info.type = "Image";
                    }
                    else if (graphicCom is UnityEngine.UI.Text)
                    {
                        info.type = "Text";
                    }
                    else if (graphicCom is UnityEngine.UI.RawImage)
                    {
                        info.type = "RawImage";
                    }
                }
            } //end else
            return info;
        }

        /// <summary>
        /// 将字符串首字母小写
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>转换后的文本</returns>
        private static string FirstCharLower(string text)
        {
            return text.Substring(0, 1).ToLower() + text.Substring(1, text.Length - 1);
        }
        /// <summary>
        /// 将字符串首字母大写
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>转换后的文本</returns>
        private static string FirstCharUpper(string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1);
        }
    }
}