using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvent : MonoBehaviour
{
    public Slider slider; // The UI slider
    public BattleManager battleManager; // Reference to the BattleManager
    public float slideSpeed = 1f;

    public int typeOfDamage; // 1 is high, 2 is regular, 3 is trash

    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
    }

    void Update() {
        if (battleManager.isEvent) {
            if (slider.value < 1) {
                slider.value += slideSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.Space)) {
                battleManager.isEvent = false;
                //Debug.Log("Slider stopped at value: " + slider.value);
                setDamage(slider.value);

                battleManager.currentCharacter.anim.SetBool("isFighting", true);
            }
        }
    }

    public void setDamage(float value) {
        if (value < 0.67f && value > 0.61) { // if it is in the green
            typeOfDamage = 1; // BIG EPIC HIGH DAMAGE
        } else if (value >= 0.67f) { // if it is to the right of the green
            if (value < 0.81) { // if it is the right yellow
                typeOfDamage = 2; // Regular commoner damage
            } else {
                typeOfDamage = 3; // Weak baby damage
            }
        } else { // if it is to the left of the green
            if (value > 0.43) { // if it is the left yellow
                typeOfDamage = 2; // Regular commoner damage
            } else {
                typeOfDamage = 3; // Weak baby damage
            }
        }

        Debug.Log("QuickTimeEvent: Set Damage to " + typeOfDamage);
    }
}
