using AvalonStudio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using AvalonStudio.Extensibility;
using AvalonStudio.Shell;

namespace AvalonStudio.Scripting
{
    public class PythonShell
    {
        private IConsole console;
        private ScriptEngine engine;
        private ScriptScope scope;
        private List<string> commandHistory;
        private int selectedIndex = 0;
        
        public void Initialise()
        {
            scope.SetVariable("shell", IoC.Get<IShell>());
        }
        
        public PythonShell(IConsole console)
        {
            this.console = console;
            commandHistory = new List<string>();

            engine = Python.CreateEngine();
            scope = engine.CreateScope();            

            Prompt();        
        }

        public void ProcessLine (string line)
        {
            console.WriteLine();
            var command = line;

            if(command != string.Empty)
            {
                commandHistory.Add(command);
                try
                {
                    var result = engine.Execute(command, scope);
                }
                catch(Exception e)
                {
                    console.WriteLine(e.Message);
                }
            }
            
            Prompt();
        }

        private void Prompt()
        {
            
        }

        public void Editor_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {            
            if (e.Modifiers == Avalonia.Input.InputModifiers.None)
            {
                switch (e.Key)
                {
                    case Avalonia.Input.Key.Enter:
                        selectedIndex = 0;
                        ProcessLine(console.ReadLine());
                        e.Handled = true;
                        break;

                    case Avalonia.Input.Key.Up:
                        console.OverWrite(commandHistory[commandHistory.Count - selectedIndex - 1]);
                        selectedIndex++;
                        e.Handled = true;
                        break;

                    case Avalonia.Input.Key.Down:
                        selectedIndex--;
                        console.OverWrite(commandHistory[commandHistory.Count - selectedIndex - 1]);
                        e.Handled = true;
                        break;

                    case Avalonia.Input.Key.Back:
                        e.Handled = true;
                        break;

                    default:
                        break;

                }
            }
        }        
    }
}
