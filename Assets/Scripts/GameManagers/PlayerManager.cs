using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // コストの最大値
    public int AvailableCost = 5;
    // 現状のコスト
    private int currentCost;

    // 0~4:Piece1/ 5~8:Piece2/ 9~11:Piece3...
    public PieceBase[] Pieces1;
    public PieceBase[] Pieces2;

    public GameObject[] PiecesObject1 = new GameObject[15];
    public GameObject[] PiecesObject2 = new GameObject[15];

    // 盤面に置かれているPieceの登録
    private List<PieceBase> puttedPieces = new List<PieceBase>();

    // 0:Piece1/ 1:Piece2/ 2:Piece3...
    public GameObject[] piecePrefabs;
    public GameObject[] PlayerGameObject;

    public Button SubmitButton;
    private TimeManager timeManager;



    //[SerializeField] 

    public void InitializePlayer()
    {
        timeManager = GetComponent<TimeManager>();
        if (!timeManager) gameObject.AddComponent<TimeManager>();

        CreateHoldingPiece(PlayerType.Player1);
        CreateHoldingPiece(PlayerType.Player2);

        foreach(GameObject piece in PiecesObject1) piece.SetActive(false);
        foreach(GameObject piece in PiecesObject2) piece.SetActive(false);
    }

    private void CreateHoldingPiece(PlayerType player)
    {
        if (player == PlayerType.Player1)
        {
            Pieces1 = new PieceBase[15];
            for (int i = 0; i < 5; i++)
            {
                Pieces1[i] = new Piece1(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[0]);
            }
            for (int i = 5; i < 9; i++)
            {
                Pieces1[i] = new Piece2(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[1]);
            }
            for (int i = 9; i < 12; i++)
            {
                Pieces1[i] = new Piece3(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[2]);
            }
            for (int i = 12; i < 14; i++)
            {
                Pieces1[i] = new Piece4(player, i);
                PiecesObject1[i] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[3]);
            }
            Pieces1[14] = new Piece5(player, 14);
            PiecesObject1[14] = ObjectCreator.CreateInObject(PlayerGameObject[0], piecePrefabs[4]);
        }
        else if (player == PlayerType.Player2)
        {
            Pieces2 = new PieceBase[15];
            for (int i = 0; i < 5; i++)
            {
                Pieces2[i] = new Piece1(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[0]);
            }
            for (int i = 5; i < 9; i++)
            {
                Pieces2[i] = new Piece2(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[1]);
            }
            for (int i = 9; i < 12; i++)
            {
                Pieces2[i] = new Piece3(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[2]);
            }
            for (int i = 12; i < 14; i++)
            {
                Pieces2[i] = new Piece4(player, i);
                PiecesObject2[i] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[3]);
            }
            Pieces2[14] = new Piece5(player, 14);
            PiecesObject1[14] = ObjectCreator.CreateInObject(PlayerGameObject[1], piecePrefabs[4]);
        }
    }

    public IEnumerator StartStrategy(GameState player)
    {
        Reset();
        if (player == GameState.Player1)
        {
            SearchPuttedPieces(Pieces1);
            SetHoldingPieceUI(Pieces1);
        }
        else if (player == GameState.Player2)
        {
            SearchPuttedPieces(Pieces2);
        }








        // タイマー作動

        // 決定ボタンが押される or 制限時間が過ぎるまで待つ(ここまだ出来てない)
        yield return SubmitButton.OnClickAsObservable().First().ToYieldInstruction();
    }

    private void Reset()
    {
        currentCost = AvailableCost;
        //puttedPieces.Clear();
    }

    private void SearchPuttedPieces(PieceBase[] pieces)
    {
        // foreach(PieceBase piece in pieces)
        // {
        //     if(piece.IsPutted) puttedPieces.Add(piece);
        // }
    }

    private void SetPuttedPiece()
    {
        foreach (PieceBase piece in puttedPieces)
        {

        }
    }

    private void SetHoldingPieceUI(PieceBase[] pieces)
    {
        IEnumerable<PieceBase> list = pieces.Where(i => !i.IsPutted);


    }

}
