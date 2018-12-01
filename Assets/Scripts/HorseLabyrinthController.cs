using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Games.HorseLabyrinth
{
    // カスタムイベント
    // Vector3：移動するHorseの目的地/ bool：Undoが押されたかどうか(Undoのときはtrue)
    public class CellClickedCallback : UnityEvent<Vector3, bool> { }
    public class HorseLabyrinthController : MonoBehaviour
    {
        public CellClickedCallback cellClickedEvent = new CellClickedCallback();

        [SerializeField] private BoardManager boardManager;
        [SerializeField] private GameObject horsePrefab;
        [SerializeField] private Button undoButton;
        private HorseManager horseManager;

        // public override void LoadQuestion(Dictionary<string, object> question)
        // {
        //     base.LoadQuestion(question);

        //     ResetAllElement();
        //     StartCoroutine(CreateBoard());
        // }

        private void ResetAllElement()
        {
            boardManager.ResetBoard();
            cellClickedEvent.RemoveAllListeners();
            if (horseManager) horseManager.DestroyHorse();
        }

        // private IEnumerator CreateBoard()
        // {
        //     cellClickedEvent.AddListener(OnCellClicked);
        //     List<object> boardList = nextQuestion["board"] as List<object>;
        //     var board = ToStringFromJson(boardList);

        //     Vector3 horsePos = new Vector3(0, 0, 0);
        //     yield return boardManager.CreateBoard(pos => horsePos = pos, cellClickedEvent, board);
        //     yield return CreateHorse(horsePos);
        //     StartCoroutine(Search());
        // }

        // JsonのListから、カンマで区切られたString型配列を返す
        private string[] ToStringFromJson(IList<object> boardList)
        {
            string boardOfString = "";

            foreach (object i in boardList)
            {
                boardOfString += Convert.ToString(i) + ',';
            }
            boardOfString = boardOfString.TrimEnd(',');
            string[] board = boardOfString.Split(',');
            return board;
        }

        // private IEnumerator CreateHorse(Vector3 startPos)
        // {
        //     var canvas = GetComponentInChildren<Canvas>().gameObject;
        //     horseManager = ObjectCreator.CreateInObject(canvas, horsePrefab).GetComponent<HorseManager>();

        //     yield return horseManager.SetHorse(startPos, boardManager.CellSizeRate, boardManager.CoulumnSize);
        // }

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

        private void OnCellClicked(Vector3 targetPos, bool isUndo)
        {
            undoButton.interactable = false;
            StartCoroutine(CheckAnswer(targetPos, isUndo));
        }

        private IEnumerator CheckAnswer(Vector3 targetPos, bool isUndo)
        {
            yield return horseManager.MoveHorse(targetPos, isUndo);
            var correct = boardManager.CheckAnswer();
            if (correct)
            {
                // OnClickAnswer();
                // yield return SubmitAnswer(correct, null);
            }
            else
            {
                yield return boardManager.SinkCell(isUndo);
                StartCoroutine(Search());
            }
        }

        public void OnClickUndoButton()
        {
            undoButton.interactable = false;
            boardManager.Undo();
        }
    }
}
