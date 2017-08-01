namespace SGDWithCocos.Utilities
{
    public static class AnimationTools
    {
        public static CocosSharp.CCSequence iconAnimationRotate = new CocosSharp.CCSequence(new CocosSharp.CCRotateTo(0.1f, 5f),
            new CocosSharp.CCRotateTo(0.2f, -10f), new CocosSharp.CCRotateTo(0.2f, 10f),
            new CocosSharp.CCRotateTo(0.2f, -10f), new CocosSharp.CCRotateTo(0.2f, 10f),
            new CocosSharp.CCRotateTo(0.1f, 0f));

    }
}
