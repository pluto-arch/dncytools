using System;

namespace Dotnetydd.Tools.Core.RandomSelector
{
    public class SingleSelector<T> : SelectorBase<T>
    {
        internal SingleSelector(WeightedSelector<T> weightedSelector) : base(weightedSelector)
        {
        }

        internal T Select()
        {
            if (WeightedSelector.Items.Count == 0)
            {
                throw new InvalidOperationException("没有元素可以筛选");
            }

            return BinarySelect(WeightedSelector.Items).Value;
        }
    }
}
