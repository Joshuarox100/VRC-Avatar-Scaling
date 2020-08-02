﻿using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using ASExtensions;

public class ASManager : UnityEngine.Object
{
    public AnimatorController[] templateAnimators = new AnimatorController[3];
    public AnimationClip templateSizes;
    public VRCExpressionsMenu templateMenu;

    public VRCAvatarDescriptor avatar;
    public VRCExpressionsMenu expressionsMenu;
    public bool addExpressionParameters = true;
    public bool insertLayers = true;
    public Vector3[] sizes = new Vector3[] { new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(3.0f, 3.0f, 3.0f) };
    public int curveType;

    private string relativePath;
    public string outputPath;

    public ASManager() {}

    public int ApplyChanges()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Avatar Scaling", "Starting", 0f);
            /*
            // Check if necessary objects are found.
            */

            if (avatar == null)
            {
                return 3;
            }
            else if (addExpressionParameters && avatar.expressionParameters == null)
            {
                return 7;
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
                    avatar.expressionsMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Scale Controls", new string[] { relativePath + Path.DirectorySeparatorChar + "Menus" })[0]), typeof(VRCExpressionsMenu));
                }
            }
            EditorUtility.DisplayProgressBar("Avatar Scaling", "Finalizing", 1.0f);

            return 0;
        }
        catch (Exception err)
        {
            Debug.LogException(err);
            return 99;
        }
    }

    private void VerifyDestination()
    {
        if (!AssetDatabase.IsValidFolder(outputPath))
        {
            if (!AssetDatabase.IsValidFolder(relativePath + Path.DirectorySeparatorChar + "Output"))
            {
                string guid = AssetDatabase.CreateFolder(relativePath, "Output");
                outputPath = AssetDatabase.GUIDToAssetPath(guid);
            }
            else
            {
                outputPath = relativePath + Path.DirectorySeparatorChar + "Output";
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
                    for (int i = 0; i < keys.Length; i++)
                        keys[i].value = sizes[i].x;
                    break;
                case "m_LocalScale.y":
                    for (int i = 0; i < keys.Length; i++)
                        keys[i].value = sizes[i].y;
                    break;
                case "m_LocalScale.z":
                    for (int i = 0; i < keys.Length; i++)
                        keys[i].value = sizes[i].z;
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
        if (!AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(target), relativePath + Path.DirectorySeparatorChar + "Templates" + Path.DirectorySeparatorChar + "Animators" + Path.DirectorySeparatorChar + "Temporary.controller"))
        {
            return false;
        }
        AssetDatabase.Refresh();

        AnimatorController cloned = null;

        string[] results = AssetDatabase.FindAssets("Temporary", new string[] { relativePath + Path.DirectorySeparatorChar + "Templates" + Path.DirectorySeparatorChar + "Animators" });

        foreach (string guid in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
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
                        srcParam[i] = param;
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
            AnimatorControllerLayer copiedLayer = layer.DeepClone();
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

        //Delete Clone
        if (!AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cloned)))
        {
            return false;
        }
        AssetDatabase.Refresh();

        return true;
    }

    //make better variable names here.
    private bool SaveController(AnimatorController source)
    {
        foreach (AnimatorControllerLayer layer in source.layers)
        {
            foreach (var subStateMachine in layer.stateMachine.stateMachines)
            {
                if (AssetDatabase.GetAssetPath(subStateMachine.stateMachine).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(subStateMachine.stateMachine, AssetDatabase.GetAssetPath(source));
                    subStateMachine.stateMachine.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            foreach (var childState in layer.stateMachine.states)
            {
                if (AssetDatabase.GetAssetPath(childState.state).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(childState.state, AssetDatabase.GetAssetPath(source));
                    childState.state.hideFlags = HideFlags.HideInHierarchy;
                }
                foreach (var stateBehavior in childState.state.behaviours)
                {
                    if (AssetDatabase.GetAssetPath(stateBehavior).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(stateBehavior, AssetDatabase.GetAssetPath(source));
                        stateBehavior.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                foreach (var stateTransition in childState.state.transitions)
                {
                    if (AssetDatabase.GetAssetPath(stateTransition).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(stateTransition, AssetDatabase.GetAssetPath(source));
                        stateTransition.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
            }
            foreach (var anyStateTransition in layer.stateMachine.anyStateTransitions)
            {
                if (AssetDatabase.GetAssetPath(anyStateTransition).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(anyStateTransition, AssetDatabase.GetAssetPath(source));
                    anyStateTransition.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            foreach (var entryTransition in layer.stateMachine.entryTransitions)
            {
                if (AssetDatabase.GetAssetPath(entryTransition).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(entryTransition, AssetDatabase.GetAssetPath(source));
                    entryTransition.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            foreach (var machineBehavior in layer.stateMachine.behaviours)
            {
                if (AssetDatabase.GetAssetPath(machineBehavior).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(machineBehavior, AssetDatabase.GetAssetPath(source));
                    machineBehavior.hideFlags = HideFlags.HideInHierarchy;
                }
            }
        }

        return true;
    }

    public bool FindTemplates()
    {
        UpdatePaths();

        string[] results = AssetDatabase.FindAssets("(ASTemplate)", new string[] { relativePath + Path.DirectorySeparatorChar + "Templates" });

        foreach (string guid in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

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

    private void UpdatePaths()
    {
        string old = relativePath;
        relativePath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("(ASTemplate)")[0]).Substring(0, AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("(ASTemplate)")[0]).LastIndexOf("Templates") - 1);
        if (relativePath == old)
            return;
        else if (outputPath == null || !AssetDatabase.IsValidFolder(outputPath))
        {
            outputPath = relativePath + Path.DirectorySeparatorChar + "Output";
        }
    }
}
