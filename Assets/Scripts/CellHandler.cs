using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Games.HorseLabyrinth
{
    public enum CellType
    {
        Default,
        Horse,
        Carrot,
        Fall,
        None
    }

    public class CellHandler : MonoBehaviour
    {
        [SerializeField] private Button cellButton;
        [SerializeField] private GameObject defaultCell, horseCell, markerCell, carrotCell, fallCell;

        private int row;
        private int column;
        private float cellSizeRate;
        private int columnSize;
        private CellType cellType;
        private Sequence sequence;

        public Button CellButton
        {
            get { return cellButton; }
        }

        public CellType CellType
        {
            get { return cellType; }
        }

        public int Row
        {
            get { return row; }
        }

        public int Column
        {
            get { return column; }
        }

        public IEnumerator SetCell(int row, int column, string cellType, float cellSizeRate, int columnSize)
        {
            this.row = row;
            this.column = column;
            this.cellSizeRate = cellSizeRate;
            this.columnSize = columnSize;
            this.gameObject.name = row.ToString() + "-" + column.ToString();
            SetType(cellType);
            yield return null;
        }

        private void SetType(string type)
        {
            switch (type)
            {
                case "0": // None
                    cellType = CellType.None;
                    break;

                case "1": // DefaultCell
                    cellType = CellType.Default;
                    ActivateCell(defaultCell);
                    break;

                case "s": // Start
                    cellType = CellType.Horse;
                    ActivateCell(horseCell);
                    break;

                case "g": // Goal
                    cellType = CellType.Carrot;
                    SetCellImageSize(carrotCell);
                    ActivateCell(defaultCell);
                    ActivateCell(carrotCell);
                    break;
            }

            ChangeButtonInteractable(false);
            SetHorseCellSize();
            SetCellImageSize(markerCell);
        }

        private void SetHorseCellSize()
        {
            float widthSize = 252 * cellSizeRate;
            float heightSize = 308 * cellSizeRate;
            horseCell.GetComponent<RectTransform>().sizeDelta = new Vector3(widthSize, heightSize);
        }

        private void SetCellImageSize(GameObject cell)
        {
            float newSize = 252 * cellSizeRate;
            int buffer = 40 - (5 * (columnSize - 3));
            cell.GetComponent<RectTransform>().sizeDelta = new Vector3(newSize, newSize);
            cell.GetComponent<RectTransform>().localPosition = new Vector3(0, buffer);

            var image = cell.GetComponentInChildren<Image>(true).gameObject;
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(newSize, newSize);
        }

        private void ActivateCell(GameObject cell)
        {
            cell.SetActive(true);
        }

        public void ChangeButtonInteractable(bool enable)
        {
            cellButton.interactable = enable;
        }

        public void ChangeAvailableMarker(bool enable)
        {
            markerCell.SetActive(enable);
        }

        public void OnClickedCell()
        {
            if (cellType == CellType.Carrot) return;
            if (sequence != null) sequence.Kill();
            ChangeDefault(horseCell);

            cellType = CellType.Horse;
            defaultCell.SetActive(false);
            fallCell.SetActive(false);
            horseCell.SetActive(true);
        }

        public void ChangeToDefault()
        {
            if (sequence != null) sequence.Kill();
            ChangeDefault(horseCell);

            cellType = CellType.Default;
            defaultCell.SetActive(true);
            horseCell.SetActive(false);
        }

        private void ChangeDefault(GameObject cell)
        {
            cell.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            cell.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            cell.GetComponentInChildren<Image>().DOFade(1f, 0.01f);
        }

        // public IEnumerator SinkCell()
        // {
        //     cellType = CellType.Fall;
        //     RectTransform previousCell = horseCell.GetComponent<RectTransform>();
        //     sequence = DOTween.Sequence()
        //         .Append(previousCell.DOShakePosition(0.4f, 15, 10))
        //         .Append(previousCell.DOScale(new Vector3(0f, 0f, 0f), 0.5f))
        //         .Join(previousCell.GetComponentInChildren<Image>().DOFade(0f, 0.4f))
        //         .InsertCallback(0.6f, () => fallCell.SetActive(true))
        //         .InsertCallback(0.7f, () => horseCell.SetActive(false));

        //     yield return null;
        // }
    }
}