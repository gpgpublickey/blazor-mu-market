namespace ConsoleApp1
{
    internal class ParallelInvoke
    {
        public ParallelInvoke()
        {
            Parallel.Invoke(() => Task1(), () => Task2());
        }

        private void Task2()
        {
            Console.WriteLine("Task 1 in parallel invoke starting");
            Thread.Sleep(1000);
            Console.WriteLine("Task 1 in parallel invoke finishing");
        }

        private void Task1()
        {
            Console.WriteLine("Task 2 in parallel invoke starting");
            Thread.Sleep(2000);
            Console.WriteLine("Task 3 in parallel invoke finishing");
        }
    }
}
