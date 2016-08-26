using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvalonStudio.TextEditor;

namespace AvalonStudio.Scripting
{
	public class PythonShellView : UserControl
	{
		public PythonShellView()
		{
			InitializeComponent();

            var editor = this.FindControl<TextEditor.TextEditor>("Console");
            editor.AddHandler(KeyDownEvent, Editor_KeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
		}

        private void Editor_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if(DataContext as PythonShellViewModel != null)
            {
                (DataContext as PythonShellViewModel).Model?.Editor_KeyDown(sender, e);
            }
        }

        private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
