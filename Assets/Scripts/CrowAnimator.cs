using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAnimator : MonoBehaviour
{
    private const string Idle = "IdleLookAround";
    private const string Fly = "Fly";
    private const string Walk = "Walk";
    
    [SerializeField] private Animator _animator;
    
    public void SetIdleAnimation()
    {
       _animator.CrossFadeInFixedTime(Idle, .25f);
    }

    public void SetTakeOffAnimation()
    {
        _animator.CrossFadeInFixedTime(Fly, .25f);
    }
    
    public void SetWalkAnimation()
    {
        _animator.CrossFadeInFixedTime(Walk, .25f);
    }
}
