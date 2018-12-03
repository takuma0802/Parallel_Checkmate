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
    [HideInInspector] public PieceProvider[] Kings;
    public bool player1win = false;
    public bool player2win = false;

    // 盤上に置くPieceのUIたち
    [HideInInspector] public PieceProvider[] PiecesObject1 = new PieceProvider[15];
    [HideInInspector] public PieceProvider[] PiecesObject2 = new PieceProvider[15];

    // 盤面に置かれているPieceの登録
    private List<PieceBase> puttedPieces = new List<PieceBase>();

    // そのターンのP１とP２のActionを登録
    private List<PlayerAction> playerActions = new List<PlayerAction>();
    private PlayerAction playerAction;
    private List<PieceBase> destroyObjects = new List<PieceBase>();

    public GameObject effect;


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
    public Button UndoButton;

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
        PutKings();

        foreach (PieceProvider piece in PiecesObject1) ChangeActiveUI(piece.gameObject, false);
        foreach (PieceProvider piece in PiecesObject2) ChangeActiveUI(piece.gameObject, false);

        ObserveStreams();
    }

    private void ObserveStreams()
    {
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
                    PutPieceInfo(x.Piece, x.NextColumn, x.NextRow, true);
                }
                else if (x.Player == PlayerType.Player2)
                {
                    PutPieceUI(PiecesObject2[x.Piece.PieceNum].gameObject, x.NextColumn, x.NextRow);
                    PutPieceInfo(x.Piece, x.NextColumn, x.NextRow, true);
                }
            }
            else if (x.Action == PieceAction.Attack)
            {
                if (x.Player == PlayerType.Player1)
                {
                    PiecesObject1[x.Piece.PieceNum].ChangeAttackIcon(true);
                    print("P1Attack！");
                }
                else if (x.Player == PlayerType.Player2)
                {
                    PiecesObject2[x.Piece.PieceNum].ChangeAttackIcon(true);
                    print("P2Attack！");
                }
            }

            playerActions.Add(x);
            StartObserve();
        });

        UndoButton.OnClickAsObservable().Subscribe(_ =>
        {
            OnClickUndoButton();
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

    private void PutKings()
    {
        Kings = new PieceProvider[2];
        Kings[0] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[10]).GetComponent<PieceProvider>();
        Kings[1] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[11]).GetComponent<PieceProvider>();

        PutPieceUI(Kings[0].gameObject, 0, 1);
        Kings[0].SetPieceUIInfo(PlayerType.Player1, -1, PieceType.King);

        PutPieceUI(Kings[1].gameObject, 7, 1);
        Kings[1].SetPieceUIInfo(PlayerType.Player2, -1, PieceType.King);
        boardManager.PutKings();
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

        DisposeAllStream();
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
        foreach(var pieceObj in PiecesObject1)
        {
            pieceObj.ChangeAttackIcon(false);
        }
        foreach(var pieceObj in PiecesObject2)
        {
            pieceObj.ChangeAttackIcon(false);
        }

        yield return null;
        SetPuttedPieces();
        ResetCost();
    }

    private void ResetCost()
    {
        currentCost = 5;
        costImage.sprite = costNumber[currentCost];
    }

    private void ChangeCost(int cost)
    {
        currentCost -= cost;
        costImage.sprite = costNumber[currentCost];
    }

    ////////  盤上のPiece配置周り
    // 指定されたPiecesの中で盤上に置かれているPieceを取得
    private void SearchPuttedPieces(PieceBase[] pieces)
    {
        foreach (PieceBase piece in pieces)
        {
            if (!piece.IsPutted) return;
            if (piece.IsDestroyed) return;
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

    private void PutPieceInfo(PieceBase target, int column, int row, bool isPutted)
    {
        target.Column = column;
        target.Row = row;
        target.IsPutted = isPutted;
        // 音
        Sound.LoadSe("9","9_komaidou");
        Sound.PlaySe("9");
    }


    // 手札周り
    private void SetHoldingPieceUI(PieceBase[] pieces)
    {
        int piece1num = pieces.Where(i => i.PieceType == PieceType.Piece1 && !i.IsPutted && !i.IsDestroyed).Count();
        int piece2num = pieces.Where(i => i.PieceType == PieceType.Piece2 && !i.IsPutted && !i.IsDestroyed).Count();
        int piece3num = pieces.Where(i => i.PieceType == PieceType.Piece3 && !i.IsPutted && !i.IsDestroyed).Count();
        int piece4num = pieces.Where(i => i.PieceType == PieceType.Piece4 && !i.IsPutted && !i.IsDestroyed).Count();
        int piece5num = pieces.Where(i => i.PieceType == PieceType.Piece5 && !i.IsPutted && !i.IsDestroyed).Count();

        holdingPieceImage[0].sprite = pieceNumberImage[piece1num];
        holdingPieceImage[1].sprite = pieceNumberImage[piece2num];
        holdingPieceImage[2].sprite = pieceNumberImage[piece3num];
        holdingPieceImage[3].sprite = pieceNumberImage[piece4num];
        holdingPieceImage[4].sprite = pieceNumberImage[piece5num];

        if (piece1num >= 1 && currentCost >= 1)
            ActivateHoldingPiece(holdingPieceButtons[0], PieceType.Piece1);
        else
            DisableHoldingPiece(holdingPieceButtons[0]);

        if (piece2num >= 1 && currentCost >= 2)
            ActivateHoldingPiece(holdingPieceButtons[1], PieceType.Piece2);
        else
            DisableHoldingPiece(holdingPieceButtons[1]);

        if (piece3num >= 1 && currentCost >= 3)
            ActivateHoldingPiece(holdingPieceButtons[2], PieceType.Piece3);
        else
            DisableHoldingPiece(holdingPieceButtons[2]);

        if (piece4num >= 1 && currentCost >= 4)
            ActivateHoldingPiece(holdingPieceButtons[3], PieceType.Piece4);
        else
            DisableHoldingPiece(holdingPieceButtons[3]);

        if (piece5num >= 1 && currentCost >= 5)
            ActivateHoldingPiece(holdingPieceButtons[4], PieceType.Piece5);
        else
            DisableHoldingPiece(holdingPieceButtons[4]);
    }

    private void ActivateHoldingPiece(Button button, PieceType pieceType)
    {
        button.interactable = true;
        var stream = button.OnClickAsObservable().Subscribe(_ =>
                {
                    OnClickAnyPieces(this.player, -1, pieceType);
                });
        _compositeDisposable.Add(stream);
    }

    private void DisableHoldingPiece(Button button)
    {
        button.interactable = false;
    }



    // Board上にあるPieceの中で、置けるPieceを監視
    private void ObserveAvailablePieces(PieceBase[] pieces, PieceProvider[] piecesObj)
    {
        foreach (var piece in pieces)
        {
            if (piece.IsDestroyed) continue;
            if (!piece.IsPutted) continue;
            if (piece.PieceCost > currentCost) continue;

            var stream = piecesObj[piece.PieceNum].pieceButton
                .OnClickAsObservable().Subscribe(_ =>
                {
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
            if (pieceNum < 0) // 手札から選択
            {
                var num = PiecesObject1.Where(x => x.PieceType == pieceType && !Pieces1[x.PieceNum].IsPutted && !Pieces1[x.PieceNum].IsDestroyed).FirstOrDefault().PieceNum;
                playerAction.Piece = Pieces1[num];
                playerAction.OnBoard = false;
                boardManager.SearchMovePoint(playerAction, false);
                return;
            }

            playerAction.Piece = Pieces1[pieceNum];
            playerAction.CurrentRow = Pieces1[pieceNum].Row;
            playerAction.CurrentColumn = Pieces1[pieceNum].Column;
            playerAction.OnBoard = true;

            ObserveOneself(playerAction, PiecesObject1[pieceNum]);
            boardManager.SearchMovePoint(playerAction, true);
        }
        else if (player == PlayerType.Player2)
        {
            if (pieceNum < 0) // 手札から選択
            {
                var num = PiecesObject2.Where(x => x.PieceType == pieceType && !Pieces2[x.PieceNum].IsPutted && !Pieces2[x.PieceNum].IsDestroyed).FirstOrDefault().PieceNum;
                playerAction.Piece = Pieces2[num];
                playerAction.OnBoard = false;
                boardManager.SearchMovePoint(playerAction, false);
                return;
            }

            playerAction.Piece = Pieces2[pieceNum];
            playerAction.CurrentRow = Pieces2[pieceNum].Row;
            playerAction.CurrentColumn = Pieces2[pieceNum].Column;
            playerAction.OnBoard = true;

            ObserveOneself(playerAction, PiecesObject2[pieceNum]);
            boardManager.SearchMovePoint(playerAction, true);
        }
    }

    private void ObserveOneself(PlayerAction action, PieceProvider pieceObj)
    {
        pieceObj.ChangeAttackIcon(true);
        var stream = pieceObj.pieceButton
                .OnClickAsObservable().Subscribe(_ =>
                {
                    action.Action = PieceAction.Attack;
                    MessageBroker.Default.Publish(action);
                });
        _compositeDisposable.Add(stream);
    }

    private void DisposeAllStream()
    {
        _compositeDisposable.Clear();
        boardManager.RemoveAllBtnAction();
        //PiecesObject1[x.Piece.PieceNum].ChangeAttackIcon(false);
    }

    private IEnumerator UndoActions()
    {
        if (playerActions.Count == 0) yield break;
        var reverseAction = playerActions.AsEnumerable().Reverse();
        foreach (PlayerAction x in reverseAction)
        {
            if (x.Action == PieceAction.Attack)
            {
                if (x.Player == PlayerType.Player1)
                {
                    PiecesObject1[x.Piece.PieceNum].ChangeAttackIcon(false);
                }
                else if (x.Player == PlayerType.Player2)
                {
                    PiecesObject2[x.Piece.PieceNum].ChangeAttackIcon(false);
                }
            }

            if (x.Action == PieceAction.Move)
            {
                if (x.Player == PlayerType.Player1)
                {
                    if (!x.OnBoard)
                    {
                        ChangeActiveUI(PiecesObject1[x.Piece.PieceNum].gameObject, false);
                        PutPieceInfo(x.Piece, 0, 0, false);
                    }
                    else
                    {
                        PutPieceUI(PiecesObject1[x.Piece.PieceNum].gameObject, x.CurrentColumn, x.CurrentRow);
                        PutPieceInfo(x.Piece, x.CurrentColumn, x.CurrentRow, true);
                    }
                }
                else if (x.Player == PlayerType.Player2)
                {
                    if (!x.OnBoard)
                    {
                        ChangeActiveUI(PiecesObject2[x.Piece.PieceNum].gameObject, false);
                        PutPieceInfo(x.Piece, 0, 0, false);
                    }
                    else
                    {
                        PutPieceUI(PiecesObject2[x.Piece.PieceNum].gameObject, x.CurrentColumn, x.CurrentRow);
                        PutPieceInfo(x.Piece, x.CurrentColumn, x.CurrentRow, true);
                    }
                }
            }
        }
        yield return new WaitForSeconds(2.0f);
    }

    private void OnClickUndoButton()
    {
        if (playerActions.Count == 0) return;

        DisposeAllStream();
        var x = playerActions.Last();

        if (x.Action == PieceAction.Move)
        {
            if (x.Player == PlayerType.Player1)
            {
                if (!x.OnBoard)
                {
                    ChangeActiveUI(PiecesObject1[x.Piece.PieceNum].gameObject, false);
                    PutPieceInfo(x.Piece, 0, 0, false);
                }
                else
                {
                    PutPieceUI(PiecesObject1[x.Piece.PieceNum].gameObject, x.CurrentColumn, x.CurrentRow);
                    PutPieceInfo(x.Piece, x.CurrentColumn, x.CurrentRow, true);
                }
            }
            else if (x.Player == PlayerType.Player2)
            {
                if (!x.OnBoard)
                {
                    ChangeActiveUI(PiecesObject2[x.Piece.PieceNum].gameObject, false);
                    PutPieceInfo(x.Piece, 0, 0, false);
                }
                else
                {
                    PutPieceUI(PiecesObject2[x.Piece.PieceNum].gameObject, x.CurrentColumn, x.CurrentRow);
                    PutPieceInfo(x.Piece, x.CurrentColumn, x.CurrentRow, true);
                }
            }
        }
        ChangeCost(-x.Piece.PieceCost);
        playerActions.RemoveAt(playerActions.Count - 1);
        StartObserve();
    }





    ///// Battle周り
    public IEnumerator StartMove()
    {
        foreach(var pieceObj in PiecesObject1)
        {
            pieceObj.ChangeAttackIcon(false);
        }
        foreach(var pieceObj in PiecesObject2)
        {
            pieceObj.ChangeAttackIcon(false);
        }

        if (playerActions.Count == 0) yield break;

        foreach (PlayerAction x in playerActions)
        {
            if (x.Action == PieceAction.Attack) continue;

            if (x.Player == PlayerType.Player1)
            {
                if (!x.OnBoard)
                {
                    ChangeActiveUI(PiecesObject1[x.Piece.PieceNum].gameObject, true);
                    PutPieceInfo(x.Piece, x.NextColumn, x.NextRow, true);
                }
                else
                {
                    PutPieceUI(PiecesObject1[x.Piece.PieceNum].gameObject, x.NextColumn, x.NextRow);
                    PutPieceInfo(x.Piece, x.NextColumn, x.NextRow, true);
                }
            }
            else if (x.Player == PlayerType.Player2)
            {
                if (!x.OnBoard)
                {
                    ChangeActiveUI(PiecesObject2[x.Piece.PieceNum].gameObject, true);
                    PutPieceInfo(x.Piece, x.NextColumn, x.NextRow, true);
                }
                else
                {
                    PutPieceUI(PiecesObject2[x.Piece.PieceNum].gameObject, x.NextColumn, x.NextRow);
                    PutPieceInfo(x.Piece, x.NextColumn, x.NextRow, true);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator ExcecuteMoveDestroy()
    {
        Debug.Log("ExcecuteMoveDestroy");
        if (playerActions.Count == 0) yield break;

        PieceBase[] puttedPieces1 = Pieces1.Where(i => i.IsPutted && !i.IsDestroyed).ToArray();
        PieceBase[] puttedPieces2 = Pieces2.Where(i => i.IsPutted && !i.IsDestroyed).ToArray();

        int[] uniqeNum1 = puttedPieces1.Select(i => i.Row * 8 + i.Column).ToArray();
        int[] uniqeNum2 = puttedPieces2.Select(i => i.Row * 8 + i.Column).ToArray();

        for (int num1 = 0; num1 < uniqeNum1.Length; num1++)
        {
            for (int num2 = 0; num2 < uniqeNum2.Length; num2++)
            {
                if (uniqeNum1[num1] != uniqeNum2[num2]) continue;

                if (puttedPieces1[num1].PieceCost > puttedPieces2[num2].PieceCost)
                {
                    destroyObjects.Add(puttedPieces2[num2]);
                }
                else if (puttedPieces1[num1].PieceCost == puttedPieces2[num2].PieceCost)
                {
                    destroyObjects.Add(puttedPieces1[num1]);
                    destroyObjects.Add(puttedPieces2[num2]);
                }
                else
                {
                    destroyObjects.Add(puttedPieces1[num1]);
                }
            }
        }

        foreach (PieceBase piece in destroyObjects)
        {
            Debug.Log(piece.Player + "の" + piece.PieceType + " " + piece.PieceNum + "をDestroy");
            DestroyPiece(piece);
        }
        destroyObjects.Clear();
        yield return new WaitForSeconds(1.0f);
    }

    private void DestroyPiece(PieceBase piece)
    {
        puttedPieces.Remove(piece);
        piece.IsDestroyed = true;
        PutPieceInfo(piece, -1, -1, false);
        if (piece.Player == PlayerType.Player1)
        {
            ChangeActiveUI(PiecesObject1[piece.PieceNum].gameObject, false);
        }
        else if (piece.Player == PlayerType.Player2)
        {
            ChangeActiveUI(PiecesObject2[piece.PieceNum].gameObject, false);
        }
        Sound.LoadSe("14","14_stop");
        Sound.PlaySe("14");
    }

    public IEnumerator StartBattle()
    {
        Debug.Log("StartBattle");
        if (playerActions.Count == 0) yield break;

        foreach (PlayerAction x in playerActions)
        {
            if (x.Action == PieceAction.Move) continue;
            if (x.Piece.IsDestroyed) continue;

            SearchAttackAvailablePoint(x);

            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1.0f);
    }

    public void SearchAttackAvailablePoint(PlayerAction playerAction)
    {
        var type = playerAction.Piece.PieceType;
        var player = playerAction.Player;
        var column = playerAction.CurrentColumn;
        var row = playerAction.CurrentRow;
        Debug.Log("SearchAttackAvailablePoint：" + player + "の" + type + "が攻撃");

        switch (type)
        {
            case PieceType.Piece1:
                CheckAttackPointOfPiece1(column, row, player);
                break;
            case PieceType.Piece2:
                CheckAttackPointOfPiece2(column, row, player);
                break;
            case PieceType.Piece3:
                CheckAttackPointOfPiece3(column, row, player);
                break;
            case PieceType.Piece4:
                CheckAttackPointOfPiece4(column, row, player);
                break;
            case PieceType.Piece5:
                CheckAttackPointOfPiece5(column, row, player);
                break;
        }
    }

    public bool CanAttack(int column, int row)
    {
        return boardManager.CanAttack(column, row);
    }

    private void PieceAttack(int column, int row, PlayerType player)
    {
        //UI表示
        //boardManager.AttackAnimation(column, row);
        PutPieceUI(effect,column,row);

        // このCellに敵がいるかチェックして追加
        var cellNum = row * 8 + column;

        if (player == PlayerType.Player1)
        {
            var pieces = Pieces2.Where(i => i.IsPutted && !i.IsDestroyed).ToArray();
            foreach (var piece in pieces)
            {
                Debug.Log(piece.Player + piece.PieceType.ToString());
                Debug.Log("piece column row"+piece.Column + piece.Row);
                var pieceNum = piece.Row * 8 + piece.Column;
                if (pieceNum == cellNum) destroyObjects.Add(piece);
            }
            if (cellNum == 15)
            {
                player1win = true;
            }
        }
        else if (player == PlayerType.Player2)
        {
            var pieces = Pieces1.Where(i => i.IsPutted && !i.IsDestroyed).ToArray();
            foreach (var piece in pieces)
            {
                Debug.Log(piece.Player + piece.PieceType.ToString());
                Debug.Log("piece column row"+piece.Column + piece.Row);
                var pieceNum = piece.Row * 8 + piece.Column;
                if (pieceNum == cellNum) destroyObjects.Add(piece);
            }
            if (cellNum == 8)
            {
                player2win = true;
            }
        }
    }

    private void CheckAttackPointOfPiece1(int column, int row, PlayerType player)
    {
        if (player == PlayerType.Player1)
        {
            if (CanAttack(column + 1, row)) PieceAttack(column + 1, row, player);
        }
        else if (player == PlayerType.Player2)
        {
            if (CanAttack(column - 1, row)) PieceAttack(column - 1, row, player);
        }
    }

    private void CheckAttackPointOfPiece2(int column, int row, PlayerType player)
    {
        if (CanAttack(column + 1, row)) PieceAttack(column + 1, row, player);
        if (CanAttack(column - 1, row)) PieceAttack(column - 1, row, player);
        if (CanAttack(column, row + 1)) PieceAttack(column, row + 1, player);
        if (CanAttack(column, row - 1)) PieceAttack(column, row - 1, player);
    }

    private void CheckAttackPointOfPiece3(int column, int row, PlayerType player)
    {
        if (CanAttack(column + 2, row - 1)) PieceAttack(column + 2, row - 1, player);
        if (CanAttack(column + 2, row + 1)) PieceAttack(column + 2, row + 1, player);
        if (CanAttack(column - 2, row - 1)) PieceAttack(column - 2, row - 1, player);
        if (CanAttack(column - 2, row + 1)) PieceAttack(column - 2, row + 1, player);
        if (CanAttack(column + 1, row + 2)) PieceAttack(column + 1, row + 2, player);
        if (CanAttack(column - 1, row + 2)) PieceAttack(column - 1, row + 2, player);
        if (CanAttack(column + 1, row - 2)) PieceAttack(column + 1, row - 2, player);
        if (CanAttack(column - 1, row - 2)) PieceAttack(column - 1, row - 2, player);
    }

    private void CheckAttackPointOfPiece4(int column, int row, PlayerType player)
    {
        if (CanAttack(column + 1, row + 1)) PieceAttack(column + 1, row + 1, player);
        if (CanAttack(column + 1, row - 1)) PieceAttack(column + 1, row - 1, player);
        if (CanAttack(column - 1, row + 1)) PieceAttack(column - 1, row + 1, player);
        if (CanAttack(column - 1, row - 1)) PieceAttack(column - 1, row - 1, player);
    }

    private void CheckAttackPointOfPiece5(int column, int row, PlayerType player)
    {
        if (CanAttack(column + 1, row + 1)) PieceAttack(column + 1, row + 1, player);
        if (CanAttack(column + 1, row - 1)) PieceAttack(column + 1, row - 1, player);
        if (CanAttack(column + 1, row)) PieceAttack(column + 1, row, player);
        if (CanAttack(column - 1, row + 1)) PieceAttack(column - 1, row + 1, player);
        if (CanAttack(column - 1, row - 1)) PieceAttack(column - 1, row - 1, player);
        if (CanAttack(column - 1, row)) PieceAttack(column - 1, row, player);
        if (CanAttack(column, row + 1)) PieceAttack(column, row + 1, player);
        if (CanAttack(column, row - 1)) PieceAttack(column, row - 1, player);
    }

    public IEnumerator ExcecuteAttackDestroy()
    {
        playerActions.Clear();

        foreach (PieceBase piece in destroyObjects)
        {
            Debug.Log(piece.Player + "の" + piece.PieceType + " " + piece.PieceNum + "をDestroy");
            DestroyPiece(piece);
        }
        destroyObjects.Clear();
        yield return null;
    }
}
