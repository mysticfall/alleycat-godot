using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.IO;
using AlleyCat.Logging;
using AlleyCat.Mesh;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using static Godot.FileDialog;

namespace AlleyCat.UI.Tool
{
    public class BlendMapGenerator : UIControl
    {
        protected LineEdit InputEdit { get; }

        protected LineEdit OutputEdit { get; }

        protected Tree SourceList { get; }

        protected Tree MorphList { get; }

        protected Label ProgressLabel { get; }

        protected ProgressBar ProgressBar { get; }

        protected Button InputButton { get; }

        protected Button OutputButton { get; }

        protected Button StartButton { get; }

        protected Button CloseButton { get; }

        protected FileDialog FileDialog { get; }

        protected Label InfoLabel { get; }

        private static readonly Regex NamePattern =
            new Regex("^([^\\s-]+)\\s*-\\s*([^\\.]+).mesh", RegexOptions.IgnoreCase);

        public BlendMapGenerator(LineEdit inputEdit,
            LineEdit outputEdit,
            Tree sourceList,
            Tree morphList,
            Label progressLabel,
            ProgressBar progressBar,
            Button inputButton,
            Button outputButton,
            Button startButton,
            Button closeButton,
            FileDialog fileDialog,
            Label infoLabel,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(inputEdit, nameof(inputEdit)).IsNotNull();
            Ensure.That(outputEdit, nameof(outputEdit)).IsNotNull();
            Ensure.That(sourceList, nameof(sourceList)).IsNotNull();
            Ensure.That(morphList, nameof(morphList)).IsNotNull();
            Ensure.That(progressLabel, nameof(progressLabel)).IsNotNull();
            Ensure.That(progressBar, nameof(progressBar)).IsNotNull();
            Ensure.That(inputButton, nameof(inputButton)).IsNotNull();
            Ensure.That(outputButton, nameof(inputButton)).IsNotNull();
            Ensure.That(startButton, nameof(startButton)).IsNotNull();
            Ensure.That(closeButton, nameof(closeButton)).IsNotNull();
            Ensure.That(fileDialog, nameof(fileDialog)).IsNotNull();
            Ensure.That(infoLabel, nameof(infoLabel)).IsNotNull();

            InputEdit = inputEdit;
            OutputEdit = outputEdit;
            SourceList = sourceList;
            MorphList = morphList;
            ProgressLabel = progressLabel;
            ProgressBar = progressBar;
            InputButton = inputButton;
            OutputButton = outputButton;
            StartButton = startButton;
            CloseButton = closeButton;
            FileDialog = fileDialog;
            InfoLabel = infoLabel;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            SourceList.CreateItem();

            SourceList.SetColumnTitle(0, Translate("ui.BlendMapGenerator.name"));
            SourceList.SetColumnTitle(1, Translate("ui.BlendMapGenerator.path"));

            SourceList.SetColumnTitlesVisible(true);

            MorphList.CreateItem();

            MorphList.SetColumnTitle(0, Translate("ui.BlendMapGenerator.name"));
            MorphList.SetColumnTitle(1, Translate("ui.BlendMapGenerator.surface"));
            MorphList.SetColumnTitle(2, Translate("ui.BlendMapGenerator.status"));
            MorphList.SetColumnTitle(3, Translate("ui.BlendMapGenerator.path"));

            MorphList.SetColumnTitlesVisible(true);

            FileDialog.CurrentDir = "res://";

            var disposed = Disposed.Where(identity);

            IObservable<Option<DirectoryInfo>> ObserveDirectoryChange(
                Button button,
                LineEdit edit,
                // This is a horrible workaround for a problem that Popup.popup_hide never gets fired.
                IObservable<Unit> anotherButtonPressed)
            {
                Option<DirectoryInfo> Validate(string path)
                {
                    var dir = new DirectoryInfo(path);
                    return dir.IsDirectory && dir.Exists ? Some(dir) : None;
                }

                var fromChooser = button.OnPress()
                    .Do(_ =>
                    {
                        FileDialog.Mode = ModeEnum.OpenDir;
                        FileDialog.ShowModal(true);
                    })
                    .Select(_ => FileDialog.OnSelectDirectory().Select(Some).TakeUntil(anotherButtonPressed))
                    .Switch()
                    .Do(dir => edit.Text = dir.Map(v => v.Path).IfNone(""));

                var fromEdit = edit.OnTextChanged()
                    .Select(Validate)
                    .Do(dir => dir.Iter(v => FileDialog.CurrentDir = v.Path));

                return fromChooser.Merge(fromEdit);
            }

            var onInputChange = ObserveDirectoryChange(InputButton, InputEdit, OutputButton.OnPress());
            var onOutputChange = ObserveDirectoryChange(OutputButton, OutputEdit, InputButton.OnPress());

            IObservable<Option<string>> ToErrorMessage(IObservable<Option<DirectoryInfo>> value, string msg) =>
                value.StartWith(None).Select(v => v.IsSome ? None : Some(msg));

            var message = Observable.CombineLatest(
                ToErrorMessage(onInputChange, Translate("error.invalid.directory.input")),
                ToErrorMessage(onOutputChange, Translate("error.invalid.directory.output")),
                (m1, m2) => m1.Concat(m2).HeadOrNone().IfNone("")
            ).Skip(1);

            message
                .TakeUntil(disposed)
                .Subscribe(m => InfoLabel.Text = m, this);

            var onDirectoryChange = Observable.CombineLatest(
                onInputChange.Select(v => v.ToObservable()).Switch(),
                onOutputChange.Select(v => v.ToObservable()).Switch(),
                (input, output) => new Paths(input, output));

            var meshes = onDirectoryChange
                .Select(v => v.Input.Contents)
                .Select(FindMeshes)
                .Select(v => v.Cast<FileInfo>().Freeze())
                .Publish();

            var meshSets = meshes.CombineLatest(onDirectoryChange, (files, paths) => (files, paths))
                .Do(v => Logger.LogDebug("Searching for base meshes in '{}'.", v.paths.Input.Path))
                .Select(v => v.files.Bind(f => MeshSet.TryCreate(f, v.paths, v.files, Logger)))
                .Select(v => v.Freeze())
                .Publish();

            var shownTasks = SourceList.OnItemSelect()
                .Select(v => v.Map(i => i.GetText(0)))
                .WithLatestFrom(meshSets, (key, s) => s.Find(v => v.Key == key).Bind(v => v.Tasks));

            var valid = meshSets.Select(set => set.Bind(v => v.Tasks).Any(t => t.State != TaskState.UpToDate));

            meshSets
                .Throttle(TimeSpan.FromSeconds(1))
                .TakeUntil(disposed)
                .Do(_ => SourceList.RemoveAllNodes())
                .Do(v => v.Iter(CreateNode))
                .Do(_ => SourceList.GetRoot().Children().HeadOrNone().Iter(c => c.Select(0)))
                .Subscribe(this);

            shownTasks
                .TakeUntil(disposed)
                .Do(_ => MorphList.RemoveAllNodes())
                .Subscribe(v => v.Iter(CreateNode), this);

            valid
                .TakeUntil(disposed)
                .Subscribe(v => StartButton.Disabled = !v, this);

            StartButton.OnPress()
                .WithLatestFrom(meshSets, (_, v) => v)
                .TakeUntil(disposed)
                .Subscribe(Start, this);

            CloseButton.OnPress()
                .TakeUntil(disposed)
                .Subscribe(_ => Quit(), this);

            meshes.Connect();
            meshSets.Connect();
        }

