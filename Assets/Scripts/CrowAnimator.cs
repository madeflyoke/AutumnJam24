using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAnimator : MonoBehaviour
{
    private enum AnimationName
    {
        None= 0,
        Idle =1,
        Fly=2,
        Walk=3,
        Glide =4
    }
    
    private const string Idle = "IdleLookAround";
    private const string Fly = "Fly";
    private const string Walk = "Walk";
    private const string Glide = "Glide";

    [SerializeField] private Animator _animator;
    private AnimationName _currentAnimation;

    public void SetIdleAnimation()
    {
        if (CheckupAnimationState(AnimationName.Idle))
        {
            _animator.CrossFadeInFixedTime(Idle, .25f);
        }
    }

    public void SetFlyAnimation()
    {
        if (CheckupAnimationState(AnimationName.Fly))
        {
            _animator.CrossFadeInFixedTime(Fly, .25f);
        }
    }
    
    public void SetWalkAnimation()
    {
        if (CheckupAnimationState(AnimationName.Walk))
        {
            _animator.CrossFadeInFixedTime(Walk, .25f);
        }
    }

    public void SetGlideAnimation()
    {
        if (CheckupAnimationState(AnimationName.Glide))
        {
            _animator.CrossFadeInFixedTime(Glide, .25f);
        }
    }

    private bool CheckupAnimationState(AnimationName animationName)
    {
        if (_currentAnimation==animationName)
        {
            return false;
        }
        _currentAnimation = animationName;
        return true;
    }
}
