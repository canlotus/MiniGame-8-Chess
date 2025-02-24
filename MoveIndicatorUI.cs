// Assets/Scripts/MoveIndicatorUI.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIndicatorUI : MonoBehaviour
{
    private int targetX;
    private int targetY;
    private ChessPieceUI selectedPiece;
    private GameController gameController;

    public void Initialize(int x, int y, ChessPieceUI piece)
    {
        targetX = x;
        targetY = y;
        selectedPiece = piece;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float x = targetX * 0.66f - 2.3f;
        float y = targetY * 0.66f - 2.3f;
        transform.position = new Vector3(x, y, -2.0f);
    }

    private void OnMouseDown()
    {
        gameController.CommitMove(selectedPiece, targetX, targetY);
    }
}