using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using TransportX.Avatars;
using TransportX.Dependency;
using TransportX.Input;
using TransportX.Network;
using TransportX.Plugins.Toukaitetudou.AvaterTest.Form;
using TransportX.Plugins.Toukaitetudou.AvaterTest.Input;
using TransportX.Rendering;
using TransportX.Spatial;
using TransportX.Traffic;

namespace TransportX.Plugins.Toukaitetudou.AvaterTest
{
    public class Avater : AvatarBase
    {

        private readonly IReadOnlyList<IInput> Inputs;

        private readonly KeyObserver ResetKey;
        public override string Title { get; } = "ビューワーアバター";

        public override string Description { get; } = "キーボード操作でカメラを移動できるビューワーアバターを提供します";

        public override string Author { get; } = "Toukaitetudou";

        public override Viewpoint DriverViewpoint { get; }

        public override Viewpoint BirdViewpoint { get; }

        public override float Width { get; }

        public override float Height { get; }

        public override float Length { get; }

        public override bool IsEnabled => true;

        public override ILanePath? Path => null;

        public override EntityDirection Heading => EntityDirection.Forward;

        public override float S => 0;

        public override float SVelocity => 0;
        float x;
        float y;
        float z;
        Quaternion rotate;
        float deltaHiTranslation = 1f;
        float deltaHiRotation = float.Pi / 180f * 0.002f;
        float deltaLowTranslation = 0.1f;
        float deltaLowRotation = float.Pi / 180f * 0.001f;
        SettingForm? Form;
        public Avater(PluginLoadContext context, AvatarBuilder builder) : base(context, builder)
        {
            Reset();
            DriverViewpoint = new DriverViewpoint(this, new SixDoF(0, 0, 0));
            BirdViewpoint = new BirdViewpoint(this, new SixDoF(0, 2, -3), 20, new Vector2(0.3f, 0));
            ResetKey = InputManager.ObserveKey(System.Windows.Input.Key.R);
            Inputs = [new KeyboardInput(InputManager)];
            
        }


        private void Form_Closed(object? sender, EventArgs e)
        {
            Form?.Dispose();
            Form= null;
        }

        private void Form_UpdateToSource(object? sender, EventArgs e)
        {
            if (Form is null) return;
            int deltaX = GetChunkDelta(Form.TransX.Value);
            int deltaZ = GetChunkDelta(Form.TransZ.Value);
            x = Form.TransX.Value - deltaX * Chunk.Size;
            y = Form.TransY.Value;
            z = Form.TransZ.Value - deltaZ * Chunk.Size;
            chunkOffset = new ChunkOffset(deltaX, deltaZ);

            static int GetChunkDelta(float delta)
            {
                return (int)float.Floor(delta / Chunk.Size);
            }
        }

