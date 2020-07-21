using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class DrawMessage : MonoBehaviour
{
    private BasePiece mBasePiece = null;
    private string mType = null;
    public int mDrawCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetLocationPercent(1280, 780, 25, 75, 20, 80, true);
    }

    // Function for receiving the selected value from the message window
    public string getType()
    {
        return mType;
    }

    // Function for saving the selected character from the message window
    public void setType(string type)
    {
        mType = type;
        mBasePiece.MoveAfterDraw();
    }

    public void showMessage(BasePiece basePiece)
    {
        mDrawCounter++;
        mBasePiece = basePiece;
        GetComponent<Canvas>().enabled = true;
    }

    public int getDrawCounter()
    {
        return mDrawCounter;
    }

    public void resetDrawCounter()
    {
        mDrawCounter = 0;
    }
}
