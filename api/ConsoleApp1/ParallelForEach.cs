namespace ConsoleApp1
{
    internal class ParallelForEach
    {
        public ParallelForEach()
        {
            var items = Enumerable.Range(0, 100);
            Parallel.ForEach(items, i =>
            {
                WorkOnItem(i);
            });
        }

        private void WorkOnItem(int i)
        {
            Console.WriteLine($"Displaying number {i}");
        }
    }
}
