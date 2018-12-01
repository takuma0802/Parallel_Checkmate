using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Games.HorseLabyrinth
{
    public class BoardManager : MonoBehaviour
    {
        public int columnSize = 3;
        public int rowSize = 8;

        [SerializeField] private GameObject cellPrefab;

        // Size周り
        private Dictionary<int, float> cellSizeRateDict = new Dictionary<int, float>(){
			// key: column count, value: cell size Rate
            {3, 1f}, {4, 0.79f}, {5, 0.68f}, {6, 0.61f}, {7, 0.53f}};
        private float defaultCellSizeWidth = 252f;
        private float defaultCellSizeHeight = 308f;
        private float cellSizeWidth;
        private float cellSizeHeight;

        // Horse周り
        private int horseRow;
        private int horseColumn;
        private Vector3 horsePosition = Vector3.zero;

        // Cell周り
        private CellClickedCallback cellClickedEvent;
        private CellHandler[,] cells;
        private List<CellHandler> previousCells = new List<CellHandler>();
        private List<CellHandler> observedCells = new List<CellHandler>();

        public float CellSizeRate
        {
            get { return cellSizeRateDict[columnSize]; }
        }

        public int CoulumnSize
        {
            get { return columnSize; }
        }

        public List<CellHandler> PreviousCells
        {
            get { return previousCells; }
        }

        public void ResetBoard()
        {
            previousCells.Clear();
            observedCells.Clear();
            cellClickedEvent = null;

            if (transform.childCount == 0) return;
            ObjectCreator.DestroyAllChild(this.gameObject);
        }

        public IEnumerator CreateBoard()
        {
            // 初期化
            cells = new CellHandler[columnSize, rowSize];
            cellSizeWidth = defaultCellSizeWidth * CellSizeRate;
            cellSizeHeight = defaultCellSizeHeight * CellSizeRate;
            var spacing = -70 * CellSizeRate;

            GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSizeWidth, cellSizeHeight);
            GetComponent<GridLayoutGroup>().spacing = new Vector2(0, spacing);
            GetComponent<GridLayoutGroup>().constraintCount = columnSize;

            for (int row = 0; row < rowSize; row++)
            {
                for (int column = 0; column < columnSize; column++)
                {
                    StartCoroutine(CreateCell(CellType.None, row, column));
                }
            }
            yield return null;
        }

        private IEnumerator CreateCell(CellType cellType, int row, int column)
        {
            CellHandler cell = ObjectCreator.CreateInObject(this.gameObject, cellPrefab).GetComponent<CellHandler>();
            yield return cell.SetCell(row, column, cellType, CellSizeRate, columnSize);
            cells[row, column] = cell;

            if (cell.CellType == CellType.Horse)
            {
                previousCells.Add(cell);
                horseRow = row;
                horseColumn = column;
                horsePosition = cell.gameObject.GetComponent<RectTransform>().localPosition;
            }
            yield return null;
        }

        //// Answering
        //置ける場所はMax8箇所なので、8箇所全て調べる
        public IEnumerator SearchMovePoint()
        {
            if (CanPut(horseRow + 2, horseColumn - 1)) ObserveCells(cells[horseRow + 2, horseColumn - 1]);
            if (CanPut(horseRow + 2, horseColumn + 1)) ObserveCells(cells[horseRow + 2, horseColumn + 1]);
            if (CanPut(horseRow - 2, horseColumn - 1)) ObserveCells(cells[horseRow - 2, horseColumn - 1]);
            if (CanPut(horseRow - 2, horseColumn + 1)) ObserveCells(cells[horseRow - 2, horseColumn + 1]);
            if (CanPut(horseRow + 1, horseColumn + 2)) ObserveCells(cells[horseRow + 1, horseColumn + 2]);
            if (CanPut(horseRow - 1, horseColumn + 2)) ObserveCells(cells[horseRow - 1, horseColumn + 2]);
            if (CanPut(horseRow + 1, horseColumn - 2)) ObserveCells(cells[horseRow + 1, horseColumn - 2]);
            if (CanPut(horseRow - 1, horseColumn - 2)) ObserveCells(cells[horseRow - 1, horseColumn - 2]);
            yield return null;
        }

        // 指定したCellに駒を置ける状況かどうかを判定する
        public bool CanPut(int row, int column)
        {
            var isInside = 0 <= row && row < columnSize && 0 <= column && column < columnSize;
            if (!isInside) return false;

            var canPut = cells[row, column].CellType == CellType.Default || cells[row, column].CellType == CellType.Carrot;
            return canPut;
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

            previousCells.Add(cell);
            horseRow = cell.Row;
            horseColumn = cell.Column;
            horsePosition = cell.gameObject.transform.localPosition;
            cellClickedEvent.Invoke(horsePosition, false);
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

        public bool CheckAnswer()
        {
            var cell = previousCells.LastOrDefault();
            if (!cell) return false;

            return cell.CellType == CellType.Carrot;
        }

        public void Undo()
        {
            if (previousCells.Count < 2) return;
            var preCell = previousCells[previousCells.Count - 2];

            RemoveAllBtnAction();
            previousCells[previousCells.Count - 1].ChangeToDefault();
            previousCells.RemoveAt(previousCells.Count - 1);
            horseRow = preCell.Row;
            horseColumn = preCell.Column;
            horsePosition = preCell.gameObject.transform.localPosition;
            cellClickedEvent.Invoke(horsePosition, true);
        }
    }
}