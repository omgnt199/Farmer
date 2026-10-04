public enum CharacterState
{
    Idle,
    Moving,
    Harvesting,
    Delivering,
    Waiting,
    Buying,
    Leaving
}

public sealed class CharacterEntity
{
    public CharacterDefinition Definition { get; }
    public CharacterState State { get; set; }
    public CharacterTask CurrentTask { get; set; }

    public CharacterEntity(CharacterDefinition definition)
    {
        Definition = definition;
        State = CharacterState.Idle;
    }
}
