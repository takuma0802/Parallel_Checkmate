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
    [SerializeField] private GameObject defaultCell, puttedCell, markerCell, attackCell;

    private int row;
    private int column;
    private CellType cellType;

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

    public void ChangeToDefault()
    {
        cellType = CellType.Default;
        defaultCell.SetActive(true);
        puttedCell.SetActive(false);
    }

    public void AttackAnimation()
    {
        //attackCell.SetActive(true);
        Sound.LoadSe("10","10_komakougeki");
        Sound.PlaySe("10");
    }

    public void SetKing()
    {
        cellType = CellType.King;
    }
}