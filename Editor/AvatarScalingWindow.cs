using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

public class AvatarScalingWindow : EditorWindow
{
    private readonly ASManager manager = new ASManager();

    int sizeTab;
    int windowTab;
    bool found;

    float min = 0.5f;
    float max = 3.0f;

    [MenuItem("Window/Avatar Scaling")]
    static void Init()
    {
        AvatarScalingWindow window = (AvatarScalingWindow)EditorWindow.GetWindow(typeof(AvatarScalingWindow), false, "Avatar Scaling");
        window.Show();
        window.minSize = new Vector2(375f, 515f);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        windowTab = GUILayout.Toolbar(windowTab, new string[] { "Setup", "About" }, GUILayout.Width(370f));
        switch (windowTab)
        {
            case 0:
                if (found)
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginArea(new Rect(0, 30f, 375f, 485f));
                    GUILayout.BeginVertical();
                    DrawSetupWindow();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("Scaling Templates not found!", MessageType.Error);
                    EditorGUILayout.HelpBox("Make sure that the original Animators and Animations included in the package haven't been renamed!", MessageType.None);
                }
                break;
            case 1:
                GUILayout.BeginVertical();
                GUILayout.BeginArea(new Rect(0, 30f, 375f, 485f));
                GUILayout.BeginVertical();
                DrawAboutWindow();
                GUILayout.EndVertical();
                GUILayout.EndArea();
                GUILayout.FlexibleSpace();
                EditorGUILayout.Space();
                GUILayout.EndVertical();
                break;
        }
    }
    private void OnFocus()
    {
        found = manager.FindTemplates();
        if (manager.avatar == null)
        {
            manager.avatar = FindObjectOfType<VRCAvatarDescriptor>();
        }
        manager.sizes[1] = (manager.avatar != null) ? manager.avatar.gameObject.transform.localScale : manager.sizes[1];
        if (sizeTab == 0)
        {
            manager.sizes[0] = new Vector3(min * manager.sizes[1].x, min * manager.sizes[1].y, min * manager.sizes[1].z);
            manager.sizes[2] = new Vector3(max * manager.sizes[1].x, max * manager.sizes[1].y, max * manager.sizes[1].z);
        }
    }
    
    private void DrawAboutWindow()
    {
        string version = (AssetDatabase.FindAssets("VERSION", new string[] { "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" }).Length > 0) ? " " + File.ReadAllText(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("VERSION", new string[] { "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" })[0])) : "";
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("<b><size=18>Avatar Scaling" + version + "</size></b>", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true }, GUILayout.Width(300f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("<size=13>Author: Joshuarox100</size>", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true, normal = new GUIStyleState() { background = null } });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("<b><size=15>Summary</size></b>", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true }, GUILayout.Width(200f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("With Avatars 3.0, you can now change the scale of your Avatar and viewpoint in realtime! This tool exists to set everything up needed for scaling to work on any given 3.0 Avatar with minimal user effort.", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true, normal = new GUIStyleState() { background = null } }, GUILayout.Width(350f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("<b><size=15>Special Thanks</size></b>", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true }, GUILayout.Width(200f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("<i><size=12>Ambiguous</size></i>\nFor helping me test things throughout the entirety of development.\n\n" +
            "<i><size=12>PhaxeNor</size></i>\nFor inspiring me to make this setup script for the tool.\n\n" +
            "<i><size=12>Mr. Doon</size></i>\nFor suggesting significant improvements to the Animators.", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true, normal = new GUIStyleState() { background = null } }, GUILayout.Width(350f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("<b><size=15>Troubleshooting</size></b>", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true }, GUILayout.Width(200f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("If you're having issues or want to contact me, you can find more information at the Github page linked below!", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true, normal = new GUIStyleState() { background = null } }, GUILayout.Width(350f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Github Repository", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.UpperCenter });
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open in Browser", GUILayout.Width(250)))
        {
            Application.OpenURL("https://github.com/Joshuarox100/VRC-Avatar-Scaling");
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }
    private void DrawSetupWindow()
    {
        GUILayout.Label("Avatar Settings", EditorStyles.boldLabel);
        GUILayout.BeginVertical(GUILayout.Height(40f));
        EditorGUI.BeginChangeCheck();
        GUILayout.FlexibleSpace();
        SelectAvatarDescriptor();
        if (manager.avatar == null)
        {
            EditorGUILayout.HelpBox("No Avatars found in the current Scene!", MessageType.Warning);
        }
        if (EditorGUI.EndChangeCheck())
        {
            manager.sizes[1] = (manager.avatar != null) ? manager.avatar.gameObject.transform.localScale : manager.sizes[1];
            if (sizeTab == 0)
            {
                manager.sizes[0] = new Vector3(min * manager.sizes[1].x, min * manager.sizes[1].y, min * manager.sizes[1].z);
                manager.sizes[2] = new Vector3(max * manager.sizes[1].x, max * manager.sizes[1].y, max * manager.sizes[1].z);
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Optional Settings", EditorStyles.boldLabel);
        manager.expressionsMenu = (VRCExpressionsMenu)EditorGUILayout.ObjectField(new GUIContent("Expressions Menu", "(Optional) The Expressions Menu you want the scaling controls added to. Leave this empty if you don't want any menus to be affected.\n(Controls will be added as a submenu.)"), manager.expressionsMenu, typeof(VRCExpressionsMenu), true);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Add Parameters", "Check the Avatar's Expression Parameters for the needed parameters for scaling. If any are not present, it will attempt to add them."), GUILayout.Width(145));
        manager.addExpressionParameters = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(manager.addExpressionParameters), new string[] { "No", "Yes" }));
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Use Existing Animators", "If Animators are already present for Gesture, Sitting, or TPose, the parameters and layer for scaling will be added to them. If an Animator is missing or this feature is disabled, a new Animator will be generated using the defaults included in the VRChat SDK and inserted into the descriptor automatically."), GUILayout.Width(145));
        manager.insertLayers = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(manager.insertLayers), new string[] { "No", "Yes" }));
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Size Settings", EditorStyles.boldLabel);
        sizeTab = GUILayout.Toolbar(sizeTab, new string[] { "Simple", "Advanced" });
        switch (sizeTab)
        {
            //Simple
            case 0:
                EditorGUILayout.HelpBox("These values will be multiplied by the scale of your Avatar.\n(0.5 is half scale, 2.0 is twice your scale)", MessageType.Info);
                EditorGUI.BeginChangeCheck();
                GUILayout.FlexibleSpace();
                min = EditorGUILayout.FloatField(new GUIContent("Minimum", "The minimum scale your avatar can be. (Multiplier)"), min);
                GUILayout.FlexibleSpace();
                max = EditorGUILayout.FloatField(new GUIContent("Maximum", "The maximum scale your avatar can be. (Multiplier)"), max);
                GUILayout.FlexibleSpace();
                if (EditorGUI.EndChangeCheck())
                {
                    manager.sizes[0] = new Vector3(min * manager.sizes[1].x, min * manager.sizes[1].y, min * manager.sizes[1].z);
                    manager.sizes[2] = new Vector3(max * manager.sizes[1].x, max * manager.sizes[1].y, max * manager.sizes[1].z);
                }
                break;
            //Advanced
            case 1:
                EditorGUILayout.HelpBox("These values will NOT be multiplied by the scale of your Avatar.", MessageType.Info);
                GUILayout.FlexibleSpace();
                manager.sizes[0] = EditorGUILayout.Vector3Field(new GUIContent("Minimum", "The minimum scale your avatar can be. (Exact)"), manager.sizes[0]);
                GUILayout.FlexibleSpace();
                manager.sizes[2] = EditorGUILayout.Vector3Field(new GUIContent("Maximum", "The maximum scale your avatar can be. (Exact)"), manager.sizes[2]);
                EditorGUILayout.Space();
                GUILayout.FlexibleSpace();
                break;
        }
        manager.curveType = EditorGUILayout.Popup(new GUIContent("Curve Type", "The curve of the Animation"), manager.curveType, new string[] { "Default", "Clamped", "Linear" });
        GUILayout.FlexibleSpace();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Destination", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Path:", new GUIStyle(GUI.skin.GetStyle("Box")) { normal = new GUIStyleState() { background = null } });
        EditorGUILayout.LabelField(new GUIContent("<i>" + manager.outputPath.Replace('\\', '/') + "</i>", "The folder where any generated files will be saved to.\n(Generated files will overwrite existing files with the same name: <AVATAR NAME>_<TEMPLATE NAME>)"), new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true });
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Select"))
        {
            string absPath = EditorUtility.OpenFolderPanel("Destination Folder", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                manager.outputPath = "Assets" + absPath.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.Space();
        DrawLine();
        if (GUILayout.Button("Apply"))
        {
            switch (manager.ApplyChanges())
            {
                case 0:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Success!", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 1:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Success!\n\nWARNING: Submenu not added.\n(Submenu already exists on the given menu!)", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 2:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Success!\n\nWARNING: Submenu not added.\n(Given Expression Menu already contains 8 controls!)", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 3:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: No Avatar selected!", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 4:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: Scale already present in parameter list, but as the wrong type!", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 5:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: SizeOp already present in parameter list, but as the wrong type!", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 6:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: One or more parameters already present in an Animator, but as the wrong type!", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 7:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: Avatar does not contain a VRCExpressionParameters object!", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 8:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: No unused Expression Parameters found!\n(At least two unused parameters are needed.)", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 9:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: Failed to create one or more files!", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 10:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: Failed to copy layers to one or more Animators!", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
                case 99:
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: An exception occured! Please look at the console for further details.", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
            }
            Selection.activeGameObject = manager.avatar.gameObject;
        }
    }

    /*
     * These next two functions are literally just code from the Expression Menu for selecting the avatar.
     */

    void SelectAvatarDescriptor()
    {
        var descriptors = FindObjectsOfType<VRCAvatarDescriptor>();
        if (descriptors.Length > 0)
        {
            //Compile list of names
            string[] names = new string[descriptors.Length];
            for (int i = 0; i < descriptors.Length; i++)
                names[i] = descriptors[i].gameObject.name;

            //Select
            var currentIndex = Array.IndexOf(descriptors, manager.avatar);
            var nextIndex = EditorGUILayout.Popup(new GUIContent("Active Avatar", "The Avatar you want to setup scaling for."), currentIndex, names);
            if (nextIndex < 0)
                nextIndex = 0;
            if (nextIndex != currentIndex)
                SelectAvatarDescriptor(descriptors[nextIndex]);
        }
        else
            SelectAvatarDescriptor(null);
    }
    void SelectAvatarDescriptor(VRCAvatarDescriptor desc)
    {
        if (desc == manager.avatar)
            return;

        manager.avatar = desc;
    }

    private void DrawLine()
    {
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
}
