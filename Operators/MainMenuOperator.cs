namespace KSP_PostProcessing.Operators
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public sealed class MainMenuOperator : PostProcessingOperator
    {
        protected override void Process()
        {
            KS3P.currentScene = KS3P.Scene.MainMenu;
            Patch(false, KS3P.Scene.MainMenu);
        }
    }
}
