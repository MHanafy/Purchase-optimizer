using System;

namespace Gluh.TechnicalTest.Domain.BruteForce
{
    //To save time, majority of below code is adapted from https://www.techtalkz.com/threads/all-possible-combinations-of-multiple-lists.77236/,
    public class Premuting<T>
    {
        public Premuting(T[][] data)
        {
            _data = data;
        }
        private readonly T[][] _data;

        private T[][] _premutations;
        private int _index;
        public delegate void Processor(T[] a);

        /// <summary>
        /// Computes premutations once, and caches it for subsequent calls.
        /// </summary>
        /// <returns></returns>
        public T[][] GetPremutations()
        {
            if (_premutations != null) return _premutations;
            var length = 1;
            for (int i = 0; i < _data.Length; i++)
            {
                length *= _data[i].Length;
            }
            _premutations = new T[length][];
            Premute(_data);
            return _premutations;
        }

        /// <summary>
        /// Runs the premutation and executes provided action for each result, without storing the results
        /// </summary>
        /// <param name="action"></param>
        public void Premute(Action<T[]> action)
        {
            Premute(new T[_data.Length], 0, _data, action);
        }

        private void Premute(T[] result, int index, T[][] data, Action<T[]> action)
        {
            foreach (T v in data[index])
            {
                result[index] = v;
                if (index >= data.Length - 1)
                {
                    action((T[]) result.Clone());
                }
                else
                {
                    Premute(result, index + 1, data, action);
                }
            }
        }
        private void Premute(T[][] data)
        {
            Premute(new T[data.Length], 0, data, AddMutation);
        }

        private void AddMutation(T[] mutation)
        { 
            _premutations[_index] = mutation;
            _index++;
        }

    }
}
