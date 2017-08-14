namespace Zebble
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class OptionsList : Canvas, FormField.IControl
    {
        RepeatDirection direction;
        public OptionsDataSource Source { get; internal set; } = new OptionsDataSource();
        public readonly AsyncEvent<Option> SelectedItemChanged = new AsyncEvent<Option>(ConcurrentEventRaisePolicy.Queue);
        public readonly ListView<OptionsDataSource.DataItem, Option> List = new ListView<OptionsDataSource.DataItem, Option>();

        public OptionsList()
        {
            Direction = RepeatDirection.Vertical;
        }

        public object Value
        {
            get => Source.Value;
            set { Source.Value = value; LoadExistingData(); }
        }

        public IEnumerable<object> DataSource
        {
            get => Source.DataSource;
            set => Source.DataSource = value;
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
            await List.UpdateSource(Source.Items);
            List.ItemViews.Do(v => v.SelectedChanged.Handle(() => SelectionChanged(v)));

            await Add(List);

            LoadExistingData();
        }

        void LoadExistingData()
        {
            if (List == null) return;

            List.ItemViews
                .Where(x => Source.SelectedValues.Contains(x.Value))
                .Except(x => x.IsSelected)
                .Do(x => x.IsSelected = true);
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

        public class Option : Stack, IListViewItem<OptionsDataSource.DataItem>
        {
            OptionsList Container => FindParent<OptionsList>();
            public readonly CheckBox CheckBox = new CheckBox { Id = "CheckBox" };
            public readonly TextView Label = new TextView { Id = "Label" };
            public OptionsDataSource.DataItem Item { get; set; }
            public readonly AsyncEvent SelectedChanged = new AsyncEvent(ConcurrentEventRaisePolicy.Queue);

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

            public object Value => Item.Value;

            public string Text => Item.Text;

            public override async Task OnInitializing()
            {
                await base.OnInitializing();

                await Add(Label.Set(x => { x.Text = Item.Text; }));
                await Add(CheckBox.Set(c => c.CheckedChanged.Handle(OnCheckedChanged)));

                Tapped.Handle(x => CheckBox.RaiseTapped());
            }

            async Task OnCheckedChanged()
            {
                Item.Selected = CheckBox.Checked;

                await SetPseudoCssState("checked", IsSelected);
                await SelectedChanged.Raise();
            }

            public override void Dispose()
            {
                SelectedChanged?.Dispose();
                base.Dispose();
            }
        }
    }
}