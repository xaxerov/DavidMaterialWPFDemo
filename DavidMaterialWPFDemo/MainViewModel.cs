using DavidMaterialWPFDemo.ViewModels;
using DavidMaterialWPFDemo.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DavidMaterialWPFDemo
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand RunExtendedDialogCommand => new AnotherCommandImplementation(ExecuteRunExtendedDialog);
        public MainViewModel()
        {
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
    }
}
