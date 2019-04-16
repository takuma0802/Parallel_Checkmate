using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    /// Manager
    private BoardManager boardManager;

    /// Player
    [HideInInspector] private bool player1win = false;
    [HideInInspector] private bool player2win = false;
    private PlayerType player; // 現状のターンのPlayer
    private List<PlayerAction> playerActions = new List<PlayerAction>(); // そのターンに起きたActionを全て登録
    private PlayerAction playerAction; // その時のPlayerActionが保存される

    /// Piece
    [Header("Piece")]
    [SerializeField] private GameObject[] piecePrefabs; // 各PieceObjectのPrefab(0:Piece1/ 1:Piece2...)
    [SerializeField] private GameObject[] PlayerGameObject; // 生成されたPieceObjectを格納する親オブジェクト

    // 各Plyerの保持しているPiece情報
    // 0~4:Piece1/ 5~8:Piece2/ 9~11:Piece3...
    private PieceBase[] Pieces1;
    private PieceBase[] Pieces2;
    private PieceProvider[] Kings = new PieceProvider[2];
    private PieceProvider[] PiecesObject1 = new PieceProvider[15];
    private PieceProvider[] PiecesObject2 = new PieceProvider[15];

    private List<PieceBase> puttedPieces = new List<PieceBase>(); // 盤上に置かれているPiece情報
    private List<PieceBase> destroyObjects = new List<PieceBase>(); // そのターンで破壊されるPiece情報
    private List<Tuple<int, int>> attackPointList = new List<Tuple<int, int>>(); // そのコマが攻撃することが出来るマスのリスト

    /// 手札
    [Header("手札")]
    [SerializeField] private Button[] holdingPieceButtons;
    [SerializeField] private Image[] holdingPieceSprites; // 各手札の画像
    [SerializeField] private Sprite[] pieceNumberSprites; // 手札の枚数を示す画像

    /// Cost
    [Header("Cost")]
    public int MaxCost = 5; // コスト最大値
    private int currentCost; // 現状のコスト
    [SerializeField] private Image costImage;
    [SerializeField] private Sprite[] costNumberSprites;

    /// その他
    [Header("Other")]
    [SerializeField] private StrategyUIPresenter StrategyUI;
    private ObjectPool objectPool; // 後々,外から渡す
    [SerializeField] private GameObject attackEffectPrefab; // 後々別クラスへ

    private CompositeDisposable _compositeDisposable = new CompositeDisposable(); // 置くことが出来るCellの監視リスト

    // メインシーンが始まった際の初期化処理
    public IEnumerator InitializePlayer(BoardManager boardManager)
    {
        this.boardManager = boardManager;
        this.objectPool = GetComponent<ObjectPool>(); // 後々別クラスへ
        objectPool.CreatePool(attackEffectPrefab, PlayerGameObject[2], 6); // 後々別クラスへ

        yield return StrategyUI.HideLowerArea();
        CreateAllPieces();
        yield return new WaitForEndOfFrame();

        foreach (PieceProvider piece in PiecesObject1) SetActivePieceUI(piece.gameObject, false);
        foreach (PieceProvider piece in PiecesObject2) SetActivePieceUI(piece.gameObject, false);
        ObserveStreams();
        yield return new WaitForEndOfFrame();
    }

    private void ObserveStreams()
    {
        // 1つのPieceに対する操作が完了した際に、PlayerActionインスタンスが送られてくる
        MessageBroker.Default.Receive<PlayerAction>().Subscribe(x =>
        {
            DisposeAllStream();
            ChangeCost(x.Piece.PieceCost);

            if (x.Action == PieceAction.Move)
            {
                StartCoroutine(ExecuteMovePiece(x));
            }
            else if (x.Action == PieceAction.Attack)
            {
                if (x.Player == PlayerType.Player1)
                {
                    PiecesObject1[x.Piece.PieceNum].ChangeAttackIcon(true);
                }
                else if (x.Player == PlayerType.Player2)
                {
                    PiecesObject2[x.Piece.PieceNum].ChangeAttackIcon(true);
                }
            }

            playerActions.Add(x);
            StartObserve();
        });

        StrategyUI.UndoButton.OnClickAsObservable().Subscribe(_ =>
        {
            DisposeAllStream();
            OnClickUndoButton();
            StartObserve();
        });
    }

    private void CreateAllPieces()
    {
        var player = PlayerType.Player1;
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

        player = PlayerType.Player2;
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

        PutKings();
    }

    private void PutKings()
    {
        Kings[0] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[10]).GetComponent<PieceProvider>();
        Kings[0].SetPieceUIInfo(PlayerType.Player1, -1, PieceType.King);
        StartCoroutine(MovePieceUI(Kings[0].gameObject, 0, 1, false));

        Kings[1] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[11]).GetComponent<PieceProvider>();
        Kings[1].SetPieceUIInfo(PlayerType.Player2, -1, PieceType.King);
        StartCoroutine(MovePieceUI(Kings[1].gameObject, 7, 1, false));

        boardManager.PutKings();
    }

    private void SetActivePieceUI(GameObject pieceObj, bool enabled)
    {
        if (enabled)
        {
            pieceObj.transform.localScale = new Vector3(0, 0, 0);
            pieceObj.SetActive(enabled);
            pieceObj.transform.DOScale(1f, 0.3f).SetEase(Ease.InQuad);
        }
        else
        {
            pieceObj.transform.localScale = new Vector3(1, 1, 1);
            pieceObj.transform.DOScale(0f, 0.3f).SetEase(Ease.OutQuint);
            pieceObj.SetActive(enabled);
        }
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
        Reset();
        yield return StrategyUI.AppearLowerArea();

        StartObserve();

        // 決定ボタンが押される
        yield return StrategyUI.TurnEndButton.OnClickAsObservable().First().ToYieldInstruction();

        DisposeAllStream();
        UndoAllActions();

        yield return StrategyUI.HideLowerArea();
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
    }

    private void Reset()
    {
        ResetCost();
        for (var i = 0; i < PiecesObject1.Length; i++)
        {
            PiecesObject1[i].ChangeAttackIcon(false);
            PiecesObject2[i].ChangeAttackIcon(false);
        }
    }

    private void ResetCost()
    {
        currentCost = MaxCost;
        costImage.sprite = costNumberSprites[currentCost];
    }

    private void ChangeCost(int cost)
    {
        currentCost -= cost;
        costImage.sprite = costNumberSprites[currentCost];
    }

    ////////  盤上のPiece配置周り
    private IEnumerator MovePieceUI(GameObject target, int column, int row, bool isAnimation)
    {
        if (isAnimation)
        {
            var targetPos = boardManager.ReturnCellLocalPosition(column, row);
            var moveSequence = target.transform.DOLocalMove(targetPos, 0.5f).SetEase(Ease.OutQuint);
            yield return moveSequence.WaitForCompletion();
        }
        else
        {
            target.transform.localPosition = boardManager.ReturnCellLocalPosition(column, row);
            SetActivePieceUI(target, true);
        }
        Sound.LoadSe("9", "9_komaidou");
        Sound.PlaySe("9");
    }

    private void SetPieceInfo(PieceBase targetPiece, int column, int row, bool isPutted, bool isDestroyed)
    {
        targetPiece.SetPieceInfo(column, row, isPutted, isDestroyed);
    }

    // 手札周り
    private void SetHoldingPieceUI(PieceBase[] pieces)
    {
        int piece1num = pieces.Where(i => i.PieceType == PieceType.Piece1 && !i.IsPutted && !i.IsDestroyed).Count();
        int piece2num = pieces.Where(i => i.PieceType == PieceType.Piece2 && !i.IsPutted && !i.IsDestroyed).Count();
        int piece3num = pieces.Where(i => i.PieceType == PieceType.Piece3 && !i.IsPutted && !i.IsDestroyed).Count();
        int piece4num = pieces.Where(i => i.PieceType == PieceType.Piece4 && !i.IsPutted && !i.IsDestroyed).Count();
        int piece5num = pieces.Where(i => i.PieceType == PieceType.Piece5 && !i.IsPutted && !i.IsDestroyed).Count();

        holdingPieceSprites[0].sprite = pieceNumberSprites[piece1num];
        holdingPieceSprites[1].sprite = pieceNumberSprites[piece2num];
        holdingPieceSprites[2].sprite = pieceNumberSprites[piece3num];
        holdingPieceSprites[3].sprite = pieceNumberSprites[piece4num];
        holdingPieceSprites[4].sprite = pieceNumberSprites[piece5num];

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
    }

    private void UndoAllActions()
    {
        if (playerActions.Count == 0) return;
        var reverseAction = playerActions.AsEnumerable().Reverse();
        foreach (PlayerAction action in reverseAction)
        {
            UndoPlayerAction(action);
        }
    }

    private void OnClickUndoButton()
    {
        if (playerActions.Count == 0) return;

        var action = playerActions.Last();
        UndoPlayerAction(action);
        ChangeCost(-action.Piece.PieceCost);
        playerActions.RemoveAt(playerActions.Count - 1);
    }

    private void UndoPlayerAction(PlayerAction pAction)
    {
        if (pAction.Action == PieceAction.Attack)
        {
            if (pAction.Player == PlayerType.Player1)
            {
                PiecesObject1[pAction.Piece.PieceNum].ChangeAttackIcon(false);
            }
            else if (pAction.Player == PlayerType.Player2)
            {
                PiecesObject2[pAction.Piece.PieceNum].ChangeAttackIcon(false);
            }
        }
        if (pAction.Action == PieceAction.Move)
        {
            if (pAction.Player == PlayerType.Player1)
            {
                if (!pAction.OnBoard)
                {
                    SetActivePieceUI(PiecesObject1[pAction.Piece.PieceNum].gameObject, false);
                    SetPieceInfo(pAction.Piece, 0, 0, false, false);
                }
                else
                {
                    StartCoroutine(MovePieceUI(PiecesObject1[pAction.Piece.PieceNum].gameObject, pAction.CurrentColumn, pAction.CurrentRow, false));
                    SetPieceInfo(pAction.Piece, pAction.CurrentColumn, pAction.CurrentRow, true, false);
                }
            }
            else if (pAction.Player == PlayerType.Player2)
            {
                if (!pAction.OnBoard)
                {
                    SetActivePieceUI(PiecesObject2[pAction.Piece.PieceNum].gameObject, false);
                    SetPieceInfo(pAction.Piece, 0, 0, false, false);
                }
                else
                {
                    StartCoroutine(MovePieceUI(PiecesObject2[pAction.Piece.PieceNum].gameObject, pAction.CurrentColumn, pAction.CurrentRow, false));
                    SetPieceInfo(pAction.Piece, pAction.CurrentColumn, pAction.CurrentRow, true, false);
                }
            }
        }
    }




    ///// Battle周り
    public IEnumerator StartMove()
    {
        if (playerActions.Count == 0) yield break;

        for (var i = 0; i < PiecesObject1.Length; i++)
        {
            PiecesObject1[i].ChangeAttackIcon(false);
            PiecesObject2[i].ChangeAttackIcon(false);
        }

        foreach (PlayerAction pAction in playerActions)
        {
            if (pAction.Action == PieceAction.Attack) continue;
            yield return ExecuteMovePiece(pAction);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator ExecuteMovePiece(PlayerAction pAction)
    {
        SetPieceInfo(pAction.Piece, pAction.NextColumn, pAction.NextRow, true, false);
        if (pAction.Player == PlayerType.Player1)
        {
            yield return MovePieceUI(PiecesObject1[pAction.Piece.PieceNum].gameObject, pAction.NextColumn, pAction.NextRow, pAction.OnBoard);
        }
        else if (pAction.Player == PlayerType.Player2)
        {
            yield return MovePieceUI(PiecesObject2[pAction.Piece.PieceNum].gameObject, pAction.NextColumn, pAction.NextRow, pAction.OnBoard);
        }
    }

    public IEnumerator ExcecuteMoveDestroy()
    {
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
            DestroyPiece(piece);
        }
        destroyObjects.Clear();
        yield return new WaitForSeconds(1.0f);
    }

    private void DestroyPiece(PieceBase piece)
    {
        SetPieceInfo(piece, -1, -1, false, true);
        if (piece.Player == PlayerType.Player1)
        {
            SetActivePieceUI(PiecesObject1[piece.PieceNum].gameObject, false);
        }
        else if (piece.Player == PlayerType.Player2)
        {
            SetActivePieceUI(PiecesObject2[piece.PieceNum].gameObject, false);
        }
        Sound.LoadSe("14", "14_stop");
        Sound.PlaySe("14");
    }

    public IEnumerator StartBattle()
    {
        if (playerActions.Count == 0) yield break;

        foreach (PlayerAction x in playerActions)
        {
            if (x.Action == PieceAction.Move) continue;
            if (x.Piece.IsDestroyed) continue;

            attackPointList.Clear();
            UpdateAttackPointList(x);
            yield return PieceAttack(x.Player);
        }
        playerActions.Clear();
    }

    public void UpdateAttackPointList(PlayerAction playerAction)
    {
        var type = playerAction.Piece.PieceType;
        var player = playerAction.Player;
        var column = playerAction.CurrentColumn;
        var row = playerAction.CurrentRow;

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

    private IEnumerator PieceAttack(PlayerType player)
    {
        foreach (var cell in attackPointList)
        {
            var column = cell.Item1;
            var row = cell.Item2;
            if (!CanAttack(column, row)) continue;

            var hitEffect = objectPool.GetObject().GetComponent<HitEffect>();
            hitEffect.ChangePosition(boardManager.ReturnCellLocalPosition(column, row));
            yield return hitEffect.HitEffectAnimation();

            // 攻撃されたCellに敵のコマがいるかチェックして追加
            var uniqueCellNum = row * 8 + column;

            if (player == PlayerType.Player1)
            {
                var pieces = Pieces2.Where(i => i.IsPutted && !i.IsDestroyed).ToArray();
                foreach (var piece in pieces)
                {
                    var pieceNum = piece.Row * 8 + piece.Column;
                    if (pieceNum == uniqueCellNum) destroyObjects.Add(piece);
                }
                if (uniqueCellNum == 15) // King
                {
                    player1win = true;
                }
            }
            else if (player == PlayerType.Player2)
            {
                var pieces = Pieces1.Where(i => i.IsPutted && !i.IsDestroyed).ToArray();
                foreach (var piece in pieces)
                {
                    var pieceNum = piece.Row * 8 + piece.Column;
                    if (pieceNum == uniqueCellNum) destroyObjects.Add(piece);
                }
                if (uniqueCellNum == 8) // King
                {
                    player2win = true;
                }
            }
        }
    }

    public bool CanAttack(int column, int row)
    {
        return boardManager.CanAttack(column, row);
    }

    private void CheckAttackPointOfPiece1(int column, int row, PlayerType player)
    {
        if (player == PlayerType.Player1)
        {
            attackPointList.Add(Tuple.Create(column + 1, row));
        }
        else if (player == PlayerType.Player2)
        {
            attackPointList.Add(Tuple.Create(column - 1, row));
        }
    }

    private void CheckAttackPointOfPiece2(int column, int row, PlayerType player)
    {
        attackPointList.Add(Tuple.Create(column, row - 1)); // 上
        attackPointList.Add(Tuple.Create(column + 1, row)); // 右
        attackPointList.Add(Tuple.Create(column, row + 1)); // 下
        attackPointList.Add(Tuple.Create(column - 1, row)); // 左
    }

    private void CheckAttackPointOfPiece3(int column, int row, PlayerType player)
    {
        attackPointList.Add(Tuple.Create(column + 1, row - 2)); // 右上上
        attackPointList.Add(Tuple.Create(column + 2, row - 1)); // 右右上
        attackPointList.Add(Tuple.Create(column + 2, row + 1)); // 右右下
        attackPointList.Add(Tuple.Create(column + 1, row + 2)); // 右下下
        attackPointList.Add(Tuple.Create(column - 1, row + 2)); // 左下下
        attackPointList.Add(Tuple.Create(column - 2, row + 1)); // 左左下
        attackPointList.Add(Tuple.Create(column - 2, row - 1)); // 左左上
        attackPointList.Add(Tuple.Create(column - 1, row - 2)); // 左上上
    }

    private void CheckAttackPointOfPiece4(int column, int row, PlayerType player)
    {
        attackPointList.Add(Tuple.Create(column + 1, row - 1)); // 右上
        attackPointList.Add(Tuple.Create(column + 1, row + 1)); // 右下
        attackPointList.Add(Tuple.Create(column - 1, row + 1)); // 左下
        attackPointList.Add(Tuple.Create(column - 1, row - 1)); // 左上
    }

    private void CheckAttackPointOfPiece5(int column, int row, PlayerType player)
    {
        attackPointList.Add(Tuple.Create(column, row - 1)); // 上
        attackPointList.Add(Tuple.Create(column + 1, row - 1)); // 右上
        attackPointList.Add(Tuple.Create(column + 1, row)); // 右
        attackPointList.Add(Tuple.Create(column + 1, row + 1)); // 右下
        attackPointList.Add(Tuple.Create(column, row + 1)); // 下
        attackPointList.Add(Tuple.Create(column - 1, row + 1)); // 左下
        attackPointList.Add(Tuple.Create(column - 1, row)); // 左
        attackPointList.Add(Tuple.Create(column - 1, row - 1)); // 左上
    }

    public IEnumerator ExcecuteAttackDestroy()
    {
        foreach (PieceBase piece in destroyObjects)
        {
            DestroyPiece(piece);
        }
        destroyObjects.Clear();
        yield return new WaitForSeconds(1f);
    }

    // 返り値 0:続行, 1:1P勝利, 2:2P勝利, 3:引き分け
    public int GetResult()
    {
        if (!player1win && !player2win) return 0;
        if (player1win && player2win) return 3;

        if (player1win)
            return 1;
        else
            return 2;
    }
}
