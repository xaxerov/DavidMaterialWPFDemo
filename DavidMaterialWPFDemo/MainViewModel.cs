using DavidMaterialWPFDemo.ViewModels;
using DavidMaterialWPFDemo.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DavidMaterialWPFDemo
{
    public class MainViewModel : ViewModelBase
    {
        public BackgroundWorker _worker;
        public ICommand RunExtendedDialogCommand => new AnotherCommandImplementation(ExecuteRunExtendedDialog);
        public ICommand RunProgressCommand => new AnotherCommandImplementation(Execute);
        public MainViewModel()
        {
            _progress = 0;
            Progress = 0;

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += worker_DoWork;
            _worker.ProgressChanged += worker_ProgressChanged;

            Execute();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        private async void ExecuteRunExtendedDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new SampleDialog()
            {
                DataContext = new SampleDialogViewModel()
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);

            //check the result...
            Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
            => Debug.WriteLine("You could intercept the open and affect the dialog using eventArgs.Session.");

        private void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool parameter &&
                parameter == false) return;

            //OK, lets cancel the close...
            eventArgs.Cancel();

            var progressViewModel = new SampleProgressDialogViewModel();
            var progressView = new SampleProgressDialog() { DataContext = progressViewModel };
            //...now, lets update the "session" with some new content!
            eventArgs.Session.UpdateContent(progressView);
            //note, you can also grab the session when the dialog opens via the DialogOpenedEventHandler

            //lets run a fake operation for 3 seconds then close this baby.
            progressViewModel.Execute();
            Task.Delay(0)
                .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }


        public void Execute(object o = null)
        {
            if (_worker.IsBusy)
                return;
            _worker.RunWorkerAsync();
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
