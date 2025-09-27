using UnityEngine;

public interface IAnimator
{
    public Animator Animator { get; }

    public void TrySetFloat(int hashID, float value) => Animator.SetFloat(hashID, value);
    public void TrySetBool(int hashID, bool value) => Animator.SetBool(hashID, value);
    public void TrySetInteger(int hashID, int value) => Animator.SetInteger(hashID, value);
    public void TrySetTrigger(int hashID) => Animator.SetTrigger(hashID);
}