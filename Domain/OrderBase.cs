using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Gluh.TechnicalTest.Domain
{
    public interface IOrderBase<T>
        where T : IOrderLineBase
    {
        IImmutableList<T> Lines { get; }
        void Add(T line);
    }

    public abstract class OrderBase<T> : IOrderBase<T>
          where T : IOrderLineBase
    {
        public OrderBase()
        {
            _lines = new List<T>();
        }
        public OrderBase(IEnumerable<T> lines)
        {
            _lines = lines.ToList();
        }
        protected List<T> _lines;
        public IImmutableList<T> Lines => _lines.ToImmutableList();

        public void Add(T line)
        {
            _lines.Add(line);
            OnLineAdded(line);
        }

        protected virtual void OnLineAdded(T line) { }
    }

    public interface IUnfulfilledOrder : IOrderBase<IOrderLineBase> { }

    public class UnfulfilledOrder : OrderBase<IOrderLineBase>, IUnfulfilledOrder
    {
        public UnfulfilledOrder() : base() { }
        public UnfulfilledOrder(IEnumerable<IOrderLineBase> lines) : base(lines) { }
    }

}
