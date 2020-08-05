using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ASExtensions
{
    public static class AnimatorExtensions
    {
        public static AnimatorControllerLayer DeepClone(this AnimatorControllerLayer layer)
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
        public static AnimatorStateMachine CloneMachine(this AnimatorStateMachine machine)
        {
            AnimatorStateMachine output = new AnimatorStateMachine();

            //All Serializable Fields (ex. Primitives)
            EditorUtility.CopySerialized(machine, output);

            //State Machines
            ChildAnimatorStateMachine[] outMachines = new ChildAnimatorStateMachine[machine.stateMachines.Length];
            for (int i = 0; i < outMachines.Length; i++)
            {
                outMachines[i] = new ChildAnimatorStateMachine
                {
                    position = machine.stateMachines[i].position,
                    stateMachine = CloneMachine(machine.stateMachines[i].stateMachine)
                };
            }
            output.stateMachines = outMachines;

            //States
            ChildAnimatorState[] outStates = new ChildAnimatorState[machine.states.Length];
            for (int i = 0; i < outStates.Length; i++)
            {
                outStates[i] = new ChildAnimatorState
                {
                    position = machine.states[i].position,
                    state = CloneState(machine.states[i].state)
                };
            }

            //State Transitions
            for (int i = 0; i < outStates.Length; i++)
            {
                AnimatorStateTransition[] outTransitions = new AnimatorStateTransition[machine.states[i].state.transitions.Length];
                for (int j = 0; j < outTransitions.Length; j++)
                {
                    outTransitions[j] = (AnimatorStateTransition)CloneTransition(machine.states[i].state.transitions[j], outStates);
                }
                outStates[i].state.transitions = outTransitions;
            }
            output.states = outStates;

            //Any Transitions
            AnimatorStateTransition[] outAnyTransitions = new AnimatorStateTransition[machine.anyStateTransitions.Length];
            for (int i = 0; i < outAnyTransitions.Length; i++)
            {
                outAnyTransitions[i] = (AnimatorStateTransition)CloneTransition(machine.anyStateTransitions[i], outStates);
            }
            output.anyStateTransitions = outAnyTransitions;

            //Entry Transitions
            AnimatorTransition[] outEntryTransitions = new AnimatorTransition[machine.entryTransitions.Length];
            for (int i = 0; i < outEntryTransitions.Length; i++)
            {
                outEntryTransitions[i] = (AnimatorTransition)CloneTransition(machine.entryTransitions[i], outStates);
            }
            output.entryTransitions = outEntryTransitions;

            //Behaviors
            StateMachineBehaviour[] outBehaviors = new StateMachineBehaviour[machine.behaviours.Length];
            for (int i = 0; i < outBehaviors.Length; i++)
            {
                outBehaviors[i] = CloneBehavior(machine.behaviours[i]);
            }
            output.behaviours = outBehaviors;

            //Default State
            foreach (ChildAnimatorState childState in outStates)
            {
                if (childState.state.name == machine.defaultState.name)
                {
                    output.defaultState = childState.state;
                    break;
                }
            }

            return output;
        }
        public static AnimatorState CloneState(this AnimatorState state)
        {
            AnimatorState output = new AnimatorState();
            EditorUtility.CopySerialized(state, output);

            StateMachineBehaviour[] outBehaviors = new StateMachineBehaviour[state.behaviours.Length];
            for (int i = 0; i < outBehaviors.Length; i++)
            {
                outBehaviors[i] = CloneBehavior(state.behaviours[i]);
            }
            output.behaviours = outBehaviors;

            return output;
        }
        public static StateMachineBehaviour CloneBehavior(this StateMachineBehaviour behavior)
        {
            StateMachineBehaviour output = (StateMachineBehaviour)ScriptableObject.CreateInstance(behavior.GetType());
            EditorUtility.CopySerialized(behavior, output);
            return output;
        }
        public static AnimatorTransitionBase CloneTransition(this AnimatorTransitionBase transition, ChildAnimatorState[] states)
        {
            AnimatorTransitionBase output = Object.Instantiate(transition);
            EditorUtility.CopySerialized(transition, output);
            for (int i = 0; i < states.Length && output.destinationState != null; i++)
            {
                if (output.destinationState.name == states[i].state.name)
                {
                    output.destinationState = states[i].state;
                    break;
                }
            }
            return output;
        }
        public static void SaveController(this AnimatorController source)
        {
            string sourcePath = AssetDatabase.GetAssetPath(source);
            foreach (AnimatorControllerLayer layer in source.layers)
            {
                //if (AssetDatabase.GetAssetPath(a.stateMachine) != "") // doesn't work for some reasons
                if (AssetDatabase.GetAssetPath(layer.stateMachine).Length == 0)
                {
                    AssetDatabase.AddObjectToAsset(layer.stateMachine, sourcePath);
                    layer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
                }
                foreach (var subStateMachine in layer.stateMachine.stateMachines)
                {
                    if (AssetDatabase.GetAssetPath(subStateMachine.stateMachine).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(subStateMachine.stateMachine, sourcePath);
                        subStateMachine.stateMachine.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                foreach (var childState in layer.stateMachine.states)
                {
                    if (AssetDatabase.GetAssetPath(childState.state).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(childState.state, sourcePath);
                        childState.state.hideFlags = HideFlags.HideInHierarchy;
                    }
                    foreach (var stateBehavior in childState.state.behaviours)
                    {
                        if (AssetDatabase.GetAssetPath(stateBehavior).Length == 0)
                        {
                            AssetDatabase.AddObjectToAsset(stateBehavior, sourcePath);
                            stateBehavior.hideFlags = HideFlags.HideInHierarchy;
                        }
                    }
                    foreach (var stateTransition in childState.state.transitions)
                    {
                        if (AssetDatabase.GetAssetPath(stateTransition).Length == 0)
                        {
                            AssetDatabase.AddObjectToAsset(stateTransition, sourcePath);
                            stateTransition.hideFlags = HideFlags.HideInHierarchy;
                        }
                    }
                }
                foreach (var anyStateTransition in layer.stateMachine.anyStateTransitions)
                {
                    if (AssetDatabase.GetAssetPath(anyStateTransition).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(anyStateTransition, sourcePath);
                        anyStateTransition.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                foreach (var entryTransition in layer.stateMachine.entryTransitions)
                {
                    if (AssetDatabase.GetAssetPath(entryTransition).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(entryTransition, sourcePath);
                        entryTransition.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                foreach (var machineBehavior in layer.stateMachine.behaviours)
                {
                    if (AssetDatabase.GetAssetPath(machineBehavior).Length == 0)
                    {
                        AssetDatabase.AddObjectToAsset(machineBehavior, sourcePath);
                        machineBehavior.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
            }
        }
    }
}
