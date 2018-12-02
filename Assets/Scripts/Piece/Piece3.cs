using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece3 : PieceBase
{
    public Piece3(PlayerType player, int pieceNum) : base(player, pieceNum) 
    {
        this.pieceType = PieceType.Piece3;
        this.pieceCost = 3;
    }

    public override void Move()
    {

    }

    public override void Attack()
    {

    }

}