        private void CreateNode(MeshSet mesh)
        {
            var node = SourceList.CreateItem(SourceList.GetRoot());

            node.SetCellMode(0, TreeItem.TreeCellMode.String);
            node.SetText(0, mesh.Key);

            node.SetCellMode(1, TreeItem.TreeCellMode.String);
            node.SetText(1, mesh.File.Directory.Path);
        }

        private void CreateNode(Task task)
        {
            var node = MorphList.CreateItem(MorphList.GetRoot());

            node.SetCellMode(0, TreeItem.TreeCellMode.String);
            node.SetText(0, task.Key);

            node.SetCellMode(1, TreeItem.TreeCellMode.String);
            node.SetText(1, task.Surface);

            node.SetCellMode(2, TreeItem.TreeCellMode.String);
            node.SetText(2, Translate($"ui.BlendMapGenerator.status.{task.State}"));

            node.SetCellMode(3, TreeItem.TreeCellMode.String);
            node.SetText(3, task.File.Name);
        }

        protected IEnumerable<IFileInfo> FindMeshes(IDirectoryContents directory)
        {
            var inSubDirs = directory.OfType<DirectoryInfo>().Bind(d => FindMeshes(d.Contents));
            var inCurrentDir = directory.Filter(f => !f.IsDirectory && f.Name.EndsWith(".mesh"));

            return inCurrentDir.Concat(inSubDirs);
        }

