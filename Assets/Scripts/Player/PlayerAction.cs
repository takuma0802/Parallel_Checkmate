using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerAction
{
    public PieceBase Piece;
    public PlayerType Player { get { return Piece.Player; } }
	public bool OnBoard;
    public int CurrentRow;
    public int CurrentColumn;
    public int NextRow;
    public int NextColumn;
    public PieceAction Action;
}
