using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

public class ASManager
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

    public string outputPath = "Assets" + Path.DirectorySeparatorChar + "Avatar Scaling" + Path.DirectorySeparatorChar + "Output";

    public ASManager() {   }

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
                    avatar.expressionsMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Scale Controls", new string[] { "Assets/Avatar Scaling/Menus" })[0]), typeof(VRCExpressionsMenu));
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
        AnimatorControllerLayer output = new AnimatorControllerLayer
        {
            name = layer.name,
            defaultWeight = layer.defaultWeight,
            avatarMask = layer.avatarMask,
            blendingMode = layer.blendingMode,
            iKPass = layer.iKPass,
            syncedLayerIndex = layer.syncedLayerIndex,
            syncedLayerAffectsTiming = layer.syncedLayerAffectsTiming,
            stateMachine = CloneMachine(layer.stateMachine)
        };
        return output;
    }

    private AnimatorStateMachine CloneMachine(AnimatorStateMachine machine)
    {
        AnimatorStateMachine output = new AnimatorStateMachine();
        EditorUtility.CopySerialized(machine, output);

        ChildAnimatorStateMachine[] outMachines = new ChildAnimatorStateMachine[machine.stateMachines.Length];
        for (int i = 0; i < machine.stateMachines.Length; i++)
        {
            outMachines[i] = new ChildAnimatorStateMachine
            {
                position = machine.stateMachines[i].position,
                stateMachine = CloneMachine(machine.stateMachines[i].stateMachine)
            };
        }
        output.stateMachines = outMachines;

        ChildAnimatorState[] outStates = new ChildAnimatorState[machine.states.Length];
        for (int i = 0; i < machine.states.Length; i++)
        {
            outStates[i] = new ChildAnimatorState
            {
                position = machine.states[i].position,
                state = CloneState(machine.states[i].state)
            };
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

        AnimatorTransition[] outEntryTransitions = new AnimatorTransition[machine.entryTransitions.Length];
        for (int i = 0; i < machine.entryTransitions.Length; i++)
        {
            outEntryTransitions[i] = CloneEntryTransition(machine.entryTransitions[i]);
        }
        AnimatorTransition[] fixedEntryTransitions = outEntryTransitions;
        foreach (AnimatorTransition transition in fixedEntryTransitions)
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
        outEntryTransitions = fixedEntryTransitions;
        output.entryTransitions = outEntryTransitions;

        foreach (ChildAnimatorState state in outStates)
        {
            //Using `state.state == machine.defaultState` doesn't actually get the correct state in very specific scenarios, so the state name is used instead.
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

    private AnimatorTransition CloneEntryTransition(AnimatorTransition transition)
    {
        AnimatorTransition output = new AnimatorTransition();
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
                if (AssetDatabase.GetAssetPath(a.stateMachine).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a.stateMachine, AssetDatabase.GetAssetPath(source));
                    a.stateMachine.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            foreach (var a in layer.stateMachine.states)
            {
                if (AssetDatabase.GetAssetPath(a.state).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a.state, AssetDatabase.GetAssetPath(source));
                    a.state.hideFlags = HideFlags.HideInHierarchy;
                }
                foreach (var b in a.state.behaviours)
                {
                    if (AssetDatabase.GetAssetPath(b).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(b, AssetDatabase.GetAssetPath(source));
                        b.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                foreach (var c in a.state.transitions)
                {
                    if (AssetDatabase.GetAssetPath(c).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(c, AssetDatabase.GetAssetPath(source));
                        c.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
            }
            foreach (var a in layer.stateMachine.anyStateTransitions)
            {
                if (AssetDatabase.GetAssetPath(a).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a, AssetDatabase.GetAssetPath(source));
                    a.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            foreach (var a in layer.stateMachine.entryTransitions)
            {
                if (AssetDatabase.GetAssetPath(a).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a, AssetDatabase.GetAssetPath(source));
                    a.hideFlags = HideFlags.HideInHierarchy;
                }
            }
            foreach (var a in layer.stateMachine.behaviours)
            {
                if (AssetDatabase.GetAssetPath(a).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(a, AssetDatabase.GetAssetPath(source));
                    a.hideFlags = HideFlags.HideInHierarchy;
                }
            }
        }

        return true;
    }

    public bool FindTemplates()
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
