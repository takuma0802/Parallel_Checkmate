using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Piece1,
    Piece2,
    Piece3,
    Piece4,
    Piece5,
    King
}

public enum PieceAction
{
    Move,
    Attack
}

public interface IPieceAction 
{
	void Move();
	void Attack();
};
