using System.Threading.Tasks;
using Olive;

namespace Zebble
{
    public class SelectableItemView : Stack
    {
        public SelectableItemView() => Direction = RepeatDirection.Horizontal;
        public readonly CheckBox CheckBox = new CheckBox().Id("CheckBox");
        public readonly TextView Label = new TextView().Id("Label");
    }

    public class SelectableItemView<TSource> : SelectableItemView, ITemplate<SelectableItem<TSource>>
    {
        public SelectableItem<TSource> Model;

        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            Label.Bind("Text", () => Model.Text).On(x => x.Tapped, () => CheckBox.UserToggled());
            // CheckBox.Bind("Checked", () => Model.Selected);

            (CheckBox as IBindableInput).InputChanged += prop => Model.Selected.SetByInput(CheckBox.Checked);
            CheckBox.CheckedChanged.Event += () => Model.Selected.Set(CheckBox.Checked);

            var rightSide = FindParent<OptionsList<TSource>>()?.CheckboxAlignment == HorizontalAlignment.Right;

            if (rightSide)
            {
                await Add(Label);
                await Add(CheckBox);
            }
            else
            {
                await Add(CheckBox);
                await Add(Label);
            }
        }
    }
}