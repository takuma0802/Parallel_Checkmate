using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum CellType
{
    Default,
    Selected,
    Maker
}

public class CellHandler : MonoBehaviour
{
    [SerializeField] private Button cellButton;
    [SerializeField] private GameObject defaultCell, selectedCell, markerCell;

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

    public IEnumerator SetCell(int row, int column, CellType cellType, float cellSizeRate, int columnSize)
    {
        this.row = row;
        this.column = column;
        this.cellSizeRate = cellSizeRate;
        this.columnSize = columnSize;
        this.gameObject.name = row.ToString() + "-" + column.ToString();
        this.cellType = cellType;
        SetType(cellType);
        yield return null;
    }

    private void SetType(CellType type)
    {
        switch (type)
        {
            case CellType.Default: // Default
                break;
            case CellType.Selected: // Selected
                break;
            case CellType.Maker: // Marker
                break;
        }

        ChangeButtonInteractable(false);
        SetCellImageSize(markerCell);
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
        if (cellType == CellType.Maker) return;
        if (sequence != null) sequence.Kill();
        ChangeDefault(selectedCell);

        cellType = CellType.Selected;
        defaultCell.SetActive(false);
        selectedCell.SetActive(true);
    }

    public void ChangeToDefault()
    {
        if (sequence != null) sequence.Kill();
        ChangeDefault(selectedCell);

        cellType = CellType.Default;
        defaultCell.SetActive(true);
        selectedCell.SetActive(false);
    }

    private void ChangeDefault(GameObject cell)
    {
        cell.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        cell.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        cell.GetComponentInChildren<Image>().DOFade(1f, 0.01f);
    }
}
