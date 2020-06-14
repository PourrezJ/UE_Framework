﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using UE_Client.Utils;
using UE_Shared;
using System;
using System.Drawing;

namespace Resurrection_Client.Helpers
{
    public static class UIHelper
    {
        internal static void DrawTexture(string textureStreamed, string textureName, float x, float y, float width, float height, float rotation, Color color)
        {
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, textureStreamed))
            {
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, textureStreamed, false);
            }
            else
            {
                Function.Call(Hash.DRAW_SPRITE, textureStreamed, textureName, x, y, width, height, rotation, color.R, color.G, color.B, color.A);
            }
        }

        internal static void DrawRect(float fromX, float fromY, float width, float height, Color color)
        {
            Function.Call(Hash.DRAW_RECT, fromX, fromY, width, height, color.R, color.G, color.B, color.A);
        }

        internal static void DrawText(string text, int font, float x, float y, float scaleX, float scaleY, Color color, bool center)
        {
            Function.Call(Hash.SET_TEXT_SCALE, scaleX, scaleY);
            Function.Call((Hash)0x50a41ad966910f03, color.R, color.G, color.B, color.A);
            Function.Call((Hash)0xADA9255D, font);
            Function.Call((Hash)0xd79334a4bb99bad1, Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", text), x, y);
            Function.Call(Hash.SET_TEXT_CENTRE, center);
        }

        internal static void DrawText(string text, Vector2 pos, Color? color = null, float scale = 0.25f,
            bool shadow = false, float shadowOffset = 1f, Alignment alignment = Alignment.Left, Font font = Font.ChaletLondon, bool center = false)
        {
            try
            {
                Function.Call((Hash)0xADA9255D, font);
                //Function.Call(Hash.SET_TEXT_PROPORTIONAL, 0);
                Function.Call(Hash.SET_TEXT_SCALE, scale, scale);
                if (shadow)
                {
                    Function.Call(Hash.SET_TEXT_DROPSHADOW, shadowOffset, 0, 0, 0, 255);
                }
                var col = color ?? Color.FromArgb(255, 255, 255);
                Function.Call((Hash)0x50a41ad966910f03, col.R, col.G, col.B, col.A);
                Function.Call(Hash.SET_TEXT_CENTRE, center);
                /*
                Function.Call(Hash.SET_TEXT_EDGE, 1, 0, 0, 0, 255);
                Function.Call(Hash.SET_TEXT_JUSTIFICATION, alignment);
                Function.Call(Hash._SET_TEXT_ENTRY, "STRING");
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
                Function.Call(Hash._DRAW_TEXT, pos.X, pos.Y);*/
                Function.Call((Hash)0xd79334a4bb99bad1, Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", text), pos.X, pos.Y);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        internal static void DrawText3D(string text, Vector3 pos, Color color, int font = 0)
        {
            float screenX = 0;
            float screenY = 0;

            API.GetScreenCoordFromWorldCoord(pos.X, pos.Y, pos.Z, ref screenX, ref screenY);
            var camCoords = Misc.GetCamDirection();
            var distance = pos.DistanceToSquared(camCoords);
    
            float scale = (4.00001f / distance) * 0.5f;
            if (scale > 0.2)
                scale = 0.2f;
            if (scale < 0.1)
                scale = 0;

            
            var fov = (1 / API.GetGameplayCamFov()) * 100;
            scale = scale * fov;

            UIHelper.DrawText(text, 1, screenX, screenY, scale, scale, Color.FromArgb(255, 255, 255), false);
        }
    }
}
