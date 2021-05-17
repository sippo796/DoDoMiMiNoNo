namespace Dominos.AppUtil
{
    public class Defines
    {
        public static readonly string StageTag = "Stage";
        public static readonly string DominoTag = "Domino";
        public static readonly string SelectedDominoTag = "SelectedDomino";
        public static readonly string StartDominoTag = "StartDomino";
        public static readonly string EraserTag = "Eraser";
        public static readonly string TapMarginTab = "TapMargin";

        public static readonly string DefaultLayerName = "Default";
        public static readonly string StageLayerName = "Stage";
        public static readonly string CollisionDominoLayerName = "CollisionDomino";
        public static readonly string UILayerName = "UI";

        public static readonly float DominoRotateSpeed = 20f;
        public static readonly float CmaeraRotateSpeed = 5f;

        public static readonly float CameraXMax = 50f;
        public static readonly float CameraXMin = 0f;

        public static readonly float DominoEraseThreshold = -2f;
        public static readonly float DominoDownThreshold = 65f;

        public static readonly float StickAddForce = 100f;

        public static readonly float DefaultDominoMass = 1f;

        public static readonly float FadeTime = 2f;
        public static readonly float StartTime = 5f;

        public static readonly string TitleSceneName = "TitleScene";
        public static readonly string SelectSceneName = "SelectScene";
        public static readonly string TimeAtackSceneName = "TimeAtackScene";

        public static readonly string TimeAtackStagePath = "Prefabs/Stage/Stage{0:00}";
        public static readonly string UnlimitedStagePath = "Prefabs/Stage/UnlimitedStage{0:00}";

        public static readonly string OpeningSoundName = "Opening";
        public static readonly string GameMusicSoundName = "GameMusic";
        public static readonly string DominoCreateSoundName = "DominoCreate";
        public static readonly string DominoStartSoundName = "DominoStart";
        public static readonly string DominoDeleteSoundName = "DominoDelete";
        public static readonly string ButtonSoundName = "Button";
        public static readonly string ClearSoundName = "Clear";
        public static readonly string DuringSoundName = "During";
        public static readonly string DominoCreateErrorSoundName = "DominoCreateError";

        public static readonly float DuplicateDistance = 0.5f;
    }
}
