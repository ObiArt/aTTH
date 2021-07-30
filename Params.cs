using System;

namespace aTTH
{
    public static class Params
    {
        public static Single _scale = 1.0f;
        public const int _internalResolutionWidth = 200;
        public const int _internalResolutionHeight = 120;

        public static void _calculateScale(int width)
        {
            _scale = (float)width / (float)_internalResolutionWidth;
        }
    }
}
