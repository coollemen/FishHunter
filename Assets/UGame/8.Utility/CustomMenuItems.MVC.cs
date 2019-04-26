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
        /// ����MVC����
        /// </summary>
        [MenuItem("GameObject/uGame/Create UI View Script", false, 20)]
        private static void CreateNewMvcScripts()
        {
            //��ȡѡ�е�object������
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
            //����ģ����
            builder.BeginClass(name + "Context", "ContextBase");
            builder.EndClass();
            //������ͼ��
            builder.BeginClass(name + "View", "UIView", ScriptBuilder.AccessType.Public, false, false, true);
            //fun start()
            builder.BeginFunction("Start",ScriptBuilder.AccessType.Public,"void",false,false,true);
            builder.WriteComments("����Զ������Ĵ��룬���ڻ�ȡUI������벻Ҫɾ����");
            builder.WriteLine("GetUIComponents();");
            builder.WriteLine("DataBindings();");
            builder.WriteComments("������Լ��Ĵ���");
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
                EditorUtility.DisplayDialog("�ļ�����", $"\"{compontsName}\"�ļ���������³ɹ���\"{filePath}\"�ļ��Ѵ��ڣ������Ҫ���´���������ɾ�����ļ���", "ȷ��");
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
            //���UI���
            List<UIComponentInfo> infos = new List<UIComponentInfo>();
            var uimarks = go.GetComponentsInChildren<UIMark>();
            for (int i = 0; i < uimarks.Length; i++)
            {
                var markCom = uimarks[i];
                var info = GetUIComponentInfo(markCom);
                infos.Add(info);
            }
            //�������ֶ�
            foreach (var info in infos)
            {
                builder.AddField(info.name, info.type);
            }
            //���GetComponents����
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
        /// ���ַ�������ĸСд
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <returns>ת������ı�</returns>
        private static string FirstCharLower(string text)
        {
            return text.Substring(0, 1).ToLower() + text.Substring(1, text.Length - 1);
        }
        /// <summary>
        /// ���ַ�������ĸ��д
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <returns>ת������ı�</returns>
        private static string FirstCharUpper(string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1);
        }
    }
}