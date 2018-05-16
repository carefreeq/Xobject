using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XObject;
using QQ;
using System.IO;

public class ObjectHandleEditor
{
    [MenuItem("GameObject/XObject/Export", priority = 0)]
    public static void ExportXObject()
    {
        if (Selection.gameObjects.Length > 0)
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            string path = EditorUtility.SaveFilePanel("导出到", Application.streamingAssetsPath, "xo文件", "xo");
            if (path != null && path.Length > 0)
                XOFormat.Export(path, Selection.gameObjects);
        }
        else
        {
            Debug.Log("未选择任何物体");
        }
    }

    [MenuItem("GameObject/XObject/Import", priority = 0)]
    public static void ImportXObject()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);
        string path = EditorUtility.OpenFilePanel("导入到", Application.streamingAssetsPath, "xo");
        if (path != null && path.Length > 0)
            XOFormat.Import(path);
    }
}
