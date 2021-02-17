using CustomizableHotkeysDemo.Properties;
using System.Windows.Forms;

namespace CustomizableHotkeysDemo
{
    public partial class Form1 : Form
    {
        private readonly KeysConverter keysConverter = new KeysConverter();

        private Keys hotkey1;
        private Keys hotkey2;

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
                textBox.Text = "None";
                return;
            }

            hotkey = e.KeyData;
            textBox.Text = keysConverter.ConvertToString(e.KeyData);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            RegisterLocalHotkey(e, textBox1, ref hotkey1);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            RegisterLocalHotkey(e, textBox2, ref hotkey2);
        }

        #region Settings

        private void Form1_Load(object sender, System.EventArgs e)
        {
            hotkey1 = Settings.Default.hotkey1;
            hotkey2 = Settings.Default.hotkey2;

            textBox1.Text = keysConverter.ConvertToString(Settings.Default.hotkey1);
            textBox2.Text = keysConverter.ConvertToString(Settings.Default.hotkey2);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.hotkey1 = hotkey1;
            Settings.Default.hotkey2 = hotkey2;

            Settings.Default.Save();
        }

        #endregion Settings
    }
}