        private void Start(Lst<MeshSet> meshSets)
        {
            var tasks = meshSets.Bind(m => m.Tasks).Filter(t => t.State != TaskState.UpToDate).Freeze();

            ProgressBar.Value = 0;
            ProgressBar.MaxValue = tasks.Count();

            var process = tasks.ToObservable()
                .TakeUntil(Disposed.Where(identity))
                .ObserveOn(Scheduler.Default)
                .Do(t => t.Run(this, LoggerFactory))
                .Do(t =>
                {
                    var selected = Optional(SourceList.GetSelected()).Map(i => i.GetText(0));

                    if (selected.Contains(t.Parent.Key))
                    {
                        MorphList.GetRoot()
                            .Children()
                            .Find(i => i.GetText(0) == t.Key && i.GetText(1) == t.Surface)
                            .Iter(n => n.SetText(2, Translate($"ui.BlendMapGenerator.status.{t.State}")));
                    }
                })
                .Publish();

            process
                .SubscribeOn(Node.GetScheduler())
                .Subscribe(_ => ProgressBar.Value += 1, this);

            var running = process.Select(_ => false).TakeLast(1).StartWith(tasks.Any());

            running
                .SubscribeOn(Node.GetScheduler())
                .Subscribe(v =>
                {
                    StartButton.Disabled = v;
                    InputButton.Disabled = v;
                    InputEdit.Editable = !v;
                    OutputButton.Disabled = v;
                    OutputEdit.Editable = !v;
                }, this);

            process.Connect();
        }

        public void Quit() => Node.GetTree().Quit();

        private struct Paths
        {
            public DirectoryInfo Input { get; }

            public DirectoryInfo Output { get; }

            public Paths(DirectoryInfo input, DirectoryInfo output)
            {
                Input = input;
                Output = output;
            }
        }

        private abstract class ListEntry : IIdentifiable
        {
            public string Key { get; }

            public FileInfo File { get; }

            public ListEntry(string key, FileInfo file)
            {
                Key = key;
                File = file;
            }

            public ArrayMesh CreateMesh() => ResourceLoader.Load<ArrayMesh>(File.Path);
        }

        private class MeshSet : ListEntry
        {
            public Paths Paths { get; }

            public string Prefix { get; }

            public Lst<string> Surfaces { get; }

            public IEnumerable<Task> Tasks => _tasks;

            private Lst<Task> _tasks;

            public MeshSet(string key, FileInfo file, Paths paths) : base(key, file)
            {
                Paths = paths;

                var parent = file.Directory.Path.Substring(Paths.Input.Path.Length);

                Prefix = string.Join(FileInfo.Separator, parent, Key).Substring(1);

                using (var mesh = ResourceLoader.Load<ArrayMesh>(file.Path))
                {
                    Surfaces = mesh.GetSurfaces().Select(s => s.Key).Filter(k => k.Any()).Freeze();
                }
            }

            public void TryAdd(FileInfo file)
            {
                IEnumerable<Task> CreateTasks(FileInfo f)
                {
                    if (f.Directory != File.Directory || f == File)
                    {
                        return Lst<Task>.Empty;
                    }

                    var m = NamePattern.Match(f.Name);

                    if (!m.Success || m.Groups[1].Value != Key)
                    {
                        return Lst<Task>.Empty;
                    }

                    var key = m.Groups[2].Value;

                    if (key.ToLower().StartsWith("morphs"))
                    {
                        Lst<string> shapes;

                        using (var mesh = ResourceLoader.Load<ArrayMesh>(f.Path))
                        {
                            shapes = mesh.GetBlendShapeNames().Freeze();
                        }

                        string NormalizeName(string name)
                        {
                            const string prefix = "morphs_";

                            if (name.ToLower().StartsWith(prefix))
                            {
                                return name.Substring(prefix.Length);
                            }

                            return name;
                        }

                        return
                            from shape in shapes
                            from surface in Surfaces
                            select new BlendShapeTask(NormalizeName(shape), f, surface, shape, this);
                    }

                    return Surfaces.Map(s => new MorphedMeshTask(key, f, s, this)).Freeze();
                }

                var newTasks = CreateTasks(file);

                _tasks = _tasks.Append(newTasks).Freeze();
            }

