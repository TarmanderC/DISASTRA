using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public UnityEngine.UI.Image[] tabImages;
    public GameObject[] pages;

    public MenuController menuController;

    void Start() {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNo) {
        for (int i = 0; i < pages.Length; i++) {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }

        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;
    }
}
