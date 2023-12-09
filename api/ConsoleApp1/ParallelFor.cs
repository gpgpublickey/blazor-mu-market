using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class ParallelFor
    {
        public ParallelFor()
        {
            var items = Enumerable.Range(0, 50).ToArray();

            static void Foo(int i)
            {
                Console.WriteLine("Task " + i);
            }

            var res = Parallel.For(0, items.Count(), (int i, ParallelLoopState s) =>
            {
                Foo(items[i]);
                if (i == 22 && !s.IsStopped)
                {
                    s.Break(); // Breaks the loop ensuring previous sequential values are iterated
                    Foo(items[i]);
                    Console.WriteLine("Loop finish through Break ParallelLoopState");
                }
                else if (i == 33)
                {
                    s.Stop(); // Breaks the loop without ensure previous values being iterated
                    Foo(items[i]);
                    Console.WriteLine("Loop finish through Stop ParallelLoopState");
                }
            });
        }
    }
}
