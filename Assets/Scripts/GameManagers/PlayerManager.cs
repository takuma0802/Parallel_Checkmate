using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // コストの最大値
    public int AvailableCost = 5;
    // 現状のコスト
    private int currentCost;
    private PlayerType player;

    // 0~4:Piece1/ 5~8:Piece2/ 9~11:Piece3...
    [HideInInspector] public PieceBase[] Pieces1;
    [HideInInspector] public PieceBase[] Pieces2;

    // 盤上に置くPieceのUIたち
    [HideInInspector] public PieceProvider[] PiecesObject1 = new PieceProvider[15];
    [HideInInspector] public PieceProvider[] PiecesObject2 = new PieceProvider[15];

    // 盤面に置かれているPieceの登録
    private List<PieceBase> puttedPieces = new List<PieceBase>();

    // そのターンのP１とP２のActionを登録
    private List<PlayerAction> pieceActions = new List<PlayerAction>();
    private PlayerAction pieceAction;


    // 0:Piece1/ 1:Piece2/ 2:Piece3...
    public GameObject[] piecePrefabs;
    public GameObject[] PlayerGameObject;
    public GameObject StrategyUI;
    public Button[] holdingPieceButtons;
    public Image[] pieceNumberImage;
    public Button SubmitButton;

    private TimeManager timeManager;
    private BoardManager boardManager;

    private CompositeDisposable _compositeDisposable = new CompositeDisposable();

    public IEnumerator InitializePlayer(BoardManager boardManager)
    {
        this.boardManager = boardManager;
        timeManager = GetComponent<TimeManager>();
        if (!timeManager) gameObject.AddComponent<TimeManager>();

        yield return CreateHoldingPiece(PlayerType.Player1);
        yield return CreateHoldingPiece(PlayerType.Player2);

        foreach (PieceProvider piece in PiecesObject1) ChangeActiveUI(piece.gameObject, false);
        foreach (PieceProvider piece in PiecesObject2) ChangeActiveUI(piece.gameObject, false);

        // 監視
        MessageBroker.Default.Receive<PlayerAction>().Subscribe(x =>
        {
            Debug.Log(x.Piece.Player);
            Debug.Log(x.Action);
            Debug.Log(x.CurrentColumn);
            Debug.Log(x.NextColumn);
            Debug.Log(x.CurrentRow);
            Debug.Log(x.NextRow);
            Debug.Log(x.Piece.PieceType);
            
            pieceActions.Add(x);
        });
    }

    private IEnumerator CreateHoldingPiece(PlayerType player)
    {
        if (player == PlayerType.Player1)
        {
            Pieces1 = new PieceBase[15];
            for (int i = 0; i < 5; i++)
            {
                Pieces1[i] = new Piece1(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[0]).GetComponent<PieceProvider>();
                PiecesObject1[i].SetPieceUIInfo(player,i,PieceType.Piece1);
            }
            for (int i = 5; i < 9; i++)
            {
                Pieces1[i] = new Piece2(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[1]).GetComponent<PieceProvider>();
                PiecesObject1[i].SetPieceUIInfo(player,i,PieceType.Piece2);
            }
            for (int i = 9; i < 12; i++)
            {
                Pieces1[i] = new Piece3(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[2]).GetComponent<PieceProvider>();
                PiecesObject1[i].SetPieceUIInfo(player,i,PieceType.Piece3);
            }
            for (int i = 12; i < 14; i++)
            {
                Pieces1[i] = new Piece4(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[3]).GetComponent<PieceProvider>();
                PiecesObject1[i].SetPieceUIInfo(player,i,PieceType.Piece4);
            }
            Pieces1[14] = new Piece5(player, 14);
            PiecesObject1[14] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[4]).GetComponent<PieceProvider>();
            PiecesObject1[14].SetPieceUIInfo(player,14,PieceType.Piece5);
        }
        else if (player == PlayerType.Player2)
        {
            Pieces2 = new PieceBase[15];
            for (int i = 0; i < 5; i++)
            {
                Pieces2[i] = new Piece1(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[0]).GetComponent<PieceProvider>();
                PiecesObject2[i].SetPieceUIInfo(player,i,PieceType.Piece1);
            }
            for (int i = 5; i < 9; i++)
            {
                Pieces2[i] = new Piece2(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[1]).GetComponent<PieceProvider>();
                PiecesObject2[i].SetPieceUIInfo(player,i,PieceType.Piece2);
            }
            for (int i = 9; i < 12; i++)
            {
                Pieces2[i] = new Piece3(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[2]).GetComponent<PieceProvider>();
                PiecesObject2[i].SetPieceUIInfo(player,i,PieceType.Piece3);
            }
            for (int i = 12; i < 14; i++)
            {
                Pieces2[i] = new Piece4(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[3]).GetComponent<PieceProvider>();
                PiecesObject2[i].SetPieceUIInfo(player,i,PieceType.Piece4);
            }
            Pieces2[14] = new Piece5(player, 14);
            PiecesObject2[14] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[4]).GetComponent<PieceProvider>();
            PiecesObject2[14].SetPieceUIInfo(player,14,PieceType.Piece5);
        }
        yield return null;
    }

    private void ChangeActiveUI(GameObject obj, bool enabled)
    {
        obj.SetActive(enabled);
    }

    public IEnumerator StartStrategy(GameState player)
    {
        if (player == GameState.Player1)
        {
            this.player = PlayerType.Player1;
        }
        else if (player == GameState.Player2)
        {
            this.player = PlayerType.Player2;
        }
        yield return Reset();

        if (player == GameState.Player1)
        {
            SetHoldingPieceUI(Pieces1);
            ObserveAvailablePieces(Pieces1, PiecesObject1);
        }
        else if (player == GameState.Player2)
        {
            SetHoldingPieceUI(Pieces2);
            ObserveAvailablePieces(Pieces2, PiecesObject2);
        }

        // 決定ボタンが押される or 制限時間が過ぎるまで待つ(ここまだ出来てない)
        yield return SubmitButton.OnClickAsObservable().First().ToYieldInstruction();
    }

    private IEnumerator Reset()
    {
        // 現在の置かれているPieceを更新
        SearchPuttedPieces(Pieces1);
        SearchPuttedPieces(Pieces2);

        yield return null;
        SetPuttedPieces();
        currentCost = AvailableCost;
        pieceActions.Clear();
    }

    ////////  盤上のPiece配置周り
    // 指定されたPiecesの中で盤上に置かれているPieceを取得
    private void SearchPuttedPieces(PieceBase[] pieces)
    {
        foreach (PieceBase piece in pieces)
        {
            if (!piece.IsPutted) return;
            if (!piece.IsDestroyed) return;
            if (puttedPieces.Contains(piece)) return;
            puttedPieces.Add(piece);
        }
    }

    private void SetPuttedPieces()
    {
        foreach (PieceBase piece in puttedPieces)
        {
            if (piece.Player == PlayerType.Player1)
            {
                PutPieceUI(PiecesObject1[piece.PieceNum].gameObject, piece.Column, piece.Row);
            }
            else if (piece.Player == PlayerType.Player2)
            {
                PutPieceUI(PiecesObject2[piece.PieceNum].gameObject, piece.Column, piece.Row);
            }
        }
    }

    private void PutPieceUI(GameObject target, int column, int row)
    {
        target.transform.localPosition = boardManager.ReturnCellLocalPosition(column, row);
        ChangeActiveUI(target, true);
    }


    // 手札周り
    private void SetHoldingPieceUI(PieceBase[] pieces)
    {
        int piece1num = pieces.Where(i => i.PieceType == PieceType.Piece1 && !i.IsPutted).Count();
        int piece2num = pieces.Where(i => i.PieceType == PieceType.Piece2 && !i.IsPutted).Count();
        int piece3num = pieces.Where(i => i.PieceType == PieceType.Piece3 && !i.IsPutted).Count();
        int piece4num = pieces.Where(i => i.PieceType == PieceType.Piece4 && !i.IsPutted).Count();
        int piece5num = pieces.Where(i => i.PieceType == PieceType.Piece5 && !i.IsPutted).Count();

        // holdingPieceButtons[0].image = pieceNumberImage[piece1num];
        // holdingPieceButtons[1].image = pieceNumberImage[piece2num];
        // holdingPieceButtons[2].image = pieceNumberImage[piece3num];
        // holdingPieceButtons[3].image = pieceNumberImage[piece4num];
        // holdingPieceButtons[4].image = pieceNumberImage[piece5num];

        if (piece1num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[0], PieceType.Piece1);
        else
            DisableHoldingPiece(holdingPieceButtons[0]);

        if (piece2num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[1], PieceType.Piece2);
        else
            DisableHoldingPiece(holdingPieceButtons[1]);

        if (piece3num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[2], PieceType.Piece3);
        else
            DisableHoldingPiece(holdingPieceButtons[2]);

        if (piece4num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[3], PieceType.Piece4);
        else
            DisableHoldingPiece(holdingPieceButtons[3]);

        if (piece5num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[4], PieceType.Piece5);
        else
            DisableHoldingPiece(holdingPieceButtons[4]);

        // アニメーションいれたいなぁ

        Debug.Log("1:" + piece1num);
        Debug.Log("2:" + piece2num);
        Debug.Log("3:" + piece3num);
        Debug.Log("4:" + piece4num);
        Debug.Log("5:" + piece5num);
    }

    private void ActivateHoldingPiece(Button button, PieceType pieceType)
    {
        button.interactable = true;
        // アルファ回復

        var stream = button.OnClickAsObservable().Subscribe(_ =>
                {
                    print(button.name + "が押されたお！");
                    OnClickAnyPieces(this.player, -1, pieceType);
                });
        _compositeDisposable.Add(stream);
    }

    private void DisableHoldingPiece(Button button)
    {
        button.interactable = false;
        // アルファ減らす
    }

    // Board上にあるPieceの中で、置けるPieceを監視
    private void ObserveAvailablePieces(PieceBase[] pieces, PieceProvider[] piecesObj)
    {
        foreach (var piece in pieces)
        {
            if (piece.IsDestroyed) return;
            if (!piece.IsPutted) return;
            if (piece.PieceCost > currentCost) return;
            var stream = piecesObj[piece.PieceNum].pieceButton
                .OnClickAsObservable().Subscribe(_ =>
                {
                    print(piece.name + "が押されたお！");
                    OnClickAnyPieces(piece.Player, piece.PieceNum, piece.PieceType);
                });
            _compositeDisposable.Add(stream);
        }
    }

    private void OnClickAnyPieces(PlayerType player, int pieceNum, PieceType pieceType)
    {
        pieceAction = new PlayerAction();
        if (player == PlayerType.Player1)
        {
            if (pieceNum >= 0) // Board上のPiece選択
            {
                pieceAction.Piece = Pieces1[pieceNum];
                pieceAction.CurrentRow = Pieces1[pieceNum].Row;
                pieceAction.CurrentColumn = Pieces1[pieceNum].Column;
                boardManager.SearchMovePoint(pieceAction,true);
            }
            else
            {
                var num = PiecesObject1.Where(x => x.PieceType == pieceType).FirstOrDefault().PieceNum;
                pieceAction.Piece = Pieces1[num];
                boardManager.SearchMovePoint(pieceAction,false);
            }
        }
    }

    private void DisposeAllStream()
    {
        _compositeDisposable.Clear();
    }
}
