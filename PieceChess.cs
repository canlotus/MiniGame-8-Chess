using System.Collections.Generic;
using UnityEngine;

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
public enum PieceColor { White, Black }

public class PieceChess
{
    public PieceType Type;
    public PieceColor Color;
    public bool HasMoved;

    public PieceChess(PieceType type, PieceColor color)
    {
        Type = type;
        Color = color;
        HasMoved = false;
    }

    public PieceChess Clone()
    {
        return new PieceChess(this.Type, this.Color) { HasMoved = this.HasMoved };
    }
}

