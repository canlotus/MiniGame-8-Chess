using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Prefabs & Engine")]
    public GameObject chessPiecePrefab;
    public GameObject moveIndicatorPrefab;
    public ChessEngine engine;
    public ChessPieceUI[,] uiPieces = new ChessPieceUI[8, 8];
    public GameObject promotionPanel; 

    [Header("Game State")]
    private string currentPlayer = "white";
    private bool gameOver = false;

    private ChessPieceUI promotionPieceUI;
    private int promotionTargetX;
    private int promotionTargetY;

    private int moveCounter = 1;
    private int lastEnPassantMove = -1;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip moveSound;   
    public AudioClip checkSound; 
    public AudioClip gameOverSound; 

    void Start()
    {
        engine = new ChessEngine();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                PieceChess p = engine.board[x, y];
                if (p != null)
                {
                    GameObject obj = Instantiate(chessPiecePrefab, Vector3.zero, Quaternion.identity);
                    ChessPieceUI cpui = obj.GetComponent<ChessPieceUI>();
                    cpui.Initialize(x, y, p);
                    uiPieces[x, y] = cpui;
                }
            }
        }
    }

    private void StartPromotion(ChessPieceUI pieceUI, int targetX, int targetY)
    {
        int fromX = pieceUI.GetXBoard();
        int fromY = pieceUI.GetYBoard();
        PieceChess pawnModel = engine.board[fromX, fromY];

        engine.board[fromX, fromY] = null;
        uiPieces[fromX, fromY] = null;

        if (uiPieces[targetX, targetY] != null)
        {
            Destroy(uiPieces[targetX, targetY].gameObject);
            uiPieces[targetX, targetY] = null;
            engine.board[targetX, targetY] = null;
        }

        engine.board[targetX, targetY] = pawnModel;

        pieceUI.SetXBoard(targetX);
        pieceUI.SetYBoard(targetY);
        pieceUI.UpdatePosition();
        uiPieces[targetX, targetY] = pieceUI;

        promotionPieceUI = pieceUI;
        promotionTargetX = targetX;
        promotionTargetY = targetY;
        promotionPanel.SetActive(true);
        PromotionSelectorUI selector = promotionPanel.GetComponent<PromotionSelectorUI>();
        selector.onPromotionSelected = OnPromotionSelected;
    }

    private void OnPromotionSelected(PieceType newType)
    {
        // Modeldeki piyonun tipini güncelle
        PieceChess pawn = engine.board[promotionPieceUI.GetXBoard(), promotionPieceUI.GetYBoard()];
        if (pawn != null && pawn.Type == PieceType.Pawn)
        {
            pawn.Type = newType;
            pawn.HasMoved = true;
        }
        promotionPieceUI.Initialize(promotionTargetX, promotionTargetY, pawn);

        promotionPanel.SetActive(false);

        PieceColor opponentColor = (currentPlayer == "white") ? PieceColor.Black : PieceColor.White;
        if (engine.IsKingInCheck(opponentColor))
        {
            if (audioSource != null && checkSound != null)
                audioSource.PlayOneShot(checkSound);
        }
        else
        {
            if (audioSource != null && moveSound != null)
                audioSource.PlayOneShot(moveSound);
        }

        NextTurn();
        ClearMoveIndicators();
        CheckForGameTermination();
    }

    public void CommitMove(ChessPieceUI pieceUI, int targetX, int targetY)
    {
        int fromX = pieceUI.GetXBoard();
        int fromY = pieceUI.GetYBoard();
        PieceColor moverColor = (pieceUI.GetPlayer() == "white") ? PieceColor.White : PieceColor.Black;

        bool isPromotionMove = false;
        if (pieceUI.gameObject.name.ToLower().Contains("pawn"))
        {
            // White için y == 7, Black için y == 0
            if ((moverColor == PieceColor.White && targetY == 7) ||
                (moverColor == PieceColor.Black && targetY == 0))
            {
                isPromotionMove = true;
            }
        }

        // Eğer piyon iki kare hamle yapıyorsa, en passant hedefi
        if (pieceUI.gameObject.name.ToLower().Contains("pawn") && Mathf.Abs(targetY - fromY) == 2)
        {
            int direction = (moverColor == PieceColor.White) ? 1 : -1;
            engine.enPassantTarget = new Vector2Int(fromX, fromY + direction);
            engine.enPassantAvailable = true;
            lastEnPassantMove = moveCounter;
        }

        if (engine.IsMoveValid(fromX, fromY, targetX, targetY, moverColor))
        {

            if (isPromotionMove)
            {
                StartPromotion(pieceUI, targetX, targetY);
                return; 
            }

            // En passant hamlesi kontrolü:
            bool enPassantCapture = false;
            if (pieceUI.gameObject.name.ToLower().Contains("pawn") &&
                Mathf.Abs(targetX - fromX) == 1 &&
                engine.enPassantAvailable &&
                engine.enPassantTarget.x == targetX &&
                engine.enPassantTarget.y == targetY &&
                engine.board[targetX, targetY] == null)
            {
                enPassantCapture = true;
            }

            // Normal hamle: modelde taşı hareket ettir
            engine.board[targetX, targetY] = engine.board[fromX, fromY];
            engine.board[fromX, fromY] = null;

            // En passant hamlesiyse
            if (enPassantCapture)
            {
                if (moverColor == PieceColor.White)
                {
                    engine.board[targetX, targetY - 1] = null;
                    if (uiPieces[targetX, targetY - 1] != null)
                    {
                        Destroy(uiPieces[targetX, targetY - 1].gameObject);
                        uiPieces[targetX, targetY - 1] = null;
                    }
                }
                else
                {
                    engine.board[targetX, targetY + 1] = null;
                    if (uiPieces[targetX, targetY + 1] != null)
                    {
                        Destroy(uiPieces[targetX, targetY + 1].gameObject);
                        uiPieces[targetX, targetY + 1] = null;
                    }
                }
            }

            pieceUI.SetXBoard(targetX);
            pieceUI.SetYBoard(targetY);
            pieceUI.UpdatePosition();
            if (uiPieces[targetX, targetY] != null && uiPieces[targetX, targetY] != pieceUI)
                Destroy(uiPieces[targetX, targetY].gameObject);
            uiPieces[targetX, targetY] = pieceUI;
            uiPieces[fromX, fromY] = null;

            // Castling (rok) kontrolü:
            if (pieceUI.gameObject.name.ToLower().Contains("king"))
            {
                int deltaX = targetX - fromX;
                if (Mathf.Abs(deltaX) == 2)
                {
                    if (deltaX > 0) // Kingside
                    {
                        int rookX = 7, rookY = fromY, newRookX = 5;
                        engine.board[newRookX, rookY] = engine.board[rookX, rookY];
                        engine.board[rookX, rookY] = null;
                        if (uiPieces[rookX, rookY] != null)
                        {
                            ChessPieceUI rookUI = uiPieces[rookX, rookY];
                            rookUI.SetXBoard(newRookX);
                            rookUI.UpdatePosition();
                            uiPieces[newRookX, rookY] = rookUI;
                            uiPieces[rookX, rookY] = null;
                        }
                    }
                    else if (deltaX < 0) // Queenside
                    {
                        int rookX = 0, rookY = fromY, newRookX = 3;
                        engine.board[newRookX, rookY] = engine.board[rookX, rookY];
                        engine.board[rookX, rookY] = null;
                        if (uiPieces[rookX, rookY] != null)
                        {
                            ChessPieceUI rookUI = uiPieces[rookX, rookY];
                            rookUI.SetXBoard(newRookX);
                            rookUI.UpdatePosition();
                            uiPieces[newRookX, rookY] = rookUI;
                            uiPieces[rookX, rookY] = null;
                        }
                    }
                }
            }

            PieceColor opponentColor = (moverColor == PieceColor.White) ? PieceColor.Black : PieceColor.White;
            if (engine.IsKingInCheck(opponentColor))
            {
                if (audioSource != null && checkSound != null)
                    audioSource.PlayOneShot(checkSound);
            }
            else
            {
                if (audioSource != null && moveSound != null)
                    audioSource.PlayOneShot(moveSound);
            }

            NextTurn();
            ClearMoveIndicators();
            CheckForGameTermination();
        }
        else
        {
            pieceUI.UpdatePosition();
            ClearMoveIndicators();
            if (engine.IsKingInCheck(moverColor))
            {
                StartCoroutine(FlashKingIndicator(moverColor));
            }
        }
    }

    private IEnumerator FlashKingIndicator(PieceColor color)
    {
        string kingName = (color == PieceColor.White) ? "white_king" : "black_king";
        ChessPieceUI kingUI = null;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (uiPieces[x, y] != null && uiPieces[x, y].gameObject.name.ToLower() == kingName)
                {
                    kingUI = uiPieces[x, y];
                    break;
                }
            }
            if (kingUI != null) break;
        }
        if (kingUI == null)
            yield break;
        Vector3 kingPos = kingUI.transform.position;
        Vector3 indicatorPos = kingPos; 
        for (int i = 0; i < 2; i++)
        {
            GameObject indicator = Instantiate(moveIndicatorPrefab, indicatorPos, Quaternion.identity);
            SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            Destroy(indicator);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ShowMoveIndicators(ChessPieceUI pieceUI, List<Vector2Int> moves)
    {
        ClearMoveIndicators();
        foreach (Vector2Int m in moves)
        {
            GameObject obj = Instantiate(moveIndicatorPrefab, Vector3.zero, Quaternion.identity);
            MoveIndicatorUI indicator = obj.GetComponent<MoveIndicatorUI>();
            indicator.Initialize(m.x, m.y, pieceUI);
        }
    }

    public void ClearMoveIndicators()
    {
        foreach (GameObject mi in GameObject.FindGameObjectsWithTag("MoveIndicator"))
        {
            Destroy(mi);
        }
    }

    public void NextTurn()
    {
        currentPlayer = (currentPlayer == "white") ? "black" : "white";
        moveCounter++; 

        if (moveCounter > lastEnPassantMove + 1)
        {
            engine.enPassantTarget = new Vector2Int(-1, -1);
            engine.enPassantAvailable = false;
        }
    }

    public string GetCurrentPlayer() { return currentPlayer; }

    public void CheckForGameTermination()
    {
        PieceColor currentColor = (currentPlayer == "white") ? PieceColor.White : PieceColor.Black;
        bool hasLegalMove = false;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                PieceChess p = engine.board[x, y];
                if (p != null && p.Color == currentColor)
                {
                    List<Vector2Int> moves = engine.GetValidMoves(x, y);
                    foreach (Vector2Int m in moves)
                    {
                        if (engine.IsMoveValid(x, y, m.x, m.y, currentColor))
                        {
                            hasLegalMove = true;
                            break;
                        }
                    }
                }
                if (hasLegalMove)
                    break;
            }
            if (hasLegalMove)
                break;
        }
        if (!hasLegalMove)
        {
            if (engine.IsKingInCheck(currentColor))
            {
                string winner = (currentColor == PieceColor.White) ? "black" : "white";
                GameOver(winner);
            }
            else
            {
                GameOver("DRAW");
            }
        }
    }

    public void GameOver(string result)
    {
        gameOver = true;

        if (audioSource != null && gameOverSound != null)
            audioSource.PlayOneShot(gameOverSound);

        GameChessController chessController = GameObject.FindObjectOfType<GameChessController>();
        if (chessController != null)
        {
            chessController.ShowGameOver(result);
        }
    }

    void Update()
    {
    }
}