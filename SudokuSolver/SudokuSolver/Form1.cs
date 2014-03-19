using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class Form1 : Form
    {
        const int cellSideLength = 35;
        const int sudokuLength = 9; //must be square number, {4,9,16,25}
        private int stepsToSolve;
        Timer timer;

        public Form1()
        {
            InitializeComponent();

            
            //Adds all the cells
            for (int i = 0; i < sudokuLength; i++)
            {
                DataGridViewTextBoxColumn newCol = new DataGridViewTextBoxColumn();
                newCol.CellTemplate = new DataGridViewTextBoxCell();
                newCol.MaxInputLength = 1;  
                newCol.Width = cellSideLength;
                newCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                newCol.CellTemplate.ValueType = typeof(int);
                dataGridView1.Columns.Add(newCol);
               
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.Height = cellSideLength;
                dataGridView1.Rows.Add(newRow);
            }



            //Bolds the lines seperating the Large squares
            int lengthPerPartition = (int)Math.Sqrt((Double)sudokuLength);
            for (int i = lengthPerPartition - 1; i < sudokuLength; i += lengthPerPartition)
            {
                dataGridView1.Columns[i].DividerWidth = 2;
                dataGridView1.Rows[i].DividerHeight = 2;
            }


            //Resizes form to specific puzzle size
            int formSideLength = sudokuLength * cellSideLength + lengthPerPartition;
            dataGridView1.Size = new Size(formSideLength, formSideLength);
            this.Size = new Size(formSideLength+ 40, formSideLength + 150);


            //Some other properties I wanted to set, no changing of cell size, etc...
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;

        }

        void timer_tick(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //Starts to solve 
        private void button1_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value != 0)
            {
                timer = new Timer();
                timer.Interval = (int)numericUpDown1.Value;
                timer.Tick += new EventHandler(timer_tick);

            }
            //solver(1, 0, 0, 0);
            try
            {
                stepsToSolve = 0;
                populate();
                solve(0, 0);
            }
            catch (Exception f) 
            {
                MessageBox.Show("Took " + stepsToSolve.ToString() + " steps to solve");
            }
        }

        private void solver(int tryNext, int colNum, int rowNum, int lastCellsAttempt)
        {
            /*
            //if(userEnteredThisValue)
            {
                if (isValid(colNum, rowNum, tryNext))
                {
                    solver(1, colNum + 1, rowNum);//gonna have to edit this to go back to start column
                }
                else return;
            }

            */

            //System.Threading.Thread.Sleep(500);
            //dataGridView1.Rows[rowNum].Cells[colNum].Value = tryN
            
            
            dataGridView1[colNum, rowNum].Value = tryNext;
            while (isValid(colNum, rowNum, tryNext) == false && (tryNext <= 9))
            {
                MessageBox.Show(colNum.ToString() + rowNum.ToString() + tryNext.ToString());
                //System.Threading.Thread.Sleep(100);
                tryNext++;
                dataGridView1[colNum, rowNum].Value = tryNext;
            
            }

            if (tryNext > sudokuLength)
            {
                //Go back one
                int nextCol = colNum - 1;
                int nextRow = rowNum;
                if (nextCol == -1)
                {
                    nextCol = 8;
                    nextRow = rowNum - 1;
                }
                solver(lastCellsAttempt + 1, nextCol, nextRow, lastCellsAttempt + 1);
            }


            else if (isValid(colNum, rowNum, tryNext))
            {
                int nextCol = colNum + 1;
                int nextRow = rowNum;
                if (nextCol == 9)
                {
                    nextCol = 0;
                    nextRow = rowNum + 1;
                }
                if (nextRow == 9)
                {
                    MessageBox.Show("Solved!");
                }

                solver(1, nextCol, nextRow, lastCellsAttempt);

            }


        }

        private void populate()
        {
            for (int i = 0; i < sudokuLength; i++)
            { 
                for (int j = 0; j < sudokuLength; j++)
                {
                    if (dataGridView1[i, j].Value == null)
                    {
                        dataGridView1[i, j].Value = 0;
                    }
                }
            }
        }

        private void solve(int row, int col)
        {
            if (row == sudokuLength)
            {
                throw new Exception("Solved");
            }

            //if (dataGridView1.Rows[row].Cells[col].Value == null || (int)dataGridView1[col, row].Value == 0)
            if ((int)dataGridView1[col, row].Value != 0)
            {
                solveNext(row, col);
            }
            else
            {
                for (int num = 1; num <= sudokuLength; num++)
                {
                    //MessageBox.Show(stepsToSolve.ToString());
                    if (numericUpDown1.Value != 0)
                    {
                        timer.Start();
                    }
                    stepsToSolve++;
                    if (isValid(col, row, num))
                    {
                        dataGridView1[col, row].Value = num;


                        solveNext(row, col);
                    }
                }
                dataGridView1[col, row].Value = 0;
            }

        }


        private void solveNext(int row, int col)
        {
            if (col < sudokuLength - 1)
            {
                solve(row, col + 1);
            }
            else
            {
                solve(row + 1, 0);
            }
        }





        private bool isValid(int colNum, int rowNum, int value)
        {


            //Check row
            for (int i = 0; i < sudokuLength; i++)
            {
                //Don't need to check with its own square
                if (i == colNum)
                {
                    continue;
                }
                else
                {
                    if (dataGridView1.Rows[rowNum].Cells[i].Value == null)
                    {
                        continue;
                    }
                    else if ((int)dataGridView1.Rows[rowNum].Cells[i].Value == value)
                    {
                        return false;
                    }
                }
            }

            //Check Column
            for (int i = 0; i < sudokuLength; i++)
            {
                //Don't need to check with its own square
                if (i == rowNum)
                {
                    continue;
                }
                else
                {
                    if (dataGridView1.Rows[i].Cells[colNum].Value == null)
                    {
                        continue;
                    }
                    else if ((int)dataGridView1.Rows[i].Cells[colNum].Value == value)
                    {
                        return false;
                    }
                }
            }


            //Check parentSquare
            int squareSideLength = (int)Math.Sqrt((double)sudokuLength);

            int col = (colNum / squareSideLength) * squareSideLength;
            int row = (rowNum / squareSideLength) * squareSideLength;

            for (int r = 0; r < squareSideLength; r++)
            {
                for (int c = 0; c < squareSideLength; c++)
                {
                    if (dataGridView1.Rows[row + r].Cells[col + c].Value == null)
                    {
                        continue;
                    }
                    else if ((row+r) == rowNum && (col+c) == colNum)
                    {
                        continue;
                    }
                    else if ((int)dataGridView1.Rows[row + r].Cells[col + c].Value == value)
                    {
                        return false;
                    }
                }
            }


            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < sudokuLength; i++)
            { 
                for (int j = 0; j < sudokuLength; j++)
                {
                    dataGridView1[i, j].Value = null;                    
                }
            }
        }

    }
}
