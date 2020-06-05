﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Students_Task_2
{
    class Program
    {
        #region Variables for the program 
        //appconfig to be able to change the path for the user
        static string path = ConfigurationManager.AppSettings["fileDirectory"];

        //declaring variable, that tracks if everything is alright
        static bool isEverythingAlright = true;

        //setting width-value for the table for the option Show all
        static int tableWidth = 170;

        //Regex for checking, if student is given right
        //It is very important to write it in the right order. Because of this an excel would probably work fine, in the WinForms Application :)
        static Regex regex = new Regex(@"([a-zA-Z]+(\s*)+){3}(([0-9]{6})+(\s*)+)(([0-9]{7})+(\s*)+)([2-6].[0-9][0-9]+(\s*)+)+");
        #endregion

        #region MainMethod
        static void Main(string[] args)
        {
            while (isEverythingAlright)
            {
                //prepare program to run smoothly
                CheckIfAllStudentsAreGivenRightDeleteEmptyRowsAndWarnUser();

                //going out from the while loop if there are no mistakes
                if (isEverythingAlright)
                {
                    isEverythingAlright = false;
                }
                else
                {
                    isEverythingAlright = true;

                    //making sure loop doesnot continue before the user can change the wrong row
                    string h = Console.ReadLine();
                }
            }

            while (true)
            {
                //Welcome user
                Console.WriteLine("Welcome to Students App");
                Console.WriteLine("Please chose a command: ");

                //Show menu
                int selectedCommand = ConsoleHelper.ShowMenu(true, "SHOW ALL", "SEARCH BY PERSONAL IDENTIFICATION NUMBER", "SEARCH BY STUDENT NUMBER"
                                                                 , "DELETE BY PERSONAL IDENTIFICATION NUMBER", "ADD STUDENT", "SHOW AVERAGE GRADES", "EXIT");


                Console.WriteLine();
                //Making commands work, as they should
                switch (selectedCommand)
                {
                    case 0:
                        ShowAllStudents();
                        string h = Console.ReadLine();
                        break;
                    case 1:
                        SearchByPersonalIdentificationNumber();
                        h = Console.ReadLine();
                        break;
                    case 2:
                        SearchByStudentNumber();
                        h = Console.ReadLine();
                        break;
                    case 3:
                        DeleteByPersonalIdentificationNumber();
                        h = Console.ReadLine();
                        break;
                    case 4:
                        AddStudent();
                        h = Console.ReadLine();
                        break;
                    case 5:
                        ShowAvarageGrades();
                        h = Console.ReadLine();
                        break;
                    case 6:
                        Exit();
                        break;
                    default:
                        break;
                }

            }
        }
        #endregion

        #region Methods used in the Mainmethod
        private static void CheckIfAllStudentsAreGivenRightDeleteEmptyRowsAndWarnUser()
        {
            //separating the students 
            string[] students = File.ReadAllLines(path);

            for (int i = 0; i < students.Length; i++)
            {
                //delete empty rows
                if (students[i] == "" || students[i] == null || IsEmptyOrWhiteSpace(students[i]))
                {
                    //making sure the method works also for consecutive empty rows
                    while ((students[i] == "" || students[i] == null || IsEmptyOrWhiteSpace(students[i])))
                    {
                        if (i < students.Length - 1)
                        {
                            RemoveAt(ref students, i);
                        }
                        else
                        {
                            students = students.SkipLast(1).ToArray();

                            //refreshing the text in the textfile, without empty rows at the end of the while loop
                            using (StreamWriter writer = new StreamWriter(path))
                            {
                                foreach (string student in students)
                                {
                                    writer.WriteLine(student);
                                }
                            }

                            return;
                        }

                        //refreshing the text in the textfile, without empty rows at the end of the while loop
                        using (StreamWriter writer = new StreamWriter(path))
                        {
                            foreach (string student in students)
                            {
                                writer.WriteLine(student);
                            }
                        }
                    }
                }

                //detecting if any of the rows do not match with the regex 
                if (regex.IsMatch(students[i]))
                {
                    continue;
                }
                else
                {
                    isEverythingAlright = false;

                    //adding warningsingnal for the user
                    students[i] = "#####Problem####  " + students[i];

                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        foreach (string student in students)
                        {
                            writer.WriteLine(student);
                        }
                    }
                }
            }

            if (!isEverythingAlright)
            {
                //saying to the user, that he has to change the student's row
                Console.WriteLine("A student/some students is/are declared wrongly. The line in the textfile begins with \"####Problem####\" " +
                    "\nPlease delete him, or change him following this pattern:" +
                    "\nfirst name   middle Name   last name   personal identification number (6 digits)   student number (7 digits)   grades");

                //opening textfile, so that the user can correct the wrong entered student
                System.Diagnostics.Process.Start("notepad", path);
            }
            else
            {
                //if every row is usable, return to start the program
                isEverythingAlright = true;
                return;
            }
        }

        private static void ShowAllStudents()
        {
            SetConsoleWidth();
            Console.BufferWidth = Console.LargestWindowWidth;
            //We don't want to show the personal identification number, as it is private
            PrintLine();

            PrintRow("last name", "middle name", "first name", "student number", "grades");

            PrintLine();

            //separating the students 
            string[] students = File.ReadAllLines(path);

            foreach (string student in students)
            {
                //separating the students informations
                string[] studentElements = student.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                //informations, that can't be changed
                string firstName = studentElements[0];
                string middleName = studentElements[1];
                string lastName = studentElements[2];
                string personalIndentificationNumber = studentElements[3];
                string studentNumber = studentElements[4];
                string grades = "";

                //grades
                for (int i = 0; i < studentElements.Length - 5; i++)
                {
                    string[] grade = new string[studentElements.Length];
                    grade[i] = studentElements[i + 5];
                    grades += $" {grade[i]}";
                }

                PrintRow(lastName, middleName, firstName, studentNumber, grades);
            }
            PrintLine();
        }

        private static void SearchByPersonalIdentificationNumber()
        {
            //Asking the user to enter the students personal identification number
            Console.WriteLine("Please enter the personal identification number of the student");

            //Saving personal identification number as string identNum
            string identNum = Console.ReadLine();
            SetConsoleWidth();

            string[] students = File.ReadAllLines(path);

            for (int i = 0; i < students.Length; i++)
            {
                string[] studentElements = students[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (studentElements[3].Contains(identNum))
                {
                    Console.WriteLine("We found the student!");

                    PrintLine();
                    PrintRow("last name", "middle name", "first name", "student number", "grades");

                    string grades = "";
                    for (int j = 0; j < studentElements.Length - 5; j++)
                    {
                        string[] grade = new string[studentElements.Length];
                        grade[j] = studentElements[j + 5];
                        grades += $" {grade[j]}";
                    }

                    PrintRow(studentElements[2], studentElements[1], studentElements[0], studentElements[4], grades);

                    return;
                }
            }
            Console.WriteLine("There is no student with this personal identification number!");
        }

        private static void SearchByStudentNumber()
        {
            Console.WriteLine("Please enter the student number of the student");
            string identNum = Console.ReadLine();

            SetConsoleWidth();

            string[] students = File.ReadAllLines(path);

            for (int i = 0; i < students.Length; i++)
            {
                string[] studentElements = students[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (studentElements[4].Contains(identNum))
                {
                    Console.WriteLine("We found the student!");

                    PrintLine();
                    PrintRow("last name", "middle name", "first name", "student number", "grades");

                    string grades = "";
                    for (int j = 0; j < studentElements.Length - 5; j++)
                    {
                        string[] grade = new string[studentElements.Length];
                        grade[j] = studentElements[j + 5];
                        grades += $" {grade[j]}";
                    }

                    PrintRow(studentElements[2], studentElements[1], studentElements[0], studentElements[4], grades);

                    return;
                }
            }
            Console.WriteLine("There is no student with this student number!");
        }

        private static void DeleteByPersonalIdentificationNumber()
        {
            Console.WriteLine("Please enter the personal identification number of the student");
            string identNum = Console.ReadLine();

            string[] students = File.ReadAllLines(path);

            for (int i = 0; i < students.Length; i++)
            {
                string[] studentElements = students[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (studentElements[3].Contains(identNum))
                {
                    Console.WriteLine($"We will delete the student {studentElements[0]} {studentElements[1]} {studentElements[2]}");

                    students[i] = null;

                    string newFileText = "";

                    foreach (string student in students)
                    {
                        if (student != null)
                        {
                            newFileText += student + "\n";
                        }
                        continue;
                    }

                    using (StreamWriter streamWriter = new StreamWriter(path))
                    {
                        streamWriter.Write(newFileText);
                    }

                    return;
                }
            }

            Console.WriteLine("There is no student with this personal identification number!");
        }

        private static void AddStudent()
        {
            SetConsoleWidth();
            //Showing entering pattern to the user
            Console.WriteLine("Use the following pattern without entering punctuation marks:" +
                    "\nfirst name   middle Name   last name   personal identification number (6 digits)   student number (7 digits)   grades");

            //Saving the students info in studentInfo string
            string studentInfo = Console.ReadLine().Trim();

            //See if it matches the pattern, and if so adding tne student 
            if (regex.IsMatch(studentInfo))
            {
                //Making sure not all whitespaces entered by the user will be printed out in the textfile
                string[] studentInfoElements = studentInfo.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                studentInfo = "";

                foreach (string information in studentInfoElements)
                {
                    studentInfo += information + " ";
                }

                //Adding student to the textfile
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(studentInfo);
                }
            }

            //If it doesnot match the pattern, user becomes only one more chance to set it right, for not getting stucked in this method
            else
            {
                Console.WriteLine("Wrong format or input. Use the following pattern without entering punctuation marks or press enter to go back to menu:" +
                    "\nfirst name   middle Name   last name   personal identification number (6 digits)   student number (7 digits)   grades");

                //Same as above 
                studentInfo = Console.ReadLine().Trim();

                if (regex.IsMatch(studentInfo))
                {
                    string[] studentInfoElements = studentInfo.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    studentInfo = "";

                    foreach (string information in studentInfoElements)
                    {
                        studentInfo += information + " ";
                    }

                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        writer.WriteLine(studentInfo);
                    }
                }
                else if (studentInfo == "")
                {
                    Console.WriteLine("Welcome back to menu!");
                }
                else
                {
                    Console.WriteLine("Again wrong format or input." +
                        "\nWelcome back to menu!");
                }
            }
        }

        private static void ShowAvarageGrades()
        {
            SetConsoleWidth();
            string[] students = File.ReadAllLines(path);

            foreach (string student in students)
            {
                string[] studentElements = student.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                double SumOfGrades = 0;

                for (int i = 5; i < studentElements.Length; i++)
                {
                    SumOfGrades += double.Parse(studentElements[i]);
                }

                Console.WriteLine($"{studentElements[0]} {studentElements[2]} with student number {studentElements[4]} has avarage grade - {(SumOfGrades / 10.0).ToString("#.##")}");
            }
        }

        private static void Exit()
        {
            Environment.Exit(0);
        }
        #endregion

        #region Methods for preparing the program
        private static bool IsEmptyOrWhiteSpace(string value) =>
                    value.All(char.IsWhiteSpace);

        private static void RemoveAt<ArraySegment>(ref ArraySegment[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                // moving elements downwards, to fill the gap at [index]
                arr[a] = arr[a + 1];
            }
            // decrement Array's size by one
            Array.Resize(ref arr, arr.Length - 1);
        }

        private static void SetConsoleWidth()
        {
            Console.WindowWidth = Console.LargestWindowWidth;
            if (Console.WindowWidth < tableWidth)
            {
                Console.BufferWidth = tableWidth;
            }
        }
        #endregion

        #region Methods for printing out a table with the students infos

        static void PrintRow(params string[] columns)
        {
            int width = 16;
            string row = "|";

            for (int i = 0; i < columns.Length - 1; i++)
            {
                row += AlignCentre(columns[i], width) + "|";
            }

            row += AlignCentre(columns[columns.Length - 1], 96) + "|";

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }
        #endregion
    }
}
