#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour {
    [SerializeField] private Image playerHPRepresentationSource;
    private Image[] playerHPRepresentation;

    public void Init(int playerMaxHP = 3) {
        playerHPRepresentation = new Image[playerMaxHP];
        playerHPRepresentation[0] = playerHPRepresentationSource;
        int childCount = transform.childCount;
        for(int i = 1; i < playerMaxHP; i++) {
            if(i == childCount - 1) {
                playerHPRepresentation[i] = transform.GetChild(i).GetComponent<Image>();
                continue;
            }
            playerHPRepresentation[i] = Instantiate(playerHPRepresentationSource, transform, false);
        }
    }

    public void PlayerUpdateHPUI(int currentHP) {
        if(currentHP < 0) {
            return;
        }
        playerHPRepresentation[currentHP].enabled = false;
    }
}
