using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flag : BasePiece
{
    public override void Setup(Color newTeamColor, Color newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Flag stuff
        mMovement = new Vector3Int(0, 0, 0);
        mDefaultSprite = Resources.Load<Sprite>("T_Flag");
        GetComponent<Image>().sprite = mDefaultSprite;
    }

    public override void Kill()
    {
        base.Kill();

        mPieceManager.mIsFlagAlive = false;
    }
}
