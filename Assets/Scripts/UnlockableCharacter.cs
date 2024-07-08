using UnityEngine;

[System.Serializable]
public class UnlockableCharacter
{
    [SerializeField] private string characterName;
    [SerializeField] private int price;
    [SerializeField] private Animator animator;

    public int getPrice(){
        return price;
    }

    public string getName() {
        return characterName;
    }

    public Animator GetAnimator() { 
        return animator;
    }
}

