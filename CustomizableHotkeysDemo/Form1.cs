using System.Windows.Forms;

namespace CustomizableHotkeysDemo
{
    public partial class Form1 : Form
    {
        private Keys hotkey1 = Keys.Control | Keys.A;
        private Keys hotkey2 = Keys.A;

        public Form1()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == hotkey1)
            {
                MessageBox.Show("Hotkey 1 was pressed");
                return true;
            }
            else if (keyData == hotkey2)
            {
                MessageBox.Show("Hotkey 2 was pressed");
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void RegisterLocalHotkey(KeyEventArgs e, TextBox textBox, ref Keys hotkey)
        {
            if (e.KeyData == Keys.LWin || e.KeyData == Keys.RWin)
            {
                // Windows keys aren't really supported
                return;
            }

            if (e.KeyData == Keys.Escape)
            {
                // Unfocus the textbox by focusing on something else
                label1.Focus();
                return;
            }

            if (e.KeyData == Keys.Back)
            {
                // Clear / disable the hotkey
                hotkey = Keys.None;
                textBox.Clear();
                return;
            }

            hotkey = e.KeyData;
            textBox.Text = new KeysConverter().ConvertToString(e.KeyData);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            RegisterLocalHotkey(e, textBox1, ref hotkey1);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            RegisterLocalHotkey(e, textBox2, ref hotkey2);
        }
    }
}
