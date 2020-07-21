using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CellState
{
    None,
    Friendly,
    Enemy,
    Free,
    OutOfBounds
}

public class Board : MonoBehaviour
{
    public GameObject mCellOrefab;

    [HideInInspector]
    public Cell[,] mAllCells = new Cell[7, 6];

    // Create the board
    public void Create()
    {
        #region Location
        // Must be sync with class PiceManager, function Setup(...), #region Location.
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetLocationPercent(700, 600, 20, 80, 0, 100, true);
        #endregion

        #region Create
        for (int x=0; x<7; x++)
        {
            for (int y=0; y<6; y++)
            {
                // Create the cell
                GameObject newCell = Instantiate(mCellOrefab, transform);

                // Position
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((x * 100) + 50, (y * 100) + 50);

                // Setup
                mAllCells[x, y] = newCell.GetComponent<Cell>();
                mAllCells[x, y].Setup(new Vector2Int(x, y), this);
            }
        }
        #endregion

        #region Color
        for (int x = 0; x < 7; x++) 
        {
            for (int y = 0; y < 6; y++) 
            {
                if ((x + y) % 2 == 0) 
                    mAllCells[x, y].GetComponent<Image>().color = new Color32(230, 220, 186, 255);

                else    // Optional - the color can be set by default (defined in Unity).
                    mAllCells[x, y].GetComponent<Image>().color = new Color32(202, 167, 132, 255);
            }
        }
        #endregion

    }

    public CellState ValidateCell(int targetX, int targetY, BasePiece checkingPiece)
    {
        // Bounds check
        if (targetX < 0 || targetX > 6) 
            return CellState.OutOfBounds;

        if (targetY < 0 || targetY > 5)
            return CellState.OutOfBounds;

        // Get cell
        Cell targetCell = mAllCells[targetX, targetY];

        // If the cell has a piece
        if (targetCell.mCurrentPiece != null)
        {
            // If friendly
            if (checkingPiece.mColor == targetCell.mCurrentPiece.mColor)
                return CellState.Friendly;

            // If enemy
            if (checkingPiece.mColor != targetCell.mCurrentPiece.mColor)
                return CellState.Enemy;
        }

        return CellState.Free;
    }
}
