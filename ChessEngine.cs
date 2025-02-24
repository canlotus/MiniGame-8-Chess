using System.Collections.Generic;
using UnityEngine;

public class ChessEngine
{
    public PieceChess[,] board = new PieceChess[8, 8];
    public Vector2Int enPassantTarget; 
    public bool enPassantAvailable; 

    public ChessEngine()
    {
        SetupBoard();
    }

    public void SetupBoard()
    {
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                board[i, j] = null;
        enPassantTarget = new Vector2Int(-1, -1);
        enPassantAvailable = false;

        board[0, 0] = new PieceChess(PieceType.Rook, PieceColor.White);
        board[1, 0] = new PieceChess(PieceType.Knight, PieceColor.White);
        board[2, 0] = new PieceChess(PieceType.Bishop, PieceColor.White);
        board[3, 0] = new PieceChess(PieceType.Queen, PieceColor.White);
        board[4, 0] = new PieceChess(PieceType.King, PieceColor.White);
        board[5, 0] = new PieceChess(PieceType.Bishop, PieceColor.White);
        board[6, 0] = new PieceChess(PieceType.Knight, PieceColor.White);
        board[7, 0] = new PieceChess(PieceType.Rook, PieceColor.White);
        for (int i = 0; i < 8; i++)
            board[i, 1] = new PieceChess(PieceType.Pawn, PieceColor.White);

        board[0, 7] = new PieceChess(PieceType.Rook, PieceColor.Black);
        board[1, 7] = new PieceChess(PieceType.Knight, PieceColor.Black);
        board[2, 7] = new PieceChess(PieceType.Bishop, PieceColor.Black);
        board[3, 7] = new PieceChess(PieceType.Queen, PieceColor.Black);
        board[4, 7] = new PieceChess(PieceType.King, PieceColor.Black);
        board[5, 7] = new PieceChess(PieceType.Bishop, PieceColor.Black);
        board[6, 7] = new PieceChess(PieceType.Knight, PieceColor.Black);
        board[7, 7] = new PieceChess(PieceType.Rook, PieceColor.Black);
        for (int i = 0; i < 8; i++)
            board[i, 6] = new PieceChess(PieceType.Pawn, PieceColor.Black);
    }

    public ChessEngine Clone()
    {
        ChessEngine clone = new ChessEngine();
        clone.board = new PieceChess[8, 8];
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                clone.board[i, j] = (board[i, j] != null) ? board[i, j].Clone() : null;
        clone.enPassantTarget = new Vector2Int(enPassantTarget.x, enPassantTarget.y);
        clone.enPassantAvailable = enPassantAvailable;
        return clone;
    }

    public bool PositionOnBoard(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }

    public List<Vector2Int> GetValidMoves(int x, int y)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        PieceChess p = board[x, y];
        if (p == null) return moves;

