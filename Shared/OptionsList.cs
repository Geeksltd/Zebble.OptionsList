namespace Zebble
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Olive;

    public class OptionsList<TSource> : CollectionView<SelectableItem<TSource>, SelectableItemView<TSource>>, IBindableInput
    {
        readonly Mvvm.CollectionViewModel<SelectableItem<TSource>> Items = new();
        event InputChanged InputChanged;

        public HorizontalAlignment CheckboxAlignment { get; set; }

        public OptionsList()
        {
            base.Source = Items;
            CssClass = "options-list";
        }

        event InputChanged IBindableInput.InputChanged { add => InputChanged += value; remove => InputChanged -= value; }

        public IEnumerable<TSource> Source
        {
            get => Items.Select(v => v.Source.Value);
            set
            {
                Items.Replace(value);
                Items.Do(x => x.Selected.ChangedByInput += () =>
                {
                    if (!MultiSelect && x.Selected.Value)
                        Items.Where(x => x.Selected.Value).Except(x).Do(x => x.Selected.SetByInput(false));

                    InputChanged?.Invoke(nameof(SelectedItem));
                    InputChanged?.Invoke(nameof(SelectedItems));
                });
            }
        }

        public IEnumerable<TSource> SelectedItems
        {
            get => Items.Where(v => v.Selected.Value).Select(v => v.Source.Value);
            set => Items.Do(i => i.Selected.Value = value.OrEmpty().Contains(i.Source.Value));
        }

        public TSource SelectedItem
        {
            get => SelectedItems.FirstOrDefault();
            set => SelectedItems = new[] { value }.ExceptNull();
        }

        public bool MultiSelect { get; set; }
    }
}