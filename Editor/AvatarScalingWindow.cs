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
    bool focused;

    int editMode;
    float minSimple = 0.5f;
    float maxSimple = 3.0f;
    Vector3 minAdvanced = new Vector3(0.5f, 0.5f, 0.5f);
    Vector3 maxAdvanced = new Vector3(3.0f, 3.0f, 3.0f);

    [MenuItem("Window/AV3 Tools/Avatar Scaling/Configure Scaling")]
    public static void ConfigureScaling()
    {
        AvatarScalingWindow window = (AvatarScalingWindow)GetWindow(typeof(AvatarScalingWindow), false, "Avatar Scaling");
        window.minSize = new Vector2(375f, 525f);
        window.wantsMouseMove = true;
        window.Show();
    }

    [MenuItem("Window/AV3 Tools/Avatar Scaling/Check For Updates")]
    public static void CheckForUpdates()
    {
        ASManager.CheckForUpdates();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginArea(new Rect((position.width / 2f) - (375f / 2f), 5f, 375f, 520f));
        windowTab = GUILayout.Toolbar(windowTab, new string[] { "Setup", "About" });
        DrawLine(false);
        switch (windowTab)
        {
            case 0:
                if (EditorApplication.isPlaying)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Scaling can only be setup outside of Play Mode.", MessageType.Info);
                }
                else if (found)
                {
                    GUILayout.BeginVertical();
                    DrawSetupWindow();
                    GUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Scaling Templates not found!", MessageType.Error);
                    EditorGUILayout.HelpBox("Make sure that the original Animators and Animations included in the package haven't been renamed!", MessageType.None);
                }
                GUILayout.EndArea();
                GUILayout.FlexibleSpace();
                EditorGUILayout.Space();
                GUILayout.EndVertical();
                break;
            case 1:
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
        manager.sizes[1] = (manager.avatar != null) ? manager.avatar.gameObject.transform.localScale : manager.sizes[1];
        if (editMode == 0)
        {
            switch (sizeTab)
            {
                case 0:
                    manager.sizes[0] = new Vector3(minSimple * manager.sizes[1].x, minSimple * manager.sizes[1].y, minSimple * manager.sizes[1].z);
                    manager.sizes[2] = new Vector3(maxSimple * manager.sizes[1].x, maxSimple * manager.sizes[1].y, maxSimple * manager.sizes[1].z);
                    break;
                case 1:
                    manager.sizes[0] = new Vector3(minAdvanced.x * manager.sizes[1].x, minAdvanced.y * manager.sizes[1].y, minAdvanced.z * manager.sizes[1].z);
                    manager.sizes[2] = new Vector3(maxAdvanced.x * manager.sizes[1].x, maxAdvanced.y * manager.sizes[1].y, maxAdvanced.z * manager.sizes[1].z);
                    break;
            }
        }        
    }
    
    private void DrawAboutWindow()
    {
        string version = (AssetDatabase.FindAssets("VERSION", new string[] { manager.relativePath }).Length > 0) ? " " + File.ReadAllText(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("VERSION", new string[] { manager.relativePath })[0])) : "";
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
        GUILayout.BeginVertical(GUILayout.Height(54f));
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
            if (editMode == 0)
            {
                switch (sizeTab)
                {
                    case 0:
                        manager.sizes[0] = new Vector3(minSimple * manager.sizes[1].x, minSimple * manager.sizes[1].y, minSimple * manager.sizes[1].z);
                        manager.sizes[2] = new Vector3(maxSimple * manager.sizes[1].x, maxSimple * manager.sizes[1].y, maxSimple * manager.sizes[1].z);
                        break;
                    case 1:
                        manager.sizes[0] = new Vector3(minAdvanced.x * manager.sizes[1].x, minAdvanced.y * manager.sizes[1].y, minAdvanced.z * manager.sizes[1].z);
                        manager.sizes[2] = new Vector3(maxAdvanced.x * manager.sizes[1].x, maxAdvanced.y * manager.sizes[1].y, maxAdvanced.z * manager.sizes[1].z);
                        break;
                }
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        DrawLine();
        GUILayout.Label("Optional Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.GetStyle("Box")), GUILayout.Height(75f));
        GUILayout.FlexibleSpace();
        manager.expressionsMenu = (VRCExpressionsMenu)EditorGUILayout.ObjectField(new GUIContent("Expressions Menu", "(Optional) The Expressions Menu you want the scaling controls added to. Leave this empty if you don't want any menus to be affected.\n(Controls will be added as a submenu.)"), manager.expressionsMenu, typeof(VRCExpressionsMenu), true);
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Add Parameters", "(Optional) Check for needed parameters within the Avatar's Expression Parameters and add them if necessary."), GUILayout.Width(145));
        manager.addExpressionParameters = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(manager.addExpressionParameters), new string[] { "No", "Yes" }));
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Use Existing Animators", "(Optional) If Animators are already present for Gesture, Sitting, or TPose, the parameters and layer for scaling will be added to them. If an Animator is missing or this feature is disabled, a new Animator will be generated using the defaults included in the VRChat SDK and inserted into the descriptor automatically."), GUILayout.Width(145));
        manager.insertLayers = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(manager.insertLayers), new string[] { "No", "Yes" }));
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Size Settings", EditorStyles.boldLabel);
        GUILayout.BeginVertical(new GUIStyle(GUI.skin.GetStyle("Box")), GUILayout.Height(120f));
        EditorGUI.BeginChangeCheck();
        sizeTab = GUILayout.Toolbar(sizeTab, new string[] { "Simple", "Advanced" });
        editMode = EditorGUILayout.Popup(new GUIContent("Edit Mode", "(Multiplier) Edit sizes relative to your initial scale.\n(Exact) Edit the exact sizes you will scale to."), editMode, new string[] { "Multiplier", "Exact" });
        if (EditorGUI.EndChangeCheck())
        {
            switch (sizeTab)
            {
                case 0:
                    switch (editMode)
                    {
                        case 0:
                            minSimple = manager.sizes[0].y / manager.sizes[1].y;
                            maxSimple = manager.sizes[2].y / manager.sizes[1].y;
                            break;
                        case 1:
                            minSimple = manager.sizes[0].y;
                            maxSimple = manager.sizes[2].y;
                            break;
                    }
                    break;
                case 1:
                    switch (editMode)
                    {
                        case 0:
                            minAdvanced = new Vector3(manager.sizes[0].x / manager.sizes[1].x, manager.sizes[0].y / manager.sizes[1].y, manager.sizes[0].z / manager.sizes[1].z);
                            maxAdvanced = new Vector3(manager.sizes[2].x / manager.sizes[1].x, manager.sizes[2].y / manager.sizes[1].y, manager.sizes[2].z / manager.sizes[1].z);
                            break;
                        case 1:
                            minAdvanced = manager.sizes[0];
                            maxAdvanced = manager.sizes[2];
                            break;
                    }
                    break;
            }
        }
        switch (sizeTab)
        {
            //Simple
            case 0:              
                EditorGUI.BeginChangeCheck();
                GUILayout.FlexibleSpace();
                minSimple = EditorGUILayout.FloatField(new GUIContent("Minimum", "The minimum scale your avatar can be."), minSimple);
                GUILayout.FlexibleSpace();
                maxSimple = EditorGUILayout.FloatField(new GUIContent("Maximum", "The maximum scale your avatar can be."), maxSimple);
                GUILayout.FlexibleSpace();
                if (EditorGUI.EndChangeCheck())
                {
                    switch (editMode)
                    {
                        case 0:
                            manager.sizes[0] = new Vector3(minSimple * manager.sizes[1].x, minSimple * manager.sizes[1].y, minSimple * manager.sizes[1].z);
                            manager.sizes[2] = new Vector3(maxSimple * manager.sizes[1].x, maxSimple * manager.sizes[1].y, maxSimple * manager.sizes[1].z);
                            break;
                        case 1:
                            manager.sizes[0] = new Vector3(minSimple, minSimple, minSimple);
                            manager.sizes[2] = new Vector3(maxSimple, maxSimple, maxSimple);
                            break;
                    }               
                }
                break;
            //Advanced
            case 1:
                EditorGUI.BeginChangeCheck();
                GUILayout.FlexibleSpace();
                minAdvanced = EditorGUILayout.Vector3Field(new GUIContent("Minimum", "The minimum scale your avatar can be."), minAdvanced);
                GUILayout.FlexibleSpace();
                maxAdvanced = EditorGUILayout.Vector3Field(new GUIContent("Maximum", "The maximum scale your avatar can be."), maxAdvanced);
                EditorGUILayout.Space();
                GUILayout.FlexibleSpace();
                if (EditorGUI.EndChangeCheck())
                {
                    switch (editMode)
                    {
                        case 0:
                            manager.sizes[0] = new Vector3(minAdvanced.x * manager.sizes[1].x, minAdvanced.y * manager.sizes[1].y, minAdvanced.z * manager.sizes[1].z);
                            manager.sizes[2] = new Vector3(maxAdvanced.x * manager.sizes[1].x, maxAdvanced.y * manager.sizes[1].y, maxAdvanced.z * manager.sizes[1].z);
                            break;
                        case 1:
                            manager.sizes[0] = minAdvanced;
                            manager.sizes[2] = maxAdvanced;
                            break;
                    }
                }
                break;
        }
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(new GUIStyle(GUI.skin.GetStyle("Box")), GUILayout.Height(40f));
        manager.curveType = EditorGUILayout.Popup(new GUIContent("Curve Type", "What curve the scaling Animation should use."), manager.curveType, new string[] { "Default", "Clamped", "Linear", "Custom" });
        if (manager.curveType == 3)
        {
            manager.customCurve = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("Reference", "Custom Animation curve to use as reference.\n(Refer to the Github for proper usage.)"), manager.customCurve, typeof(AnimationClip), true);
        }
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Output Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.GetStyle("Box")), GUILayout.Height(50f));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Destination", "The folder where generated files will be saved to."), new GUIStyle(GUI.skin.GetStyle("Box")) { alignment = TextAnchor.MiddleLeft, normal = new GUIStyleState() { background = null } });
        GUILayout.FlexibleSpace();
        string displayPath = manager.outputPath.Replace('\\', '/');
        displayPath = ((new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true, hover = GUI.skin.GetStyle("Button").active }).CalcSize(new GUIContent("<i>" + displayPath + "</i>")).x > 210) ? "..." + displayPath.Substring((displayPath.IndexOf('/', displayPath.Length - 29) != -1) ? displayPath.IndexOf('/', displayPath.Length - 29) : displayPath.Length - 29) : displayPath;
        while ((new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true, hover = GUI.skin.GetStyle("Button").active }).CalcSize(new GUIContent("<i>" + displayPath + "</i>")).x > 210)
        {
            displayPath = "..." + displayPath.Substring(4);
        }
        if (GUILayout.Button("<i>" + displayPath + "</i>", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true, hover = GUI.skin.GetStyle("Button").active }, GUILayout.MinWidth(210)))
        {
            string absPath = EditorUtility.OpenFolderPanel("Destination Folder", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                manager.outputPath = "Assets" + absPath.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Overwrite All", "Automatically overwrite existing files if needed."), GUILayout.Width(145));
        manager.autoOverwrite = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(manager.autoOverwrite), new string[] { "No", "Yes" }));
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Settings", GUILayout.Width(360f / 2)))
        {
            ResetSettings();
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Apply Changes", GUILayout.Width(360f / 2)))
        {
            string message = "If this appears, then something has gone horribly wrong.\nPlease file a bug report with steps to reproduce.";
            switch (manager.ApplyChanges())
            {
                case 0:
                    message = "Success!";
                    break;
                case 1:
                    message = "Success!\n\nWARNING: Submenu not added.\n(Submenu already exists on the given menu!)";
                    break;
                case 2:
                    message = "Success!\n\nWARNING: Submenu not added.\n(Given Expression Menu already contains 8 controls!)";
                    break;
                case 3:
                    message = "Failed!\n\nERROR: No Avatar selected!";
                    break;
                case 4:
                    message = "Failed!\n\nERROR: Scale already present in parameter list, but as the wrong type!";
                    break;
                case 5:
                    message = "Failed!\n\nERROR: SizeOp already present in parameter list, but as the wrong type!";
                    break;
                case 6:
                    message = "Failed!\n\nERROR: One or more parameters already present in an Animator, but as the wrong type!";
                    break;
                case 7:
                    message = "Failed!\n\nERROR: Avatar does not contain a VRCExpressionParameters object!";
                    break;
                case 8:
                    message = "Failed!\n\nERROR: No unused Expression Parameters found!\n(At least two unused parameters are needed.)";
                    break;
                case 9:
                    message = "Failed!\n\nERROR: No Animation curve provided as reference!";
                    break;
                case 10:
                    message = "Failed!\n\nERROR: Provided reference curve cannot be used!";
                    break;
                case 11:
                    message = "Failed!\n\nERROR: Failed to create one or more files!";
                    break;
                case 12:
                    message = "Failed!\n\nERROR: Failed to copy layers to one or more Animators!";
                    break;
                case 98:
                    message = "Canceled!";
                    break;
                case 99:
                    message = "Failed!\n\nERROR: An exception occured!\nPlease look at the console for further details.";
                    break;
            }
            if (EditorUtility.DisplayDialog("Avatar Scaling", message, "Close"))
            {
                EditorUtility.ClearProgressBar();
                Selection.activeGameObject = manager.avatar.gameObject;
            }
        }
        GUILayout.EndHorizontal();
        if (mouseOverWindow != null && mouseOverWindow == this)
        {
            Repaint();
            focused = true;
        }
        else if (focused && mouseOverWindow == null)
        {
            Repaint();
            focused = false;
        }
    }

    private void ResetSettings()
    {
        editMode = 0;
        minSimple = 0.5f;
        maxSimple = 3.0f;
        minAdvanced = new Vector3(0.5f, 0.5f, 0.5f);
        maxAdvanced = new Vector3(3.0f, 3.0f, 3.0f);

        manager.expressionsMenu = null;
        manager.addExpressionParameters = true;
        manager.insertLayers = true;
        manager.sizes[0] = new Vector3(minSimple * manager.sizes[1].x, minSimple * manager.sizes[1].y, minSimple * manager.sizes[1].z);
        manager.sizes[2] = new Vector3(maxSimple * manager.sizes[1].x, maxSimple * manager.sizes[1].y, maxSimple * manager.sizes[1].z);
        manager.curveType = 0;
        manager.outputPath = manager.relativePath + Path.DirectorySeparatorChar + "Output";
        manager.autoOverwrite = false;
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

    private void DrawLine(bool addSpace)
    {
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        if (addSpace)
        {
            EditorGUILayout.Space();
        }
    }
}
