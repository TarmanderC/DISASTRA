using System.Collections.Generic;
using UnityEngine;

public class KnightBattle : MonoBehaviour
{
    public List<GameObject> currentParty = new List<GameObject>();

    public int Gold;

    void Start() {
        for (int i = 0; i < currentParty.Count; i++) {
            if (currentParty[i] != null) {
                currentParty[i].GetComponent<Character>().characterData.baseAttack = currentParty[i].GetComponent<Character>().baseAttack;
                currentParty[i].GetComponent<Character>().characterData.maxHealth = currentParty[i].GetComponent<Character>().maxHealth;
                currentParty[i].GetComponent<Character>().characterData.currentHealth = currentParty[i].GetComponent<Character>().currentHealth;
            }
        }
    }
}
