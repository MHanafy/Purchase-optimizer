﻿using System;

namespace Gluh.TechnicalTest.Domain
{
    public interface IAllocator
    {
        void Allocate(IRequirementBatch batch);
        /// <summary>
        /// Defines order of execution, lower priority executes first.
        /// </summary>
        int Priority { get; }
        int Complexity { get; }
        event EventHandler<ProgressEventArgs> Progress;
    }

    public abstract class AllocatorBase : IAllocator
    {
        public AllocatorBase(int priority = 0, int complexity = 1)
        {
            Priority = priority;
            Complexity = complexity;
        }
        public int Priority { get; }
        /// <summary>
        /// Used to indicate complexity, for progress reporting.
        /// </summary>
        public int Complexity { get; }

        public event EventHandler<ProgressEventArgs> Progress;
        protected void OnProgress(bool done, long current = 1, long total = 1, string activity = null)
        {
            Progress?.Invoke(this, new ProgressEventArgs(done, current, total, activity));
        }

        protected abstract void AllocateAll(IRequirementBatch batch);
  
        public void Allocate(IRequirementBatch batch)
        {
            OnAllocating(batch);
            AllocateAll(batch);
            OnProgress(true);
        }

        /// <summary>
        /// Only override this if Allocate gets called from derived code
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        protected virtual void OnAllocating(IRequirementBatch batch) { }

    }
}
