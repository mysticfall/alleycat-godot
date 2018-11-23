using System;
using Godot;

namespace AlleyCat.Common
{
    public class ValidationException : NodeException
    {
        protected ValidationException(string message, Node node) : base(message, node)
        {
        }

        protected ValidationException(string message, Exception innerException, Node node) :
            base(message, innerException, node)
        {
        }
    }
}
