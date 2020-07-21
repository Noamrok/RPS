using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System;

public class BasePiece : EventTrigger
{
    [HideInInspector]
    public Color mColor = Color.clear;

    protected Cell mOriginalCell = null;
    protected Cell mCurrentCell = null;

    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;

    protected Cell mTargetCell = null;

    protected Vector3Int mMovement = Vector3Int.one;
    protected List<Cell> mHighlightedCells = new List<Cell>();

    protected Sprite mHiddenSprite = null;
    protected Sprite mDefaultSprite = null;

    public virtual void Setup(Color newTeamColor, Color newSpriteColor, PieceManager newPieceManager)
    {
        mPieceManager = newPieceManager;

        mColor = newTeamColor;
        GetComponent<Image>().color = newSpriteColor;
        mRectTransform = GetComponent<RectTransform>();

        // Initializes the soldier's image in hidden mode
        mHiddenSprite = Resources.Load<Sprite>("T_HiddenSoldier");
    }

    public void Place(Cell newCell)
    {
        // Cell stuff
        mCurrentCell = newCell;
        mOriginalCell = newCell;
        mCurrentCell.mCurrentPiece = this;

        // Object stuff
        transform.position = newCell.transform.position;
        gameObject.SetActive(true);

    }
    
    public virtual void Reset()     // Maybe not virtual
    {
        Kill();

        Place(mOriginalCell);
    }

    public virtual void Kill()
    {
        // Clear current cell
        mCurrentCell.mCurrentPiece = null;

        // Remove piece
        gameObject.SetActive(false);
    }

    public virtual void Hide()
    {
        GetComponent<Image>().sprite = mHiddenSprite;
    }

    public virtual void Unhide()
    {
        GetComponent<Image>().sprite = mDefaultSprite;
    }

