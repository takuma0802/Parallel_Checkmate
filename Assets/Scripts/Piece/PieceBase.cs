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

    private int row;
    private int column;

    public int Row
    {
        get { return row; }
        set { row = value; }
    }
    public int Column
    {
        get { return column; }
        set { column = value; }
    }
    
    private bool isPutted;
    private bool isDestroyed;
    
    public bool IsPutted
    {
        get { return isPutted; }
        set { isPutted = value; }
    }
    public bool IsDestroyed
    {
        get { return isDestroyed; }
        set { isDestroyed = value; }
    }

    public PieceBase(PlayerType player, int pieceNum)
    {
        this.playerType = player;
        this.pieceNum = pieceNum;
    }

    public virtual void Move() { }
    public virtual void Attack() { }
}
