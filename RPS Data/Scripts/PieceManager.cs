using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PieceManager : MonoBehaviour
{
    [HideInInspector]
    public bool mIsFlagAlive = true; 

    public GameObject mPiecePrefab;

    public Board mBoard = null;

    public bool isBlackTurn;

    public DrawMessage drawMessage;
    public WinnerMessage winnerMessage;
    public Button exitRoomButton;

    public List<BasePiece> mWhitePieces = null;
    public List<BasePiece> mBlackPieces = null;
    private List<BasePiece> mChangedPieces = new List<BasePiece>();

    public CountdownTimer timer;

    private string[] mWhitePieceOrder = new string[14];
    private string[] mBlackPieceOrder = new string[14];

    private Dictionary<string, Type> mPieceLibrary = new Dictionary<string, Type>()
    {
        {"F",  typeof(Flag)},
        {"P",  typeof(Paper)},
        {"R",  typeof(Rock)},
        {"S",  typeof(Scissors)},
        {"T",  typeof(Trap)}
    };

    private string[] randomPieces()
    {
        string[] piecesPlaces = new string[14];
      
        List<int> isTaken = new List<int>();
        for (int i = 0; i < piecesPlaces.Length; i++)
            isTaken.Add(i);
        string[] pieces = new string[3] { "R", "P", "S" };

        // Rand place for the flag
        int randPlaceForFlag = isTaken[UnityEngine.Random.Range(7, isTaken.Count)];   // 7 - place flag in the outer row  ,  0 - place flag in some row
        piecesPlaces[randPlaceForFlag] = "F";
        isTaken.Remove(randPlaceForFlag);

        // Rand place for the trap according to the place of the flag
        int[] optPlacesForTrap;
        switch (randPlaceForFlag)
        {
            // Not necessary - needed only if flag place in some row (value 0)
            /*
            case 0:
                optPlacesForTrap = new int[] { 1, 7 };
                break;
            case 6:
                optPlacesForTrap = new int[] { 5, 13 };
                break;
            */

            case 7:
                optPlacesForTrap = new int[] { 0, 8 };
                break;
            case 13:
                optPlacesForTrap = new int[] { 12, 6 };
                break;
            default:
                // Not necessary - needed only if flag place in some row (value 0)
                /*
                if (randPlaceForFlag > 0 && randPlaceForFlag < 6)
                    optPlacesForTrap = new int[] { randPlaceForFlag + 1, randPlaceForFlag - 1, randPlaceForFlag + 7 };
                else
                */
            
                optPlacesForTrap = new int[] { randPlaceForFlag + 1, randPlaceForFlag - 1, randPlaceForFlag - 7 };
                break;
        }
        int randPlaceForTrap = optPlacesForTrap[UnityEngine.Random.Range(0, optPlacesForTrap.Length)];
        piecesPlaces[randPlaceForTrap] = "T";
        isTaken.Remove(randPlaceForTrap);

        // Random the rest of the pawns: 4 Rock, 4 Paper, 4 Scissors
        int randPlaceForPawn;
        for (int p = 0; p < pieces.Length; p++)
            for (int i = 0; i < 4; i++)
            {
                randPlaceForPawn = isTaken[UnityEngine.Random.Range(0, isTaken.Count)];
                piecesPlaces[randPlaceForPawn] = pieces[p];
                isTaken.Remove(randPlaceForPawn);
            }
        return piecesPlaces;
    }

    public void Setup(Board board)
    {
        #region Location
        // Must be sync with class Board, function Create(), #region Location.
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetLocationPercent(700, 600, 20, 80, 0, 100, true);
        #endregion

        // Save board reference
        mBoard = board;

        // Create pieces order, white and black pieces, and place them
        PiecesSetup();

        // Timer
        timer.mSpriteColor = PlayerPrefs.GetInt("PlayerNumInRoom") == 1 ? mBlackPieces[0].GetComponent<Image>().color : mWhitePieces[0].GetComponent<Image>().color;

        // PlayerNumInRoom 1 takes the first step in the game
        Color firstPlayerToMove = PlayerPrefs.GetInt("PlayerNumInRoom") == 1 ? Color.black : Color.white;
        SwitchSides(firstPlayerToMove);
    }

    private void PiecesSetup()
    {
        // Create my piece order
        mWhitePieceOrder = randomPieces();

        // Save my piece order in DB
        StartCoroutine(DBSavePieceOrder(mWhitePieceOrder));

        // Get opponent piece order from DB
        mBlackPieceOrder = DBGetPieceOrder();

        string temp;
        for (int i = 0; i < 3; i++)
        {
            temp = mBlackPieceOrder[i];
            mBlackPieceOrder[i] = mBlackPieceOrder[6 - i];
            mBlackPieceOrder[6 - i] = temp;
            temp = mBlackPieceOrder[7 + i];
            mBlackPieceOrder[7 + i] = mBlackPieceOrder[13 - i];
            mBlackPieceOrder[13 - i] = temp;
        }

        // Create white pieces
        mWhitePieces = CreatePieces(Color.white, new Color32(80, 124, 159, 255), mBoard, mWhitePieceOrder);

        // Create black pieces
        mBlackPieces = CreatePieces(Color.black, new Color32(210, 95, 64, 255), mBoard, mBlackPieceOrder);

        // Hide opponent soldiers
        HidePieces(mBlackPieces);
        
        // Place pieces
        PlacePieces(1, 0, mWhitePieces, mBoard);
        PlacePieces(4, 5, mBlackPieces, mBoard);
    }

    private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor, Board board, string[] pieceOrder)
    {
        List<BasePiece> newPieces = new List<BasePiece>();

        for (int i=0; i< pieceOrder.Length; i++)
        {     
            // Get the type, apply to new object
            string key = pieceOrder[i];
            Type pieceType = mPieceLibrary[key];

            // Store new piece
            BasePiece newPiece = CreatePiece(pieceType);
            newPieces.Add(newPiece);

            //Setup piece
            newPiece.Setup(teamColor, spriteColor, this);
        }

        return newPieces;
    }

    private BasePiece CreatePiece(Type pieceType)
    {
        // Create new object
        GameObject newPieceObject = Instantiate(mPiecePrefab);
        newPieceObject.transform.SetParent(transform);

        // Set scale and position
        newPieceObject.transform.localScale = new Vector3(1, 1, 1);
        newPieceObject.transform.localRotation = Quaternion.identity;

        BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);

        return newPiece;
    }

    private void PlacePieces(int pawnRow, int royaltyRow, List<BasePiece> pieces, Board board)
    {
        for (int i = 0; i < 7; i++) 
        {
            // Place pawns
            pieces[i].Place(board.mAllCells[i, pawnRow]);

            // Place royalty
            pieces[i + 7].Place(board.mAllCells[i, royaltyRow]);
        }
    }

    public void HidePieces(List<BasePiece> pieces)
    {
        foreach (BasePiece piece in pieces)
            piece.Hide();
    }

    public void UnhidePieces(List<BasePiece> pieces)
    {
        foreach (BasePiece piece in pieces)
            piece.Unhide();
    }

    public void SetInteractiove(List<BasePiece> allPieces, bool value)
    {
        foreach (BasePiece piece in allPieces)
            piece.enabled = value;

        // Set changed piece interactivity
        foreach (BasePiece piece in mChangedPieces)
        {
            if (piece.mColor == allPieces[0].mColor)
                piece.enabled = value;
        }
    }

    public void SwitchSides(Color color)
    {
        if (!mIsFlagAlive)
        {
            #region Back to menu
            // Timer disable
            timer.PauseCount();
            timer.HideCounter();

            // Eliminating the possibility of moving soldiers
            SetInteractiove(mWhitePieces, false);
            SetInteractiove(mBlackPieces, false);

            // Set winner name in the winner message
            if (!isBlackTurn)
                winnerMessage.GetComponentsInChildren<Text>()[1].text = PlayerPrefs.GetString("Username");
            else
                winnerMessage.GetComponentsInChildren<Text>()[1].text = DBGetWinner();

            // Show winner message
            winnerMessage.GetComponent<Canvas>().enabled = true;
            exitRoomButton.interactable = false;

            return;
            #endregion


            #region Start new game
            /*
            // Reset pieces
            ResetPieces();        // Or use ResetPiecesLocation();

            // Flag has risen from dead
            mIsFlagAlive = true;

            // Chance color to black, so white can go first again
            color = Color.black;

            // Timer
            timer.mSpriteColor = mBlackPieces[0].GetComponent<Image>().color;
            */
            #endregion
        }

        isBlackTurn = color == Color.white ? true : false;

        // Opponent Turn to move
        if (isBlackTurn)
        {
            StartCoroutine(DoOpponentMove());
        }

        // Set interactivity
        SetInteractiove(mWhitePieces, !isBlackTurn);

        // Eliminate the possibility of moving the opponent's troops manually
        SetInteractiove(mBlackPieces, false);            // צריך את זה בגלל היצירה של החייל החדש - אפשר להפריד למתודות שונות

        // Timer
        timer.ResetCount();
        timer.mSpriteColor = timer.mSpriteColor == mBlackPieces[0].GetComponent<Image>().color ? mWhitePieces[0].GetComponent<Image>().color : mBlackPieces[0].GetComponent<Image>().color;
        timer.SetColor(timer.mSpriteColor);                  // אפשרי להוסיף שדה בחייל ששומר את הצבע האמיתי שלו
        timer.mColor = color == Color.white ? Color.black : Color.white;
    }

    public void ResetPiecesLocation()
    {
        for (int i = 1; i <= 2; i++)   // This should only be done once, but when doing so twice the flag bug is resolved.
        {
            // Reset changed
            foreach(BasePiece piece in mChangedPieces)
            {
                piece.Kill();
                Destroy(piece.gameObject);
            }
            mChangedPieces.Clear();

            // Reset white
            foreach (BasePiece piece in mWhitePieces)
                piece.Reset();

            // Reset black
            foreach (BasePiece piece in mBlackPieces)
                piece.Reset();

            // Re-hide opponent soldiers
            HidePieces(mBlackPieces);
        }
    }

    public void ResetPieces()
    {
        for (int i = 1; i <= 2; i++)   // This should only be done once, but when doing so twice the flag bug is resolved.
        {
            // Reset changed
            foreach (BasePiece piece in mChangedPieces)
            {
                piece.Kill();
                Destroy(piece.gameObject);
            }
            mChangedPieces.Clear();

            // Reset white
            foreach (BasePiece piece in mWhitePieces)
            {
                piece.Kill();
                Destroy(piece.gameObject);
            }
            mWhitePieces.Clear();

            // Reset black
            foreach (BasePiece piece in mBlackPieces)
            {
                piece.Kill();
                Destroy(piece.gameObject);
            }
            mBlackPieces.Clear();

            PiecesSetup();
        }
    }

    public void ChangePiece(BasePiece piece, Cell cell, Color teamColor, Color spriteColor, String newType)
    {
        // Kill piece
        piece.Kill();

        // Create
        Type pieceType = mPieceLibrary[newType];
        BasePiece changedPiece = CreatePiece(pieceType);
        changedPiece.Setup(teamColor, spriteColor, this);

        // Place
        changedPiece.Place(cell);

        // Add
        mChangedPieces.Add(changedPiece);
    }

    #region DB stuff
    IEnumerator DBSavePieceOrder(string[] pieceOrder)
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Board/PostInitPawns";
        WWWForm form = new WWWForm();
        string strPieceOrder = string.Join(",", pieceOrder);
        form.AddField("initPawnsLoc", strPieceOrder);
        form.AddField("username", PlayerPrefs.GetString("Username"));
        form.AddField("playerNum", PlayerPrefs.GetInt("PlayerNumInRoom"));  

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();
        }
    }

    private string[] DBGetPieceOrder()
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Board/GetInitPawnsLoc/" + PlayerPrefs.GetString("Username") + "/" + PlayerPrefs.GetInt("PlayerNumInRoom");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            webRequest.SendWebRequest();

            while (!webRequest.isDone);
            return JsonHelper.FromJson<string>(JsonHelper.FixJson(webRequest.downloadHandler.text));
        }
    }

    private int[] DBGetLastMove()
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Board/GetLastMove/" + PlayerPrefs.GetString("Username") + "/" + PlayerPrefs.GetInt("PlayerNumInRoom");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            webRequest.SendWebRequest();

            while (!webRequest.isDone) ;
            string[] lastMoveSA = JsonHelper.FromJson<string>(JsonHelper.FixJson(webRequest.downloadHandler.text));

            if(lastMoveSA[0]== "")
                return null;

            int[] lastMoveIA = Array.ConvertAll(lastMoveSA, int.Parse);

            // Indexing Adjustment
            lastMoveIA[0] = 6 - lastMoveIA[0];
            lastMoveIA[1] = 5 - lastMoveIA[1];
            lastMoveIA[2] = 6 - lastMoveIA[2];
            lastMoveIA[3] = 5 - lastMoveIA[3];

            return lastMoveIA;
        }
    }

    IEnumerator DoOpponentMove()
    {
        while (isBlackTurn)
        {
            int[] lastMove = DBGetLastMove();
            if (lastMove == null)
                yield return new WaitForSeconds(0.2f);
            else
            {
                mBoard.mAllCells[lastMove[0], lastMove[1]].mCurrentPiece.OpponentMove(mBoard.mAllCells[lastMove[2], lastMove[3]]);
                break;
            }
        }
    }

    string DBGetWinner()
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Board/GetWinner/" + PlayerPrefs.GetString("Username") + "/" + PlayerPrefs.GetInt("PlayerNumInRoom");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            webRequest.SendWebRequest();

            while (!webRequest.isDone) ;
            return webRequest.downloadHandler.text.Replace("\"", "");
        }
    }
    #endregion
}
