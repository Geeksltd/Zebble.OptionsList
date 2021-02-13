using System.Threading.Tasks;

namespace Zebble
{
    public class SelectableItemView<TSource> : Stack, ITemplate<SelectableItem<TSource>>
    {
        public SelectableItem<TSource> Model = new SelectableItem<TSource>();

        CheckBox CheckBox = new CheckBox().Id("CheckBox");
        TextView Label = new TextView().Id("Label");

        public SelectableItemView()
        {
            Direction = RepeatDirection.Horizontal;
            Label.Bind("Text", () => Model.Text);
            CheckBox.Bind("Checked", () => Model.Selected);
        }

        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            await Add(CheckBox);
            await Add(Label);
        }
    }
}