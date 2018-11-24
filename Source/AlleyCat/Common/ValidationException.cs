using System;
using Godot;

namespace AlleyCat.Common
{
    public class ValidationException : NodeException
    {
        public ValidationException(string message, Node node) : base(message, node)
        {
        }

        public ValidationException(string message, Exception innerException, Node node) :
            base(message, innerException, node)
        {
        }
    }
}
