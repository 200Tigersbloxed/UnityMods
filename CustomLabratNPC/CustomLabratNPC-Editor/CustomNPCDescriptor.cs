using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace CustomLabratNPC
{
    [Serializable]
    public class NPCEvent : UnityEvent<NPCEvents>{}
    
    enum Hint{}

    public class CustomNPCDescriptor : MonoBehaviour
    {
        [SerializeField] public NPCEnum NPCType = NPCEnum.Unknown;
        [HideInInspector] public bool isInstanced;
        [HideInInspector] public string Creator; 
        public string NPCDisplayName;
        [HideInInspector] public string UnityVersion;

        [Help("As of right now, this does not do anything. \n" +
              "It will be used in a later update adding an in-game UI.")]
        [SerializeField] public Texture2D NPCIcon;

        [Help("Scripting Section \n" +
              "Section for setting up custom scripting for NPCs.")]
        [SerializeField] private Hint _;
        
        [Help("NPCEvents require a separate dll with the code to be loaded at runtime! \n" +
              "Please use Animators for easier creation of events", MessageType.Warning)]
        [SerializeField] private Hint __;
        [SerializeField] public NPCEvent OnSCPEvent = new NPCEvent();

        [Help("Animating Section \n" + 
              "Section for animating NPCs with Events, for those who don't wanna code.")]
        [SerializeField] private Hint ___;

        [SerializeField] public List<RuntimeAnimatorController> AnimatorEvents = new List<RuntimeAnimatorController>();
        [HideInInspector] public List<AnimatorPlayables> PlayableAnimators = new List<AnimatorPlayables>();

        public void SetParameter<T>(string Name, T Value)
        {
            foreach (AnimatorPlayables animatorPlayable in PlayableAnimators)
            {
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Boolean:
                        animatorPlayable.AnimatorControllerPlayable.SetBool(Name, Convert.ToBoolean(Value));
                        break;
                    case TypeCode.Int32:
                        animatorPlayable.AnimatorControllerPlayable.SetInteger(Name, Convert.ToInt32(Value));
                        break;
                    case TypeCode.Single:
                        animatorPlayable.AnimatorControllerPlayable.SetFloat(Name, Convert.ToSingle(Value));
                        break;
                    default:
                        Debug.Log(
                            $"Failed to SetParameter {Name} with {Value} with Type {typeof(T).DeclaringType?.Name}");
                        break;
                }   
            }
        }

        public void FireTrigger(string Name)
        {
            foreach (AnimatorPlayables animatorPlayables in PlayableAnimators)
            {
                animatorPlayables.AnimatorControllerPlayable.SetTrigger(Name);
            }
        }

        public T GetParameter<T>(string Name, AnimatorPlayables target = null)
        {
            // If one isn't defined, just find the first parameter
            if (target == null)
            {
                foreach (AnimatorPlayables animatorPlayable in PlayableAnimators)
                {
                    switch (Type.GetTypeCode(typeof(T)))
                    {
                        case TypeCode.Boolean:
                            return (T) Convert.ChangeType(animatorPlayable.AnimatorControllerPlayable.GetBool(Name),
                                typeof(T));
                        case TypeCode.Int32:
                            return (T) Convert.ChangeType(animatorPlayable.AnimatorControllerPlayable.GetInteger(Name),
                                typeof(T));
                        case TypeCode.Single:
                            return (T) Convert.ChangeType(animatorPlayable.AnimatorControllerPlayable.GetFloat(Name),
                                typeof(T));
                        default:
                            Debug.Log(
                                $"Failed to GetParameter {Name} with Type {typeof(T).DeclaringType?.Name}");
                            break;
                    }
                }
            }
            else
            {
                AnimatorPlayables animatorPlayable = target;
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Boolean:
                        return (T) Convert.ChangeType(animatorPlayable.AnimatorControllerPlayable.GetBool(Name),
                            typeof(T));
                    case TypeCode.Int32:
                        return (T) Convert.ChangeType(animatorPlayable.AnimatorControllerPlayable.GetInteger(Name),
                            typeof(T));
                    case TypeCode.Single:
                        return (T) Convert.ChangeType(animatorPlayable.AnimatorControllerPlayable.GetFloat(Name),
                            typeof(T));
                    default:
                        Debug.Log(
                            $"Failed to GetParameter {Name} with Type {typeof(T).DeclaringType?.Name}");
                        break;
                }
            }
            // lmao why????
            return (T) Convert.ChangeType(null, typeof(T));
        }

        private void OnEnable()
        {
            // Setup Model Name (only matters in Editor)
            if (string.IsNullOrEmpty(NPCDisplayName))
                NPCDisplayName = gameObject.name;
            // Initiate the PlayableGraphs
            Animator animator = null;
            try
            {
                animator = gameObject.GetComponent<Animator>();
            }
            catch (Exception)
            {
                Debug.LogWarning("Failed to find Animator on model");
            }
            if (animator != null)
            {
                foreach (RuntimeAnimatorController animatorEvent in AnimatorEvents)
                {
                    PlayableGraph playableGraph;
                    AnimatorControllerPlayable animatorControllerPlayable =
                        AnimationPlayableUtilities.PlayAnimatorController(animator, animatorEvent, out playableGraph);
                    PlayableAnimators.Add(new AnimatorPlayables
                    {
                        PlayableGraph = playableGraph,
                        AnimatorControllerPlayable = animatorControllerPlayable,
                        TargetAnimatorController = animatorEvent
                    });
                    playableGraph.Play();
                }
            }
        }

        private void OnDestroy()
        {
            // Destroy the PlayableGraphs
            foreach (AnimatorPlayables animatorPlayables in PlayableAnimators)
            {
                animatorPlayables.PlayableGraph.Destroy();
                animatorPlayables.AnimatorControllerPlayable.Destroy();
            }
            PlayableAnimators.Clear();
        }
    }

    public enum NPCEnum
    {
        Unknown,
        SCP173
    }

    public enum NPCEvents
    {
        Unknown,
        Instantiated,
        Destroyed,
        Killed
    }
}