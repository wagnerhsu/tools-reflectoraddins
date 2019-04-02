using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Reflector;
using Reflector.CodeModel;

namespace AssemblyCollection.Order
{
    /// <summary>
    /// Orders the Assembly list by alphabetical order
    /// </summary>
    public class OrderPackage : IPackage
    {
        private IAssemblyManager assemblyManager;
        private ICommandBarManager commandBarManager;
        private ICommandBarSeparator separator;
        private ICommandBarButton button;

        /// <summary>
        /// Loads the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public void Load(IServiceProvider serviceProvider)
        {
            // Get AssemblyManager reference for reordering
            this.assemblyManager = (IAssemblyManager)serviceProvider.GetService(typeof(IAssemblyManager));

            // Create a command button, separator on the Tools menu and wire up the button click event
            this.commandBarManager = (ICommandBarManager)serviceProvider.GetService(typeof(ICommandBarManager));
            this.separator = this.commandBarManager.CommandBars["Tools"].Items.AddSeparator();
            this.button = this.commandBarManager.CommandBars["Tools"].Items.AddButton("Reorder Assemblies", 
                new EventHandler(this.OrderAssemblyListButton_Click), Keys.Control | Keys.Shift | Keys.R);
        }

        /// <summary>
        /// Unloads this instance.
        /// </summary>
        public void Unload()
        {
            this.commandBarManager.CommandBars["Tools"].Items.Remove(this.button);
            this.commandBarManager.CommandBars["Tools"].Items.Remove(this.separator);
        }

        /// <summary>
        /// Handles the Click event of the OrderAssemblyListButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OrderAssemblyListButton_Click(object sender, EventArgs e)
        {
            IAssemblyCollection assemblies = this.assemblyManager.Assemblies;
            SortedList<string, IAssembly> newAssemblyOrder = new SortedList<string, IAssembly>();

            // Add to new SortedList
            foreach(IAssembly assembly in assemblies)
            {
                // Will filter out any duplicate assemblies if they somehow exist
                string key = assembly.ToString();
                if (!newAssemblyOrder.ContainsKey(key))
                {
                    newAssemblyOrder.Add(key, assembly);    
                }
            }

            assemblies.Clear();
            // Remove each item and add in order
            foreach (KeyValuePair<string, IAssembly> item in newAssemblyOrder)
            {
                assemblies.Add(item.Value);
            }
            
        }
    }
}
