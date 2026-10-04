using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Farmer/Character Definition")]
public sealed class CharacterDefinition : ScriptableObject
{
    public string id;
    public float moveSpeed = 3.5f;
    public float workSpeed = 1f;
}
