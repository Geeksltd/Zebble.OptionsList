namespace Zebble
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    public class SelectableItem<TSource> : Mvvm.ViewModel<TSource>
    {
        public Bindable<string> Text => Source.Get(x => x.ToString());
        public readonly Bindable<bool> Selected = new();
    }


}