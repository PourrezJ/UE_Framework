namespace UE_Shared
{
    public enum Sex
    {
        Male,
        Girl,
        Other
    }

    public class Identite
    {
        public Sex Gender = Sex.Male;
        public string LastName = "Doe";
        public string FirstName = "John";
        public int Age = 18;
        public string Nationality = "Americaine";
    }

    public class HeadOverlay
    {
        /*
            0               Blemishes             0 - 23,   255  
            1               Facial Hair           0 - 28,   255  
            2               Eyebrows              0 - 33,   255  
            3               Ageing                0 - 14,   255  
            4               Makeup                0 - 74,   255  
            5               Blush                 0 - 6,    255  
            6               Complexion            0 - 11,   255  
            7               Sun Damage            0 - 10,   255  
            8               Lipstick              0 - 9,    255  
            9               Moles/Freckles        0 - 17,   255  
            10              Chest Hair            0 - 16,   255  
            11              Body Blemishes        0 - 11,   255  
            12              Add Body Blemishes    0 - 1,    255  
            */
        public int Index = 255;
        public float Opacity = 1.0f;
        public int Color = 0;
        public int SecondaryColor = 0;
    }

    public class HeadBlendData
    {
        public byte ShapeFirst;
        public byte ShapeSecond;
        public byte ShapeThird;
        public byte SkinFirst;
        public byte SkinSecond;
        public byte SkinThird;
        public float ShapeMix = 0.5f;
        public float SkinMix = 0.5f;
        public float ThirdMix;
    }

    public class PedCharacter
    {  
        public Identite Identite;
        public HeadBlendData HeadBlendData;

        public HeadOverlay[] HeadOverlay;

        public PedCharacter()
        {
            Identite = new Identite();
            HeadBlendData = new HeadBlendData();

            HeadOverlay = new HeadOverlay[12];
            for (int i = 0; i < HeadOverlay.Length; i++)
                HeadOverlay[i] = new HeadOverlay();
        }
    }
}
