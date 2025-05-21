using System;
using UnityEngine;

namespace Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int VerticalDirection = Animator.StringToHash("VerticalDirection");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int IsDashing = Animator.StringToHash("IsDashing");
        private static readonly int Die = Animator.StringToHash("Die");

        [SerializeField] private Animator animator;
        
        public void SetRunning(bool isRunning) => animator.SetBool(IsRunning, isRunning);
        
        public void SetVerticalDirection(float value) => animator.SetFloat(VerticalDirection, Mathf.Sign(value));
        
        public void TriggerHorizontalDirectionTransition() => animator.Play("ChangeDirection");
        
        public void TriggerAttack() => animator.SetTrigger(Attack);
        public void TriggerDeath() => animator.SetTrigger(Die);
        
        public void StartDash() => animator.SetBool(IsDashing, true);
        
        public void EndDash() => animator.SetBool(IsDashing, false);
    }
}