﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Artemis.Core;
using SkiaSharp;

namespace Artemis.UI.Shared
{
    /// <inheritdoc />
    /// <summary>
    ///     Converts  <see cref="T:Artemis.Core.Models.Profile.ColorGradient" /> into a
    ///     <see cref="T:System.Windows.Media.GradientStopCollection" />.
    /// </summary>
    [ValueConversion(typeof(List<ColorGradientStop>), typeof(GradientStopCollection))]
    public class ColorGradientToGradientStopsConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<ColorGradientStop> colorGradients = (List<ColorGradientStop>) value;
            GradientStopCollection collection = new();
            if (colorGradients == null)
                return collection;

            foreach (ColorGradientStop c in colorGradients.OrderBy(s => s.Position))
                collection.Add(new GradientStop(Color.FromArgb(c.Color.Alpha, c.Color.Red, c.Color.Green, c.Color.Blue), c.Position));
            return collection;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GradientStopCollection collection = (GradientStopCollection) value;
            List<ColorGradientStop> colorGradients = new();
            if (collection == null)
                return colorGradients;

            foreach (GradientStop c in collection.OrderBy(s => s.Offset))
                colorGradients.Add(new ColorGradientStop(new SKColor(c.Color.R, c.Color.G, c.Color.B, c.Color.A), (float) c.Offset));
            return colorGradients;
        }
    }
}