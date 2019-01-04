using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece4 : PieceBase
{
    public Piece4(PlayerType player, int pieceNum) : base(player, pieceNum) 
    {
        this.pieceType = PieceType.Piece4;
        this.pieceCost = 4;
    }

    public override void Move()
    {

    }

    public override void Attack()
    {

    }

}
