using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TestAppRPA
{
    public partial class Form1 : Form
    {
        Random num1 = new Random();
        Random num2 = new Random();
        Random symbol = new Random();
        Random attemptNo = new Random();
        String[] arrSymbol = new string[4] { "+", "-", "*", "/" };
        Label numb1 = new Label();
        Label numb2 = new Label();
        Label symb = new Label();
        Button next = new Button();
        TextBox answer = new TextBox();
        Button fin = new Button();
        Form appForm = new Form();
        String currentSymb;
        int countWin = 0;
        int winNum;
        DateTime start;
        DateTime end;
        public Form1()
        {
            InitializeComponent();
            numb1.Font = new Font(numb1.Font.FontFamily, 10);
            numb2.Font = new Font(numb2.Font.FontFamily, 10);
            symb.Font = new Font(symb.Font.FontFamily, 10);
            answer.Size = new Size(145, 70);
            winNum = attemptNo.Next(5, 20);
            description.Text = String.Format("There will be {0} attempts to complete this challenge", winNum.ToString());
            appForm = Form.ActiveForm;
            button1.Text = "Start";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start = DateTime.Now;
            Controls.Remove(sender as Button);
            if (System.Diagnostics.Process.GetProcessesByName("Calculator").Count() == 0)
            {
                System.Diagnostics.Process.Start("C:\\Windows\\System32\\calc.exe");
            }
            int rndNum1 = num1.Next(0, 100);
            int rndNum2 = num2.Next(0, 50);
            int rndSym = symbol.Next(0, 3);
            currentSymb = arrSymbol[rndSym];
            numb1.Location = new Point(175, 100);
            numb2.Location = new Point(225, 100);
            symb.Location = new Point(200, 100);
            answer.Location = new Point(150, 150);
            next.Location = new Point(175, 180);
            fin.Location = new Point(175, 180);
            numb1.AutoSize = true;
            numb2.AutoSize = true;
            symb.AutoSize = true;
            numb1.Text = rndNum1.ToString();
            numb2.Text = rndNum2.ToString();
            symb.Text = currentSymb;
            answer.Text = "";
            next.Text = "Next";
            fin.Text = "Restart?";
            fin.AutoSize = true;
            fin.Name = "Restart";
            numb1.Name = "Number 1";
            numb2.Name = "Number 2";
            symb.Name = "Symbol";
            answer.Name = "Answer";
            next.Name = "Next";
            next.Enabled = false;
            Controls.Add(numb1);
            Controls.Add(numb2);
            Controls.Add(symb);
            Controls.Add(answer);
            Controls.Add(next);
            timer1.Interval = (1000000000);
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
            next.Click += new EventHandler(click_BtnNext);
            answer.KeyPress += new KeyPressEventHandler(KeyDownAnswer);
            fin.Click += new EventHandler(clickFin);
            Controls.Remove(description);
        }

        private void click_BtnNext(object sender, EventArgs e)
        {
            int ans = 0;
            switch (currentSymb)
            {
                case "+":
                    ans = Convert.ToInt32(numb1.Text) + Convert.ToInt32(numb2.Text);
                    break;
                case "*":
                    ans = Convert.ToInt32(numb1.Text) * Convert.ToInt32(numb2.Text);
                    break;
                case "/":
                    ans = Convert.ToInt32(numb1.Text) / Convert.ToInt32(numb2.Text);
                    break;
                case "-":
                    ans = Convert.ToInt32(numb1.Text) - Convert.ToInt32(numb2.Text);
                    break;
            }
            if(ans == Convert.ToInt32(answer.Text))
            {         
                reset(timer1);
                countWin = countWin + 1;
                if (countWin == winNum)
                {
                    end = DateTime.Now;
                    TimeSpan timeFin = end - start;
                    double mSeconds = (end - start).TotalMilliseconds;
                    RemoveAllControls();
                    Label congrats = new Label();
                    congrats.Text = String.Format("Congratulations! You finished {2} answers with a time of {0:c}:{1}", timeFin, mSeconds.ToString().Replace(",",":"), winNum.ToString());
                    congrats.Location = new Point(220, 300);
                    congrats.Font = new Font(congrats.Font.FontFamily, 18);
                    congrats.ForeColor = Color.Red;
                    congrats.AutoSize = true;
                    Controls.Add(congrats);
                }
                else
                {
                    numb1.Text = num1.Next(0, 100).ToString();
                    numb2.Text = num2.Next(0, 50).ToString();
                    currentSymb = arrSymbol[symbol.Next(0, 3)];
                    symb.Text = currentSymb;
                    answer.Clear();
                }
            }
        }

        public static IEnumerable<Control> GetAllControls(Control root)
        {
            var stack = new Stack<Control>();
            stack.Push(root);

            while (stack.Any())
            {
                var next = stack.Pop();
                foreach (Control child in next.Controls)
                    stack.Push(child);
                yield return next;
            }
        }

        private void RemoveAllControls()
        {
            this.Controls.Clear();            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RemoveAllControls();
            (sender as Timer).Stop();
            MessageBox.Show("Not quick enough! Try again.");
            Controls.Add(fin);
            reset(timer1);
            countWin = 0;
        }
        
        public static void reset(Timer timer)
        {
            timer.Stop();
            timer.Start();
        }

        private void KeyDownAnswer(object sender, KeyPressEventArgs e)
        {
                if ((e.KeyChar == (char)Keys.Back) == false && (e.KeyChar == (char)Keys.Enter) == false)
                {
                    int i;
                    if (int.TryParse(e.KeyChar.ToString(), out i))
                    {
                        next.Enabled = true;
                    }
                    else
                    {
                        reset(timer1);
                        RemoveAllControls();
                        MessageBox.Show("That is not a number!");
                        Controls.Add(fin);
                        countWin = 0;
                    }
                }
                else if ((e.KeyChar == (char)Keys.Enter) && (answer.Text.Equals("") == false))
                {
                    next.PerformClick();
                }
        }

        private void clickFin(object sender, EventArgs e)
        {
            RemoveAllControls();
            Controls.Add(button1);
            answer.KeyPress -= KeyDownAnswer;
            timer1.Tick -= timer1_Tick;
            next.Click -= click_BtnNext;
            fin.Click -= clickFin;
            reset(timer1);
        }
    }
}
