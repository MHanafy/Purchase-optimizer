
namespace Gluh.TechnicalTest.Domain.Allocators
{
    /// <summary>
    /// Moves all requirements that can't be fulfilled into Unfulfilled.
    /// </summary>
    public class NostockAllocator : AllocatorBase
    {
        public NostockAllocator(int priority = 50) : base(priority)
        {
        }
        protected override void AllocateAll(IRequirementBatch batch)
        {
            batch.ProcessNoStock();
        }
    }
}
