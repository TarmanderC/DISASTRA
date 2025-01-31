using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    public KnightBattle knightBattle;
    public TextMeshProUGUI coinText;

    void Update()
    {
        coinText.text = knightBattle.Gold.ToString();
    }
}
