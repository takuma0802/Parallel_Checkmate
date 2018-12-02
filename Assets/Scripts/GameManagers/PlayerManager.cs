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
    public int currentCost;
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
    private List<PlayerAction> playerActions = new List<PlayerAction>();
    private PlayerAction playerAction;


    // 0:Piece1/ 1:Piece2/ 2:Piece3...
    public GameObject[] piecePrefabs;
    public GameObject[] PlayerGameObject;
    public GameObject StrategyUI;
    public Button[] holdingPieceButtons;
    public Image[] holdingPieceImage;
    public Sprite[] pieceNumberImage;
    public Image costImage;
    public Sprite[] costNumber;
    public Button SubmitButton;

    private TimeManager timeManager;
    private BoardManager boardManager;

    private bool canAction;

    private CompositeDisposable _compositeDisposable = new CompositeDisposable();

    public IEnumerator InitializePlayer(BoardManager boardManager)
    {
        this.boardManager = boardManager;
        timeManager = GetComponent<TimeManager>();
        if (!timeManager) gameObject.AddComponent<TimeManager>();
        canAction = false;

        ChangeActiveUI(StrategyUI, false);

        yield return CreateHoldingPiece(PlayerType.Player1);
        yield return CreateHoldingPiece(PlayerType.Player2);

        foreach (PieceProvider piece in PiecesObject1) ChangeActiveUI(piece.gameObject, false);
        foreach (PieceProvider piece in PiecesObject2) ChangeActiveUI(piece.gameObject, false);

        // 監視
        MessageBroker.Default.Receive<PlayerAction>().Subscribe(x =>
        {
            canAction = false;
            DisposeAllStream();
            ChangeCost(x.Piece.PieceCost);

            if (x.Action == PieceAction.Move)
            {
                if (x.Player == PlayerType.Player1)
                {
                    PutPieceUI(PiecesObject1[x.Piece.PieceNum].gameObject, x.NextColumn, x.NextRow);
                    x.Piece.IsPutted = true;
                }
                else if (x.Player == PlayerType.Player2)
                {
                    PutPieceUI(PiecesObject2[x.Piece.PieceNum].gameObject, x.NextColumn, x.NextRow);
                    x.Piece.IsPutted = true;
                }
            }
            else if (x.Action == PieceAction.Attack)
            {
                if (x.Player == PlayerType.Player1)
                {
                    // PiecesObject1[x.Piece.PieceNum]
                    print("P1Attack！");
                }
                else if (x.Player == PlayerType.Player2)
                {
                    //PutPieceUI(PiecesObject2[x.Piece.PieceNum].gameObject, x.NextColumn, x.NextRow);
                    print("P1Attack！");
                }
            }

            playerActions.Add(x);
            StartObserve();
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
                PiecesObject1[i].SetPieceUIInfo(player, i, PieceType.Piece1);
            }
            for (int i = 5; i < 9; i++)
            {
                Pieces1[i] = new Piece2(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[1]).GetComponent<PieceProvider>();
                PiecesObject1[i].SetPieceUIInfo(player, i, PieceType.Piece2);
            }
            for (int i = 9; i < 12; i++)
            {
                Pieces1[i] = new Piece3(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[2]).GetComponent<PieceProvider>();
                PiecesObject1[i].SetPieceUIInfo(player, i, PieceType.Piece3);
            }
            for (int i = 12; i < 14; i++)
            {
                Pieces1[i] = new Piece4(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[3]).GetComponent<PieceProvider>();
                PiecesObject1[i].SetPieceUIInfo(player, i, PieceType.Piece4);
            }
            Pieces1[14] = new Piece5(player, 14);
            PiecesObject1[14] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[4]).GetComponent<PieceProvider>();
            PiecesObject1[14].SetPieceUIInfo(player, 14, PieceType.Piece5);
        }
        else if (player == PlayerType.Player2)
        {
            Pieces2 = new PieceBase[15];
            for (int i = 0; i < 5; i++)
            {
                Pieces2[i] = new Piece1(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[5]).GetComponent<PieceProvider>();
                PiecesObject2[i].SetPieceUIInfo(player, i, PieceType.Piece1);
            }
            for (int i = 5; i < 9; i++)
            {
                Pieces2[i] = new Piece2(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[6]).GetComponent<PieceProvider>();
                PiecesObject2[i].SetPieceUIInfo(player, i, PieceType.Piece2);
            }
            for (int i = 9; i < 12; i++)
            {
                Pieces2[i] = new Piece3(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[7]).GetComponent<PieceProvider>();
                PiecesObject2[i].SetPieceUIInfo(player, i, PieceType.Piece3);
            }
            for (int i = 12; i < 14; i++)
            {
                Pieces2[i] = new Piece4(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[8]).GetComponent<PieceProvider>();
                PiecesObject2[i].SetPieceUIInfo(player, i, PieceType.Piece4);
            }
            Pieces2[14] = new Piece5(player, 14);
            PiecesObject2[14] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[9]).GetComponent<PieceProvider>();
            PiecesObject2[14].SetPieceUIInfo(player, 14, PieceType.Piece5);
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
        StartObserve();

        // 決定ボタンが押される
        yield return SubmitButton.OnClickAsObservable().First().ToYieldInstruction();

        ChangeActiveUI(StrategyUI, false);
        yield return UndoActions();
    }

    private void StartObserve()
    {
        if (player == PlayerType.Player1)
        {
            SetHoldingPieceUI(Pieces1);
            ObserveAvailablePieces(Pieces1, PiecesObject1);
        }
        else if (player == PlayerType.Player2)
        {
            SetHoldingPieceUI(Pieces2);
            ObserveAvailablePieces(Pieces2, PiecesObject2);
        }
        ChangeActiveUI(StrategyUI, true);
        canAction = true;
    }

    private IEnumerator Reset()
    {
        // 現在の置かれているPieceを更新
        SearchPuttedPieces(Pieces1);
        SearchPuttedPieces(Pieces2);

        yield return null;
        SetPuttedPieces();
        ResetCost();
        if (player == PlayerType.Player1) playerActions.Clear();
    }

    private void ResetCost()
    {
        currentCost = 5;
        costImage.sprite = costNumber[currentCost];
    }

    private void ChangeCost(int cost)
    {
        currentCost -= cost;
        Debug.Log(currentCost);
        costImage.sprite = costNumber[currentCost];
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

        holdingPieceImage[0].sprite = pieceNumberImage[piece1num];
        holdingPieceImage[1].sprite = pieceNumberImage[piece2num];
        holdingPieceImage[2].sprite = pieceNumberImage[piece3num];
        holdingPieceImage[3].sprite = pieceNumberImage[piece4num];
        holdingPieceImage[4].sprite = pieceNumberImage[piece5num];

        if (piece1num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[0], PieceType.Piece1, 1);
        else
            DisableHoldingPiece(holdingPieceButtons[0]);

        if (piece2num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[1], PieceType.Piece2, 2);
        else
            DisableHoldingPiece(holdingPieceButtons[1]);

        if (piece3num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[2], PieceType.Piece3, 3);
        else
            DisableHoldingPiece(holdingPieceButtons[2]);

        if (piece4num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[3], PieceType.Piece4, 4);
        else
            DisableHoldingPiece(holdingPieceButtons[3]);

        if (piece5num >= 1)
            ActivateHoldingPiece(holdingPieceButtons[4], PieceType.Piece5, 5);
        else
            DisableHoldingPiece(holdingPieceButtons[4]);

        // アニメーションいれたいなぁ

    }

    private void ActivateHoldingPiece(Button button, PieceType pieceType, int cost)
    {
        button.interactable = true;
        // アルファ回復
        if (cost > currentCost) return;
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
        playerAction = new PlayerAction();
        if (player == PlayerType.Player1)
        {
            if (pieceNum >= 0) // Board上のPiece選択
            {
                playerAction.Piece = Pieces1[pieceNum];
                playerAction.CurrentRow = Pieces1[pieceNum].Row;
                playerAction.CurrentColumn = Pieces1[pieceNum].Column;
                playerAction.OnBoard = true;
                boardManager.SearchMovePoint(playerAction, true);
            }
            else
            {
                var num = PiecesObject1.Where(x => x.PieceType == pieceType && !Pieces1[x.PieceNum].IsPutted).FirstOrDefault().PieceNum;
                playerAction.Piece = Pieces1[num];
                playerAction.OnBoard = false;
                boardManager.SearchMovePoint(playerAction, false);
            }
        }
        else if (player == PlayerType.Player2)
        {
            if (pieceNum >= 0) // Board上のPiece選択
            {
                playerAction.Piece = Pieces2[pieceNum];
                playerAction.CurrentRow = Pieces2[pieceNum].Row;
                playerAction.CurrentColumn = Pieces2[pieceNum].Column;
                playerAction.OnBoard = true;
                boardManager.SearchMovePoint(playerAction, true);
            }
            else
            {
                var num = PiecesObject2.Where(x => x.PieceType == pieceType && !Pieces2[x.PieceNum].IsPutted).FirstOrDefault().PieceNum;
                playerAction.Piece = Pieces2[num];
                playerAction.OnBoard = false;
                boardManager.SearchMovePoint(playerAction, false);
            }
        }
    }
    private void DisposeAllStream()
    {
        _compositeDisposable.Clear();
    }

    private IEnumerator UndoActions()
    {
        if (playerActions.Count == 0) yield break;
        foreach (PlayerAction x in playerActions)
        {
            if (x.Action == PieceAction.Attack) continue;

            if (x.Player == PlayerType.Player1)
            {
                if (!x.OnBoard)
                {
                    ChangeActiveUI(PiecesObject1[x.Piece.PieceNum].gameObject, false);
                }
                else
                {
                    PutPieceUI(PiecesObject1[x.Piece.PieceNum].gameObject, x.CurrentColumn, x.CurrentRow);
                }
            }
            else if (x.Player == PlayerType.Player2)
            {
                if (!x.OnBoard)
                {
                    ChangeActiveUI(PiecesObject2[x.Piece.PieceNum].gameObject, false);
                }
                else
                {
                    PutPieceUI(PiecesObject2[x.Piece.PieceNum].gameObject, x.CurrentColumn, x.CurrentRow);
                }
            }
        }
        yield return new WaitForSeconds(2.0f);
    }
}