            public static Option<MeshSet> TryCreate(
                FileInfo file,
                Paths paths,
                IEnumerable<FileInfo> meshes,
                ILogger logger)
            {
                var matcher = NamePattern.Match(file.Name);

                if (!matcher.Success || matcher.Groups[2].Value.ToLower() != "base")
                {
                    return None;
                }

                var key = matcher.Groups[1].Value;
                var meshSet = Try(new MeshSet(key, file, paths));

                meshSet.ToEither().LeftAsEnumerable().Iter(e =>
                {
                    logger.LogError(e, "Failed to read mesh resource '{}'.", file.Path);
                });

                var result = meshSet.ToOption();

                result.Iter(m => meshes.Iter(m.TryAdd));

                return result;
            }
        }

        private abstract class Task : ListEntry
        {
            public string Surface { get; }

            public MeshSet Parent { get; }

            public DirectoryInfo OutputDir { get; }

            public TaskState State { get; private set; }

            public string Prefix { get; }

            public Task(string key, FileInfo file, string surface, MeshSet parent) : base(key, file)
            {
                Surface = surface;
                Parent = parent;

                var output = parent.Paths.Output.Path;
                var prefix = parent.Prefix;

                OutputDir = new DirectoryInfo(string.Join(FileInfo.Separator, output, prefix));
                Prefix = $"{key} - {Surface}";

                var metadata = OutputDir.GetFile(Prefix + ".json");

                if (metadata.Exists)
                {
                    var outdated = parent.File.LastModified.CompareTo(metadata.LastModified) > 1;
                    State = outdated ? TaskState.NeedsUpdate : TaskState.UpToDate;
                }
                else
                {
                    State = TaskState.New;
                }
            }

            protected abstract IMeshData<MorphableVertex> CreateMorphedMesh(ArrayMesh source, ArrayMesh target);

            public void Run(Node context, ILoggerFactory loggerFactory)
            {
                try
                {
                    var writer = new BlendMapWriter(context, loggerFactory);

                    using (var source = Parent.CreateMesh())
                    {
                        using (var target = CreateMesh())
                        {
                            var blendShape = CreateMorphedMesh(source, target);

                            writer.Write(blendShape, Prefix, OutputDir);
                        }
                    }

                    State = TaskState.UpToDate;
                }
                catch (Exception e)
                {
                    var logger = loggerFactory.CreateLogger(typeof(BlendMapGenerator));

                    logger.Log(LogLevel.Error, e, $"Failed to generate blend map for '{Key}'.");

                    State = TaskState.Error;
                }
            }
        }

        private class MorphedMeshTask : Task
        {
            public MorphedMeshTask(string key, FileInfo file, string surface, MeshSet parent) :
                base(key, file, surface, parent)
            {
            }

            protected override IMeshData<MorphableVertex> CreateMorphedMesh(ArrayMesh source, ArrayMesh target)
            {
                var baseMesh = source.GetSurfaces().Filter(v => v.Key == Surface).Head();
                var morph = target.GetSurfaces().Filter(v => v.Key == Surface).Head();

                return baseMesh.Data.Join(Key, morph.Data);
            }
        }

        private class BlendShapeTask : Task
        {
            public string BlendShape { get; }

            public BlendShapeTask(string key, FileInfo file, string surface, string blendShape, MeshSet parent) :
                base(key, file, surface, parent)
            {
                BlendShape = blendShape;
            }

            protected override IMeshData<MorphableVertex> CreateMorphedMesh(ArrayMesh source, ArrayMesh target)
            {
                var baseMesh = source.GetSurfaces().Filter(v => v.Key == Surface).Head();

                var morph = target.GetSurfaces()
                    .Filter(v => v.Key == Surface)
                    .Bind(v => v.BlendShapes.Find(b => b.Key == BlendShape))
                    .Head();

                return baseMesh.Data.Join(Key, morph);
            }
        }

        private enum TaskState
        {
            UpToDate,
            NeedsUpdate,
            New,
            Error
        }
    }
}
