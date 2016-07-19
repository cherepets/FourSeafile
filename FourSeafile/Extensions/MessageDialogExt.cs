using System;
using Windows.UI.Popups;

namespace FourSeafile.Extensions
{
    public static class MessageDialogExt
    {
        public static MessageDialog WithCommand(this MessageDialog dialog, string label)
        {
            dialog.Commands.Add(new UICommand(label));
            return dialog;
        }

        public static MessageDialog WithCommand(this MessageDialog dialog, string label, Action callback)
        {
            dialog.Commands.Add(new UICommand(label, (IUICommand command) => callback()));
            return dialog;
        }

        public static MessageDialog WithCommand(this MessageDialog dialog, string label, Action<IUICommand> callback)
        {
            dialog.Commands.Add(new UICommand(label, (IUICommand command) => callback(command)));
            return dialog;
        }

        public static MessageDialog SetCancelCommandIndex(this MessageDialog dialog, uint index)
        {
            dialog.CancelCommandIndex = index;
            return dialog;
        }

        public static MessageDialog SetDefaultCommandIndex(this MessageDialog dialog, uint index)
        {
            dialog.DefaultCommandIndex = index;
            return dialog;
        }
    }
}
