using System;

namespace Gluh.TechnicalTest.Domain.BruteForce
{
    //part of below code is adapted from https://www.techtalkz.com/threads/all-possible-combinations-of-multiple-lists.77236/,
    public class Premuting<T>
    {
        public Premuting(T[][] data)
        {
            _data = data;
            long length = 1;
            for (int i = 0; i < _data.Length; i++)
            {
                length *= _data[i].Length;
            }
            _total = length;
        }
        private readonly T[][] _data;
        private long _total;

        private T[][] _premutations;
        private long _index;
        public delegate void Processor(T[] a);

        /// <summary>
        /// Computes premutations once, and caches it for subsequent calls.
        /// </summary>
        /// <returns></returns>
        public T[][] GetPremutations()
        {
            if (_premutations != null) return _premutations;
            if(_total > int.MaxValue)
            {
                throw new InvalidOperationException($"Can't process mutations that exceed {int.MaxValue}, use {nameof(Premute)} instead.");
            }
            _premutations = new T[_total][];
            Premute(_data);
            return _premutations;
        }

        /// <summary>
        /// Runs the premutation and executes provided action for each result, without storing the results
        /// </summary>
        /// <param name="action"></param>
        public void Premute(Action<T[], long, long> action)
        {
            current = 0;
            Premute(new T[_data.Length], 0, _data, action);
        }

        private void Premute(T[] result, int index, T[][] data, Action<T[], long, long> action)
        {
            foreach (T v in data[index])
            {
                result[index] = v;
                if (index >= data.Length - 1)
                {
                    current++;
                    action((T[]) result.Clone(), current, _total);
                }
                else
                {
                    Premute(result, index + 1, data, action);
                }
            }
        }

        long current;
        private void Premute(T[][] data)
        {
            current = 0;
            Premute(new T[data.Length], 0, data, AddMutation);
        }

        private void AddMutation(T[] mutation, long current, long total)
        { 
            _premutations[_index] = mutation;
            _index++;
        }

    }
}
