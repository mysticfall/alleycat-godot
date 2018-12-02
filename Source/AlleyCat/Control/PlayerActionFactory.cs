using System;
using AlleyCat.Common;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public abstract class PlayerActionFactory<T> : InputActionFactory<T> where T : PlayerAction
    {
        protected override Validation<string, T> CreateService(
            string key, string displayName, ITriggerInput input, ILoggerFactory loggerFactory)
        {
            return CreateService(key, displayName, this.FindClosestAncestor<IPlayerControl>, input, loggerFactory);
        }

        protected abstract Validation<string, T> CreateService(
            string key, 
            string displayName, 
            Func<Option<IPlayerControl>> control, 
            ITriggerInput input, 
            ILoggerFactory loggerFactory);
    }
}
