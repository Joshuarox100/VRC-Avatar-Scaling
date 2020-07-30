using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
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
    }
    private void OnDestroy()
    {
        ScalingSetupWindow window = (ScalingSetupWindow)EditorWindow.GetWindow(typeof(ScalingSetupWindow), false, "Avatar Scaling Setup");
        window = null;
    }
    private void OnGUI()
    {
        if (found)
        {
            DrawWindow();
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
        avatar = (VRCAvatarDescriptor)EditorGUILayout.ObjectField("Avatar", avatar, typeof(VRCAvatarDescriptor), true);
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
        expressionsMenu = (VRCExpressionsMenu)EditorGUILayout.ObjectField("Expressions Menu", expressionsMenu, typeof(VRCExpressionsMenu), true);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Add Neccessary Parameters");
        GUILayout.FlexibleSpace();
        addExpressionParameters = EditorGUILayout.Toggle(addExpressionParameters, GUILayout.Width(15));
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
                    min = EditorGUILayout.FloatField("Minimum", min);
                    max = EditorGUILayout.FloatField("Maximum", max);
                if (EditorGUI.EndChangeCheck())
                {
                    sizes[0] = new Vector3(min * sizes[1].x, min * sizes[1].y, min * sizes[1].z);
                    sizes[2] = new Vector3(max * sizes[1].x, max * sizes[1].y, max * sizes[1].z);
                }
                break;
            //Advanced
            case 1:
                EditorGUILayout.HelpBox("These values will NOT be multiplied by the scale of your Avatar.", MessageType.Info);
                sizes[0] = EditorGUILayout.Vector3Field("Minimum", sizes[0]);
                sizes[2] = EditorGUILayout.Vector3Field("Maximum", sizes[2]);
                break;
        }
        EditorGUILayout.Space();
        DrawLine();
        GUILayout.Label("Destination", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Path:", GUILayout.Width(35));
        EditorGUILayout.LabelField(outputPath);
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
            }
        }
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
        // Check avatar's ExpressionParameters for needed parameters. Skip step if present, attempt to append to list if absent. In cases where the list is full, inform the user and abort. 
        */



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

        /*
        // Create copy of all templates in destination folder.
        */

        //Check if folder exists
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

        //Create sorting folders
        AssetDatabase.CreateFolder(outputPath, "Animators");
        AssetDatabase.CreateFolder(outputPath, "Animations");

        //Copy templates
        if (!AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(templateAnimators[0]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Gesture.controller") ||
        !AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(templateAnimators[1]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Sitting.controller") ||
        !AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(templateAnimators[2]), outputPath + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_TPose.controller") ||
        !AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(templateSizes), outputPath + Path.DirectorySeparatorChar + "Animations" + Path.DirectorySeparatorChar + avatar.gameObject.name + "_Sizes.anim"))
        {
            return 1;
        }
        AssetDatabase.Refresh();

        AnimatorController gesture = new AnimatorController();
        AnimatorController sitting = new AnimatorController();
        AnimatorController tpose = new AnimatorController();
        AnimationClip sizeSettings = new AnimationClip();

        string[] results = AssetDatabase.FindAssets(avatar.gameObject.name + "_", new string[] { outputPath });
        foreach (string guid in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            //Gesture Animator
            if (path.Contains(avatar.gameObject.name + "_Gesture.controller"))
            {
                gesture = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
            }
            //Sitting Animator
            else if (path.Contains(avatar.gameObject.name + "_Sitting.controller"))
            {
                sitting = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
            }
            //TPose Animator
            else if (path.Contains(avatar.gameObject.name + "_TPose.controller"))
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
        // Modify copy of AnimationClip to use new sizes.
        */

        foreach (var binding in AnimationUtility.GetCurveBindings(sizeSettings))
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(sizeSettings, binding);
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
            AnimationUtility.SetEditorCurve(sizeSettings, binding, curve);
        }

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

        return 0;
    }

    public bool FindTemplates()
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
