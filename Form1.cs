using System;
using System.IO;
using System.Windows.Forms;

namespace Notepad
{
    public partial class Form1 : Form
    {
        //Declare variables used several times later
        private string filename = string.Empty;
        private bool textChanged = false;

        //Start of program
        public Form1()
        {
            InitializeComponent();
            Text = "Unnamed";
        }

        //NEWBUTTON
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //This, while clunky, creates better functionality than this.Close()
            //Makes sure we are asked if we're sure if we have edited the file and doens't do anyhting
            //if we tell it to cancel and doesn't close the program if we click no
            var eventArgs = new FormClosingEventArgs(CloseReason.None, false);
            Form1_FormClosing(null, eventArgs);
            if (eventArgs.Cancel)
            {
                return;
            }

            //If we do want to go through with this then we reset everything to the default state
            richTextBox1.Text = string.Empty;
            Text = "Unnamed";
            textChanged = false;
            filename = string.Empty;
        }

        //OPENBUTTON
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Same as function above
            var eventArgs = new FormClosingEventArgs(CloseReason.None, false);
            Form1_FormClosing(null, eventArgs);
            if (eventArgs.Cancel)
            {
                return;
            }

            //We open a file dialog and if one is selected then load it
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textChanged = false;
                richTextBox1.Text = File.ReadAllText(openFileDialog1.FileName);
                filename = openFileDialog1.FileName;
                Text = openFileDialog1.SafeFileName;
            }
        }

        //SAVEBUTTON
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        //Save, has its own function rather than just being save button since it is called elsewhere
        private void SaveFile()
        {
            //Remove the star and set to false
            Text = Text.Remove(Text.Length - 1);
            textChanged = false;

            //If we don't already have a file, we're editing
            if (string.IsNullOrEmpty(filename))
            {
                SaveAs();
            }

            //If we were already working on a file then just save our text to it
            else
            {
                File.WriteAllText(filename, richTextBox1.Text);
            }
        }

        //SAVEASBUTTON
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            //We show a dialog and when a file is selected to be saved we put all our text there
            //and update all the variables to point to that file
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = saveFileDialog1.FileName;
                Text = filename;
                string path = Path.GetFullPath(saveFileDialog1.FileName);
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(richTextBox1.Text);
                }
            }
        }

        //EXITBUTTON
        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Add star to show text edited since last save, used in parts of the program to know
        //if we have edited the text
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!textChanged)
            {
                Text = Text + "*";
            }
            textChanged = true;
        }

        //Tell user their changes are not saved and save if requested
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textChanged)
            {
                DialogResult dialogResult = MessageBox.Show("You have unsaved edits. Save?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.Yes)
                {
                    SaveFile();
                }
                if (dialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else { }
            }
        }
    }
}
