namespace UnityEditor
{
    static class CustomAnimatedRuleTileMenu
    {
        [MenuItem("Assets/Create/Custom Animated Rule Tile Script", false, 89)]
        static void CreateCustomAnimatedRuleTile()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Tilemap/Tiles/Animated Rule Tile/ScriptTemplates/NewCustomAnimatedRuleTile.cs.txt", "NewCustomAnimatedRuleTile.cs");
        }
    }
}
