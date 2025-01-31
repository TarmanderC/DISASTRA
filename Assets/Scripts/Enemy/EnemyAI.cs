using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private BattleManager bm;
    
    void Start() {
        bm = gameObject.GetComponent<BattleManager>();
    }

    public Attack DetermineAttack(Character enemy)
    {
        if (enemy.attacks == null || enemy.attacks.Count == 0)
        {
            Debug.LogWarning($"{enemy.characterName} has no available attacks.");
            return null;
        }

        // Randomly select an attack
        return enemy.attacks[UnityEngine.Random.Range(0, enemy.attacks.Count)];
    }

    public Character DetermineTarget(GameObject[] players)
    {
        var alivePlayers = players
            .Select(playerObj => playerObj.GetComponent<Character>())
            .Where(player => player.currentHealth > 0)
            .ToList();

        if (alivePlayers.Count == 0)
        {
            return null; // No valid targets
        }

        // Randomly select a target
        return alivePlayers[UnityEngine.Random.Range(0, alivePlayers.Count)];
    }
}
