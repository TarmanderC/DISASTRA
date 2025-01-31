using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int maxHealth;
    public int currentHealth;

    public static int level = 1;
    public int baseAttack;
    public static int maxExp = 100;
    public static int currentExp = 0;
    public Sprite characterImage;


    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }
    
    public void UpdateMaxHealth(int newMaxHealth) {
        maxHealth = newMaxHealth;
    }
}
