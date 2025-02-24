using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceUI : MonoBehaviour
{
    private int xBoard;
    private int yBoard;
    private string player;
    private GameController gameController;

    [Header("White Pieces")]
    public Sprite whitePawn;
    public Sprite whiteKnight;
    public Sprite whiteBishop;
    public Sprite whiteRook;
    public Sprite whiteQueen;
    public Sprite whiteKing;

    [Header("Black Pieces")]
    public Sprite blackPawn;
    public Sprite blackKnight;
    public Sprite blackBishop;
    public Sprite blackRook;
    public Sprite blackQueen;
    public Sprite blackKing;

    public void Initialize(int x, int y, PieceChess pieceData)
    {
        xBoard = x;
        yBoard = y;
        player = (pieceData.Color == PieceColor.White) ? "white" : "black";
        gameObject.name = player + "_" + pieceData.Type.ToString().ToLower();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        UpdatePosition();
        // Sprite ataması:
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (pieceData.Color == PieceColor.White)
        {
            switch (pieceData.Type)
            {
                case PieceType.Pawn: sr.sprite = whitePawn; break;
                case PieceType.Knight: sr.sprite = whiteKnight; break;
                case PieceType.Bishop: sr.sprite = whiteBishop; break;
                case PieceType.Rook: sr.sprite = whiteRook; break;
                case PieceType.Queen: sr.sprite = whiteQueen; break;
                case PieceType.King: sr.sprite = whiteKing; break;
            }
        }
        else
        {
            switch (pieceData.Type)
            {
                case PieceType.Pawn: sr.sprite = blackPawn; break;
                case PieceType.Knight: sr.sprite = blackKnight; break;
                case PieceType.Bishop: sr.sprite = blackBishop; break;
                case PieceType.Rook: sr.sprite = blackRook; break;
                case PieceType.Queen: sr.sprite = blackQueen; break;
                case PieceType.King: sr.sprite = blackKing; break;
            }
        }
    }

    public int GetXBoard() { return xBoard; }
    public int GetYBoard() { return yBoard; }
    public string GetPlayer() { return player; }
    public void SetXBoard(int x) { xBoard = x; }
    public void SetYBoard(int y) { yBoard = y; }

    public void UpdatePosition()
    {
        float x = xBoard * 0.66f - 2.3f;
        float y = yBoard * 0.66f - 2.3f;
        transform.position = new Vector3(x, y, -1.0f);
    }

    private void OnMouseDown()
    {
        if (gameController.GetCurrentPlayer() == player)
        {
            List<Vector2Int> moves = gameController.engine.GetValidMoves(xBoard, yBoard);
            gameController.ShowMoveIndicators(this, moves);
        }
    }

    public void UpdatePromotion(PieceType newType)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (player == "white")
        {
            switch (newType)
            {
                case PieceType.Queen: sr.sprite = whiteQueen; break;
                case PieceType.Rook: sr.sprite = whiteRook; break;
                case PieceType.Bishop: sr.sprite = whiteBishop; break;
                case PieceType.Knight: sr.sprite = whiteKnight; break;
            }
        }
        else
        {
            switch (newType)
            {
                case PieceType.Queen: sr.sprite = blackQueen; break;
                case PieceType.Rook: sr.sprite = blackRook; break;
                case PieceType.Bishop: sr.sprite = blackBishop; break;
                case PieceType.Knight: sr.sprite = blackKnight; break;
            }
        }
    }
}