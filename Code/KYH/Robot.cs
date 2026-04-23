using UnityEngine;

public class Robot : MonoBehaviour
{
    public Animator animator;

    public void Attack()
    {
        animator.SetBool("Attack", true);
    }
}
