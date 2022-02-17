// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Layout;
using osu.Framework.Timing;
using osu.Game.Configuration;
using osu.Game.Rulesets.Osu.Skinning.Legacy;
using osuTK;
using osuTK.Graphics;
using osuTK.Graphics.ES30;

namespace osu.Game.Rulesets.Osu.UI.Cursor
{
    public class CursorTrail : Drawable, IRequireHighFrequencyMousePosition
    {
        private const int max_sprites = 2048;

        /// <summary>
        /// An exponentiating factor to ease the trail fade.
        /// </summary>
        protected virtual float FadeExponent => 1.7f;

        private readonly BindableBool hueOverride = new BindableBool();
        private readonly BindableBool rainbow = new BindableBool();
        private readonly BindableFloat hue = new BindableFloat();
        private readonly BindableDouble rainbowFreq = new BindableDouble();
        private readonly BindableFloat size = new BindableFloat();
        private readonly BindableFloat fadeDuration = new BindableFloat();
        private readonly BindableFloat density = new BindableFloat();

        private readonly TrailPart[] parts = new TrailPart[max_sprites];
        private int currentIndex;
        private IShader shader;
        private double timeOffset;
        private float time;

        private Anchor trailOrigin = Anchor.Centre;

        protected Anchor TrailOrigin
        {
            get => trailOrigin;
            set
            {
                trailOrigin = value;
                Invalidate(Invalidation.DrawNode);
            }
        }

        public CursorTrail()
        {
            // as we are currently very dependent on having a running clock, let's make our own clock for the time being.
            Clock = new FramedClock();

            RelativeSizeAxes = Axes.Both;

            for (int i = 0; i < max_sprites; i++)
            {
                // -1 signals that the part is unusable, and should not be drawn
                parts[i].InvalidationID = -1;
            }

            AddLayout(partSizeCache);
        }

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders, MConfigManager config)
        {
            config.BindWith(MSetting.CursorTrailHue, hue);
            config.BindWith(MSetting.CursorTrailRainbow, rainbow);
            config.BindWith(MSetting.CursorTrailRainbowFreq, rainbowFreq);
            config.BindWith(MSetting.CursorTrailHueOverride, hueOverride);
            config.BindWith(MSetting.CursorTrailSize, size);
            config.BindWith(MSetting.CursorTrailDensity, density);
            config.BindWith(MSetting.CursorTrailFadeDuration, fadeDuration);

            shader = shaders.Load(@"CursorTrail", FragmentShaderDescriptor.TEXTURE);
            size.BindValueChanged(_ => partSizeCache.Invalidate());
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            ResetTime();
        }

        private Texture texture = Texture.WhitePixel;

        public Texture Texture
        {
            get => texture;
            set
            {
                if (texture == value)
                    return;

                texture = value;
                Invalidate(Invalidation.DrawNode);
            }
        }

        private readonly LayoutValue<Vector2> partSizeCache = new LayoutValue<Vector2>(Invalidation.DrawInfo | Invalidation.RequiredParentSizeToFit | Invalidation.Presence);

        private Vector2 defaultSize => new Vector2(Texture.DisplayWidth, Texture.DisplayHeight) * DrawInfo.Matrix.ExtractScale().Xy;

        private Vector2 partSize => partSizeCache.IsValid ? partSizeCache.Value : partSizeCache.Value = new Vector2(size.Value) * defaultSize;

        /// <summary>
        /// The amount of time to fade the cursor trail pieces.
        /// </summary>
        protected virtual double FadeDuration => 300;

        public override bool IsPresent => true;

        protected override void Update()
        {
            base.Update();

            Invalidate(Invalidation.DrawNode);

            time = (float)((Time.Current - timeOffset) / FadeDuration);
        }

        protected void ResetTime()
        {
            for (int i = 0; i < parts.Length; ++i)
            {
                parts[i].Time -= time;

                if (parts[i].InvalidationID != -1)
                    ++parts[i].InvalidationID;
            }

            time = 0;
            timeOffset = Time.Current;
        }

        /// <summary>
        /// Whether to interpolate mouse movements and add trail pieces at intermediate points.
        /// </summary>
        protected virtual bool InterpolateMovements => true;

        protected virtual float IntervalMultiplier => 1.0f;
        protected virtual bool AvoidDrawingNearCursor => false;

