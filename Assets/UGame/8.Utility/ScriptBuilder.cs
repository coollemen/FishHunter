using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UGame
{
    /// <summary>
    /// c#脚本创建类
    /// </summary>
    public class ScriptBuilder
    {
        /// <summary>
        /// 权限类型
        /// </summary>
        public enum AccessType
        {
            Public,
            Private,
            Protected
        }

        /// <summary>
        /// 字符串创建器
        /// </summary>
        private StringBuilder _builder;

        /// <summary>
        /// 换行符
        /// </summary>
        public string lineBreak = "\r\n";

        /// <summary>
        /// 缩进等级
        /// </summary>
        public int IndentLevel { get; set; }

        public ScriptBuilder()
        {
            _builder = new StringBuilder();
        }

        private void Write(string context, bool needIndent = true)
        {
            if (needIndent)
            {
                context = GetIndent() + context;
            }

            _builder.Append(context);
        }

        private string GetIndent()
        {
            string indent = "";
            for (int i = 0; i < IndentLevel; i++)
            {
                indent += "   ";
            }

            return indent;
        }

        /// <summary>
        /// 添加引用类库
        /// </summary>
        /// <param name="name">类库名</param>
        public void AddUsingLib(string name)
        {
            this.Write("using " + name + ";");
            this.NewLine();
        }

        /// <summary>
        /// 开始添加命名空间
        /// </summary>
        /// <param name="name"></param>
        public void BeginNamespace(string name)
        {
            this.Write("namespace " + name);
            this.NewLine();
            this.BeginLeftCurlyBracket();
        }

        /// <summary>
        /// 结束命名空间
        /// </summary>
        public void EndNamespace()
        {
            this.EndRightCurlyBracket();
        }

        /// <summary>
        /// 添加空的类
        /// </summary>
        /// <param name="name">类名</param>
        public void AddSimpleClass(string name)
        {
            this.BeginClass(name);
            this.NewLine();
            this.EndClass();
        }

        /// <summary>
        /// 开始添加类
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="parent">父类</param>
        /// <param name="access">访问权限</param>
        /// <param name="isStatic">是否是静态类</param>
        /// <param name="isAbstract">是否是抽象类</param>
        /// <param name="isPartial">是不是分布在多个文件</param>
        public void BeginClass(string className, string parent = "MonoBehaviour", AccessType access = AccessType.Public,
            bool isStatic = false, bool isAbstract = false, bool isPartial = false)
        {
            //生成文本
            string accessText = "";
            switch (access)
            {
                case AccessType.Public:
                    accessText = "public";
                    break;
                case AccessType.Private:
                    accessText = "private";
                    break;
                case AccessType.Protected:
                    accessText = "protected";
                    break;
                default: break;
            }

            string staticText = isStatic ? " static" : "";
            string abstractText = isAbstract ? " abstract" : "";
            string partialText = isPartial ? " partial" : "";
            string parentText = string.IsNullOrEmpty(parent) ? "" : " : " + parent;
            string content = $"{accessText}{staticText}{abstractText}{partialText} class {className}{parentText}";
            //写入内容
            this.Write(content);
            this.NewLine();
            this.BeginLeftCurlyBracket();
        }

        /// <summary>
        /// 结束类
        /// </summary>
        public void EndClass()
        {
            this.EndRightCurlyBracket();
        }

        /// <summary>
        /// 添加空函数
        /// </summary>
        /// <param name="name">函数名</param>
        /// <param name="paraName">函数参数</param>
        public void AddSimpleFunction(string name, params string[] paraNames)
        {
            this.BeginFunction(name, AccessType.Public, "void", false, false, false, paraNames);
            this.NewLine();
            this.EndFunction();
        }

        /// <summary>
        /// 开始添加函数
        /// </summary>
        /// <param name="funName">函数名</param>
        /// <param name="access">访问权限</param>
        /// <param name="returnType">返回值类型</param>
        /// <param name="isStatic">是否是静态函数</param>
        /// <param name="isAbstract">是否是抽象函数</param>
        /// <param name="isOverride">是否是复写函数</param>
        /// <param name="paraNames">参数</param>
        public void BeginFunction(string funName, AccessType access = AccessType.Public, string returnType = "void",
            bool isStatic = false, bool isAbstract = false, bool isOverride = false, params string[] paraNames)
        {
            //生成文本
            string accessText = "";
            switch (access)
            {
                case AccessType.Public:
                    accessText = "public";
                    break;
                case AccessType.Private:
                    accessText = "private";
                    break;
                case AccessType.Protected:
                    accessText = "protected";
                    break;
                default: break;
            }

            string staticText = isStatic ? " static" : "";
            string abstractText = isAbstract ? " abstract" : "";
            string overrideText = isOverride ? " override" : "";
            StringBuilder b = new StringBuilder();
            if (paraNames.Length > 0)
            {
                foreach (var p in paraNames)
                {
                    b.Append(p + ",");
                }

                b.Remove(b.Length - 1, 1);
            }

            string content =
                $"{accessText}{overrideText}{staticText}{abstractText} {returnType} {funName}({b.ToString()})";
            this.Write(content);
            this.NewLine();
            this.BeginLeftCurlyBracket();
        }

        /// <summary>
        /// 结束函数
        /// </summary>
        public void EndFunction()
        {
            this.EndRightCurlyBracket();
        }

        public void AddField(string fieldName, string type,AccessType access = AccessType.Public, bool isStatic = false)
        {
            //生成文本
            string accessText = "";
            switch (access)
            {
                case AccessType.Public:
                    accessText = "public";
                    break;
                case AccessType.Private:
                    accessText = "private";
                    break;
                case AccessType.Protected:
                    accessText = "protected";
                    break;
                default: break;
            }

            string staticText = isStatic ? " static" : "";
            string content = $"{accessText}{staticText} {type} {fieldName};";
            this.WriteLine(content);
        }
        public void AddProperty(string propertyName, string type, AccessType access = AccessType.Public, bool isStatic = false)
        {
            //生成文本
            string accessText = "";
            switch (access)
            {
                case AccessType.Public:
                    accessText = "public";
                    break;
                case AccessType.Private:
                    accessText = "private";
                    break;
                case AccessType.Protected:
                    accessText = "protected";
                    break;
                default: break;
            }

            string staticText = isStatic ? " static" : "";
            string content = $"{accessText}{staticText} {type} {propertyName}"+"{ get; set; }";
            this.WriteLine(content);
        }
        /// <summary>
        /// 开始左括号
        /// </summary>
        public void BeginLeftBracket()
        {
            this.Write("(", false);
        }

        /// <summary>
        /// 结束右括号
        /// </summary>
        public void EndRightBracket()
        {
            this.Write(")", false);
        }

        /// <summary>
        /// 添加一对括号
        /// </summary>
        public void EmptyBrackets()
        {
            this.Write("()", false);
        }

        /// <summary>
        /// 开始左大括号
        /// </summary>
        public void BeginLeftCurlyBracket()
        {
            this.Write("{");
            this.NewLine();
            this.IndentLevel+=2;
        }

        /// <summary>
        /// 结束右大括号
        /// </summary>
        public void EndRightCurlyBracket()
        {
            this.IndentLevel-=2;
//            this.NewLine();
            this.Write("}");
            this.NewLine();
        }

        /// <summary>
        /// 添加一对大括号
        /// </summary>
        public void EmptyCurlyBrackets()
        {
            this.Write("{}", false);
        }

        /// <summary>
        /// 添加一行内容
        /// </summary>
        /// <param name="context"></param>
        public void WriteLine(string context)
        {
            this.Write(context);
            this.NewLine();
        }

        /// <summary>
        /// 添加注释
        /// </summary>
        /// <param name="context"></param>
        public void WriteComments(string context)
        {
            this.Write("//" + context);
            this.NewLine();
        }

        public void NewLine()
        {
            this._builder.Append(lineBreak);
        }

        public override string ToString()
        {
            return this._builder.ToString();
        }
    }
}