using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum CellType
{
    Default,
    Putted,
    Maker,
    King
}

public class CellHandler : MonoBehaviour
{
    [SerializeField] private Button cellButton;
    [SerializeField] private GameObject defaultCell, puttedCell, markerCell;

    private int row;
    private int column;
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

    public IEnumerator SetCell(int column, int row, CellType cellType)
    {
        this.column = column;
        this.row = row;
        this.gameObject.name = row.ToString() + "-" + column.ToString();
        this.cellType = cellType;
        ChangeButtonInteractable(false);
        yield return null;
    }

    public void ChangeButtonInteractable(bool enable)
    {
        cellButton.interactable = enable;
    }

    public void ChangeAvailableMarker(bool enable)
    {
        markerCell.SetActive(enable);
    }

    public void ChangeToPutted()
    {
        if (cellType == CellType.Maker) return;
        if (sequence != null) sequence.Kill();
        //ChangeDefault(puttedCell);

        cellType = CellType.Putted;
        defaultCell.SetActive(false);
        puttedCell.SetActive(true);
    }

    public void ChangeToDefault()
    {
        if (sequence != null) sequence.Kill();
        //ChangeDefault(puttedCell);

        cellType = CellType.Default;
        defaultCell.SetActive(true);
        puttedCell.SetActive(false);
    }

    // private void ChangeDefault(GameObject cell)
    // {
    //     cell.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    //     cell.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
    //     cell.GetComponentInChildren<Image>().DOFade(1f, 0.01f);
    // }

    public void SetKing()
    {
        cellType = CellType.King;
    }
}
