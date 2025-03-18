namespace FuzzPhyte.UI.Editor
{
    using UnityEngine;
    using UnityEditor;

    [InitializeOnLoad]
    public static class FPUI_ExecutionOrder
    {
        static FPUI_ExecutionOrder()
        {
            SetExecutionOrder(typeof(FuzzPhyte.UI.FPUI_DragDropManager), -25);
            SetExecutionOrder(typeof(FuzzPhyte.UI.FPUI_MatchManager), -20);
        }
        static void SetExecutionOrder(System.Type scriptType, int order)
        {
            string scriptName = scriptType.Name;
            MonoScript script = FindMonoScript(scriptType);

            if (script == null)
            {
                Debug.LogError($"Script {scriptName} not found. Ensure the name is correct.");
                return;
            }

            int currentOrder = MonoImporter.GetExecutionOrder(script);
            if (currentOrder != order)
            {
                MonoImporter.SetExecutionOrder(script, order);
                Debug.Log($"Set execution order for {scriptName} to {order}");
            }
        }

        static MonoScript FindMonoScript(System.Type scriptType)
        {
            string[] guids = AssetDatabase.FindAssets($"{scriptType.Name} t:MonoScript");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (monoScript != null && monoScript.GetClass() == scriptType)
                {
                    return monoScript;
                }
            }
            return null;
        }
    }

}
