using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TransportX.Spatial;

namespace TransportX.Plugins.Toukaitetudou.AvaterTest.Form
{
    /// <summary>
    /// SettingForm.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingForm : Window
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        public ReactivePropertySlim<float> TransX => Model.TransX;

        public ReactivePropertySlim<float> TransY => Model.TransY;

        public ReactivePropertySlim<float> TransZ =>Model.TransZ;

        public ReactivePropertySlim<float> QuatX =>Model.QuatX;

        public ReactivePropertySlim<float> QuatY =>Model.QuatY;

        public ReactivePropertySlim<float> QuatZ => Model.QuatZ;

        public ReactivePropertySlim<float> QuatW =>Model.QuatW;

        public ReactivePropertySlim<float> RotateX => Model.RotateX;

        public ReactivePropertySlim<float> RotateY => Model.RotateY;

        public ReactivePropertySlim<float> RotateZ => Model.RotateZ;
        public event EventHandler? UpdateToSource
        {
            add { Model.UpdateToSource += value; }
            remove { Model.UpdateToSource -= value; }
        }

        public event EventHandler? ApplyEuler
        {
            add { Model.ApplyEuler += value; }
            remove { Model.ApplyEuler -= value; }
        }

        public void Update(in Pose pose,in ChunkOffset chunkOffset)=>Model.Update(pose,chunkOffset);
        public void Dispose()
        {
            Model.Dispose();
        }
    }
}