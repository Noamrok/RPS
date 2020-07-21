using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }
    public static void SetLeftPercent(this RectTransform rt, float leftPercent)
    {
        float left = (Screen.width * (leftPercent / 100));
        rt.SetLeft(left);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }
    public static void SetRightPercent(this RectTransform rt, float rightPercent)
    {
        float right = (Screen.width * (rightPercent / 100));
        rt.SetRight(right);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
    public static void SetTopPercent(this RectTransform rt, float topPercent)
    {
        float top = (Screen.height * (topPercent / 100));
        rt.SetTop(top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
    public static void SetBottomPercent(this RectTransform rt, float bottomPercent)
    {
        float bottom = (Screen.height * (bottomPercent / 100));
        rt.SetBottom(bottom);
    }

    public static void SetScale(this RectTransform rt, float scaleX, float scaleY, float scaleZ)       // 3D
    {
        rt.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
    public static void SetScale(this RectTransform rt, float scaleX, float scaleY)                     // 2D
    {
        rt.SetScaleX(scaleX);
        rt.SetScaleY(scaleY);
    }

    public static void SetScaleX(this RectTransform rt, float scaleX)
    {
        rt.SetScale(scaleX, rt.localScale.y, rt.localScale.z);
    }
    public static void SetScaleY(this RectTransform rt, float scaleY)
    {
        rt.SetScale(rt.localScale.x, scaleY, rt.localScale.z);
    }
    public static void SetScaleZ(this RectTransform rt, float scaleZ)
    {
        rt.SetScale(rt.localScale.x, rt.localScale.y, scaleZ);
    }


    // arguments - cartesian 2D coordinate system
    public static void SetLocationPercent(this RectTransform rt, float componentWidth, float componentHeight, float leftLimitPercent, float rightLimitPercent, float bottomLimitPercent, float topLimitPercent, bool saveRatio)
    {
        // Convert to cartesian 2D coordinate system
        // Without these two lines, the coordinate arguments behavior like margins in css.
        rightLimitPercent = 100 - rightLimitPercent;
        topLimitPercent = 100 - topLimitPercent;

        // Convert percent to pixels
        float left = (Screen.width * (leftLimitPercent / 100));
        float right = (Screen.width * (rightLimitPercent / 100));
        float top = (Screen.height * (topLimitPercent / 100));
        float bottom = (Screen.height * (bottomLimitPercent / 100));

        // Calculate the area designated to the component
        float width = Screen.width - left - right;
        float height = Screen.height - top - bottom;

        // Scale adjustment
        if (saveRatio)
        {
            float scale = Math.Min((width / componentWidth), (height / componentHeight));
            rt.SetScale(scale, scale);
        }
        else
        {
            float scaleX = (width / componentWidth);
            float scaleY = (height / componentHeight);
            // float scaleZ = ...         // Just for 3D
            rt.SetScale(scaleX, scaleY);
        }

        // Component Location
        rt.SetLeft(left + ((width - componentWidth) / 2));
        rt.SetRight(right + ((width - componentWidth) / 2));
        rt.SetTop(top + ((height - componentHeight) / 2));
        rt.SetBottom(bottom + ((height - componentHeight) / 2));
    }

    public static void PutCenterScreen(this RectTransform rt, float componentWidth, float componentHeight, float minHorizontalMarginPercent, float minVerticalMarginPercent, bool saveRatio)
    {
        // Arguments check
        if ((minHorizontalMarginPercent < 0 || minHorizontalMarginPercent > 40) || (minVerticalMarginPercent < 0 || minVerticalMarginPercent > 40))
        {
            return;
        }

        // Scale adjustment
        if (saveRatio)
        {
            float scale = Math.Min((Screen.width / componentWidth) * ((100 - minHorizontalMarginPercent) / 100), (Screen.height / componentHeight) * ((100 - minVerticalMarginPercent) / 100));
            rt.SetScale(scale, scale);
        }
        else
        {
            float scaleX = (Screen.width / componentWidth) * ((100 - minHorizontalMarginPercent) / 100);
            float scaleY = (Screen.height / componentHeight) * ((100 - minVerticalMarginPercent) / 100);
            // float scaleZ = ...         // Just for 3D
            rt.SetScale(scaleX, scaleY);
        }

        // Component Location
        rt.SetLeft((Screen.width - componentWidth) / 2);
        rt.SetRight((Screen.width - componentWidth) / 2);
        rt.SetTop((Screen.height - componentHeight) / 2);
        rt.SetBottom((Screen.height - componentHeight) / 2);
    }
    // HorizontalMarginPercent = VerticalMarginPercent  ->  marginPercent          // Auto margin
    public static void PutCenterScreen(this RectTransform rt, float componentWidth, float componentHeight, float marginPercent)
    {
        rt.PutCenterScreen(componentWidth, componentHeight, marginPercent, marginPercent, true);
    }
}
