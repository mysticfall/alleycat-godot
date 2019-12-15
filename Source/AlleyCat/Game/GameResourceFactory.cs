using System;
using System.Linq;
using AlleyCat.Common;
using AlleyCat.Game.Generic;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    public abstract class GameResourceFactory<T> : Resource, IGameResourceFactory<T> where T : IGameResource
    {
        public Validation<string, T> Service => _service.IfNone((_service = CreateResource()).Head());

        Validation<string, object> IServiceFactory.Service => Service.Map(v => (object) v);

        public Validation<string, string> ValidateName =>
            ResourceName.TrimToOption().ToValidation("Missing resource name.");

        private Option<Validation<string, T>> _service;

        protected abstract Validation<string, T> CreateResource();

        protected override void Dispose(bool disposing)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            _service
                .Bind(s => s.SuccessAsEnumerable())
                .OfType<IDisposable>()
                .Iter(s => s.DisposeQuietly());

            _service = Fail<string, T>("The factory has been disposed.");

            base.Dispose(disposing);
        }
    }
}
