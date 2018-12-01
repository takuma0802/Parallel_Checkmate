using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// カスタムイベント
// Vector3：移動するHorseの目的地/ bool：Undoが押されたかどうか(Undoのときはtrue)
public class CellClickedCallback : UnityEvent<Vector3, bool> { }
public class HorseLabyrinthController : MonoBehaviour
{
    public CellClickedCallback cellClickedEvent = new CellClickedCallback();

    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject horsePrefab;
    [SerializeField] private Button undoButton;

    private IEnumerator CreateBoard()
    {
        //List<object> boardList = nextQuestion["board"] as List<object>;
        //var board = ToStringFromJson(boardList);

        Vector3 horsePos = new Vector3(0, 0, 0);
        //yield return boardManager.CreateBoard(pos => horsePos = pos, cellClickedEvent, board);
        StartCoroutine(Search());
        yield return null;
    }

    private IEnumerator Search()
    {
        yield return boardManager.SearchMovePoint();
        if (boardManager.PreviousCells.Count > 1)
        {
            undoButton.interactable = true;
        }
        else
        {
            undoButton.interactable = false;
        }
    }

    public void OnClickUndoButton()
    {
        undoButton.interactable = false;
        boardManager.Undo();
    }
}

