using System;
using UnityEngine;

public enum CharacterTaskType
{
    MoveToConstruction,
    Harvest,
    MoveToMarket,
    Deliver,
    MoveToQueue,
    Buy,
    Leave
}

public sealed class CharacterTask
{
    private readonly Action<Character> onComplete;

    public CharacterTaskType Type { get; }
    public Transform Target { get; }
    public float Duration { get; }
    public CharacterState StateWhileWorking { get; }

    public CharacterTask(
        CharacterTaskType type,
        Transform target,
        float duration,
        CharacterState stateWhileWorking,
        Action<Character> onComplete = null)
    {
        Type = type;
        Target = target;
        Duration = Mathf.Max(0f, duration);
        StateWhileWorking = stateWhileWorking;
        this.onComplete = onComplete;
    }

    public void Complete(Character character) => onComplete?.Invoke(character);
}
