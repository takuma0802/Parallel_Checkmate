using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece1 : PieceBase
{
    public Piece1(PlayerType player, int pieceNum) : base(player, pieceNum) 
    {
        this.pieceType = PieceType.Piece1;
        this.pieceCost = 1;
    }

    public override void Move()
    {

    }

    public override void Attack()
    {

    }
}
