using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PieceProvider : MonoBehaviour
{

    public Button pieceButton;
    public GameObject AttackIcon;

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
	}

    public void ChangeAttackIcon(bool enable)
    {
        AttackIcon.SetActive(enable);
    }

    // direction => -1 or 1 (クソ〜〜〜)
    public IEnumerator AttackAnimation(int direction)
    {
        var moveSequence = transform.DOPunchPosition(new Vector3(direction * 35f,0,0),0.2f);
        yield return moveSequence.WaitForCompletion();
    }
}
