using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using AlleyCat.IO;
using Godot;

namespace AlleyCat.UI
{
    public static class FileDialogExtensions
    {
        public static IObservable<FileInfo> OnSelectFile(this FileDialog dialog)
        {
            return dialog.FromSignal("file_selected")
                .SelectMany(args => args.HeadOrNone().OfType<string>().ToObservable())
                .Select(path => new FileInfo(path));
        }

        public static IObservable<IEnumerable<FileInfo>> OnSelectFiles(this FileDialog dialog)
        {
            return dialog.FromSignal("files_selected")
                .SelectMany(args => args.HeadOrNone().OfType<string[]>().ToObservable())
                .Select(paths => paths.Map(p => new FileInfo(p)));
        }

        public static IObservable<DirectoryInfo> OnSelectDirectory(this FileDialog dialog)
        {
            return dialog.FromSignal("dir_selected")
                .SelectMany(args => args.HeadOrNone().OfType<string>().ToObservable())
                .Select(path => new DirectoryInfo(path));
        }
    }
}
