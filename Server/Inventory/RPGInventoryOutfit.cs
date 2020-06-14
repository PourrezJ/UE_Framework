﻿using Newtonsoft.Json;
namespace UE_Server
{
    public class RPGOutfitSlots
    {
        public int position;
        public string name;
        [JsonProperty("class")]
        public string classe;
        public bool dataDrop;

        public RPGOutfitSlots(int position, string name, string classe, bool dataDrop)
        {
            this.position = position;
            this.name = name;
            this.classe = classe;
            this.dataDrop = dataDrop;
        }
    }
}
