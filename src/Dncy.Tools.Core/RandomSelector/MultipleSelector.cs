﻿using System;
using System.Collections.Generic;

namespace DotnetGeek.Tools
{
    public class MultipleSelector<T> : SelectorBase<T>
    {
        internal MultipleSelector(WeightedSelector<T> weightedSelector) : base(weightedSelector)
        {
        }

        internal List<T> Select(int count)
        {
            Validate(ref count);
            var items = new List<WeightedItem<T>>(WeightedSelector.Items);
            var resultList = new List<T>();

            do
            {
                var item = WeightedSelector.Option.AllowDuplicate ? BinarySelect(items) : LinearSelect(items);
                resultList.Add(item.Value);
                if (!WeightedSelector.Option.AllowDuplicate)
                {
                    items.Remove(item);
                }
            } while (resultList.Count < count);
            return resultList;
        }

        private void Validate(ref int count)
        {
            if (count <= 0)
            {
                throw new InvalidOperationException("The number of filtered items must be greater than 0.");
            }

            var items = WeightedSelector.Items;

            if (items.Count == 0)
            {
                throw new InvalidOperationException("No elements can be filtered.");
            }

            if (!WeightedSelector.Option.AllowDuplicate && items.Count < count)
            {
                count = items.Count;
            }
        }
    }
}
