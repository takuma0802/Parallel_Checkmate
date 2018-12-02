using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;

public class BoardManager : MonoBehaviour
{
    public int columnSize;
    public int rowSize;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject backGround;

    // Size周り
    // private Dictionary<int, float> cellSizeRateDict = new Dictionary<int, float>(){
    // 		// key: column count, value: cell size Rate
    //         {3, 1f}, {4, 0.79f}, {5, 0.68f}, {6, 0.61f}, {8, 0.8f}};
    private float cellSize = 100f;
    // private float cellSizeWidth;
    // private float cellSizeHeight;

    // Cell周り
    // column * row
    private CellHandler[,] cells;
    private List<CellHandler> previousCells = new List<CellHandler>();
    private List<CellHandler> observedCells = new List<CellHandler>();

    private PlayerAction playerAction;

    // public float CellSizeRate
    // {
    //     get { return cellSizeRateDict[columnSize]; }
    // }

    public int CoulumnSize
    {
        get { return columnSize; }
    }

    public List<CellHandler> PreviousCells
    {
        get { return previousCells; }
    }

    public IEnumerator CreateBoard()
    {
        ResetBoard();
        // 初期化
        ObjectCreator.CreateInObject(this.gameObject, backGround);
        cells = new CellHandler[columnSize, rowSize];

        GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
        GetComponent<GridLayoutGroup>().constraintCount = columnSize;

        for (int row = 0; row < rowSize; row++)
        {
            for (int column = 0; column < columnSize; column++)
            {
                StartCoroutine(CreateCell(CellType.Default, row, column));
            }
        }
        PutKing();
        yield return null;
    }

    public void ResetBoard()
    {
        previousCells.Clear();
        observedCells.Clear();

        if (transform.childCount == 0) return;
        ObjectCreator.DestroyAllChild(this.gameObject);
    }

    private IEnumerator CreateCell(CellType cellType, int row, int column)
    {
        CellHandler cell = ObjectCreator.CreateInObject(this.gameObject, cellPrefab).GetComponent<CellHandler>();
        yield return cell.SetCell(row, column, cellType, columnSize);
        cells[column, row] = cell;

        yield return null;
    }

    private void PutKing()
    {
        // King置け
    }


    public void Undo()
    {
        if (previousCells.Count < 2) return;
        var preCell = previousCells[previousCells.Count - 2];

        RemoveAllBtnAction();
        previousCells[previousCells.Count - 1].ChangeToDefault();
        previousCells.RemoveAt(previousCells.Count - 1);
    }

    public Vector2 ReturnCellLocalPosition(int column, int row)
    {
        return cells[column, row].gameObject.transform.localPosition;
    }

    public void SearchMovePoint(PlayerAction playerAction, bool onBoard)
    {
        this.playerAction = playerAction;

        if (!onBoard)
        {
            SearchMovePointOfHolding(playerAction.Piece.Player);
        }
        else
        {
            var type = playerAction.Piece.PieceType;
            var column = playerAction.CurrentColumn;
            var row = playerAction.CurrentRow;

            switch (type)
            {
                case PieceType.Piece1:
                    SearchMovePointOfPiece1(column, row);
                    break;
                case PieceType.Piece2:
                    SearchMovePointOfPiece2(column, row);
                    break;
                case PieceType.Piece3:
                    SearchMovePointOfPiece3(column, row);
                    break;
                case PieceType.Piece4:
                    SearchMovePointOfPiece4(column, row);
                    break;
                case PieceType.Piece5:
                    SearchMovePointOfPiece5(column, row);
                    break;
            }
        }
    }

    // 指定したCellに駒を置ける状況かどうかを判定する
    public bool CanPut(int row, int column)
    {
        var isInside = 0 <= row && row < columnSize && 0 <= column && column < columnSize;
        if (!isInside) return false;

        var canPut = cells[column, row].CellType == CellType.Default;
        return canPut;
    }

    private void SearchMovePointOfHolding(PlayerType player)
    {
        if (player == PlayerType.Player1)
        {
            ObserveCells(cells[0, 0]);
            ObserveCells(cells[0, 1]);
            ObserveCells(cells[0, 2]);
            ObserveCells(cells[1, 0]);
            ObserveCells(cells[1, 1]);
            ObserveCells(cells[1, 2]);
        }
        else
        {
            ObserveCells(cells[6, 0]);
            ObserveCells(cells[6, 1]);
            ObserveCells(cells[6, 2]);
            ObserveCells(cells[7, 0]);
            ObserveCells(cells[7, 1]);
            ObserveCells(cells[7, 2]);
        }
    }

    private void SearchMovePointOfPiece1(int row, int column)
    {
    }
    private void SearchMovePointOfPiece2(int row, int column)
    {
    }
    private void SearchMovePointOfPiece3(int row, int column)
    {
        if (CanPut(column + 2, row - 1)) ObserveCells(cells[column + 2, row - 1]);
        if (CanPut(column + 2, row + 1)) ObserveCells(cells[column + 2, row + 1]);
        if (CanPut(column - 2, row - 1)) ObserveCells(cells[column - 2, row - 1]);
        if (CanPut(column - 2, row + 1)) ObserveCells(cells[column - 2, row + 1]);
        if (CanPut(column + 1, row + 2)) ObserveCells(cells[column + 1, row + 2]);
        if (CanPut(column - 1, row + 2)) ObserveCells(cells[column - 1, row + 2]);
        if (CanPut(column + 1, row - 2)) ObserveCells(cells[column + 1, row - 2]);
        if (CanPut(column - 1, row - 2)) ObserveCells(cells[column - 1, row - 2]);
    }
    private void SearchMovePointOfPiece4(int row, int column)
    {

    }
    private void SearchMovePointOfPiece5(int row, int column)
    {

    }

    private void ObserveCells(CellHandler cell)
    {
        cell.ChangeButtonInteractable(true);
        cell.ChangeAvailableMarker(true);
        observedCells.Add(cell);
        cell.CellButton.onClick.AddListener(() => OnClickedCellButton(cell));
    }

    private void OnClickedCellButton(CellHandler cell)
    {
        RemoveAllBtnAction();
        playerAction.NextColumn = cell.Column;
        playerAction.NextRow = cell.Row;
        playerAction.Action = PieceAction.Move;

        MessageBroker.Default.Publish(playerAction);

        // previousCells.Add(cell);

    }

    // 監視されているButtonの監視を解除する
    public void RemoveAllBtnAction()
    {
        foreach (var cell in observedCells)
        {
            cell.ChangeAvailableMarker(false);
            cell.ChangeButtonInteractable(false);
            cell.CellButton.onClick.RemoveAllListeners();
        }
    }
}