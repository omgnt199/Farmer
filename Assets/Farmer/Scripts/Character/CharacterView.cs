using UnityEngine;

public sealed class CharacterView
{
    private static readonly int MoveHash = Animator.StringToHash("IsMove");
    private static readonly int CarryMoveHash = Animator.StringToHash("IsCarryMove");
    private static readonly int EmptyHash = Animator.StringToHash("IsEmpty");
    private readonly Animator animator;

    public CharacterView(Animator animator)
    {
        this.animator = animator;
    }

    public void SetState(CharacterState state, bool isCarryingProduct)
    {
        if (animator == null)
            return;

        var isMoving = state == CharacterState.Moving || state == CharacterState.Leaving;
        animator.SetBool(MoveHash, isMoving && !isCarryingProduct);
        animator.SetBool(CarryMoveHash, isMoving && isCarryingProduct);
        animator.SetBool(EmptyHash, !isCarryingProduct);
    }
}
