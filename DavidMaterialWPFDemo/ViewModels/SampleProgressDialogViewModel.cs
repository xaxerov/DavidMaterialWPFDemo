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

        public int Execute()
        {
            while (Progress < 100)
            {
                ++Progress;
                //Thread.Sleep(10);
            }
            return 0;
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