        private void Form_ApplyEuler(object? sender, EventArgs e)
        {
            if(Form is null) return;
            rotate =
                Quaternion.CreateFromAxisAngle(Vector3.UnitX, Form.RotateX.Value) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitY, Form.RotateY.Value) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Form.RotateZ.Value);
        }

        void Reset()
        {
            x = 0;
            y = 0;
            z = 0;
            rotate = Quaternion.Identity;
            chunkOffset = ChunkOffset.Identity;
            TeleportTo(new WorldPose(0, 0, Pose.Identity));
        }
        public override bool Spawn(ILanePath path, EntityDirection heading, float s)
        {
            throw new NotImplementedException();
        }
        ChunkOffset chunkOffset;
        public override void SubTick(TimeSpan elapsed)
        {
            IInput input = Inputs[0];
            if (input.IsReset)
            {
                Reset();
                foreach (TransformedModel model in Structure)
                {
                    model.Pose = model.BasePose * WorldPose.Pose;
                }
            }
            if (input.IsOpenForm)
            {
                if (Form is null)
                {
                    Form = new();
                    Form.Owner = Application.Current.MainWindow;
                    Form.Closed += Form_Closed;
                    Form.UpdateToSource += Form_UpdateToSource;
                    Form.ApplyEuler += Form_ApplyEuler;
                    Form.Show();
                }
                Form.Activate();
            }
            if (input.IsWorld)
            {
                if (input.IsSpeedShift)
                {
                    if (input.IsRightMove)
                    {
                        x += deltaHiTranslation;
                    }
                    if (input.IsLeftMove)
                    {
                        x -= deltaHiTranslation;
                    }
                    if (input.IsUpMove)
                    {
                        y += deltaHiTranslation;
                    }
                    if (input.IsDownMove)
                    {
                        y -= deltaHiTranslation;
                    }
                    if (input.IsFowardMove)
                    {
                        z += deltaHiTranslation;
                    }
                    if (input.IsBackMove)
                    {
                        z -= deltaHiTranslation;
                    }
                    if (input.IsRotateXPlus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation)), deltaHiRotation);
                    }
                    if (input.IsRotateXMinus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation)), -deltaHiRotation);
                    }
                    if (input.IsRotateYPlus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation)), deltaHiRotation);
                    }
                    if (input.IsRotateYMinus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation)), -deltaHiRotation);
                    }
                    if (input.IsRotateZPlus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation)), deltaHiRotation);
                    }
                    if (input.IsRotateZMinus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation)), -deltaHiRotation);
                    }
                }
                else
                {
                    if (input.IsRightMove)
                    {
                        x += deltaLowTranslation;
                    }
                    if (input.IsLeftMove)
                    {
                        x -= deltaLowTranslation;
                    }
                    if (input.IsUpMove)
                    {
                        y += deltaLowTranslation;
                    }
                    if (input.IsDownMove)
                    {
                        y -= deltaLowTranslation;
                    }
                    if (input.IsFowardMove)
                    {
                        z += deltaLowTranslation;
                    }
                    if (input.IsBackMove)
                    {
                        z -= deltaLowTranslation;
                    }
                    if (input.IsRotateXPlus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation)), deltaLowRotation);
                    }
                    if (input.IsRotateXMinus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation)), -deltaLowRotation);
                    }
                    if (input.IsRotateYPlus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation)), deltaLowRotation);
                    }
                    if (input.IsRotateYMinus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation)), -deltaLowRotation);
                    }
                    if (input.IsRotateZPlus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation)), deltaLowRotation);
                    }
                    if (input.IsRotateZMinus)
                    {
                        rotate *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation)), -deltaLowRotation);
                    }
                }
            }
            else
            {
                Matrix4x4 matrix = Matrix4x4.Identity;
                Vector3 translation = Vector3.Zero;
                Quaternion rot = Quaternion.Identity;
                if (input.IsSpeedShift)
                {
                    if (input.IsRightMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation) * deltaHiTranslation;
                    }
                    if (input.IsLeftMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation) * -deltaHiTranslation;
                    }
                    if (input.IsUpMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation) * deltaHiTranslation;
                    }
                    if (input.IsDownMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation) * -deltaHiTranslation;
                    }
                    if (input.IsFowardMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation) * deltaHiTranslation;
                    }
                    if (input.IsBackMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation) * -deltaHiTranslation;
                    }
                    if (input.IsRotateXPlus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation), deltaHiRotation);
                    }
                    if (input.IsRotateXMinus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation), -deltaHiRotation);
                    }
                    if (input.IsRotateYPlus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation), deltaHiRotation);
                    }
                    if (input.IsRotateYMinus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation), -deltaHiRotation);
                    }
                    if (input.IsRotateZPlus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation), deltaHiRotation);
                    }
                    if (input.IsRotateZMinus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation), -deltaHiRotation);
                    }
                }
                else
                {
                    if (input.IsRightMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation) * deltaLowTranslation;
                    }
                    if (input.IsLeftMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitX, WorldPose.Pose.Orientation) * -deltaLowTranslation;
                    }
                    if (input.IsUpMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation) * deltaLowTranslation;
                    }
                    if (input.IsDownMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitY, WorldPose.Pose.Orientation) * -deltaLowTranslation;
                    }
                    if (input.IsFowardMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation) * deltaLowTranslation;
                    }
                    if (input.IsBackMove)
                    {
                        translation += Vector3.Transform(Vector3.UnitZ, WorldPose.Pose.Orientation) * -deltaLowTranslation;
                    }
                    if (input.IsRotateXPlus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, deltaLowRotation);
                    }
                    if (input.IsRotateXMinus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, -deltaLowRotation);
                    }
                    if (input.IsRotateYPlus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaLowRotation);
                    }
                    if (input.IsRotateYMinus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, -deltaLowRotation);
                    }
                    if (input.IsRotateZPlus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, deltaLowRotation);
                    }
                    if (input.IsRotateZMinus)
                    {
                        rot *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -deltaLowRotation);
                    }
                }
                x += translation.X;
                y += translation.Y;
                z += translation.Z;
                rotate *= rot;

            }

            if (Camera.DrawChunkCount <= int.Abs(Camera.WorldPose.ChunkX - WorldPose.ChunkX)
                || Camera.DrawChunkCount <= int.Abs(Camera.WorldPose.ChunkZ - WorldPose.ChunkZ))
            {
                Structure.Freeze();
                return;
            }
            else
            {
                Structure.Unfreeze();
            }
            base.SubTick(elapsed);
            WorldPose wp = new WorldPose(chunkOffset.DeltaX, chunkOffset.DeltaZ, new Pose(new Vector3(x, y, z), rotate));
            ChunkOffset co = TeleportTo(wp);
            chunkOffset += co;
            x -= co.DeltaX * Chunk.Size;
            z -= co.DeltaZ * Chunk.Size;
            Form?.Update(WorldPose.Pose, chunkOffset);
        }


        public override void Tick(TimeSpan elapsed)
        {
        }
        public override void Dispose()
        {
            Form?.Close();
            Form = null;
            base.Dispose();
        }
    }
}