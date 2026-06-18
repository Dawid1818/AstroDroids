using Microsoft.Xna.Framework;

namespace AstroDroids.Extensions
{
    public static class MathHelperEx
    {
        public static float LerpAngle(float currentAngle, float targetAngle, float t)
        {
            float difference = targetAngle - currentAngle;

            while (difference < -MathHelper.Pi)
                difference += MathHelper.TwoPi;
            while (difference > MathHelper.Pi)
                difference -= MathHelper.TwoPi;

            return currentAngle + difference * t;
        }
    }
}
