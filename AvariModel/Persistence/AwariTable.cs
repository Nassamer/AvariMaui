using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariModel.Persistence
{
    public class AwariTable
    {
        private int[,] fieldValues; // mezőértékek
        private bool[,] fieldLocks; // mező zárolások
        private bool doubleSteps = false;
        private bool trippleSteps = false;

        public int[,] FieldValues { get { return fieldValues; } }
        public bool[,] FieldLocks { get { return fieldLocks; } }

        public bool GetDoubleSteps { get { return doubleSteps; } }
        public bool GetTripleSteps { get { return trippleSteps; } }

        public int Size 
        { 
            get { return fieldValues.GetLength(0); } 
        }

        public int this[int x, int y] { get { return GetValue(x, y); } }



        public AwariTable(int tableSize)
        {
            fieldValues = new int[tableSize, tableSize];
            fieldLocks = new bool[tableSize, tableSize];
        }

        public int GetValue(int x, int y)
        {
            return fieldValues[x, y];
        }

        public void StepValue(int x, int y)
        {
            int position = y;
            int num = fieldValues[x, y];
            fieldValues[x, y] = 0;

            int ciklus = 0;



            while (num >= ciklus)
            {
                if (position < fieldValues.GetLength(1))
                {

                    if (position != y)
                    {
                        fieldValues[x, position] = fieldValues[x, position] + 1;
                    }
                    position++;


                }
                else
                {
                    if (x == 0)
                    {
                        fieldValues[1, 0] = fieldValues[1, 0] + 1;
                        if (num == ciklus && doubleSteps && !trippleSteps)
                        {
                            trippleSteps = true;
                        }
                        else if (num == ciklus && doubleSteps && trippleSteps)
                        {
                            doubleSteps = false;
                            trippleSteps = false;
                        }
                        else if (num == ciklus && !doubleSteps)
                        {
                            doubleSteps = true;
                        }
                    }
                    else if (x == 2)
                    {
                        fieldValues[1, (fieldValues.GetLength(1)) - 1] += 1;
                        if (num == ciklus && doubleSteps && !trippleSteps)
                        {
                            trippleSteps = true;
                        }
                        else if (num == ciklus && doubleSteps && trippleSteps)
                        {
                            doubleSteps = false;
                            trippleSteps = false;
                        }
                        else if (num == ciklus && !doubleSteps)
                        {
                            doubleSteps = true;
                        }
                    }
                    position = 0;
                }
                ciklus++;
                if (ciklus > num)
                {
                    if (position == 0)
                    {
                        if (fieldValues[x, position] == 1)
                        {
                            if (x == 0)
                            {
                                fieldValues[x, position] += fieldValues[2, position];
                                fieldValues[2, position] = 0;

                            }
                            else if (x == 2)
                            {
                                fieldValues[x, position] += fieldValues[0, position];
                                fieldValues[0, position] = 0;
                            }
                        }
                    }
                    else if (position == 8)
                    {
                        if (fieldValues[x, position - 1] == 1)
                        {
                            if (x == 0)
                            {
                                fieldValues[x, position - 1] += fieldValues[2, position - 1];
                                fieldValues[2, position - 1] = 0;

                            }
                            else if (x == 2)
                            {
                                fieldValues[x, position - 1] += fieldValues[0, position - 1];
                                fieldValues[0, position - 1] = 0;
                            }
                        }
                    }
                }
            }



            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < fieldValues.GetLength(1); j++)
                {
                    if (x == 0)
                    {
                        fieldLocks[0, j] = true;
                    }
                    else if (x == 2)
                    {
                        fieldLocks[2, j] = true;
                    }
                }
            }
        }

        public void SetValue(int x, int y, int value)
        {
            fieldValues[x, y] = value;
        }

        public void SetLock(int x, int y)
        {
            fieldLocks[x, y] = true;
        }


        public bool Sum()
        {
            int first = 0;
            int second = 0;

            for (int i = 0; i < fieldValues.GetLength(1); i++)
            {
                first += fieldValues[0, i];
                second += fieldValues[2, i];
            }
            return first == 0 || second == 0;
        }

        public bool IsLocked(int x, int y)
        {
            if (x < 0 || x >= fieldValues.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= fieldValues.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            return fieldLocks[x, y];
        }

        public bool IsFirstRow(int x)
        {
            if (x == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsSecondRow(int x)
        {
            if (x == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsThirdRow(int x)
        {
            if (x == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsEmpty(int x, int y)
        {
            int value = fieldValues[x, y];
            return value == 0;
        }
    }
}
