using UnityEngine;
using UnityEngine.UI;
using System;

public class PromotionSelectorUI : MonoBehaviour
{
    public Button queenButton;
    public Button rookButton;
    public Button bishopButton;
    public Button knightButton;

    public Action<PieceType> onPromotionSelected;

    void Start()
    {
        queenButton.onClick.AddListener(() => PromotionSelected(PieceType.Queen));
        rookButton.onClick.AddListener(() => PromotionSelected(PieceType.Rook));
        bishopButton.onClick.AddListener(() => PromotionSelected(PieceType.Bishop));
        knightButton.onClick.AddListener(() => PromotionSelected(PieceType.Knight));
    }

    private void PromotionSelected(PieceType type)
    {
        if (onPromotionSelected != null)
            onPromotionSelected.Invoke(type);
        // Seçim yapıldıktan sonra paneli kapat:
        gameObject.SetActive(false);
    }
}