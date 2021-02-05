namespace Zebble
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    public class OptionsList : Canvas, FormField.IControl, IBindableInput
    {
        RepeatDirection direction;
        public OptionsDataSource Source { get; set; } = new();
        public readonly AsyncEvent<Option> SelectedItemChanged = new(ConcurrentEventRaisePolicy.Queue);
        public readonly OptionsListView List = new();

        public OptionsList() => Direction = RepeatDirection.Vertical;

        public object Value
        {
            get => Source.Value;
            set
            {
                SetDefaultValues(value);
                Source.Value = value;
                LoadExistingData();
            }
        }

        private void SetDefaultValues(object value)
        {
            if (value == null)
                List.ItemViews.Where(x => x.IsSelected).Do(x => x.UnSelectOption());
            else if (Source.Value != value)
                List.ItemViews.Where(x => x.Value == Source.Value).Do(x => x.UnSelectOption());
        }

        public IEnumerable<object> DataSource
        {
            get => Source.DataSource;
            set => Source.DataSource = value;
        }

        public IBindable BindableSource
        {
            set => value.AddBinding(this, nameof(DataSource));
        }

        public bool MultiSelect
        {
            get => Source.MultiSelect;
            set => Source.MultiSelect = value;
        }

        public RepeatDirection Direction
        {
            get => direction;
            set
            {
                direction = value;
                SetPseudoCssState("horizontal", value == RepeatDirection.Horizontal).RunInParallel();
                SetPseudoCssState("vertical", value == RepeatDirection.Vertical).RunInParallel();
            }
        }

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            List.Direction = Direction;
            List.SelectionChangedHandler = row => SelectionChanged(row);
            List.ListItemsShownHandler = async () => await LoadExistingData();

            await Add(List);
        }

        async Task LoadExistingData()
        {
            if (List == null) return;

            //var toShow = Source.Items.Where(i => Source.SelectedValues.Contains(i.Value)).ToList();
            //toShow.AddRange(Source.Items.Except(i => Source.SelectedValues.Contains(i.Value)));

            if (Source.Items.Any()) await List.UpdateSource(Source.Items);

            List.ItemViews
                .Where(x => Source.SelectedValues.Contains(x.Value))
                .Except(x => x.IsSelected)
                .Do(x => x.SelectOption());
        }

        Task SelectionChanged(Option row)
        {
            if (row.IsSelected && !Source.MultiSelect)
            {
                List.ItemViews.Except(row).Where(c => c.IsSelected)
                    .Do(v => v.IsSelected = false);
            }

            return SelectedItemChanged.Raise(row);
        }

        public override void Dispose()
        {
            SelectedItemChanged?.Dispose();
            base.Dispose();
        }

        public void AddBinding(Bindable bindable) => SelectedItemChanged.Handle(() => bindable.SetUserValue(Value));

        public class Option : Stack, IRecyclerListViewItem<OptionsDataSource.DataItem>
        {
            OptionsList Container => FindParent<OptionsList>();
            bool IsProgrammaticallySelection;
            public readonly CheckBox CheckBox = new() { Id = "CheckBox" };
            public readonly TextView Label = new() { Id = "Label" };
            public Bindable<OptionsDataSource.DataItem> Item { get; } = new();
            OptionsDataSource.DataItem IListViewItem<OptionsDataSource.DataItem>.Item { get => Item.Value; set => Item.Set(value); }

            public readonly AsyncEvent SelectedChanged = new(ConcurrentEventRaisePolicy.Queue);

            public Option() : base(RepeatDirection.Horizontal) { }

            public override async Task ApplyStyles()
            {
                await base.ApplyStyles();

                if (Container.Direction == RepeatDirection.Horizontal)
                {
                    CheckBox.Alignment = Alignment.Left;
                    Label.AutoSizeWidth = true;
                }
                else
                {
                    Label.AutoSizeWidth = false;
                    CheckBox.Alignment = Alignment.Right;
                }
            }

            public bool IsSelected
            {
                get => CheckBox.Checked;
                set
                {
                    if (CheckBox.Checked == value) return;
                    CheckBox.Checked = value;
                    OnCheckedChanged().RunInParallel();
                }
            }

            public object Value => Item.Value.Value;

            public string Text => Item.Value.Text;

            public override async Task OnInitializing()
            {
                await base.OnInitializing();

                await Add(Label.Bind(nameof(Label.Text), () => Item, x => x.Text));
                await Add(CheckBox.Set(c => c.CheckedChanged.Handle(OnCheckedChanged)));

                Tapped.Handle(x => CheckBox.RaiseTapped());
                LongPressed.Handle(x => CheckBox.RaiseLongPressed(x));
            }

            async Task OnCheckedChanged()
            {
                if (!Item.Value.Selected && CheckBox.Checked && IsProgrammaticallySelection)
                    IsProgrammaticallySelection = false;

                Item.Value.Selected = CheckBox.Checked;

                await SetPseudoCssState("checked", IsSelected);
                if (!IsProgrammaticallySelection) await SelectedChanged.Raise();
            }

            public void SelectOption() { IsProgrammaticallySelection = true; IsSelected = true; }
            public void UnSelectOption() { IsProgrammaticallySelection = true; IsSelected = false; }

            public override void Dispose()
            {
                SelectedChanged?.Dispose();
                base.Dispose();
            }
        }

        public class OptionsListView : RecyclerListView<OptionsDataSource.DataItem, Option>
        {
            public Action<Option> SelectionChangedHandler { get; set; }
            public Action ListItemsShownHandler { get; set; }

            public OptionsListView() : base()
            {
                Shown.AwaitRaiseCompletion().ContinueWith(result =>
                {
                    if (result.IsCompleted) ListItemsShownHandler?.Invoke();
                });
            }

            public override async Task<TView> Add<TView>(TView child, bool awaitNative = false)
            {
                var result = await base.Add(child, awaitNative);

                var option = result as Option;
                if (option != null) option.SelectedChanged.Handle(() => SelectionChangedHandler(option));

                return result;
            }
        }
    }
}