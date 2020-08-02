using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ASExtensions
{
    public static class LayerExtensions
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

        private static AnimatorStateMachine CloneMachine(AnimatorStateMachine machine)
        {
            AnimatorStateMachine output = new AnimatorStateMachine();

            //All Serializable Fields (ex. Primitives)
            EditorUtility.CopySerialized(machine, output);

            //State Machines
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

            //States
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
                AnimatorStateTransition[] outTransitions = new AnimatorStateTransition[machine.states[i].state.transitions.Length];
                for (int j = 0; j < machine.states[i].state.transitions.Length; j++)
                {
                    outTransitions[j] = (AnimatorStateTransition)CloneTransition(machine.states[i].state.transitions[j], outStates);
                }
                outStates[i].state.transitions = outTransitions;
            }
            output.states = outStates;

            //Any Transitions
            AnimatorStateTransition[] outAnyTransitions = new AnimatorStateTransition[machine.anyStateTransitions.Length];
            for (int i = 0; i < machine.anyStateTransitions.Length; i++)
            {
                outAnyTransitions[i] = (AnimatorStateTransition)CloneTransition(machine.anyStateTransitions[i], outStates);
            }
            output.anyStateTransitions = outAnyTransitions;

            //Entry Transitions
            AnimatorTransition[] outEntryTransitions = new AnimatorTransition[machine.entryTransitions.Length];
            for (int i = 0; i < machine.entryTransitions.Length; i++)
            {
                outEntryTransitions[i] = (AnimatorTransition)CloneTransition(machine.entryTransitions[i], outStates);
            }
            output.entryTransitions = outEntryTransitions;

            //Default State
            foreach (ChildAnimatorState childState in outStates)
            {
                //Using `state.state == machine.defaultState` doesn't actually get the correct state in very specific scenarios, so the state name is used instead. State names must be unique per machine so this doesn't cause issues.
                if (childState.state.name == machine.defaultState.name)
                {
                    output.defaultState = childState.state;
                    break;
                }
            }

            return output;
        }

        private static AnimatorState CloneState(AnimatorState state)
        {
            AnimatorState output = new AnimatorState();
            EditorUtility.CopySerialized(state, output);

            StateMachineBehaviour[] outBehaviors = new StateMachineBehaviour[state.behaviours.Length];
            for (int i = 0; i < state.behaviours.Length; i++)
            {
                outBehaviors[i] = CloneStateBehavior(state.behaviours[i]);
            }
            output.behaviours = outBehaviors;

            return output;
        }

        private static AnimatorTransitionBase CloneTransition(AnimatorTransitionBase transition, ChildAnimatorState[] states)
        {
            AnimatorTransitionBase output = Object.Instantiate(transition);
            EditorUtility.CopySerialized(transition, output);
            for (int i = 0; i < states.Length; i++)
            {
                if (output.destinationState != null && output.destinationState.name == states[i].state.name)
                {
                    output.destinationState = states[i].state;
                    break;
                }
            }
            return output;
        }

        private static StateMachineBehaviour CloneStateBehavior(StateMachineBehaviour behavior)
        {
            StateMachineBehaviour output = (StateMachineBehaviour)ScriptableObject.CreateInstance(behavior.GetType());
            EditorUtility.CopySerialized(behavior, output);
            return output;
        }
    }
}
