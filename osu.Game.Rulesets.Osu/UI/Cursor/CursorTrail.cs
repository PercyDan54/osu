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
using osuTK;
using osuTK.Graphics;
using osuTK.Graphics.ES30;

namespace osu.Game.Rulesets.Osu.UI.Cursor
{
    public class CursorTrail : Drawable, IRequireHighFrequencyMousePosition
    {
        private const int max_sprites = 2048;

        private readonly Bindable<bool> hueOverride = new BindableBool();
        private readonly Bindable<bool> hueShift = new BindableBool();
        private readonly Bindable<float> hue = new Bindable<float>();
        private readonly Bindable<float> hueSpeed = new Bindable<float>();
        private readonly Bindable<float> size = new Bindable<float>();
        private readonly Bindable<float> fadeDuration = new Bindable<float>();
        private readonly Bindable<float> density = new Bindable<float>();

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
            config.BindWith(MSetting.CursorTrailHueShift, hueShift);
            config.BindWith(MSetting.CursorTrailHueSpeed, hueSpeed);
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
            resetTime();
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

        private void resetTime()
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

        private Vector2? lastPosition;
        private readonly InputResampler resampler = new InputResampler();

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            Vector2 pos = e.ScreenSpaceMousePosition;

            if (lastPosition == null)
            {
                lastPosition = pos;
                resampler.AddPosition(lastPosition.Value);
                return base.OnMouseMove(e);
            }

            foreach (Vector2 pos2 in resampler.AddPosition(pos))
            {
                Trace.Assert(lastPosition.HasValue);

                if (InterpolateMovements)
                {
                    Vector2 pos1 = lastPosition.Value;
                    Vector2 diff = pos2 - pos1;
                    float distance = diff.Length;
                    Vector2 direction = diff / distance;

                    float interval = defaultSize.X / 2.5f * IntervalMultiplier / density.Value;

                    for (float d = interval; d < distance; d += interval)
                    {
                        lastPosition = pos1 + direction * d;
                        addPart(lastPosition.Value);
                    }
                }
                else
                {
                    lastPosition = pos2;
                    addPart(lastPosition.Value);
                }
            }

            return base.OnMouseMove(e);
        }

        private void addPart(Vector2 screenSpacePosition)
        {
            parts[currentIndex].Position = screenSpacePosition;
            parts[currentIndex].Time = time + fadeDuration.Value / (float)FadeDuration;
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

                texture.TextureGL.Bind();

                RectangleF textureRect = texture.GetTextureRect();

                foreach (var part in parts)
                {
                    if (part.InvalidationID == -1)
                        continue;

                    if (time - part.Time >= 1)
                        continue;

                    var colour = hueOverride ? (ColourInfo)Colour4.FromHSV(Source.hue.Value + (Source.hueShift.Value ? time / (150 / Source.hueSpeed.Value) % 1 : 0), 1, 1) : DrawColourInfo.Colour;

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
