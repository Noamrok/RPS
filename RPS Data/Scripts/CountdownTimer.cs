using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    private bool isCounting = false;
    private float currentTime = 0f;
    private float startingTime = 15f;

    [SerializeField] Text countdownText = null;

    public PieceManager mPieceManager;
    public Color mColor;
    public Color mSpriteColor;
    private int whiteTimeoutCounter;
    private int blackTimeoutCounter;

    // Start is called before the first frame update
    void Start()
    {
        // Location
        RectTransform rect = countdownText.GetComponent<RectTransform>();
        rect.SetLocationPercent(100, 100, 80, 100, 40, 60, true);

        // Default
        currentTime = startingTime;
        ShowCounter();
        StartCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCounting)
            return;

        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if (currentTime <= 1)
        {
            ResetCount();
            if (mPieceManager.isBlackTurn)
                blackTimeoutCounter++;
            else
                whiteTimeoutCounter++;
            if (blackTimeoutCounter == 4 || whiteTimeoutCounter == 4)
            {
                mPieceManager.mIsFlagAlive = false;
                mPieceManager.isBlackTurn = !mPieceManager.isBlackTurn;
            }
            mPieceManager.SwitchSides(mColor);
        }
    }

    public void ResetCount()
    {
        currentTime = startingTime;
    }

    public void ResetTimeoutCounter()
    {
        if (mPieceManager.isBlackTurn)
            blackTimeoutCounter = 0;
        else
            whiteTimeoutCounter = 0;
    }

    public void StartCount()
    {
        isCounting = true;
    }

    public void HideCounter()
    {
        countdownText.enabled = false;
    }

    public void ShowCounter()
    {
        countdownText.enabled = true;
    }

    public void PauseCount()
    {
        isCounting = false;
    }

    public void SetColor(Color color)
    {
        countdownText.color = color; 
    }
}
