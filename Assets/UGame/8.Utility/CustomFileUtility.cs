using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace UGame
{
    /// <summary>
    /// 自定义文件创建辅助工具
    /// </summary>
    public class CustomFileUtility
    {
        /// <summary>
        /// 创建文件类的文件
        /// </summary>
        /// <param name="fileEx"></param>
        public static void CreateFile(string fileEx,string content="Empty")
        {
            //获取当前所选择的目录（相对于Assets的路径）
            var selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var path = Application.dataPath.Replace("Assets", "") + "/";
            var newFileName = "New" + fileEx.ToUpper() + "File" + "." + fileEx;
            var newFilePath = selectPath + "/" + newFileName;
            var fullPath = path + newFilePath;

            //简单的重名处理
            if (File.Exists(fullPath))
            {
                var newName = "new_" + fileEx + "-" + UnityEngine.Random.Range(0, 100) + "." + fileEx;
                newFilePath = selectPath + "/" + newName;
                fullPath = fullPath.Replace(newFileName, newName);
            }

            //如果是空白文件，编码并没有设成UTF-8
            File.WriteAllText(fullPath, content, Encoding.UTF8);

            AssetDatabase.Refresh();

            //选中新创建的文件
            var asset = AssetDatabase.LoadAssetAtPath(newFilePath, typeof(Object));
            Selection.activeObject = asset;
        }
    }
}