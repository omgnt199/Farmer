using System.Collections.Generic;
using UnityEngine;
public class GameUIOverlay : MonoBehaviour, IOverlayGameUI
{
    protected bool isOverlay = false;
    public static List<GameUIOverlay> gameUIOverlays = new List<GameUIOverlay>();

    public static bool IsGameUIOverlay
    {
        get
        {
            if (gameUIOverlays.Count == 0) return false;
            foreach (var uiOverlay in gameUIOverlays)
            {
                if (uiOverlay.IsOverlayGameUI()) return true;
            }
            return false;
        }
    }
    void Awake()
    {
        gameUIOverlays.Add(this);
    }
    void OnDestroy()
    {
        gameUIOverlays.Remove(this);
    }
    void OnEnable()
    {
        isOverlay = true;
    }
    void OnDisable()
    {
        isOverlay = false;
    }
    public bool IsOverlayGameUI()
    {
        return isOverlay;
    }
}