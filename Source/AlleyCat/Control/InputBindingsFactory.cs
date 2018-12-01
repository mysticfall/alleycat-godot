using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    [AutowireContext]
    public class InputBindingsFactory : GameObjectFactory<InputBindings>
    {
        [Export]
        public bool Active { get; set; } = true;

        [Service(local: true)]
        public IEnumerable<IInput> Inputs { get; set; }

        protected override Validation<string, InputBindings> CreateService(ILoggerFactory loggerFactory) =>
            new InputBindings(Inputs, Active, loggerFactory);
    }
}
