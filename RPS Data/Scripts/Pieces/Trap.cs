using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trap : BasePiece
{
    public override void Setup(Color newTeamColor, Color newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Trap stuff
        mMovement = new Vector3Int(0, 0, 0);
        mDefaultSprite = Resources.Load<Sprite>("T_Trap");
        GetComponent<Image>().sprite = mDefaultSprite;
    }
}
