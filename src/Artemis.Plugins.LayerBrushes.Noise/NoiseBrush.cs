﻿using System;
using Artemis.Core.Models.Profile;
using Artemis.Core.Models.Profile.LayerProperties;
using Artemis.Core.Plugins.LayerBrush;
using Artemis.Plugins.LayerBrushes.Noise.Utilities;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class NoiseBrush : LayerBrush
    {
        private static readonly Random Rand = new Random();
        private readonly OpenSimplexNoise _noise;
        private float _z;

        public NoiseBrush(Layer layer, LayerBrushDescriptor descriptor) : base(layer, descriptor)
        {
            MainColorProperty = RegisterLayerProperty<SKColor>("Brush.MainColor", "Main color", "The main color of the noise.");
            ScaleProperty = RegisterLayerProperty<SKSize>("Brush.Scale", "Scale", "The scale of the noise.");
            AnimationSpeedProperty = RegisterLayerProperty<float>("Brush.AnimationSpeed", "Animation speed", "The speed at which the noise moves.");
            ScaleProperty.InputAffix = "%";

            _z = Rand.Next(0, 4096);
            _noise = new OpenSimplexNoise(Rand.Next(0, 4096));
        }

        public LayerProperty<SKColor> MainColorProperty { get; set; }
        public LayerProperty<SKSize> ScaleProperty { get; set; }
        public LayerProperty<float> AnimationSpeedProperty { get; set; }

        public override void Update(double deltaTime)
        {
            // TODO: Come up with a better way to use deltaTime
            _z += AnimationSpeedProperty.CurrentValue / 500f / 0.04f * (float) deltaTime;

            if (_z >= float.MaxValue)
                _z = 0;

            base.Update(deltaTime);
        }

        public override void Render(SKCanvas canvas, SKPath path, SKPaint paint)
        {
            return;
            var mainColor = MainColorProperty.CurrentValue;
            var scale = ScaleProperty.CurrentValue;

            // Scale down the render path to avoid computing a value for every pixel
            var width = (int) (Math.Max(path.Bounds.Width, path.Bounds.Height) / scale.Width);
            var height = (int) (Math.Max(path.Bounds.Width, path.Bounds.Height) / scale.Height);
            var opacity = (float) Math.Round(mainColor.Alpha / 255.0, 2, MidpointRounding.AwayFromZero);
            using (var bitmap = new SKBitmap(new SKImageInfo(width, height)))
            {
                bitmap.Erase(new SKColor(0, 0, 0, 0));
                // Only compute pixels inside LEDs, due to scaling there may be some rounding issues but it's neglect-able
                foreach (var artemisLed in Layer.Leds)
                {
                    var xStart = artemisLed.AbsoluteRenderRectangle.Left / scale.Width;
                    var xEnd = artemisLed.AbsoluteRenderRectangle.Right / scale.Width;
                    var yStart = artemisLed.AbsoluteRenderRectangle.Top / scale.Height;
                    var yEnd = artemisLed.AbsoluteRenderRectangle.Bottom / scale.Height;

                    for (var x = xStart; x < xEnd; x++)
                    {
                        for (var y = yStart; y < yEnd; y++)
                        {
                            var v = _noise.Evaluate(scale.Width * x / width, scale.Height * y / height, _z);
                            var alpha = (byte) ((v + 1) * 127 * opacity);
                            // There's some fun stuff we can do here, like creating hard lines
                            // if (alpha > 128)
                            //     alpha = 255;
                            // else
                            //     alpha = 0;
                            bitmap.SetPixel((int) x, (int) y, new SKColor(mainColor.Red, mainColor.Green, mainColor.Blue, alpha));
                        }
                    }
                }

                using (var sh = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror, SKMatrix.MakeScale(scale.Width, scale.Height)))
                {
                    paint.Shader = sh;
                    canvas.DrawPath(Layer.LayerShape.Path, paint);
                }
            }
        }
    }
}