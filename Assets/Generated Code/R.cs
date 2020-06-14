namespace ABXY.AssetLink
{
    public class R
    {
        [System.Flags]
        public enum BikeComponentTypes { Nothing = 0, Everything = ~0, FrontWheel = 1<<0, HandleBars = 1<<1, Pedals = 1<<2, RearWheel = 1<<3, Seat = 1<<4};
        /// <summary>The time in seconds of each game</summary>
        public static  readonly float Game_Time = 160f;
        public class Character_Controller_Properties
        {
            /// <summary>The player movement speed</summary>
            public static  readonly float Player_Speed = 0.125f;
            public static  readonly float GrabRadius = 1f;
        }
    }
}
