using System;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Autowire
{
    [NonInjectable]
    public class AutowiredNode : BaseNode
    {
        public override void _Ready()
        {
            base._Ready();

            try
            {
                this.Autowire();
            }
            catch (Exception e)
            {
                GD.Print(e.ToString());
            }
        }
    }
}