        private Vector2? lastPosition;
        private readonly InputResampler resampler = new InputResampler();

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            AddTrail(e.ScreenSpaceMousePosition);
            return base.OnMouseMove(e);
        }

        protected void AddTrail(Vector2 position)
        {
            if (InterpolateMovements)
            {
                if (!lastPosition.HasValue)
                {
                    lastPosition = position;
                    resampler.AddPosition(lastPosition.Value);
                    return;
                }

                foreach (Vector2 pos2 in resampler.AddPosition(position))
                {
                    Trace.Assert(lastPosition.HasValue);

                    Vector2 pos1 = lastPosition.Value;
                    Vector2 diff = pos2 - pos1;
                    float distance = diff.Length;
                    Vector2 direction = diff / distance;

                    float interval = defaultSize.X / 2.5f * IntervalMultiplier / density.Value;
                    float stopAt = distance - (AvoidDrawingNearCursor ? interval : 0);

                    for (float d = interval; d < stopAt; d += interval)
                    {
                        lastPosition = pos1 + direction * d;
                        addPart(lastPosition.Value);
                    }
                }
            }
            else
            {
                lastPosition = position;
                addPart(lastPosition.Value);
            }
        }

        private void addPart(Vector2 screenSpacePosition)
        {
            parts[currentIndex].Position = screenSpacePosition;
            parts[currentIndex].Time = time + fadeDuration.Value / (this is LegacyCursorTrail ? 500 : (float)FadeDuration);
            ++parts[currentIndex].InvalidationID;

            currentIndex = (currentIndex + 1) % max_sprites;
        }

        protected override DrawNode CreateDrawNode() => new TrailDrawNode(this);

        private struct TrailPart
        {
            public Vector2 Position;
            public float Time;
            public long InvalidationID;
        }

        private class TrailDrawNode : DrawNode
        {
            protected new CursorTrail Source => (CursorTrail)base.Source;

            private IShader shader;
            private Texture texture;

            private float time;
            private float fadeExponent;

            private readonly TrailPart[] parts = new TrailPart[max_sprites];
            private Vector2 size;
            private Vector2 originPosition;

            private readonly QuadBatch<TexturedTrailVertex> vertexBatch = new QuadBatch<TexturedTrailVertex>(max_sprites, 1);
            private bool hueOverride;

            public TrailDrawNode(CursorTrail source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                shader = Source.shader;
                texture = Source.texture;
                size = Source.partSize;
                time = Source.time;
                hueOverride = Source.hueOverride.Value;
                fadeExponent = Source.FadeExponent;

                originPosition = Vector2.Zero;

                if (Source.TrailOrigin.HasFlagFast(Anchor.x1))
                    originPosition.X = 0.5f;
                else if (Source.TrailOrigin.HasFlagFast(Anchor.x2))
                    originPosition.X = 1f;

                if (Source.TrailOrigin.HasFlagFast(Anchor.y1))
                    originPosition.Y = 0.5f;
                else if (Source.TrailOrigin.HasFlagFast(Anchor.y2))
                    originPosition.Y = 1f;

                Source.parts.CopyTo(parts, 0);
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                shader.Bind();
                shader.GetUniform<float>("g_FadeClock").UpdateValue(ref time);
                shader.GetUniform<float>("g_FadeExponent").UpdateValue(ref fadeExponent);

                texture.TextureGL.Bind();

                RectangleF textureRect = texture.GetTextureRect();

                foreach (var part in parts)
                {
                    if (part.InvalidationID == -1)
                        continue;

                    if (time - part.Time >= 1)
                        continue;

                    var colour = hueOverride ? (ColourInfo)Colour4.FromHSV(Source.hue.Value, 1, 1) : DrawColourInfo.Colour;

                    if (Source.rainbow.Value)
                    {
                        float partTime = part.Time;
                        double freq = Source.rainbowFreq.Value;
                        double red = Math.Sin(freq + partTime) * 127 + 128;
                        double green = Math.Sin(freq + 2 + partTime) * 127 + 128;
                        double blue = Math.Sin(freq + 4 + partTime) * 127 + 128;
                        colour = (ColourInfo)new Color4((byte)red, (byte)green, (byte)blue, byte.MaxValue);
                    }

                    vertexBatch.Add(new TexturedTrailVertex
                    {
                        Position = new Vector2(part.Position.X - size.X * originPosition.X, part.Position.Y + size.Y * (1 - originPosition.Y)),
                        TexturePosition = textureRect.BottomLeft,
                        TextureRect = new Vector4(0, 0, 1, 1),
                        Colour = colour.BottomLeft.Linear,
                        Time = part.Time
                    });

                    vertexBatch.Add(new TexturedTrailVertex
                    {
                        Position = new Vector2(part.Position.X + size.X * (1 - originPosition.X), part.Position.Y + size.Y * (1 - originPosition.Y)),
                        TexturePosition = textureRect.BottomRight,
                        TextureRect = new Vector4(0, 0, 1, 1),
                        Colour = colour.BottomRight.Linear,
                        Time = part.Time
                    });

                    vertexBatch.Add(new TexturedTrailVertex
                    {
                        Position = new Vector2(part.Position.X + size.X * (1 - originPosition.X), part.Position.Y - size.Y * originPosition.Y),
                        TexturePosition = textureRect.TopRight,
                        TextureRect = new Vector4(0, 0, 1, 1),
                        Colour = colour.TopRight.Linear,
                        Time = part.Time
                    });

                    vertexBatch.Add(new TexturedTrailVertex
                    {
                        Position = new Vector2(part.Position.X - size.X * originPosition.X, part.Position.Y - size.Y * originPosition.Y),
                        TexturePosition = textureRect.TopLeft,
                        TextureRect = new Vector4(0, 0, 1, 1),
                        Colour = colour.TopLeft.Linear,
                        Time = part.Time
                    });
                }

                vertexBatch.Draw();
                shader.Unbind();
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);

                vertexBatch.Dispose();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TexturedTrailVertex : IEquatable<TexturedTrailVertex>, IVertex
        {
            [VertexMember(2, VertexAttribPointerType.Float)]
            public Vector2 Position;

            [VertexMember(4, VertexAttribPointerType.Float)]
            public Color4 Colour;

            [VertexMember(2, VertexAttribPointerType.Float)]
            public Vector2 TexturePosition;

            [VertexMember(4, VertexAttribPointerType.Float)]
            public Vector4 TextureRect;

            [VertexMember(1, VertexAttribPointerType.Float)]
            public float Time;

            public bool Equals(TexturedTrailVertex other)
            {
                return Position.Equals(other.Position)
                       && TexturePosition.Equals(other.TexturePosition)
                       && Colour.Equals(other.Colour)
                       && Time.Equals(other.Time);
            }
        }
    }
}
