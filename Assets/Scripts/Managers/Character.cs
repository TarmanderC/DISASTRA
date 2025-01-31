using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public CharacterData characterData;
    public int currentHealth;
    public int maxHealth;
    public String characterName;

    public int baseAttack;


    public List<Attack> attacks;
    public int TU;



    private EnemyAI enemyAI;

    public BattleManager bm;
    private UIManager uiManager;
    public Attack currentAttack;

    public GameObject timelineIcon;
    public GameObject healthIcon;

    public Animator anim;

    public bool currentSlideOrNo;

    public bool isPlayer;
    public bool isDead;
    internal object theData;

    private SpriteRenderer spriteRenderer;
    public Material originalMaterial;

    public Vector3 originalPosition;


    private float knockbackDistance = 1.5f; // Distance target moves back (Adjustable)

    void Awake() {
        if (characterData.currentHealth <= 0)
        {
            characterData.currentHealth = characterData.maxHealth;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;

        originalPosition = transform.position;

        maxHealth = characterData.maxHealth;
        currentHealth = characterData.currentHealth;
    }

    void Start()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        anim = gameObject.GetComponent<Animator>();
        uiManager = bm.gameObject.GetComponent<UIManager>();

        if (!isPlayer) {
            currentHealth = characterData.currentHealth;
        }
        isDead = false;
    }

    public void StartTurn(BattleManager battleManager) {
        //Debug.Log("Character: Start Turn");
        spriteRenderer.material = originalMaterial;
        bm = battleManager;
        uiManager = bm.gameObject.GetComponent<UIManager>();
        currentAttack = null;

        if (bm.players.Contains(gameObject) && isPlayer) {
            uiManager.actionButtonPanel.SetActive(true);
            uiManager.DisplayAttackButtons(this, attacks);
            uiManager.attackButton.SetActive(true);
            uiManager.itemButton.SetActive(true);
            uiManager.backButton.SetActive(false);
        } else if (!isPlayer) {
            //Debug.Log("Character: it is the enemy's turn");

            uiManager.actionButtonPanel.SetActive(false);
            bm.StartEnemyTurn(bm.currentCharacter);
        }
    }

    public void SetCurrentAttack(Attack attack) {
        //Debug.Log("Character: Set Current Attack");
        currentAttack = attack;
        bm.uiManager.ShowEnemySelection(true);

        //Debug.Log($"{characterName} changed attack to: {currentAttack.attackName}");
    }

    public void Attack(Character target, int damage) {
        //Debug.Log($"{characterName} is attacking {target.characterName} for {damage} damage!");
        target.TakeDamage((int) (damage * 0.6), !isPlayer);

        if (currentAttack != null && currentAttack.isAOE) {
            if (bm.players.Contains(gameObject)) {
                foreach (GameObject enemy in bm.enemies) {
                    enemy.GetComponent<Character>().TakeDamage((int) (damage * 0.6), !isPlayer);
                }
            } else if (bm.enemies.Contains(gameObject)) {
                foreach (GameObject player in bm.players) {
                    player.GetComponent<Character>().TakeDamage((int) (damage * 0.6), !isPlayer);
                }
            }
        }

        TU += currentAttack.TUCost;
        if (currentAttack.doesItSlide) {
            currentSlideOrNo = true;
        } else {
            currentSlideOrNo = false;
        }
        currentAttack = null;
    }

    public void BattleHeal(int amount) {
        //Debug.Log($"{characterName} is healing for {amount} health!");
        currentHealth += amount;

        if (currentHealth > characterData.maxHealth) {
            currentHealth = characterData.maxHealth;
        }

        characterData.UpdateHealth(currentHealth);

        uiManager.UpdatePlayerHealth();
        uiManager.UpdateEnemyHealth();
    }
    
    public void Heal(int amount) {
        //Debug.Log($"{characterName} is healing for {amount} health!");
        currentHealth += amount;

        Debug.Log(currentHealth);

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }

        Debug.Log(currentHealth);  

        characterData.UpdateHealth(currentHealth);

        Debug.Log(currentHealth);
    }

    public void TakeDamage(int amount, bool isThePlayer, float knockbackStrength = 1f) {
        //currentHealth -= amount;
        currentHealth -= amount;
        
        if (currentHealth <= 0) {
            currentHealth = 0;
            isDead = true;
            Die();
        }

        //Debug.Log("Damage taken: " + amount);
        //Debug.Log($"{characterName}'s new health: {currentHealth}");

        characterData.UpdateHealth(currentHealth);

        uiManager.UpdateEnemyHealth();
        uiManager.UpdatePlayerHealth();

        knockbackDistance = knockbackStrength; // Set knockback strength before playing effect
        spriteRenderer.material = originalMaterial;
        StartCoroutine(ImpactEffect(isThePlayer));
        
    }

    private void Die() {
        //Debug.Log($"{characterName} has been defeated.");
        isDead = true;

        // Remove the enemy from the turn queue
        bm.queue = new List<Character>(bm.queue.Where(character => character != this));

        // Remove the enemy from the BattleManager's enemies list

        uiManager.UpdatePlayerHealth();
        uiManager.UpdateEnemyHealth();

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public IEnumerator goBack() {
        //Debug.Log("Character: Go Back");
        anim.SetBool("isFighting", false);

        foreach (GameObject enemy in bm.enemies) {
            enemy.GetComponent<Character>().spriteRenderer.material = originalMaterial;
        }

        foreach (GameObject player in bm.players) {
            player.GetComponent<Character>().spriteRenderer.material = originalMaterial;
        }

        yield return new WaitForSeconds(0.5f);

        if (currentSlideOrNo) {
            bm.Slide(bm.currentPosition, false); // Slide back after the attack
        } else {
            bm.EndTurn(bm.currentCharacter);
        }

        //Debug.Log("Character: Finish Quick Time Event");
        uiManager.quickTimeEventBar.SetActive(false);
        uiManager.quickTimeEvent.slider.value = 0;
    }

    public void EnemyAttack() {
        //Debug.Log("Enemy attack");

        int baseDamage = currentAttack.damage;
        int finalDamage = baseDamage;
        finalDamage *= characterData.baseAttack;

        Attack(bm.selectedEnemy.GetComponent<Character>(),finalDamage);
        anim.SetBool("isFighting", true);
    }

    public void DoAttack() {
        int baseDamage = currentAttack.damage;
        int finalDamage = baseDamage;

        switch (bm.uiManager.quickTimeEvent.typeOfDamage) {
            case 1: // High damage
                finalDamage = Mathf.CeilToInt(baseDamage * 1.5f); // 50% extra damage
                break;
            case 2: // Regular damage
                finalDamage = baseDamage; // No modification
                break;
            case 3: // Weak damage
                finalDamage = Mathf.FloorToInt(baseDamage * 0.5f); // 50% less damage
                break;
        }

        finalDamage *= characterData.baseAttack;
        Attack(bm.selectedEnemy.GetComponent<Character>(), finalDamage);
    }

    public void clearAttack() {
        //Debug.Log("Character: Clear Attack");
        currentAttack = null;

        //Debug.Log($"Clear! {characterName}'s attack is: {currentAttack}");
    }

    public void ResetCharacter() {
        if (isDead) {
            characterData.currentHealth = 1;
        }

        isDead = false;
        currentAttack = null;

        anim.SetBool("isFighting", false);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    private IEnumerator ImpactEffect(bool isThePlayer)
    {
        // **Flash White Effect**
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Material originalMaterial = spriteRenderer.material;
        Material whiteMaterial = new Material(Shader.Find("TextMeshPro/Distance Field")) { color = Color.white };
        spriteRenderer.material = whiteMaterial;

        float direction = isThePlayer ? -1f : 1f;

        // **Knockback Effect Variables**
        Vector3 originalPosition = transform.position;
        Vector3 knockbackPosition = originalPosition + new Vector3(direction * knockbackDistance, 0, 0);

        float duration = 0.15f; // Total time for knockback
        float elapsed = 0f;

        // **Ease-Out (Fast start, slow stop) for Knockback**
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease-out effect
            transform.position = Vector3.Lerp(originalPosition, knockbackPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = knockbackPosition; // Ensure final position

        yield return new WaitForSeconds(0.02f); // Small delay before returning

        elapsed = 0f;

        // **Ease-In (Slow start, fast end) for Returning**
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f); // Ease-in effect
            transform.position = Vector3.Lerp(knockbackPosition, originalPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // **Restore Material**
        spriteRenderer.material = originalMaterial;
        transform.position = originalPosition; // Ensure back to original position
    }
}
