using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound
{
    class Program
    {
        private static int[] prices, weights;
        private static double[] values;
        private static int count;
        private static int totalWeight;

        static void Main(string[] args)
        {
            //string[] input = { "20 20 20 20 10 10 10 10 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40", "20 20 20 20 30 30 30 30 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1", "14" };
            //string[] input = { "4 8 4 4 6", "8 13 9 7 12", "12" };
            //string[] input = { "4 6 4 3 1", "8 10 9 4 1", "10" };
            //string[] input = { "8 6 4 2 6", "11 16 10 3 12", "14" };
            string[] input = { "6 4 8 4 6", "8 7 10 5 9", "20" };

            totalWeight = int.Parse(input[2]);
            weights = input[0].Split().Select(int.Parse).ToArray();
            prices = input[1].Split().Select(int.Parse).ToArray();
            count = weights.Length;
            values = new double[count];
            int[] next = new int[count];

            fillValues();
            GenerateNextArray(next);

            PriorityQueue queue = new PriorityQueue();

            double maxValue = values.Max();
            int maxValueIndex = values.ToList().IndexOf(maxValue);

            if (weights[maxValueIndex] <= totalWeight)
                queue.Add(new Vertex(0, maxValueIndex, weights[maxValueIndex],
                    prices[maxValueIndex] + (totalWeight - weights[maxValueIndex]) * values[next[maxValueIndex]], ImmutableList<int>.Empty.Add(maxValueIndex)));
            queue.Add(new Vertex(0, maxValueIndex, 0, totalWeight * values[next[maxValueIndex]], ImmutableList<int>.Empty));

            while (true)
            {

                Vertex v = queue.Pop();

                int nextLevel = v.level + 1;
                if (nextLevel == count)
                {
                    if (v.estimation == 0 && v.cachedMass == 0 && v.prevVertexes.Count == 0)
                    {
                        Console.WriteLine(@"К сожалению, при данной конфигурации вместимости вашего рюкзачка мы не смогли подобрать оптимальный набор вещей. Приходите завтра.");
                        break;
                    }
                    PrintAnswer(v);
                    break;
                }
                var aggregate = v.prevVertexes.Aggregate(0, (sum, x) => sum + prices[x]);
                int nextIndex = next[nextLevel];
                if (nextLevel == count - 1)
                {
                    if (v.cachedMass + weights[nextIndex] <= totalWeight)
                    {
                        queue.Add(new Vertex(nextLevel, nextIndex, v.cachedMass + weights[nextIndex],
                            aggregate +
                            prices[nextIndex],
                            v.prevVertexes.Add(nextIndex)));
                    }
                    //v.cachedMass = 0;

                    queue.Add(new Vertex(nextLevel, nextIndex, v.cachedMass,
                    aggregate/* +  values[next[nextLevel ]]*/, v.prevVertexes));
                }

                else
                {
                    if (v.cachedMass + weights[nextIndex] <= totalWeight)
                    {
                        queue.Add(new Vertex(nextLevel, nextIndex, v.cachedMass + weights[nextIndex],
                            aggregate +
                            prices[nextIndex] +
                            (totalWeight - v.cachedMass - weights[nextIndex]) * values[next[nextLevel + 1]],
                            v.prevVertexes.Add(nextIndex)));
                    }
                    //v.cachedMass = 0;

                    queue.Add(new Vertex(nextLevel, nextIndex, v.cachedMass,
                        aggregate + (totalWeight - v.cachedMass) * values[next[nextLevel + 1]], v.prevVertexes));
                }

            }

            Console.ReadKey();
        }
        private static void PrintAnswer(Vertex v)
        {
            Console.WriteLine(v.ToString());
        }
        private static void GenerateNextArray(int[] next)
        {
            List<Tuple<int, double>> _sort = new List<Tuple<int, double>>();
            for (var i = 0; i < values.Length; i++)
            {
                double d = values[i];
                _sort.Add(new Tuple<int, double>(i, d));
            }

            _sort = _sort.OrderByDescending(x => x.Item2).ToList();

            for (int i = 0; i < _sort.Count; i++)
            {
                next[i] = _sort[i].Item1;
            }

            //next[_sort.Count - 1] = -1;
        }

        static void fillValues()
        {
            for (int i = 0; i < count; i++)
            {
                double v = (double)prices[i] / weights[i];
                values[i] = v;
            }
        }
    }

    class Vertex
    {
        public int level;
        public int index;
        public int cachedMass;
        public double estimation;
        public ImmutableList<int> prevVertexes;

        public Vertex(int level, int index, int cachedMass, double estimation, ImmutableList<int> prevVertexes)
        {
            this.level = level;
            this.index = index;
            this.cachedMass = cachedMass;
            this.estimation = estimation;
            this.prevVertexes = prevVertexes;
        }
        public override string ToString()
        {
            return "Стоимость " + estimation + ", Вес  " + cachedMass + " , Элементы: " + 
                   string.Join(", ", prevVertexes.Select(x => x + 1));
        }
    }

    class PriorityQueue
    {
        private List<Vertex> _list = new List<Vertex>();

        public void Add(Vertex v)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].estimation < v.estimation)
                {
                    _list.Insert(i, v);
                    return;
                }
            }

            _list.Add(v);
        }

        public Vertex Pop()
        {
            Vertex vertex = _list.First();
            _list.Remove(vertex);
            return vertex;
        }

        public Vertex Peak()
        {
            Vertex vertex = _list.First();
            return vertex;
        }

    }
}