using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Board mBoard;
    public PieceManager mPieceManager;

    void Start()
    {
        //Create the board
        mBoard.Create();

        // Create pieces
        mPieceManager.Setup(mBoard);

    }
}
