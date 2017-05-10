using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGame
{
    public class stack
    {
        private int StackSize;
        public int StackSizeSet
        {
            get { return StackSize; }
            set { StackSize = value; }
        }
        public int top;
        public Cell[] C;
        public stack()
        {
            StackSizeSet = 162;
            C = new Cell[StackSizeSet];
            top = -1;
        }
        public stack(int capacity)
        {
            StackSizeSet = capacity;
            C = new Cell[StackSizeSet];
            top = -1;
        }
        public bool isEmpty()
        {
            if (top == -1) return true;

            return false;
        }
        public void Push(Cell element)
        {
            if (top == (StackSize - 1))
            {
                //  Console.WriteLine("Stack is full!");
            }

            else
            {

                C[++top] = element;
                // Console.WriteLine("Item pushed successfully!");
            }
        }
        public object Pop()
        {
            if (isEmpty())
            {
                //     Console.WriteLine("Stack is empty!");
                return "No elements";
            }
            else
            {
                //  Console.WriteLine("POPED");
                //  Console.Write("TOP:");
                //     Console.Write(top);

                return C[top--];
            }
        }
        public object Peak()
        {
            if (isEmpty())
            {

                //   Console.WriteLine("Stack is empty!");
                return "No elements";
            }
            else
            {
                return C[top];
            }
        }
    }
}
