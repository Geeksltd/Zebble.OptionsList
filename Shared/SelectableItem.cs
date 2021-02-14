namespace Zebble
{
    using Olive;

    public class SelectableItem<TSource> : Mvvm.ViewModel<TSource>
    {
        public Bindable<string> Text => Source.Get(x => x.ToStringOrEmpty());
        public readonly TwoWayBindable<bool> Selected = new();
    }
}