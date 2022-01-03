using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public static class ExtensionAnimator
{
    public static AnimatorState GetAnimatorState(this AnimatorController ac, int layer, string statename)
    {
        AnimatorControllerLayer acl = ac.layers[layer];
        
        foreach (var state in acl.stateMachine.states)
        {
            if(state.state.name == statename) return state.state;
        }
        return null;
    }
    public static AnimationClip GetAnimationClip(this Animator animator, string name)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if(clip.name == name) return clip;
        }
        return null;
    }
}
