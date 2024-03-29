﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Server
{
    public static class Events
    {
        public static void Init()
        {
            GameMode.RegisterEventHandler("RegisterCoords", new Action<string, string, string, string, string>(SaveCoords));
        }

        private static void SaveCoords(string coordName, string posx, string posy, string posz, string heading)
        {
            StreamWriter coordsFile;
            if (!File.Exists("SavedCoords.txt"))
            {
                coordsFile = new StreamWriter("SavedCoords.txt");
            }
            else
            {
                coordsFile = File.AppendText("SavedCoords.txt");
            }
            var data = $"| {coordName} | pos: new Vector3({posx}f,{posy}f,{posz}f); rot: new Vector3(0,0,{heading}f);";
            Logger.Info(data);
            coordsFile.WriteLine(data);
            coordsFile.Close();
        }
    }
}