        if (p.Type == PieceType.Pawn)
        {
            int direction = (p.Color == PieceColor.White) ? 1 : -1;
            if (PositionOnBoard(x, y + direction) && board[x, y + direction] == null)
                moves.Add(new Vector2Int(x, y + direction));
            int startRow = (p.Color == PieceColor.White) ? 1 : 6;
            if (y == startRow &&
                PositionOnBoard(x, y + direction) && board[x, y + direction] == null &&
                PositionOnBoard(x, y + 2 * direction) && board[x, y + 2 * direction] == null)
                moves.Add(new Vector2Int(x, y + 2 * direction));
            if (PositionOnBoard(x - 1, y + direction) && board[x - 1, y + direction] != null &&
                board[x - 1, y + direction].Color != p.Color)
                moves.Add(new Vector2Int(x - 1, y + direction));
            if (PositionOnBoard(x + 1, y + direction) && board[x + 1, y + direction] != null &&
                board[x + 1, y + direction].Color != p.Color)
                moves.Add(new Vector2Int(x + 1, y + direction));
            if (enPassantAvailable)
            {
                if (x - 1 == enPassantTarget.x && y + direction == enPassantTarget.y)
                    moves.Add(new Vector2Int(x - 1, y + direction));
                if (x + 1 == enPassantTarget.x && y + direction == enPassantTarget.y)
                    moves.Add(new Vector2Int(x + 1, y + direction));
            }
        }
        else if (p.Type == PieceType.Knight)
        {
            int[,] offsets = new int[,] { { 1, 2 }, { 2, 1 }, { -1, 2 }, { -2, 1 }, { 1, -2 }, { 2, -1 }, { -1, -2 }, { -2, -1 } };
            for (int i = 0; i < 8; i++)
            {
                int nx = x + offsets[i, 0], ny = y + offsets[i, 1];
                if (PositionOnBoard(nx, ny) &&
                    (board[nx, ny] == null || board[nx, ny].Color != p.Color))
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
        else if (p.Type == PieceType.Bishop)
        {
            int[,] dirs = new int[,] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
            for (int d = 0; d < 4; d++)
            {
                int dx = dirs[d, 0], dy = dirs[d, 1];
                int nx = x + dx, ny = y + dy;
                while (PositionOnBoard(nx, ny) && board[nx, ny] == null)
                {
                    moves.Add(new Vector2Int(nx, ny));
                    nx += dx; ny += dy;
                }
                if (PositionOnBoard(nx, ny) && board[nx, ny] != null && board[nx, ny].Color != p.Color)
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
        else if (p.Type == PieceType.Rook)
        {
            int[,] dirs = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
            for (int d = 0; d < 4; d++)
            {
                int dx = dirs[d, 0], dy = dirs[d, 1];
                int nx = x + dx, ny = y + dy;
                while (PositionOnBoard(nx, ny) && board[nx, ny] == null)
                {
                    moves.Add(new Vector2Int(nx, ny));
                    nx += dx; ny += dy;
                }
                if (PositionOnBoard(nx, ny) && board[nx, ny] != null && board[nx, ny].Color != p.Color)
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
        else if (p.Type == PieceType.Queen)
        {
            List<Vector2Int> rookMoves = GetValidMovesForDummy(x, y, new PieceChess(PieceType.Rook, p.Color));
            List<Vector2Int> bishopMoves = GetValidMovesForDummy(x, y, new PieceChess(PieceType.Bishop, p.Color));
            moves.AddRange(rookMoves);
            moves.AddRange(bishopMoves);
        }
        else if (p.Type == PieceType.King)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx, ny = y + dy;
                    if (PositionOnBoard(nx, ny) &&
                        (board[nx, ny] == null || board[nx, ny].Color != p.Color))
                        moves.Add(new Vector2Int(nx, ny));
                }
            }
            // Castling hamleleri (basit hali):
            if (!p.HasMoved && x == 4)
            {
                // Kingside:
                if (board[5, y] == null && board[6, y] == null)
                {
                    PieceChess rook = board[7, y];
                    if (rook != null && rook.Type == PieceType.Rook && !rook.HasMoved)
                        moves.Add(new Vector2Int(6, y));
                }
                // Queenside:
                if (board[3, y] == null && board[2, y] == null && board[1, y] == null)
                {
                    PieceChess rook = board[0, y];
                    if (rook != null && rook.Type == PieceType.Rook && !rook.HasMoved)
                        moves.Add(new Vector2Int(2, y));
                }
            }
        }
        return moves;
    }

    private List<Vector2Int> GetValidMovesForDummy(int x, int y, PieceChess dummy)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        if (dummy.Type == PieceType.Bishop)
        {
            int[,] dirs = new int[,] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
            for (int d = 0; d < 4; d++)
            {
                int dx = dirs[d, 0], dy = dirs[d, 1];
                int nx = x + dx, ny = y + dy;
                while (PositionOnBoard(nx, ny) && board[nx, ny] == null)
                {
                    moves.Add(new Vector2Int(nx, ny));
                    nx += dx; ny += dy;
                }
                if (PositionOnBoard(nx, ny) && board[nx, ny] != null && board[nx, ny].Color != dummy.Color)
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
        else if (dummy.Type == PieceType.Rook)
        {
            int[,] dirs = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
            for (int d = 0; d < 4; d++)
            {
                int dx = dirs[d, 0], dy = dirs[d, 1];
                int nx = x + dx, ny = y + dy;
                while (PositionOnBoard(nx, ny) && board[nx, ny] == null)
                {
                    moves.Add(new Vector2Int(nx, ny));
                    nx += dx; ny += dy;
                }
                if (PositionOnBoard(nx, ny) && board[nx, ny] != null && board[nx, ny].Color != dummy.Color)
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
        return moves;
    }

    public bool IsKingInCheck(PieceColor color)
    {
        int kingX = -1, kingY = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceChess p = board[i, j];
                if (p != null && p.Type == PieceType.King && p.Color == color)
                {
                    kingX = i; kingY = j;
                    break;
                }
            }
            if (kingX != -1) break;
        }
        if (kingX == -1) return true;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceChess p = board[i, j];
                if (p != null && p.Color != color)
                {
                    List<Vector2Int> moves = GetValidMoves(i, j);
                    foreach (var move in moves)
                        if (move.x == kingX && move.y == kingY)
                            return true;
                }
            }
        }
        return false;
    }

    public bool IsMoveValid(int fromX, int fromY, int toX, int toY, PieceColor moverColor)
    {
        ChessEngine clone = this.Clone();
        if (!clone.PositionOnBoard(fromX, fromY) || !clone.PositionOnBoard(toX, toY))
            return false;
        PieceChess movingPiece = clone.board[fromX, fromY];
        if (movingPiece == null || movingPiece.Color != moverColor)
            return false;
        // Piyon iki kare hamlesi: önce ara hamleyi simüle et.
        if (movingPiece.Type == PieceType.Pawn && System.Math.Abs(toY - fromY) == 2)
        {
            int direction = (moverColor == PieceColor.White) ? 1 : -1;
            if (clone.board[fromX, fromY + direction] != null)
                return false;
            clone.board[fromX, fromY] = null;
            clone.board[fromX, fromY + direction] = movingPiece;
            if (clone.IsKingInCheck(moverColor))
                return false;
            clone = this.Clone(); 
        }
        clone.board[fromX, fromY] = null;
        clone.board[toX, toY] = movingPiece;
        return !clone.IsKingInCheck(moverColor);
    }
}