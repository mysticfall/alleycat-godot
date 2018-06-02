using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    [Singleton(typeof(EntityLabel))]
    public class EntityLabel : Panel
    {
        [Node]
        public Label Title { get; private set; }

        public virtual void Show([CanBeNull] IEntity entity)
        {
            Visible = entity != null;

            Title.Text = entity?.DisplayName;
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
