using System;
using System.Windows.Forms;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    [Serializable]
    public partial class ScriptEditor : UserControl
    {

        private WFVariableDefCollection variableCollection;



        public ScriptEditor()
        {
            InitializeComponent();
        }



        public string Script
        {
            get
            {
                return scriptTextBox.Text;
            }
            set
            {
                scriptTextBox.Text = value;
            }
        }

        public WFVariableDefCollection VariableCollection
        {
            get
            {
                return variableCollection;
            }
            set
            {
                if (value == null)
                    validateButton.Enabled = false;
                else
                    validateButton.Enabled = true;
                variableCollection = value;
            }
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            Exception cee = null;
            tabControl.SelectedTab = outputTabPage;
            if (!ScriptHelper.Validate(scriptTextBox.Text, variableCollection, out cee))
                outputTextBox.Text = cee.Message;
            else
                outputTextBox.Text = "Nie wykryto błedów składniowych.";
        }
    }
}
