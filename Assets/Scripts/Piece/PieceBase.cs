using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceBase : IPieceAction
{
    // Pieceの種類
    protected PieceType pieceType;
    public PieceType PieceType { get { return pieceType; } }

    protected PlayerType playerType;
    public PlayerType Player { get { return playerType; } }

    // Pieceの固有番号（0~14）
    protected int pieceNum;
    public int PieceNum { get { return pieceNum; } }

    protected int pieceCost;
    public int PieceCost { get { return pieceCost; } }

    protected int column;
    protected int row;
    protected bool isPutted;
    protected bool isDestroyed;

    public int Column { get { return column; } }
    public int Row { get { return row; } }
    public bool IsPutted { get { return isPutted; } }
    public bool IsDestroyed { get { return isDestroyed; } }

    public PieceBase(PlayerType player, int pieceNum)
    {
        this.playerType = player;
        this.pieceNum = pieceNum;
    }

    public void SetPieceInfo(int column, int row, bool isPutted, bool isDestroyed)
    {   
        this.column = column;
        this.row = row;
        this.isPutted = isPutted;
        this.isDestroyed = isDestroyed;
    }

    public virtual void Move() { }
    public virtual void Attack() { }
}