    #region Movement
    private void CreateCellPath(int xDirection, int yDirection, int movement)
    {
        // Target position
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        // Check each cell
        for (int i = 1; i <= movement; i++)
        {
            currentX += xDirection;
            currentY += yDirection;

            // Get the state of the target cell
            CellState cellState = CellState.None;
            cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY, this);

            // If enemy, add to list, break
            if (cellState == CellState.Enemy)
            {
                mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
                break;
            }

            // If the cell is not free, break
            if (cellState != CellState.Free)
                break;

            //Add to list
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
        }
    }

    protected virtual void CheckPathing()
    {
        // Horizontal
        CreateCellPath(1, 0, mMovement.x);
        CreateCellPath(-1, 0, mMovement.x);

        // Vertical
        CreateCellPath(0, 1, mMovement.y);
        CreateCellPath(0, -1, mMovement.y);

        // Upper diagonal
        CreateCellPath(1, 1, mMovement.z);
        CreateCellPath(-1, 1, mMovement.z);

        // Lower diagonal
        CreateCellPath(-1, -1, mMovement.z);
        CreateCellPath(1, -1, mMovement.z);
    }

    protected void ShowCells()
    {
        foreach (Cell cell in mHighlightedCells)
            cell.mOutlineImage.enabled = true;
    }

    protected void ClearCells()
    {
        foreach (Cell cell in mHighlightedCells)
            cell.mOutlineImage.enabled = false;

        mHighlightedCells.Clear();
    }

    protected virtual void Move()
    {
        // If there is no enemy
        if (mTargetCell.mCurrentPiece is null)
        {
            // Clear current
            mCurrentCell.mCurrentPiece = null;

            // Switch cells
            mCurrentCell = mTargetCell;
            mCurrentCell.mCurrentPiece = this;

            // Move on board
            transform.position = mCurrentCell.transform.position;
            mTargetCell = null;

            // End turn
            mPieceManager.SwitchSides(mColor);

            return;
        }

        // If it's a draw
        if ((mCurrentCell.mCurrentPiece is Rock && mTargetCell.mCurrentPiece is Rock) ||
           (mCurrentCell.mCurrentPiece is Paper && mTargetCell.mCurrentPiece is Paper) ||
           (mCurrentCell.mCurrentPiece is Scissors && mTargetCell.mCurrentPiece is Scissors))
        {
            // Timer disable
            mPieceManager.timer.PauseCount();
            mPieceManager.timer.HideCounter();

            // Eliminating the possibility of moving soldiers
            mPieceManager.SetInteractiove(mPieceManager.mWhitePieces, false);
            mPieceManager.SetInteractiove(mPieceManager.mBlackPieces, false);

            // Choosing a new type message
            mPieceManager.drawMessage.showMessage(this);

            // Disable exit room button after draw window opens.
            mPieceManager.exitRoomButton.interactable = false;

            return;
        }

        // Unhide soldiers
        mTargetCell.mCurrentPiece.Unhide();
        mCurrentCell.mCurrentPiece.Unhide();

        // If the attacker loses
        if (mTargetCell.mCurrentPiece is Trap ||
           (mCurrentCell.mCurrentPiece is Rock && mTargetCell.mCurrentPiece is Paper) ||
           (mCurrentCell.mCurrentPiece is Paper && mTargetCell.mCurrentPiece is Scissors) ||
           (mCurrentCell.mCurrentPiece is Scissors && mTargetCell.mCurrentPiece is Rock))
        {
            // Remove current
            mCurrentCell.RemovePiece();
        }

        // If the attacker wins
        else
        {
            if (mPieceManager.drawMessage.getDrawCounter() == 0)
            {
                // Remove enemy piece
                mTargetCell.RemovePiece();

                // Switch cells
                mTargetCell.mCurrentPiece = mCurrentCell.mCurrentPiece;

                // Clear current
                mCurrentCell.mCurrentPiece = null;
                mCurrentCell = mTargetCell;

                // Move on board
                transform.position = mCurrentCell.transform.position;
                mTargetCell = null;
            }

            else
            {
                // Remove enemy piece
                mTargetCell.RemovePiece();

                // Switch cells
                mCurrentCell.mCurrentPiece.Place(mTargetCell);

                // Clear current
                mCurrentCell.mCurrentPiece = null;
            }
        }

        mPieceManager.drawMessage.resetDrawCounter();     // This is not a draw
        mPieceManager.timer.ResetTimeoutCounter();
        mPieceManager.SwitchSides(mColor);
    }

    public void OpponentMove(Cell targetCell)
    {
        mTargetCell = targetCell;
        Move();
    }

    public void MoveAfterDraw()
    {
        // Enable exit room button after draw window is closed
        mPieceManager.exitRoomButton.interactable = true;

        // Get my selected type from drawMessage window
        string myChosenType = mPieceManager.drawMessage.getType();

        // Save my type in DB
        DBSaveChosenType(myChosenType);     // StartCoroutine() doing bugs

        // Get opponent selected type from DB
        string enemyChosenType = DBGetChosenType();
        
        // Timer enable
        mPieceManager.timer.ShowCounter();
        mPieceManager.timer.StartCount();

        // Change pieces type
        mPieceManager.ChangePiece(mCurrentCell.mCurrentPiece, mCurrentCell, mCurrentCell.mCurrentPiece.mColor, mCurrentCell.mCurrentPiece.GetComponent<Image>().color, mPieceManager.isBlackTurn == false ? myChosenType : enemyChosenType);      // Change attacker soldier
        mPieceManager.ChangePiece(mTargetCell.mCurrentPiece, mTargetCell, mTargetCell.mCurrentPiece.mColor, mTargetCell.mCurrentPiece.GetComponent<Image>().color, mPieceManager.isBlackTurn == false ? enemyChosenType : myChosenType);         // Change attacked soldier

        Move();
    }
    #endregion

    #region Events
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        // Test for cells
        CheckPathing();

        // Show valid cells
        ShowCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        // Follow pointer
        transform.position += (Vector3)eventData.delta;

        // Check for overlapping available squares
        foreach (Cell cell in mHighlightedCells)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition))
            {
                // If the mouse is within a valid cell, get it, and break.
                mTargetCell = cell;
                break;
            }

            // If the mouse is not within any highlighted cell, we don't have a valid move.
            mTargetCell = null;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // Hide
        ClearCells();

        // Return to priginal position
        if (!mTargetCell)
        {
            transform.position = mCurrentCell.gameObject.transform.position;
            return;
        }

        // Save my move in DB
        DBSaveMove();

        // Move to new cell and end turn
        Move();
    }
    #endregion

    #region DB stuff
    void DBSaveChosenType(string chosenType)
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Board/PostDrawChoice";
        WWWForm form = new WWWForm();
        form.AddField("choice", chosenType);
        form.AddField("username", PlayerPrefs.GetString("Username"));
        form.AddField("playerNum", PlayerPrefs.GetInt("PlayerNumInRoom"));  

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            www.SendWebRequest();
            while (!www.isDone) ;
        }
    }

    private string DBGetChosenType()
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Board/GetDrawChoice/" + PlayerPrefs.GetString("Username") + "/" + PlayerPrefs.GetInt("PlayerNumInRoom");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            webRequest.SendWebRequest();

            while (!webRequest.isDone) ;
            return webRequest.downloadHandler.text.Replace("\"", "");
        }
    }

   void DBSaveMove()
    {
        string moveStr = mCurrentCell.mBoardPosition.x + "," + mCurrentCell.mBoardPosition.y + "," + mTargetCell.mBoardPosition.x + "," + mTargetCell.mBoardPosition.y;

        string uri = "https://rpsnood.azurewebsites.net/api/Board/PostNewMove";
        WWWForm form = new WWWForm();
        form.AddField("lastMove", moveStr);
        form.AddField("username", PlayerPrefs.GetString("Username"));
        form.AddField("playerNum", PlayerPrefs.GetInt("PlayerNumInRoom"));  

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            www.SendWebRequest();
            while (!www.isDone) ;
        }
    }
    #endregion
}
