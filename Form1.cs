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

namespace CalculateMathExpr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

      

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            int error;
            string result = CalculateMathExpr(txtExpr.Text, out error).ToString();

            if (error >= 0) txtResult.Text = result;

        }

        public double CalculateMathExpr(string expr, out int errCode)
        {
            errCode = 0;

            Stack<double> stkNumbers = new Stack<double>(); // Only store numbers in stack
            List<string> liststrPrioritizedBlock = PrioritizeMathBlock(expr);
           
            double number;

            //Final calculation by using prioritized list
            foreach (string block in liststrPrioritizedBlock)
            {
                if (double.TryParse(block, out number))
                {
                    stkNumbers.Push(number);
                }
                else
                {
                    double num1 = stkNumbers.Pop();
                    double num2 = stkNumbers.Pop();

                    switch (block)
                    {
                        case "+":
                            stkNumbers.Push(num2 + num1);
                            break;
                        case "-":
                            stkNumbers.Push(num2 - num1);
                            break;
                        case "*":
                            stkNumbers.Push(num2 * num1);
                            break;
                        case "/":
                            stkNumbers.Push(num2 / num1);
                            break;
                    }
                }
            }
            if (liststrPrioritizedBlock.Count == 0)
            {
                errCode = -1;
                return -1;
            }

            return stkNumbers.Pop();
        }

        /// <summary>
        /// Prioritize all numbers and operator in sequence first before final calculation with priority
        /// </summary>
        /// <param name="expr">Math expression</param>
        /// <returns></returns>
        private List<string> PrioritizeMathBlock(string expr)
        {
            List<string> liststrPrioBlock = new List<string>(); //Require store list without fifo concept
            Stack<string> stkOperator = new Stack<string>(); //use to store temp operator for analysis purpose
            Regex validateNumberRegex = new Regex(@"-?\d+(?:\.\d+)?|\+|\-|\*|\/|[(]|[)]");
            //Regex validateOpr = new Regex(@"\+|\-|\*|\/");

            string[] blocks = expr.Split(' ');
            double tryDoub;
            foreach (string block in blocks)
            {
                //Exception scenario
                if (!validateNumberRegex.IsMatch(block))
                {
                    txtResult.Text = "Wrong math expression";
                    return new List<string>();
                }


                if (double.TryParse(block, out tryDoub))
                {
                    liststrPrioBlock.Add(block);
                }
                else if (block == "(")
                {
                    stkOperator.Push(block);
                }
                else if (block == ")")
                {
                    while (stkOperator.Peek() != "(")
                    {
                        liststrPrioBlock.Add(stkOperator.Pop());
                    }
                    stkOperator.Pop();
                }
                else
                {
                    //Once operator +-*/ found, loop until all temp operator finish prioritize
                    while (stkOperator.Count > 0 && GetPriority(block) <= GetPriority(stkOperator.Peek()))
                    {
                        //add into final prioritized list if previous operator is analyzed (Non-bracket)
                        liststrPrioBlock.Add(stkOperator.Pop());
                    }
                    stkOperator.Push(block);
                }
            }

            while (stkOperator.Count > 0)
            {
                liststrPrioBlock.Add(stkOperator.Pop());
            }

            return liststrPrioBlock;
        }

        /// <summary>
        /// Provide priority for operator to reorganize sequence
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private int GetPriority(string op)
        {
            switch (op)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
