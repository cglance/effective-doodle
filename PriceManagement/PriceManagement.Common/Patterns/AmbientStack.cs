using System;
using System.Collections.Generic;
using System.Threading;

namespace PriceManagement.Common.Patterns
{
    public class AmbientStack<T>
    {
        private readonly AsyncLocal<Stack<T>> _ambient = new AsyncLocal<Stack<T>>();

        public T Peek()
        {
            Stack<T> stack = _ambient.Value;
            if (stack == null)
            {
                throw new InvalidOperationException("Cannot Peek an empty stack");
            }
            
            return stack.Peek();
        }

        public bool TryPeek(out T top)
        {
            Stack<T> stack = _ambient.Value;
            if (stack != null && stack.Count > 0)
            {
                top = stack.Peek();
                return true;
            }

            top = default(T);
            return false;
        }

        public T Pop()
        {
            Stack<T> stack = _ambient.Value;
            if (stack == null)
            {
                throw new InvalidOperationException("Cannot Pop an empty stack");
            }

            T top = stack.Pop();

            if (stack.Count == 0)
            {
                _ambient.Value = null;
            }

            return top;
        }

        public bool TryPop(out T top)
        {
            Stack<T> stack = _ambient.Value;
            if (stack != null && stack.Count > 0)
            {
                top = stack.Pop();

                if (stack.Count == 0)
                {
                    _ambient.Value = null;
                }

                return true;
            }

            top = default(T);
            return false;
        }

        public void Push(T value)
        {
            var stack = _ambient.Value;
            if (stack == null)
            {
                _ambient.Value = stack = new Stack<T>();
            }
            stack.Push(value);
        }
    }
}
