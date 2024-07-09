using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class UnlockableCharacter
{
    [SerializeField] private string characterName;
    [SerializeField] private int price;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text priceText = null;
    private bool unlocked = false ;

    public void setPriceText(){
        if (priceText != null){
            if (!unlocked){
                priceText.text = "Price : " + price.ToString();
            }
            else{
                priceText.text = "Unlocked";
            }
        }
    }
    public int getPrice(){
        return price;
    }

    public string getName() {
        return characterName;
    }

    public Animator GetAnimator() { 
        return animator;
    }

    public bool isUnlocked()
    {
        return unlocked;
    }

    public void unlock()
    {
        unlocked = true;
    }
}

