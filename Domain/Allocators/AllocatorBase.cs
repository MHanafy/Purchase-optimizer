namespace Gluh.TechnicalTest.Domain
{
    public interface IAllocator
    {
        void Allocate(IRequirementBatch batch);
        /// <summary>
        /// Defines order of execution, lower priority executes first.
        /// </summary>
        int Priority { get; }
    }

    public abstract class AllocatorBase : IAllocator
    {
        public AllocatorBase(int priority = 0)
        {
            Priority = priority;
        }
        public int Priority { get; }

        protected abstract void AllocateAll(IRequirementBatch batch);
  
        public void Allocate(IRequirementBatch batch)
        {
            OnAllocating(batch);
            AllocateAll(batch);
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
