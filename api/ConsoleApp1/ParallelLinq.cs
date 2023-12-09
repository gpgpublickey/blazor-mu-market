namespace ConsoleApp1
{
    internal class ParallelLinq
    {
        public ParallelLinq(bool isForcingParallelism = false)
        {
            Person[] people = new Person[]
            {
                new Person{City = "Multiverso fantino", Name = "Rontaco"},
                new Person{City = "Jupiter", Name = "Jazzita"},
                new Person{City = "La plata", Name = "Chazzkiboom"},
                new Person{City = "La plata", Name = "Vamoajua"},
                new Person{City = "Rosario", Name = "AnaMariaDeLosDolores"},
                new Person{City = "Rosario", Name = "Uniflornio"},
                new Person{City = "Rosario", Name = "Yoruel"},
                new Person{City = "Villa 31", Name = "Milei"},
                new Person{City = "Capital", Name = "Patricia"},
                new Person{City = "Tigre", Name = "Massa"},
            };
            ParallelQuery<Person> result;

            if (isForcingParallelism)
            {
                result = from person in people.AsParallel()
                         .AsOrdered()
                         .WithDegreeOfParallelism(4)
                         .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                         where person.City == "Rosario"
                         select person;
            }
            else
            {
                result = from person in people.AsParallel()
                             where person.City == "Rosario"
                             select person;
            }

            foreach (var person in result)
                Console.WriteLine(person.Name);

            Console.WriteLine("Finished processing");
        }
    }

    internal class Person
    {
        public string Name { get; set; }

        public string City { get; set; }
    }
}
