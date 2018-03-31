using System;
using System.IO;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class ErrorExtensions
    {
        public static void ThrowIfNecessary(
            this Error error, [CanBeNull] Func<Error, string> message = null)
        {
            if (error == Error.Ok) return;

            var code = Enum.GetName(typeof(Error), error);
            var arg = message?.Invoke(error) ?? $"Operation failed with code: '{code}(error)'";

            Exception exception;

            switch (error)
            {
                case Error.Unauthorized:
                case Error.FileNoPermission:
                    exception = new UnauthorizedAccessException(arg);
                    break;
                case Error.ParameterRangeError:
                    exception = new ArgumentOutOfRangeException(null, arg);
                    break;
                case Error.OutOfMemory:
                    exception = new OutOfMemoryException(arg);
                    break;
                case Error.FileBadDrive:
                case Error.FileBadPath:
                case Error.FileNotFound:
                    exception = new FileNotFoundException(arg);
                    break;
                case Error.FileAlreadyInUse:
                case Error.FileCantOpen:
                case Error.FileCantRead:
                case Error.FileCorrupt:
                case Error.FileMissingDependencies:
                case Error.FileUnrecognized:
                    exception = new FileLoadException(arg);
                    break;
                case Error.FileEof:
                    exception = new EndOfStreamException(arg);
                    break;
                case Error.FileCantWrite:
                case Error.CantAcquireResource:
                case Error.CantOpen:
                case Error.CantCreate:
                case Error.AlreadyInUse:
                case Error.Locked:
                    exception = new IOException(arg);
                    break;
                case Error.Timeout:
                    exception = new TimeoutException(arg);
                    break;
                case Error.InvalidData:
                    exception = new InvalidDataException(arg);
                    break;
                case Error.InvalidParameter:
                    exception = new ArgumentException(null, arg);
                    break;
                default:
                    exception = new InvalidOperationException(arg);
                    break;
            }

            throw exception;
        }
    }
}
