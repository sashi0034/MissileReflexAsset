﻿#nullable enable

using UnityEngine;

namespace MissileReflex.Src.Utils
{
    public static class Logger
    {
        public static void Print(string text)
        {
            var showingText = fixText(text);
            UnityEngine.Debug.Log(showingText);
            LogCanvas.Instance?.Print(showingText);
        }
        
        private static string fixText(string text)
        {
            return $"[{Time.frameCount}] {text}";
        }
    }
}