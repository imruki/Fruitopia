using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<UnlockableCharacter> unlockableCharacters = new List<UnlockableCharacter>();

    public List<UnlockableCharacter> GetCharacters() { return unlockableCharacters; }

    private void Start()
    {
        foreach (UnlockableCharacter character in GetCharacters())
        {
            character.setPriceText();
        }
    }
}
