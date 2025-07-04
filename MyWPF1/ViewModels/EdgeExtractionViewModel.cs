﻿using HalconDotNet;
using System.Collections.ObjectModel;

namespace MyWPF1.ViewModels
{
    public class EdgeExtractionViewModel : ToolBaseViewModel
    {
        public override HObject CurrentResultImage => _resultImage;
        // 可选项
        public ObservableCollection<string> FilterOptions { get; } =
        [
            "deriche1","deriche2","lanser1",
            "lanser2","shen","mshen","canny","sobel_fast"
        ];

        // 绑定属性
        private string _filter = "canny";
        public string Filter
        {
            get => _filter;
            set { if (_filter != value) { _filter = value; OnPropertyChanged(); Apply(); } }
        }

        private double _alpha = 1.0;
        public double Alpha
        {
            get => _alpha;
            set { if (_alpha != value) { _alpha = value; OnPropertyChanged(); Apply(); } }
        }

        private double _low = 20.0;
        public double LowThreshold
        {
            get => _low;
            set { if (_low != value) { _low = value; OnPropertyChanged(); Apply(); } }
        }

        private double _high = 40.0;
        public double HighThreshold
        {
            get => _high;
            set { if (_high != value) { _high = value; OnPropertyChanged(); Apply(); } }
        }

        public override void Apply()
        {
            if (_inputImage == null || !_inputImage.IsInitialized())
                return;

            // 如果彩色图，先转灰度
            HOperatorSet.CountChannels(_inputImage, out HTuple channels);
            HObject gray = _inputImage;
            if (channels.I > 1)
            {
                HOperatorSet.Rgb1ToGray(_inputImage, out gray);
            }

            if (LowThreshold > HighThreshold)
                LowThreshold = HighThreshold - 1;
            // 调用 EdgesImage：输出幅值和方向
            HOperatorSet.EdgesSubPix(
                gray,
                out HObject Edges,
                new HTuple(Filter),
                new HTuple(Alpha),
                new HTuple(LowThreshold),
                new HTuple(HighThreshold)
            );

            // 把幅值图当结果显示
            _resultImage = Edges;

            _hWindowControl.HalconWindow.ClearWindow();
            _hWindowControl.HalconWindow.DispObj(_resultImage);
            OnPropertyChanged(nameof(CurrentResultImage));

            // 清理
            if (gray != _inputImage) gray.Dispose();
        }
    }
}
