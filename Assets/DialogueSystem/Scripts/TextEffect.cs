using System;
using UnityEngine;
using AntoineFoucault.Utilities;

namespace DialogueSystem
{
    [Serializable]
    public struct TextEffect
    {
        public VisualExtensions.ColorSquare Colors;

        [Header("Typewriter Params")]
        public float ScaleTime;
        public VectorExtensions.Vector3AnimationCurve ScaleAnimations;

        public TextEffect(VisualExtensions.ColorSquare colors, float scaleTime, VectorExtensions.Vector3AnimationCurve scaleAnimations)
        {
            Colors = colors;
            ScaleTime = scaleTime;
            ScaleAnimations = scaleAnimations;
        }

        public Vector3 GetCurrentScale(float timer)
        {
            float percentile = timer / ScaleTime;
            float x = ScaleAnimations.x.Evaluate(percentile);
            float y = ScaleAnimations.y.Evaluate(percentile);
            float z = ScaleAnimations.z.Evaluate(percentile);
            Vector3 targetScale = new Vector3(x, y, z);
            return targetScale;
        }

        public Vector3 ReturnScaledPosition(Vector3 position, Vector3 scale, Vector3 center)
        {
            Vector3 direction = (position - center);
            return Vector3.Scale(direction, scale);
        }

        public Vector3 ReturnAddedScaledPosition(Vector3 position, Vector3 scale, Vector3 center)
        {
            Vector3 direction = (position - center);
            return Vector3.Scale(direction, scale - Vector3.one);
        }

    }

    [Serializable]
    public struct DisplacementParams
    {
        public enum MathFunction
        {
            COS = 0,
            SIN = 1,
            TAN = 2,
            NONE = 3,
        }

        [Header("Base Params")]
        public MathFunction Function;
        public float TimeAmount;
        public Vector3 AddedOriginAmount;
        public VectorExtensions.Vector3MinMaxRange AddedRandomAmount;
        public float GlobalMultiplier;

        public DisplacementParams(MathFunction function, float time, Vector3 originAdd, float globalMultiply, VectorExtensions.Vector3MinMaxRange randomAdd)
        {
            Function = function;
            TimeAmount = time;
            AddedOriginAmount = originAdd;
            AddedRandomAmount = randomAdd;
            GlobalMultiplier = globalMultiply;
        }

        public float GetTotalValue(Vector3 origin)
        {
            float addedOrigin = origin.x * AddedOriginAmount.x + origin.y * AddedOriginAmount.y + origin.z * AddedOriginAmount.z;
            Vector3 randomRange = AddedRandomAmount.GetRandomRange();
            float addedRandom = randomRange.x + randomRange.y + randomRange.z;

            float value = Time.time * TimeAmount;
            value += addedOrigin;
            value += addedRandom;
            
            if (Function == MathFunction.COS) value = Mathf.Cos(value);
            else if (Function == MathFunction.SIN) value = Mathf.Sin(value);
            else if (Function == MathFunction.TAN) value = Mathf.Tan(value);

            value *= GlobalMultiplier;

            return value;
        }
    }

    [Serializable]
    public struct Vector3Displacement
    {
        public DisplacementParams x;
        public DisplacementParams y;
        public DisplacementParams z;

        public Vector3Displacement(DisplacementParams x, DisplacementParams y, DisplacementParams z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3 GetTotalFunction(Vector3 origin)
        {
            return new Vector3(x.GetTotalValue(origin), y.GetTotalValue(origin), z.GetTotalValue(origin));
        }
    }
}
