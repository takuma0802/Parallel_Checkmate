using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceProvider : MonoBehaviour
{

    public Button pieceButton;

    private PlayerType player;
    private int pieceNum;
    public PieceType PieceType;


    public PlayerType Player { get { return player; } }
    public int PieceNum { get { return pieceNum; } }

    public void SetPieceUIInfo(PlayerType player, int pieceNum,PieceType type)
    {
        this.player = player;
        this.pieceNum = pieceNum;
        this.PieceType = type;
    }

	public void ChangeButtonInteractive(bool enable)
	{
		pieceButton.interactable = enable;
		// ButtonのImage変更??
	}
}
