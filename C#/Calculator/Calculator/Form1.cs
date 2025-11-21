using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private static List<Button> _calculatorButtons;
        public Form1()
        {
            InitializeComponent();
            AdjustDisplayFont();

            _calculatorButtons = new List<Button>()
            {
                bOne, bTwo, bThree, bFour, bFive, bSix, bSeven, bEight, bNine, bZero, bComma, bMinus, bMultiply, bPlus, bDivide, bLeftBracket, bRightBracket
            };

            AddListenersForButtons();
        }

        /// <summary>
        /// Adds click event listeners to all calculator buttons.
        /// </summary>
        private void AddListenersForButtons()
        {
            _calculatorButtons.ForEach(b => b.Click += CalculatorButtonClick);
        }

        #region Listeners

        /// <summary>
        /// Handles the click event for calculator buttons to append their text to the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculatorButtonClick(object sender, EventArgs e)
        {
            textBox1.AppendText((sender as Button).Text);
        }

        /// <summary>
        /// Handles the click event for the "Enter" button to evaluate the expression in the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bEnter_Click(object sender, EventArgs e)
        {
            try
            {
                decimal result = ExpressionEvaluator.Evaluate(textBox1.Text);

                textBox1.Text = result.ToString();
            }
            catch (DivideByZeroException ex)
            {
                MessageBox.Show("Can't divide by Zero: " + ex.Message, "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Invalid expression format: " + ex.Message, "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpeced error in calculation: " + ex.Message, "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Adjusts the font size of the display to fit within the display area.
        /// </summary>
        private void AdjustDisplayFont()
        {
            Int32 padding = 4;
            Int32 targetHeight = textBox1.ClientSize.Height - padding;

            if (targetHeight <= 0)
                return;

            Single emSize = targetHeight * 0.75f;
            if (emSize < 1f)
                emSize = 1f;

            textBox1.Font = new Font(
                textBox1.Font.FontFamily,
                emSize,
                FontStyle.Regular,
                GraphicsUnit.Pixel);
        }
        #endregion

        private void bBackspace_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength <= 0)
                return;

            textBox1.Text = textBox1.Text.Substring(0, textBox1.TextLength - 1);
            textBox1.SelectionStart = textBox1.TextLength;
        }
    }
}
