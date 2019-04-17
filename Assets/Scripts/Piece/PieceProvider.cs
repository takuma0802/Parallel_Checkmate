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

    private bool willAttack = false;
    private Image attackImage;
    private Tweener attackSequence;

    void Start()
    {
        attackImage = AttackIcon.GetComponent<Image>();
    }

    public void SetPieceUIInfo(PlayerType player, int pieceNum,PieceType type)
    {
        this.player = player;
        this.pieceNum = pieceNum;
        this.PieceType = type;
    }

    public void ChangeAttackIcon(bool enable)
    {
        if(!willAttack)
        {
            AttackIcon.SetActive(enable);
        }
    }

    // direction => -1 or 1 (クソコード〜〜〜)
    public IEnumerator AttackAnimation(int direction)
    {
        var moveSequence = transform.DOPunchPosition(new Vector3(direction * 35f,0,0),0.2f);
        yield return moveSequence.WaitForCompletion();
    }

    public void SelectedAttackAction(bool enabled)
    {
        if(enabled)
        {
            // 攻撃選択された用
            ChangeAttackIcon(true);
            willAttack = true;
            PlayAttackIconAnim();
        }
        else
        {
            // Undoされた用
            willAttack = false;
            ChangeAttackIcon(false);
            KillAttackIconAnim();
        }
    }

    private void PlayAttackIconAnim()
    {
        attackSequence = attackImage.DOFade(0.3f, 0.7f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
    }

    private void KillAttackIconAnim()
    {
        attackSequence.Kill();
        var color = attackImage.color;
        color.a = 1f;
        attackImage.color = color;
    }
}
