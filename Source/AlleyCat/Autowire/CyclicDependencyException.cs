using System;

namespace AlleyCat.Autowire
{
    public class CyclicDependencyException : InvalidOperationException
    {
        public CyclicDependencyException()
        {
        }

        public CyclicDependencyException(string message) : base(message)
        {
        }
    }
}
