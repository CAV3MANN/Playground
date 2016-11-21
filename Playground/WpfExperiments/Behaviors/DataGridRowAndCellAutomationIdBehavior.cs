using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace WpfExperiments.Behaviors
{
    //Usage:
    //xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
    //<DataGrid ItemsSource = "{Binding Collection}"
    //          AutomationProperties.AutomationId="DG1">
    //    <i:Interaction.Behaviors>
    //        <local:DataGridRowAndCellAutomationIdBehavior/>
    //    </i:Interaction.Behaviors>
    //</DataGrid>

    //This whole behavior could (most likely) be accomplished through proper styling
    public sealed class DataGridRowAndCellAutomationIdBehavior : Behavior<DataGrid>
    {
        private string dataGridAutomationId = "";

        protected override void OnAttached()
        {
            base.OnAttached();
            var dataGrid = AssociatedObject as DataGrid;
            dataGridAutomationId = dataGrid.GetValue(AutomationProperties.AutomationIdProperty) as string;
            dataGrid.LoadingRow += AddAutomationIdToRowHandler;
            AddStyleToDataGridCellStyle(dataGrid);
        }

        private void AddStyleToDataGridCellStyle(DataGrid dataGrid)
        {
            var automationIdSetter = CreateAutomationIdStyleForDataGridCells();

            if (dataGrid.CellStyle == null)
            {
                dataGrid.CellStyle = new Style();
            }

            dataGrid.CellStyle.Setters.Add(automationIdSetter);
        }

        private void AddAutomationIdToRowHandler(object sender, DataGridRowEventArgs e)
        {
            var rowIndex = e.Row.GetIndex();
            e.Row.SetValue(AutomationProperties.AutomationIdProperty, $"DG:{dataGridAutomationId}|R:{rowIndex}");
        }

        private Setter CreateAutomationIdStyleForDataGridCells()
        {
            var rowAutomationIdBinding = new Binding("(AutomationProperties.AutomationId)");
            rowAutomationIdBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1);

            var columnIndexBinding = new Binding("Column.DisplayIndex");
            columnIndexBinding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);

            var multiBinding = new MultiBinding();
            multiBinding.Bindings.Add(rowAutomationIdBinding);
            multiBinding.Bindings.Add(columnIndexBinding);
            multiBinding.StringFormat = "{0}|C:{1}";

            return new Setter(AutomationProperties.AutomationIdProperty, multiBinding);
        }
    }
}
