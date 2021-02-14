using System.Threading.Tasks;
using Olive;

namespace Zebble
{
    public class SelectableItemView<TSource> : Stack, ITemplate<SelectableItem<TSource>>
    {
        public SelectableItem<TSource> Model;

        CheckBox CheckBox = new CheckBox().Id("CheckBox");
        TextView Label = new TextView().Id("Label");

        public SelectableItemView() => Direction = RepeatDirection.Horizontal;

        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            await Add(CheckBox);
            await Add(Label);

            Label.Bind("Text", () => Model.Text);
            CheckBox.Bind("Checked", () => Model.Selected);
        }
    }
}