using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rock : BasePiece
{
    public override void Setup(Color newTeamColor, Color newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Rock stuff
        mMovement = new Vector3Int(1, 1, 0);
        mDefaultSprite = Resources.Load<Sprite>("T_Rock");
        GetComponent<Image>().sprite = mDefaultSprite;
    }
}
