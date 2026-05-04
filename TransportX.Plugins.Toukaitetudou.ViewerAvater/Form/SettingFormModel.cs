using Reactive.Bindings;
using System;
using System.Windows.Input;
using TransportX.Spatial;

namespace TransportX.Plugins.Toukaitetudou.AvaterTest.Form
{
    public class SettingFormModel:IDisposable
    {
        class ActionCommand : ICommand
        {
            Action Action;
            public event EventHandler? CanExecuteChanged;
            public ActionCommand(Action action)
            {
                Action = action;
            }

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                Action.Invoke();
            }
        }

        public ReactivePropertySlim<float> TransX { get; }
        public ReactivePropertySlim<float> TransY { get; }
        public ReactivePropertySlim<float> TransZ { get; }

        public ReactivePropertySlim<float> QuatX { get; }
        public ReactivePropertySlim<float> QuatY { get; }
        public ReactivePropertySlim<float> QuatZ { get; }
        public ReactivePropertySlim<float> QuatW { get; }

        public ReactivePropertySlim<float> RotateX { get; }
        public ReactivePropertySlim<float> RotateY { get; }
        public ReactivePropertySlim<float> RotateZ { get; }

        public event EventHandler? UpdateToSource;
        public event EventHandler? ApplyEuler;

        public ICommand ApplyEulerCommand { get; }
        public SettingFormModel()
        {
            TransX = new();
            TransY = new();
            TransZ = new();
            QuatX = new();
            QuatY = new();
            QuatZ = new();
            QuatW = new();
            RotateX = new();
            RotateY = new();
            RotateZ = new();
            TransX.PropertyChanged += PropertyChanged;
            TransY.PropertyChanged += PropertyChanged;
            TransZ.PropertyChanged += PropertyChanged;


            ApplyEulerCommand = new ActionCommand(() => ApplyEuler?.Invoke(this, EventArgs.Empty));
            isChangeFromForm = true;
        }
        bool isChangeFromForm;
        private void PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (isChangeFromForm)
            {
                UpdateToSource?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Update(in Pose pose, in ChunkOffset chunkOffset)
        {
            isChangeFromForm = false;
            TransX.Value = pose.Position.X + chunkOffset.DeltaX * Chunk.Size;
            TransY.Value = pose.Position.Y;
            TransZ.Value = pose.Position.Z + chunkOffset.DeltaZ * Chunk.Size;
            QuatX.Value = pose.Orientation.X;
            QuatY.Value = pose.Orientation.Y;
            QuatZ.Value = pose.Orientation.Z;
            QuatW.Value = pose.Orientation.W;
            isChangeFromForm = true;
        }

        public void Dispose()
        {
            TransX.Dispose();
            TransY.Dispose();
            TransZ.Dispose();
            QuatX.Dispose();
            QuatY.Dispose();
            QuatZ.Dispose();
            QuatW.Dispose();
            RotateX.Dispose();
            RotateY.Dispose();
            RotateZ.Dispose();
        }
    }
}