using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CustomizableHotkeysDemo.Properties;

namespace CustomizableHotkeysDemo
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly KeysConverter keysConverter = new KeysConverter();

        private Keys hotkey1;
        private Keys hotkey2;
        private Keys hotkey3;
        private Keys hotkey4;

        public Form1()
        {
            InitializeComponent();

            this.Select();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            hotkey1 = Settings.Default.hotkey1;
            hotkey2 = Settings.Default.hotkey2;
            hotkey3 = Settings.Default.hotkey3;
            hotkey4 = Settings.Default.hotkey4;

            textBox1.Text = keysConverter.ConvertToString(Settings.Default.hotkey1);
            textBox2.Text = keysConverter.ConvertToString(Settings.Default.hotkey2);
            textBox3.Text = keysConverter.ConvertToString(Settings.Default.hotkey3);
            textBox4.Text = keysConverter.ConvertToString(Settings.Default.hotkey4);

            // Register all of our global hotkeys
            RegisterHotKey(this.Handle, 0, ConvertKeys(Settings.Default.hotkey3)[0], ConvertKeys(Settings.Default.hotkey3)[1]);
            RegisterHotKey(this.Handle, 1, ConvertKeys(Settings.Default.hotkey4)[0], ConvertKeys(Settings.Default.hotkey4)[1]);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.hotkey1 = hotkey1;
            Settings.Default.hotkey2 = hotkey2;
            Settings.Default.hotkey3 = hotkey3;
            Settings.Default.hotkey4 = hotkey4;
            Settings.Default.Save();

            // Unregister all of our global hotkeys
            UnregisterHotKey(this.Handle, 0);
            UnregisterHotKey(this.Handle, 1);
        }

        // Process local hotkeys
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

        // Process global hotkeys
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();
                switch (id)
                {
                    case 0:
                        this.Activate();
                        MessageBox.Show("Hotkey 3 was pressed");
                        break;
                    case 1:
                        this.Activate();
                        MessageBox.Show("Hotkey 4 was pressed");
                        break;
                }
            }
        }


        private void RegisterLocalHotkey(KeyEventArgs e, TextBox textBox, ref Keys hotkey)
        {
            if (e.KeyData.HasFlag(Keys.LWin) || e.KeyData.HasFlag(Keys.RWin))
            {
                // Windows keys aren't really supported
                return;
            }

            if (e.KeyData == Keys.Escape)
            {
                // Unfocus the textbox by focusing on something else
                this.label1.Focus();
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

        private void RegisterGlobalHotkey(KeyEventArgs e, TextBox textBox, ref Keys hotkey, int id)
        {
            if (e.KeyData.HasFlag(Keys.LWin) || e.KeyData.HasFlag(Keys.RWin))
            {
                // Windows keys aren't really supported
                return;
            }

            if (e.KeyData == Keys.Escape)
            {
                // Unfocus the textbox by focusing on something else
                this.label1.Focus();
                return;
            }

            if (e.KeyData == Keys.Back)
            {
                // Clear / disable the hotkey
                UnregisterHotKey(this.Handle, id);
                textBox.Clear();
                return;
            }

            int modifiers = 0;
            if (e.Alt) // 262144
                modifiers |= 1;
            if (e.Control) // 131072
                modifiers |= 2;
            if (e.Shift) // 65536
                modifiers |= 4;

            UnregisterHotKey(this.Handle, id);
            RegisterHotKey(this.Handle, id, modifiers, (int)e.KeyCode);

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

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            RegisterGlobalHotkey(e, textBox3, ref hotkey3, 0);
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            RegisterGlobalHotkey(e, textBox4, ref hotkey4, 1);
        }

        private int[] ConvertKeys(Keys hotkey)
        {
            int modifiers = 0;
            if (hotkey.HasFlag(Keys.Alt))
                modifiers |= 1;
            if (hotkey.HasFlag(Keys.Control))
                modifiers |= 2;
            if (hotkey.HasFlag(Keys.Shift))
                modifiers |= 4;

            int keys = (int)hotkey;
            keys &= ~(int)Keys.Alt;
            keys &= ~(int)Keys.Control;
            keys &= ~(int)Keys.Shift;

            return new int[] { modifiers, keys };
        }
    }
}
