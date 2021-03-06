﻿using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace Artemis.Core
{
    /// <summary>
    ///     A gradient containing a list of <see cref="ColorGradientStop" />s
    /// </summary>
    public class ColorGradient : CorePropertyChanged
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="ColorGradient" /> class
        /// </summary>
        public ColorGradient()
        {
            Stops = new List<ColorGradientStop>();
        }

        /// <summary>
        ///     Gets a list of all the <see cref="ColorGradientStop" />s in the gradient
        /// </summary>
        public List<ColorGradientStop> Stops { get; }

        /// <summary>
        ///     Gets all the colors in the color gradient
        /// </summary>
        /// <param name="timesToRepeat">The amount of times to repeat the colors</param>
        /// <returns></returns>
        public SKColor[] GetColorsArray(int timesToRepeat = 0)
        {
            if (timesToRepeat == 0)
                return Stops.OrderBy(c => c.Position).Select(c => c.Color).ToArray();

            List<SKColor> colors = Stops.OrderBy(c => c.Position).Select(c => c.Color).ToList();
            List<SKColor> result = new();

            for (int i = 0; i <= timesToRepeat; i++)
                result.AddRange(colors);

            return result.ToArray();
        }

        /// <summary>
        ///     Gets all the positions in the color gradient
        /// </summary>
        /// <param name="timesToRepeat">
        ///     The amount of times to repeat the positions, positions will get squished together and
        ///     always stay between 0.0 and 1.0
        /// </param>
        /// <returns></returns>
        public float[] GetPositionsArray(int timesToRepeat = 0)
        {
            if (timesToRepeat == 0)
                return Stops.OrderBy(c => c.Position).Select(c => c.Position).ToArray();

            // Create stops and a list of divided stops
            List<float> stops = Stops.OrderBy(c => c.Position).Select(c => c.Position / (timesToRepeat + 1)).ToList();
            List<float> result = new();

            // For each repeat cycle, add the base stops to the end result
            for (int i = 0; i <= timesToRepeat; i++)
            {
                List<float> localStops = stops.Select(s => s + result.LastOrDefault()).ToList();
                result.AddRange(localStops);
            }

            return result.ToArray();
        }

        /// <summary>
        ///     Triggers a property changed event of the <see cref="Stops" /> collection
        /// </summary>
        public void OnColorValuesUpdated()
        {
            OnPropertyChanged(nameof(Stops));
        }

        /// <summary>
        ///     Gets a color at any position between 0.0 and 1.0 using interpolation
        /// </summary>
        /// <param name="position">A position between 0.0 and 1.0</param>
        public SKColor GetColor(float position)
        {
            if (!Stops.Any())
                return SKColor.Empty;

            ColorGradientStop[] stops = Stops.OrderBy(x => x.Position).ToArray();
            if (position <= 0) return stops[0].Color;
            if (position >= 1) return stops[^1].Color;
            ColorGradientStop left = stops[0];
            ColorGradientStop? right = null;
            foreach (ColorGradientStop stop in stops)
            {
                if (stop.Position >= position)
                {
                    right = stop;
                    break;
                }

                left = stop;
            }

            if (right == null || left == right)
                return left.Color;

            position = (float) Math.Round((position - left.Position) / (right.Position - left.Position), 2);
            byte a = (byte) ((right.Color.Alpha - left.Color.Alpha) * position + left.Color.Alpha);
            byte r = (byte) ((right.Color.Red - left.Color.Red) * position + left.Color.Red);
            byte g = (byte) ((right.Color.Green - left.Color.Green) * position + left.Color.Green);
            byte b = (byte) ((right.Color.Blue - left.Color.Blue) * position + left.Color.Blue);
            return new SKColor(r, g, b, a);
        }

        /// <summary>
        ///     Gets a new ColorGradient with colors looping through the HSV-spectrum
        /// </summary>
        /// <returns></returns>
        public static ColorGradient GetUnicornBarf()
        {
            const int amount = 8;
            ColorGradient gradient = new();

            for (int i = 0; i <= amount; i++)
            {
                float percent = i / (float)amount;
                gradient.Stops.Add(new ColorGradientStop(SKColor.FromHsv(360f * percent, 100, 100), percent));
            }

            return gradient;
        }
    }
}