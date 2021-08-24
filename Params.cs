using System;
using System.Collections.Generic;

namespace aTTH
{
    public static class Params
    {
        public static float _scale = 1.0f;
        public const int _internalResolutionWidth = 150;
        public const int _internalResolutionHeight = 90;
        public static bool _gamepadUsed = true;
        public static float _movementDeadzone = 0.25f;
        public static float _lookingDeadzone = 0.05f; //my gamepad is quite bad
        public static void _calculateScale(int width)
        {
            _scale = (float)width / _internalResolutionWidth;
        }
    }
}
