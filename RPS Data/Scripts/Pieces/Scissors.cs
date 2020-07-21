using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scissors : BasePiece
{
    public override void Setup(Color newTeamColor, Color newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Scissors stuff
        mMovement = new Vector3Int(1, 1, 0);
        mDefaultSprite = Resources.Load<Sprite>("T_Scissors");
        GetComponent<Image>().sprite = mDefaultSprite;
    }
}
