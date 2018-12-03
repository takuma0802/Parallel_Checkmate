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
    public float cellSize = 100f;

    // Cell周り
    // column * row
    private CellHandler[,] cells;
    private List<CellHandler> previousCells = new List<CellHandler>();
    private List<CellHandler> observedCells = new List<CellHandler>();

    private PlayerAction playerAction;
    private List<CellHandler> puttedCellList = new List<CellHandler>();

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
                StartCoroutine(CreateCell(column, row));
            }
        }
        yield return null;
    }

    public void ResetBoard()
    {
        previousCells.Clear();
        observedCells.Clear();

        if (transform.childCount == 0) return;
        ObjectCreator.DestroyAllChild(this.gameObject);
    }

    private IEnumerator CreateCell(int column, int row)
    {
        CellHandler cell = ObjectCreator.CreateInObject(this.gameObject, cellPrefab).GetComponent<CellHandler>();
        yield return cell.SetCell(column, row, CellType.Default);
        cells[column, row] = cell;

        yield return null;
    }

    public void PutKings()
    {
        cells[0,1].SetKing();
        cells[7,1].SetKing();
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
        //if (this.playerAction.Piece == playerAction.Piece) 
        RemoveAllBtnAction();
        this.playerAction = playerAction;

        if (!onBoard) //手札
        {
            SearchMovePointOfHolding(playerAction.Piece.Player);
        }
        else //Board上
        {
            var type = playerAction.Piece.PieceType;
            var column = playerAction.CurrentColumn;
            var row = playerAction.CurrentRow;

            switch (type)
            {
                case PieceType.Piece1:
                    SearchMovePointOfPiece1(column, row, playerAction.Piece.Player);
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
    public bool CanPut(int column, int row)
    {
        var isInside = 0 <= row && row < rowSize && 0 <= column && column < columnSize;
        if (!isInside) return false;

        var canPut = cells[column, row].CellType == CellType.Default;
        return canPut;
    }

    public bool CanAttack(int column,int row)
    {
        var isInside = 0 <= row && row < rowSize && 0 <= column && column < columnSize;
        if (!isInside) return false;

        var canAttack = cells[column, row].CellType == CellType.Putted;
        return canAttack;
    }

    private void SearchMovePointOfHolding(PlayerType player)
    {
        if (player == PlayerType.Player1)
        {
            ObserveCells(cells[0, 0]);
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
            ObserveCells(cells[7, 2]);
        }
    }

    private void SearchMovePointOfPiece1(int column, int row, PlayerType player)
    {
        if (player == PlayerType.Player1)
        {
            if (CanPut(column + 1, row)) ObserveCells(cells[column + 1, row]);
        }
        else if (player == PlayerType.Player2)
        {
            if (CanPut(column - 1, row)) ObserveCells(cells[column - 1, row]);
        }
    }
    private void SearchMovePointOfPiece2(int column, int row)
    {
        if (CanPut(column + 1, row)) ObserveCells(cells[column + 1, row]);
        if (CanPut(column - 1, row)) ObserveCells(cells[column - 1, row]);
        if (CanPut(column, row + 1)) ObserveCells(cells[column, row + 1]);
        if (CanPut(column, row - 1)) ObserveCells(cells[column, row - 1]);
    }
    private void SearchMovePointOfPiece3(int column, int row)
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
    private void SearchMovePointOfPiece4(int column, int row)
    {
        if (CanPut(column + 1, row + 1)) ObserveCells(cells[column + 1, row + 1]);
        if (CanPut(column + 1, row - 1)) ObserveCells(cells[column + 1, row - 1]);
        if (CanPut(column - 1, row + 1)) ObserveCells(cells[column - 1, row + 1]);
        if (CanPut(column - 1, row - 1)) ObserveCells(cells[column - 1, row - 1]);
    }
    private void SearchMovePointOfPiece5(int column, int row)
    {
        if (CanPut(column + 1, row + 1)) ObserveCells(cells[column + 1, row + 1]);
        if (CanPut(column + 1, row - 1)) ObserveCells(cells[column + 1, row - 1]);
        if (CanPut(column + 1, row)) ObserveCells(cells[column + 1, row]);
        if (CanPut(column - 1, row + 1)) ObserveCells(cells[column - 1, row + 1]);
        if (CanPut(column - 1, row - 1)) ObserveCells(cells[column - 1, row - 1]);
        if (CanPut(column - 1, row)) ObserveCells(cells[column - 1, row]);
        if (CanPut(column, row + 1)) ObserveCells(cells[column, row + 1]);
        if (CanPut(column, row - 1)) ObserveCells(cells[column, row - 1]);
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
        //cell.ChangeToPutted();

        playerAction.NextColumn = cell.Column;
        playerAction.NextRow = cell.Row;
        playerAction.Action = PieceAction.Move;

        MessageBroker.Default.Publish(playerAction);
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

    public List<CellHandler> ReturnPuttedCellList()
    {
        puttedCellList.Clear();
        foreach(CellHandler cell in cells)
        {
            if(cell.CellType == CellType.Putted) puttedCellList.Add(cell);
        }
        return puttedCellList;
    }
}