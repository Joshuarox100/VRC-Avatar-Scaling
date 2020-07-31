using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

public class ScalingSetupWindow : EditorWindow
{
    //Window Stuff

    int tab;
    bool found;

    float min = 0.5f;
    float max = 3.0f;

    [MenuItem("Window/Avatar Scaling/Setup")]
    static void Init()
    {
        ScalingSetupWindow window = (ScalingSetupWindow)EditorWindow.GetWindow(typeof(ScalingSetupWindow), false, "Avatar Scaling Setup");
        window.Show();
        window.minSize = new Vector2(375f, 440f);
    }

    //Fixes weird bug where menu refuses to reopen.
    private void OnDestroy()
    {
        ScalingSetupWindow window = (ScalingSetupWindow)EditorWindow.GetWindow(typeof(ScalingSetupWindow), false, "Avatar Scaling Setup");
        window = null;
    }
    private void OnGUI()
    {
        if (found)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginArea(new Rect(0, 0, 375f, 440f));
            GUILayout.BeginVertical();
            DrawWindow();
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
    }
    private void OnFocus()
    {
        found = FindTemplates();
        if (avatar == null)
        {
            avatar = FindObjectOfType<VRCAvatarDescriptor>();
        }
        sizes[1] = (avatar != null) ? avatar.gameObject.transform.localScale : sizes[1];
        if (tab == 0)
        {
            sizes[0] = new Vector3(min * sizes[1].x, min * sizes[1].y, min * sizes[1].z);
            sizes[2] = new Vector3(max * sizes[1].x, max * sizes[1].y, max * sizes[1].z);
        }
    }
    private void DrawWindow()
    {
        GUILayout.Label("Avatar Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        SelectAvatarDescriptor();
        if (avatar == null)
        {
            EditorGUILayout.HelpBox("No Avatars found in the current Scene!", MessageType.Warning);
        }
        if (EditorGUI.EndChangeCheck())
        {
            sizes[1] = (avatar != null) ? avatar.gameObject.transform.localScale : sizes[1];
            if (tab == 0)
            {
                sizes[0] = new Vector3(min * sizes[1].x, min * sizes[1].y, min * sizes[1].z);
                sizes[2] = new Vector3(max * sizes[1].x, max * sizes[1].y, max * sizes[1].z);
            }
        }
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Optional Settings", EditorStyles.boldLabel);
        expressionsMenu = (VRCExpressionsMenu)EditorGUILayout.ObjectField(new GUIContent("Expressions Menu", "(Optional) The Expressions Menu you want the scaling controls added to. Leave this empty if you don't want any menus to be affected.\n(Controls will be added as a submenu.)"), expressionsMenu, typeof(VRCExpressionsMenu), true);
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Add Parameters", "Check the Avatar's Expression Parameters for the needed parameters for scaling. If any are not present, it will attempt to add them."), GUILayout.Width(145));
        addExpressionParameters = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(addExpressionParameters), new string[] { "No", "Yes" }));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Use Existing Animators", "If Animators are already present for Gesture, Sitting, or TPose, the parameters and layer for scaling will be added to them. If an Animator is missing or this feature is disabled, a new Animator will be generated using the defaults included in the VRChat SDK and inserted into the descriptor automatically."), GUILayout.Width(145));
        insertLayers = Convert.ToBoolean(GUILayout.Toolbar(Convert.ToInt32(insertLayers), new string[] { "No", "Yes" }));
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Size Settings", EditorStyles.boldLabel);
        tab = GUILayout.Toolbar(tab, new string[] { "Simple", "Advanced" });
        switch (tab)
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
                GUILayout.FlexibleSpace();
                break;
        }
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Destination", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Path:", GUILayout.Width(35));
        EditorGUILayout.LabelField(new GUIContent(outputPath, "The folder where any generated files will be saved to.\n(Generated files will overwrite existing files with the same name: <AVATAR NAME>_<TEMPLATE NAME>)"));
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
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "Success!", "Close");
                    break;
                case 1:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: Failed to create one or more files!", "Close");
                    break;
                case 2:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: Given Expression Menu already contains 8 controls!", "Close");
                    break;
                case 3:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: No Avatar selected!", "Close");
                    break;
                case 4:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: Scale already present in parameter list, but as the wrong type!", "Close");
                    break;
                case 5:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: SizeOp already present in parameter list, but as the wrong type!", "Close");
                    break;
                case 6:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: No unused Expression Parameters found! At least two unused parameters are needed.", "Close");
                    break;
                case 7:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: Avatar does not contain a VRCExpressionParameters object!", "Close");
                    break;
                case 8:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: One or more parameters already present in an Animator, but as the wrong type!", "Close");
                    break;
                case 9:
                    EditorUtility.DisplayDialog("Avatar Scaling Setup", "ERROR: Failed to copy layers to one or more Animators!", "Close");
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

    public string outputPath = "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Output";
    public int ApplyChanges()
    {
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

        //Check if output folder exists and use the default path if it doesn't
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

        //Copy SDK templates (if needed)

        AnimatorController gesture = (avatar.baseAnimationLayers[2].animatorController != null && insertLayers) ? (AnimatorController)avatar.baseAnimationLayers[2].animatorController : null;
        AnimatorController sitting = (avatar.specialAnimationLayers[0].animatorController != null && insertLayers) ? (AnimatorController)avatar.specialAnimationLayers[0].animatorController : null;
        AnimatorController tpose = (avatar.specialAnimationLayers[1].animatorController != null && insertLayers) ? (AnimatorController)avatar.specialAnimationLayers[1].animatorController : null;
        AnimationClip sizeSettings = new AnimationClip();
        
        if (gesture == null)
        {
            if (!AssetDatabase.IsValidFolder(outputPath + Path.DirectorySeparatorChar + "Animators"))
                AssetDatabase.CreateFolder(outputPath, "Animators");
            if (!AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("vrc_AvatarV3HandsLayer", new string[] { "Assets" + Path.DirectorySeparatorChar + "VRCSDK" + Path.DirectorySeparatorChar + "Examples3" + Path.DirectorySeparatorChar + "Animation" + Path.DirectorySeparatorChar + "Controllers" })[0]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Gesture.controller"))
            {
                return 1;
            }
        }

        if (sitting == null)
        {
            if (!AssetDatabase.IsValidFolder(outputPath + Path.DirectorySeparatorChar + "Animators"))
                AssetDatabase.CreateFolder(outputPath, "Animators");
            if (!AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("vrc_AvatarV3SittingLayer", new string[] { "Assets" + Path.DirectorySeparatorChar + "VRCSDK" + Path.DirectorySeparatorChar + "Examples3" + Path.DirectorySeparatorChar + "Animation" + Path.DirectorySeparatorChar + "Controllers" })[0]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Sitting.controller"))
            {
                return 1;
            }
        }

        if (tpose == null)
        {
            if (!AssetDatabase.IsValidFolder(outputPath + Path.DirectorySeparatorChar + "Animators"))
                AssetDatabase.CreateFolder(outputPath, "Animators");
            if (!AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("vrc_AvatarV3UtilityTPose", new string[] { "Assets" + Path.DirectorySeparatorChar + "VRCSDK" + Path.DirectorySeparatorChar + "Examples3" + Path.DirectorySeparatorChar + "Animation" + Path.DirectorySeparatorChar + "Controllers" })[0]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_TPose.controller"))
            {
                return 1;
            }
        }

        if (!AssetDatabase.IsValidFolder(outputPath + Path.DirectorySeparatorChar + "Animations"))
            AssetDatabase.CreateFolder(outputPath, "Animations");
        if (!AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(templateSizes), outputPath + Path.DirectorySeparatorChar + "Animations" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Sizes.anim"))
        {
            return 1;
        }

        AssetDatabase.Refresh();

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

        if (!AddLayersParameters(gesture, templateAnimators[0]) ||
            !AddLayersParameters(sitting, templateAnimators[1]) ||
            !AddLayersParameters(tpose, templateAnimators[2]))
        {
            return 8;
        }

        /*
        // Modify copy of AnimationClip to use new sizes.
        */

        ModifyAnimation(sizeSettings);

        /*
        // Replace reference to template AnimationClip in Gesture with the modified one.
        */

        //Get copied layers
        AnimatorControllerLayer[] layers = gesture.layers;
        AnimatorControllerLayer scalingLayer = new AnimatorControllerLayer();
        
        //Get scaling layer
        foreach (AnimatorControllerLayer layer in layers)
        {
            if (layer.name == "Scaling")
            {
                scalingLayer = layer;
                break;
            }
        }
        if (scalingLayer == null)
        {
            return 9;
        }
        
        //Replace all instances of the template Animation with the new one
        for (int i = 0; i < scalingLayer.stateMachine.states.Length; i++)
        {
            if (AssetDatabase.GetAssetPath(scalingLayer.stateMachine.states[i].state.motion) == AssetDatabase.GetAssetPath(templateSizes))
            {
                scalingLayer.stateMachine.states[i].state.motion = sizeSettings;
            }
        }

        //Update the scaling layer in the list
        for(int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name == "Scaling") 
            {
                layers[i] = scalingLayer;
                break;
            }
        }

        //Update Animator with modified layers
        gesture.layers = layers;

        /*
        // Add new Animators to the Avatar Descriptor if possible. 
        */
        
        //Enable custom layers
        if (avatar.customizeAnimationLayers == false)
        {
            avatar.customizeAnimationLayers = true;
        }

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
                return 6;
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

        if (expressionsMenu != null)
        {
            if (expressionsMenu.controls.Count == 8)
            {
                return 2;
            }
            else
            {
                expressionsMenu.controls.Add(templateMenu.controls[0]);
            }
        }
        else
        {
            if (avatar.expressionsMenu == null)
            {
                avatar.expressionsMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Scale Controls", new string[] { "Assets/Avatar Scaling/Menus" })[0]), typeof(VRCExpressionsMenu));
            }
        }

        return 0;
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
                    curve.keys = keys;
                    break;
                case "m_LocalScale.y":
                    keys[0].value = sizes[0].y;
                    keys[1].value = sizes[1].y;
                    keys[2].value = sizes[2].y;
                    curve.keys = keys;
                    break;
                case "m_LocalScale.z":
                    keys[0].value = sizes[0].z;
                    keys[1].value = sizes[1].z;
                    keys[2].value = sizes[2].z;
                    curve.keys = keys;
                    break;
            }
            AnimationUtility.SetEditorCurve(anim, binding, curve);
        }
    }

    private bool AddLayersParameters(AnimatorController source, AnimatorController target)
    {
        //Check and Add Parameters
        AnimatorControllerParameter[] srcParam = source.parameters;
        AnimatorControllerParameter[] tarParam = target.parameters;
        
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
        AnimatorControllerLayer[] tarLayers = target.layers;

        foreach (AnimatorControllerLayer layer in tarLayers)
        {
            bool exists = false;
            for (int i = 0; i < srcLayers.Length; i++)
            {
                if (layer.name == srcLayers[i].name)
                {
                    srcLayers[i] = layer;
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                AnimatorControllerLayer[] temp = new AnimatorControllerLayer[srcLayers.Length + 1];
                srcLayers.CopyTo(temp, 0);
                temp[temp.Length - 1] = layer;
                srcLayers = temp;
            }
        }

        source.layers = srcLayers;

        return true;
    }

    private bool FindTemplates()
    {
        string[] results = AssetDatabase.FindAssets("(ASTemplate)", new string[] { "Assets/Avatar Scaling/Templates" });

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
