using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DavidMaterialWPFDemo.ViewModels
{
    public class SampleProgressDialogViewModel : ViewModelBase
    {
        public SampleProgressDialogViewModel()
        {
            _progress = 0;
            Progress = 0;
        }

        public bool Execute()
        {
            bool success = false;
            while (Progress <= 100)
            {
                Thread.Sleep(100);
                ++Progress;
            }
            success = true;
            return success;
        }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set
            {
                if (value >= 0 && value <= 100)
                    SetProperty(ref _progress, value);
            }
        }
    }
}
