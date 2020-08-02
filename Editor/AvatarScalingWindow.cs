using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

public class AvatarScalingWindow : EditorWindow
{
    //Window Stuff

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

    //Fixes weird bug where menu refuses to reopen.
    private void OnDestroy()
    {
        AvatarScalingWindow window = (AvatarScalingWindow)EditorWindow.GetWindow(typeof(AvatarScalingWindow), false, "Avatar Scaling");
        window = null;
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
        found = FindTemplates();
        if (avatar == null)
        {
            avatar = FindObjectOfType<VRCAvatarDescriptor>();
        }
        sizes[1] = (avatar != null) ? avatar.gameObject.transform.localScale : sizes[1];
        if (sizeTab == 0)
        {
            sizes[0] = new Vector3(min * sizes[1].x, min * sizes[1].y, min * sizes[1].z);
            sizes[2] = new Vector3(max * sizes[1].x, max * sizes[1].y, max * sizes[1].z);
        }
    }

    private void DrawAboutWindow()
    {
        string version = (AssetDatabase.FindAssets("VERSION", new string[] { "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Editor" }).Length > 0) ? " " + File.ReadAllText(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("VERSION", new string[] { "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Editor" })[0])) : "";
        GUILayout.Label("Avatar Scaling" + version, new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Author: Joshuarox100", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Summary", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.UpperCenter });
        GUILayout.Box("With Avatars 3.0, you can now change the scale of your Avatar and viewpoint in realtime! This tool exists to set everything up needed for scaling to work on any given 3.0 Avatar with minimal user effort.\n");
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Special Thanks", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.UpperCenter });
        GUILayout.Box("<i>Ambiguous</i>\nFor helping me test things throughout the entirety of development.\n\n" +
            "<i>PhaxeNor</i>\nFor inspiring me to make this setup script for the tool.\n\n" +
            "<i>Mr. Doon</i>\nFor suggesting significant improvements to the Animators.\n", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true });
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Troubleshooting", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.UpperCenter });
        GUILayout.Box("If you're having issues or want to contact me, you can find more information at the Github page linked below!\n", new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true });
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
        EditorGUILayout.Space();

    }
    private void DrawSetupWindow()
    {
        GUILayout.Label("Avatar Settings", EditorStyles.boldLabel);
        GUILayout.BeginVertical(GUILayout.Height(40f));
        EditorGUI.BeginChangeCheck();
        GUILayout.FlexibleSpace();
        SelectAvatarDescriptor();
        if (avatar == null)
        {
            EditorGUILayout.HelpBox("No Avatars found in the current Scene!", MessageType.Warning);
        }
        if (EditorGUI.EndChangeCheck())
        {
            sizes[1] = (avatar != null) ? avatar.gameObject.transform.localScale : sizes[1];
            if (sizeTab == 0)
            {
                sizes[0] = new Vector3(min * sizes[1].x, min * sizes[1].y, min * sizes[1].z);
                sizes[2] = new Vector3(max * sizes[1].x, max * sizes[1].y, max * sizes[1].z);
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Optional Settings", EditorStyles.boldLabel);
        expressionsMenu = (VRCExpressionsMenu)EditorGUILayout.ObjectField(new GUIContent("Expressions Menu", "(Optional) The Expressions Menu you want the scaling controls added to. Leave this empty if you don't want any menus to be affected.\n(Controls will be added as a submenu.)"), expressionsMenu, typeof(VRCExpressionsMenu), true);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Add Parameters", "Check the Avatar's Expression Parameters for the needed parameters for scaling. If any are not present, it will attempt to add them."), GUILayout.Width(145));
        addExpressionParameters = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(addExpressionParameters), new string[] { "No", "Yes" }));
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Use Existing Animators", "If Animators are already present for Gesture, Sitting, or TPose, the parameters and layer for scaling will be added to them. If an Animator is missing or this feature is disabled, a new Animator will be generated using the defaults included in the VRChat SDK and inserted into the descriptor automatically."), GUILayout.Width(145));
        insertLayers = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(insertLayers), new string[] { "No", "Yes" }));
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
                    sizes[0] = new Vector3(min * sizes[1].x, min * sizes[1].y, min * sizes[1].z);
                    sizes[2] = new Vector3(max * sizes[1].x, max * sizes[1].y, max * sizes[1].z);
                }
                break;
            //Advanced
            case 1:
                EditorGUILayout.HelpBox("These values will NOT be multiplied by the scale of your Avatar.", MessageType.Info);
                GUILayout.FlexibleSpace();
                sizes[0] = EditorGUILayout.Vector3Field(new GUIContent("Minimum", "The minimum scale your avatar can be. (Exact)"), sizes[0]);
                GUILayout.FlexibleSpace();
                sizes[2] = EditorGUILayout.Vector3Field(new GUIContent("Maximum", "The maximum scale your avatar can be. (Exact)"), sizes[2]);
                EditorGUILayout.Space();
                GUILayout.FlexibleSpace();
                break;
        }
        curveType = EditorGUILayout.Popup(new GUIContent("Curve Type", "The curve of the Animation"), curveType, new string[] { "Default", "Clamped", "Linear" });
        GUILayout.FlexibleSpace();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Destination", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Path:", new GUIStyle(GUI.skin.GetStyle("Box")) { normal = new GUIStyleState() { background = null } });
        EditorGUILayout.LabelField(new GUIContent("<i>" + outputPath + "</i>", "The folder where any generated files will be saved to.\n(Generated files will overwrite existing files with the same name: <AVATAR NAME>_<TEMPLATE NAME>)"), new GUIStyle(GUI.skin.GetStyle("Box")) { richText = true });
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Select"))
        {
            string absPath = EditorUtility.OpenFolderPanel("Destination Folder", "", "");
            if (absPath.StartsWith(Application.dataPath))
            {
                outputPath = "Assets" + absPath.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.Space();
        DrawLine();
        if (GUILayout.Button("Apply"))
        {
            switch (ApplyChanges())
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
                    if (EditorUtility.DisplayDialog("Avatar Scaling Setup", "Failed!\n\nERROR: An Exception occured! Please look at the console for further details.", "Close"))
                        EditorUtility.ClearProgressBar();
                    break;
            }
        }
    }

    /*
     * These next two functions are literally just code from the Expression Menu for selecting the avatar.
     */

    void SelectAvatarDescriptor()
    {
        var descriptors = GameObject.FindObjectsOfType<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
        if (descriptors.Length > 0)
        {
            //Compile list of names
            string[] names = new string[descriptors.Length];
            for (int i = 0; i < descriptors.Length; i++)
                names[i] = descriptors[i].gameObject.name;

            //Select
            var currentIndex = System.Array.IndexOf(descriptors, avatar);
            var nextIndex = EditorGUILayout.Popup(new GUIContent("Active Avatar", "The Avatar you want to setup scaling for."), currentIndex, names);
            if (nextIndex < 0)
                nextIndex = 0;
            if (nextIndex != currentIndex)
                SelectAvatarDescriptor(descriptors[nextIndex]);
        }
        else
            SelectAvatarDescriptor(null);
    }
    void SelectAvatarDescriptor(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor desc)
    {
        if (desc == avatar)
            return;

        avatar = desc;
    }

    private void DrawLine()
    {
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    //Scripting Stuff

    private AnimatorController[] templateAnimators = new AnimatorController[3];
    private AnimationClip templateSizes;
    private VRCExpressionsMenu templateMenu;

    public VRCAvatarDescriptor avatar;
    public VRCExpressionsMenu expressionsMenu;
    public bool addExpressionParameters = true;
    public bool insertLayers = true;
    public Vector3[] sizes = new Vector3[] { new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(3.0f, 3.0f, 3.0f) };
    public int curveType;

    public string outputPath = "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Output";
    public int ApplyChanges()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Avatar Scaling", "Starting", 0f);
            /*
            // Check if an Avatar is selected. 
            */

            if (avatar == null)
            {
                return 3;
            }

            /*
            // Create any files needed in destination folder.
            */

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Checking Destination", 0.05f);
            VerifyDestination();

            //Copy SDK templates (if needed)
            if (insertLayers)
            {
                EditorUtility.DisplayProgressBar("Avatar Scaling", "Looking for Existing Animators", 0.1f);
            }

            AnimatorController gesture = (avatar.baseAnimationLayers[2].animatorController != null && insertLayers) ? (AnimatorController)avatar.baseAnimationLayers[2].animatorController : null;
            AnimatorController sitting = (avatar.specialAnimationLayers[0].animatorController != null && insertLayers) ? (AnimatorController)avatar.specialAnimationLayers[0].animatorController : null;
            AnimatorController tpose = (avatar.specialAnimationLayers[1].animatorController != null && insertLayers) ? (AnimatorController)avatar.specialAnimationLayers[1].animatorController : null;
            AnimationClip sizeSettings = new AnimationClip();

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Creating New Files", 0.2f);

            if (gesture == null || gesture == templateAnimators[0] || gesture == templateAnimators[1] || gesture == templateAnimators[2])
            {
                if (!AssetDatabase.IsValidFolder(outputPath + Path.DirectorySeparatorChar + "Animators"))
                    AssetDatabase.CreateFolder(outputPath, "Animators");
                if (!AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("vrc_AvatarV3HandsLayer", new string[] { "Assets" + Path.DirectorySeparatorChar + "VRCSDK" + Path.DirectorySeparatorChar + "Examples3" + Path.DirectorySeparatorChar + "Animation" + Path.DirectorySeparatorChar + "Controllers" })[0]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Gesture.controller"))
                {
                    return 9;
                }
            }

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Creating New Files", 0.25f);

            if (sitting == null || sitting == templateAnimators[0] || sitting == templateAnimators[1] || sitting == templateAnimators[2])
            {
                if (!AssetDatabase.IsValidFolder(outputPath + Path.DirectorySeparatorChar + "Animators"))
                    AssetDatabase.CreateFolder(outputPath, "Animators");
                if (!AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("vrc_AvatarV3SittingLayer", new string[] { "Assets" + Path.DirectorySeparatorChar + "VRCSDK" + Path.DirectorySeparatorChar + "Examples3" + Path.DirectorySeparatorChar + "Animation" + Path.DirectorySeparatorChar + "Controllers" })[0]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Sitting.controller"))
                {
                    return 9;
                }
            }

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Creating New Files", 0.3f);

            if (tpose == null || tpose == templateAnimators[0] || tpose == templateAnimators[1] || tpose == templateAnimators[2])
            {
                if (!AssetDatabase.IsValidFolder(outputPath + Path.DirectorySeparatorChar + "Animators"))
                    AssetDatabase.CreateFolder(outputPath, "Animators");
                if (!AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("vrc_AvatarV3UtilityTPose", new string[] { "Assets" + Path.DirectorySeparatorChar + "VRCSDK" + Path.DirectorySeparatorChar + "Examples3" + Path.DirectorySeparatorChar + "Animation" + Path.DirectorySeparatorChar + "Controllers" })[0]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_TPose.controller"))
                {
                    return 9;
                }
            }

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Creating New Files", 0.35f);

            if (!AssetDatabase.IsValidFolder(outputPath + Path.DirectorySeparatorChar + "Animations"))
                AssetDatabase.CreateFolder(outputPath, "Animations");
            if (!AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(templateSizes), outputPath + Path.DirectorySeparatorChar + "Animations" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Sizes.anim"))
            {
                return 9;
            }

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Refreshing Asset Database", 0.4f);
            AssetDatabase.Refresh();

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Loading New Files", 0.45f);

            string[] results = AssetDatabase.FindAssets(avatar.gameObject.name + "_", new string[] { outputPath });
            foreach (string guid in results)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                //Gesture Animator
                if (path.Contains(avatar.gameObject.name + "_Gesture.controller") && gesture == null)
                {
                    gesture = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
                }
                //Sitting Animator
                else if (path.Contains(avatar.gameObject.name + "_Sitting.controller") && sitting == null)
                {
                    sitting = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
                }
                //TPose Animator
                else if (path.Contains(avatar.gameObject.name + "_TPose.controller") && tpose == null)
                {
                    tpose = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
                }
                //Size Settings Anim
                else if (path.Contains(avatar.gameObject.name + "_Sizes.anim"))
                {
                    sizeSettings = (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
                }
            }

            /*
            // Append scaling layers to Animators. 
            */

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Appending Layers", 0.5f);
            if (!AddLayersParameters(gesture, templateAnimators[0]))
            {
                return 6;
            }
            EditorUtility.DisplayProgressBar("Avatar Scaling", "Appending Layers", 0.65f);
            if (!AddLayersParameters(sitting, templateAnimators[1]))
            {
                return 6;
            }
            EditorUtility.DisplayProgressBar("Avatar Scaling", "Appending Layers", 0.8f);
            if (!AddLayersParameters(tpose, templateAnimators[2]))
            {
                return 6;
            }

            /*
            // Modify copy of AnimationClip to use new sizes.
            */

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Configuring Animations", 0.825f);
            ModifyAnimation(sizeSettings);

            /*
            // Replace reference to template AnimationClip in Gesture with the modified one.
            */

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Configuring Animators", 0.85f);
            if (!ReplaceAnimation(gesture, "Scaling", templateSizes, sizeSettings))
            {
                return 10;
            }

            /*
            // Add new Animators to the Avatar Descriptor if possible. 
            */

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Updating Avatar Descriptor", 0.875f);

            //Enable custom layers
            if (avatar.customizeAnimationLayers == false)
            {
                avatar.customizeAnimationLayers = true;
            }

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Updating Avatar Descriptor", 0.9f);
            //Add Gesture
            if (!avatar.baseAnimationLayers[2].isEnabled)
            {
                avatar.baseAnimationLayers[2].isEnabled = true;
                avatar.baseAnimationLayers[2].isDefault = false;
                avatar.baseAnimationLayers[2].animatorController = gesture;
            }
            else if (avatar.baseAnimationLayers[2].animatorController == null)
            {
                avatar.baseAnimationLayers[2].animatorController = gesture;
                avatar.baseAnimationLayers[2].isDefault = false;
            }

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Updating Avatar Descriptor", 0.92f);
            //Add Sitting
            if (!avatar.specialAnimationLayers[0].isEnabled)
            {
                avatar.specialAnimationLayers[0].isEnabled = true;
                avatar.specialAnimationLayers[0].isDefault = false;
                avatar.specialAnimationLayers[0].animatorController = sitting;
            }
            else if (avatar.specialAnimationLayers[0].animatorController == null)
            {
                avatar.specialAnimationLayers[0].animatorController = sitting;
                avatar.specialAnimationLayers[0].isDefault = false;
            }
            EditorUtility.DisplayProgressBar("Avatar Scaling", "Updating Avatar Descriptor", 0.94f);
            //Add TPose
            if (!avatar.specialAnimationLayers[1].isEnabled)
            {
                avatar.specialAnimationLayers[1].isEnabled = true;
                avatar.specialAnimationLayers[1].isDefault = false;
                avatar.specialAnimationLayers[1].animatorController = tpose;
            }
            else if (avatar.specialAnimationLayers[1].animatorController == null)
            {
                avatar.specialAnimationLayers[1].animatorController = tpose;
                avatar.specialAnimationLayers[1].isDefault = false;
            }

            /*
            // Check avatar's ExpressionParameters for needed parameters. Skip if present, attempt to append to list if absent. In cases where the list is full, inform the user and abort. 
            */

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Finalizing", 0.96f);
            if (addExpressionParameters)
            {
                if (avatar.expressionParameters == null)
                {
                    return 7;
                }

                VRCExpressionParameters avatarParameters = avatar.expressionParameters;
                int count = 0;
                bool scalePresent = false;
                bool sizeOpPresent = false;
                for (int i = 0; i < avatarParameters.parameters.Length; i++)
                {
                    switch (avatarParameters.parameters[i].name)
                    {
                        case "Scale":
                            if (avatarParameters.parameters[i].valueType != VRCExpressionParameters.ValueType.Float)
                            {
                                return 4;
                            }
                            scalePresent = true;
                            break;
                        case "SizeOp":
                            if (avatarParameters.parameters[i].valueType != VRCExpressionParameters.ValueType.Int)
                            {
                                return 5;
                            }
                            sizeOpPresent = true;
                            break;
                        case "":
                            break;
                        default:
                            count++;
                            break;
                    }
                }

                if ((count >= 15 && !scalePresent && !sizeOpPresent) || (count == 16 && (!scalePresent || !sizeOpPresent)))
                {
                    return 8;
                }
                else
                {
                    if (!scalePresent)
                    {
                        for (int i = 0; i < avatarParameters.parameters.Length; i++)
                        {
                            if (avatarParameters.parameters[i].name == "")
                            {
                                avatarParameters.parameters[i].name = "Scale";
                                avatarParameters.parameters[i].valueType = VRCExpressionParameters.ValueType.Float;
                                break;
                            }
                        }
                    }
                    if (!sizeOpPresent)
                    {
                        for (int i = 0; i < avatarParameters.parameters.Length; i++)
                        {
                            if (avatarParameters.parameters[i].name == "")
                            {
                                avatarParameters.parameters[i].name = "SizeOp";
                                avatarParameters.parameters[i].valueType = VRCExpressionParameters.ValueType.Int;
                                break;
                            }
                        }
                    }
                }
            }

            /*
            // Check if a Expressions Menu was provided and attempt to add Scale Controls as a submenu to it. If none was provided then assign the template to the descriptor if the slot is empty.
            */

            EditorUtility.DisplayProgressBar("Avatar Scaling", "Finalizing", 0.98f);
            if (expressionsMenu != null)
            {
                if (expressionsMenu.controls.Count == 8)
                {
                    return 2;
                }
                else
                {
                    bool exists = false;
                    foreach (VRCExpressionsMenu.Control control in expressionsMenu.controls)
                    {
                        if (control.type == VRCExpressionsMenu.Control.ControlType.SubMenu && control.subMenu == templateMenu.controls[0].subMenu)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        expressionsMenu.controls.Add(templateMenu.controls[0]);
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else
            {
                if (avatar.expressionsMenu == null)
                {
                    avatar.expressionsMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Scale Controls", new string[] { "Assets/Avatar Scaling/Menus" })[0]), typeof(VRCExpressionsMenu));
                }
            }
            EditorUtility.DisplayProgressBar("Avatar Scaling", "Finalizing", 1.0f);

            return 0;
        }
        catch (Exception err)
        {
            Debug.LogError(err);
            return 99;
        }
    }

    private void VerifyDestination()
    {
        if (!AssetDatabase.IsValidFolder(outputPath))
        {
            if (!AssetDatabase.IsValidFolder("Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Output"))
            {
                string guid = AssetDatabase.CreateFolder("Assets" + Path.DirectorySeparatorChar + "Avatar Scaling", "Output");
                outputPath = AssetDatabase.GUIDToAssetPath(guid);
            }
            else
            {
                outputPath = "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Output";
            }
        }
    }

    private void ModifyAnimation(AnimationClip anim)
    {
        foreach (var binding in AnimationUtility.GetCurveBindings(anim))
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(anim, binding);
            Keyframe[] keys = curve.keys;
            switch (binding.propertyName)
            {
                case "m_LocalScale.x":
                    keys[0].value = sizes[0].x;
                    keys[1].value = sizes[1].x;
                    keys[2].value = sizes[2].x;
                    break;
                case "m_LocalScale.y":
                    keys[0].value = sizes[0].y;
                    keys[1].value = sizes[1].y;
                    keys[2].value = sizes[2].y;
                    break;
                case "m_LocalScale.z":
                    keys[0].value = sizes[0].z;
                    keys[1].value = sizes[1].z;
                    keys[2].value = sizes[2].z;
                    break;
            }
            curve.keys = keys;
            switch (curveType)
            {
                case 1:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        AnimationUtility.SetKeyBroken(curve, i, true);
                        AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
                    }
                    break;
                case 2:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        AnimationUtility.SetKeyBroken(curve, i, true);
                        AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                        AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                    }
                    break;
            }
            AnimationUtility.SetEditorCurve(anim, binding, curve);
        }
    }

    private bool ReplaceAnimation(AnimatorController source, string layerName, AnimationClip oldAnim, AnimationClip newAnim)
    {
        AnimatorControllerLayer[] layers = source.layers;
        AnimatorControllerLayer selectedLayer = new AnimatorControllerLayer();

        foreach (AnimatorControllerLayer layer in layers)
        {
            if (layer.name == layerName)
            {
                selectedLayer = layer;
                break;
            }
        }
        if (selectedLayer == null)
        {
            return false;
        }

        for (int i = 0; i < selectedLayer.stateMachine.states.Length; i++)
        {
            if (AssetDatabase.GetAssetPath(selectedLayer.stateMachine.states[i].state.motion) == AssetDatabase.GetAssetPath(oldAnim))
            {
                selectedLayer.stateMachine.states[i].state.motion = newAnim;
            }
        }

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name == layerName)
            {
                layers[i] = selectedLayer;
                break;
            }
        }

        source.layers = layers;

        return true;
    }

    private bool AddLayersParameters(AnimatorController source, AnimatorController target)
    {
        //Clone Target Animator
        if (!AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(target), "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Templates" + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + "Temporary.controller"))
        {
            return false;
        }
        AssetDatabase.Refresh();

        AnimatorController cloned = null;

        string[] results = AssetDatabase.FindAssets("Temporary", new string[] { "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Templates" + Path.DirectorySeparatorChar + "Animators" });

        foreach (string guid in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            //Debug.Log([Avatar Scaling Setup] Clone : path);
            if (path.Contains("Temporary.controller"))
            {
                cloned = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
            }
        }

        if (cloned == null)
        {
            return false;
        }

        //Check and Add Parameters
        AnimatorControllerParameter[] srcParam = source.parameters;
        AnimatorControllerParameter[] tarParam = cloned.parameters;

        foreach (AnimatorControllerParameter param in tarParam)
        {
            bool exists = false;
            for (int i = 0; i < srcParam.Length; i++)
            {
                if (param.name == srcParam[i].name)
                {
                    if (param.type == srcParam[i].type)
                    {
                        exists = true;
                        break;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (!exists)
            {
                AnimatorControllerParameter[] temp = new AnimatorControllerParameter[srcParam.Length + 1];
                srcParam.CopyTo(temp, 0);
                temp[temp.Length - 1] = param;
                srcParam = temp;
            }
        }

        source.parameters = srcParam;

        //Check and Add/Replace Layers
        AnimatorControllerLayer[] srcLayers = source.layers;
        AnimatorControllerLayer[] tarLayers = cloned.layers;

        foreach (AnimatorControllerLayer layer in tarLayers)
        {
            for (int i = 0; i < srcLayers.Length; i++)
            {
                if (layer.name == srcLayers[i].name)
                {
                    AssetDatabase.RemoveObjectFromAsset(layer.stateMachine);
                    source.RemoveLayer(i);
                    break;
                }
            }
            source.AddLayer(layer.name);
            srcLayers = source.layers;
            AnimatorControllerLayer copiedLayer = DeepCloneLayer(layer);
            srcLayers[srcLayers.Length - 1] = copiedLayer;
            source.layers = srcLayers;
            //if (AssetDatabase.GetAssetPath(a.stateMachine) != "") // doesn't work for some reasons
            if (AssetDatabase.GetAssetPath(copiedLayer.stateMachine).Length == 0)
            {
                AssetDatabase.AddObjectToAsset(copiedLayer.stateMachine, AssetDatabase.GetAssetPath(source));
                copiedLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        if (!SaveController(source))
        {
            return false;
        }

        //Delete clone
        if (!AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cloned)))
        {
            return false;
        }
        AssetDatabase.Refresh();

        return true;
    }

    private AnimatorControllerLayer DeepCloneLayer(AnimatorControllerLayer layer)
    {
        AnimatorControllerLayer output = new AnimatorControllerLayer();
        output.name = layer.name;
        output.defaultWeight = layer.defaultWeight;
        output.avatarMask = layer.avatarMask;
        output.blendingMode = layer.blendingMode;
        output.iKPass = layer.iKPass;
        output.syncedLayerIndex = output.syncedLayerIndex;
        output.syncedLayerAffectsTiming = output.syncedLayerAffectsTiming;
        output.stateMachine = CloneMachine(layer.stateMachine);
        return output;
    }

    private AnimatorStateMachine CloneMachine(AnimatorStateMachine machine)
    {
        AnimatorStateMachine output = new AnimatorStateMachine();
        EditorUtility.CopySerialized(machine, output);

        ChildAnimatorStateMachine[] outMachines = new ChildAnimatorStateMachine[machine.stateMachines.Length];
        for (int i = 0; i < machine.stateMachines.Length; i++)
        {
            outMachines[i] = new ChildAnimatorStateMachine();
            outMachines[i].position = machine.stateMachines[i].position;
            outMachines[i].stateMachine = CloneMachine(machine.stateMachines[i].stateMachine);
        }
        output.stateMachines = outMachines;

        ChildAnimatorState[] outStates = new ChildAnimatorState[machine.states.Length];
        for (int i = 0; i < machine.states.Length; i++)
        {
            outStates[i] = new ChildAnimatorState();
            outStates[i].position = machine.states[i].position;
            outStates[i].state = CloneState(machine.states[i].state);
        }
        for (int i = 0; i < machine.states.Length; i++)
        {
            AnimatorStateTransition[] fixedTransitions = outStates[i].state.transitions;
            foreach (AnimatorStateTransition transition in fixedTransitions)
            {
                for (int j = 0; j < machine.states.Length; j++)
                {
                    if (transition.destinationState != null && transition.destinationState.name == machine.states[j].state.name)
                    {
                        transition.destinationState = outStates[j].state;
                        break;
                    }
                }
            }
            outStates[i].state.transitions = fixedTransitions;
        }
        output.states = outStates;

        AnimatorStateTransition[] outAnyTransitions = new AnimatorStateTransition[machine.anyStateTransitions.Length];
        for (int i = 0; i < machine.anyStateTransitions.Length; i++)
        {
            outAnyTransitions[i] = CloneStateTransition(machine.anyStateTransitions[i]);
        }
        AnimatorStateTransition[] fixedAnyTransitions = outAnyTransitions;
        foreach (AnimatorStateTransition transition in fixedAnyTransitions)
        {
            for (int j = 0; j < machine.states.Length; j++)
            {
                if (transition.destinationState != null && transition.destinationState.name == machine.states[j].state.name)
                {
                    transition.destinationState = outStates[j].state;
                    break;
                }
            }
        }
        outAnyTransitions = fixedAnyTransitions;
        output.anyStateTransitions = outAnyTransitions;

        foreach (ChildAnimatorState state in outStates)
        {
            if (state.state.name == machine.defaultState.name)
            {
                output.defaultState = state.state;
                break;
            }
        }

        return output;
    }

    private AnimatorState CloneState(AnimatorState state)
    {
        AnimatorState output = new AnimatorState();
        EditorUtility.CopySerialized(state, output);
        
        AnimatorStateTransition[] outTransitions = new AnimatorStateTransition[state.transitions.Length];
        for (int i = 0; i < state.transitions.Length; i++)
        {
            outTransitions[i] = CloneStateTransition(state.transitions[i]);
        }
        output.transitions = outTransitions;

        StateMachineBehaviour[] outBehaviors = new StateMachineBehaviour[state.behaviours.Length];
        for (int i = 0; i < state.behaviours.Length; i++)
        {
            outBehaviors[i] = CloneStateBehaviors(state.behaviours[i]);
        }
        output.behaviours = outBehaviors;

        return output;
    }

    private AnimatorStateTransition CloneStateTransition(AnimatorStateTransition transition)
    {
        AnimatorStateTransition output = new AnimatorStateTransition();
        EditorUtility.CopySerialized(transition, output);
        return output;
    }

    private StateMachineBehaviour CloneStateBehaviors(StateMachineBehaviour behavior)
    {
        StateMachineBehaviour output = (StateMachineBehaviour)ScriptableObject.CreateInstance(behavior.GetType());
        EditorUtility.CopySerialized(behavior, output);
        return output;
    }

    private bool SaveController(AnimatorController source)
    {
        foreach (AnimatorControllerLayer layer in source.layers)
        {
            foreach (var a in layer.stateMachine.stateMachines)
            {
                //if (AssetDatabase.GetAssetPath(a.stateMachine) != "") // doesn't work for some reasons
                if (AssetDatabase.GetAssetPath(a.stateMachine).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a.stateMachine, AssetDatabase.GetAssetPath(source));
                    a.stateMachine.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            foreach (var a in layer.stateMachine.states)
            {
                //if (AssetDatabase.GetAssetPath(a.stateMachine) != "") // doesn't work for some reasons
                if (AssetDatabase.GetAssetPath(a.state).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a.state, AssetDatabase.GetAssetPath(source));
                    a.state.hideFlags = HideFlags.HideInHierarchy;
                }
                foreach (var b in a.state.behaviours)
                {
                    //if (AssetDatabase.GetAssetPath(a.stateMachine) != "") // doesn't work for some reasons
                    if (AssetDatabase.GetAssetPath(b).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(b, AssetDatabase.GetAssetPath(source));
                        b.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                foreach (var c in a.state.transitions)
                {
                    //if (AssetDatabase.GetAssetPath(a.stateMachine) != "") // doesn't work for some reasons
                    if (AssetDatabase.GetAssetPath(c).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(c, AssetDatabase.GetAssetPath(source));
                        c.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
            }
            foreach (var a in layer.stateMachine.anyStateTransitions)
            {
                //if (AssetDatabase.GetAssetPath(a.stateMachine) != "") // doesn't work for some reasons
                if (AssetDatabase.GetAssetPath(a).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a, AssetDatabase.GetAssetPath(source));
                    a.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            foreach (var a in layer.stateMachine.behaviours)
            {
                //if (AssetDatabase.GetAssetPath(a.stateMachine) != "") // doesn't work for some reasons
                if (AssetDatabase.GetAssetPath(a).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a, AssetDatabase.GetAssetPath(source));
                    a.hideFlags = HideFlags.HideInHierarchy;
                }
            }
        }

        return true;
    }
    private bool FindTemplates()
    {
        string[] results = AssetDatabase.FindAssets("(ASTemplate)", new string[] { "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Templates" });

        foreach (string guid in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            //Debug.Log([Avatar Scaling Setup] Template : path);
            
            //Gesture Animator
            if (path.Contains("Gesture (ASTemplate).controller"))
            {
                templateAnimators[0] = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
            }
            //Sitting Animator
            else if (path.Contains("Sitting (ASTemplate).controller"))
            {
                templateAnimators[1] = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
            }
            //TPose Animator
            else if (path.Contains("TPose (ASTemplate).controller"))
            {
                templateAnimators[2] = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
            }
            //Size Settings Anim
            else if (path.Contains("Size Settings (ASTemplate).anim"))
            {
                templateSizes = (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
            }
            //Scaling Expression Menu
            else if (path.Contains("Submenu (ASTemplate).asset"))
            {
                templateMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(path, typeof(VRCExpressionsMenu));
            }
        }

        //Check for missing files
        if (templateSizes == null || templateMenu == null)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < templateAnimators.Length; i++)
            {
                if (templateAnimators[i] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
