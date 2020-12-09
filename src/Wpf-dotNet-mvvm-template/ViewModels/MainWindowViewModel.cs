using System;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using WpfDotNetMvvmTemplate.Models;
using System.Data.SQLite;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.ComponentModel.Design;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.CompilerServices;

namespace WpfDotNetMvvmTemplate.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Binding sources
        public string Title { get; } = "Esercizio SQLite";
        public ObservableCollection<Member> Members { get; private set; } = new ObservableCollection<Member>();

        #region Editing menu
        private string _EditingFirstName = "";
        public string EditingFirstName
        {
            get => this._EditingFirstName;
            set
            {
                this._EditingFirstName = value;
                NotifyPropertyChanged();
            }
        }

        private string _EditingLastName = "";
        public string EditingLastName
        {
            get => this._EditingLastName;
            set
            {
                this._EditingLastName = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime _EditingBirthDate;
        public DateTime EditingBirthDate
        {
            get => this._EditingBirthDate;
            set
            {
                this._EditingBirthDate = value;
                NotifyPropertyChanged();
            }
        }

        private string _EditingHeight = "";
        public string EditingHeight
        {
            get => this._EditingHeight;
            set
            {
                this._EditingHeight = value;
                NotifyPropertyChanged();
            }
        }

        private string _AlertLabel;
        public string AlertLabel
        {
            get => this._AlertLabel;
            set
            {
                this._AlertLabel = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
        public Member SelectedMember { get; set; }
        public RelayCommand CreateTableCommand { get; private set; }
        public RelayCommand DeleteTableCommand { get; private set; }
        public RelayCommand AddMemberCommand { get; private set; }
        public RelayCommand EditMemberCommand { get; private set; }
        public RelayCommand DeleteMembersSelectedCommand { get; private set; }
        #endregion

        private SQLiteConnection SqliteConnection = null;
        public MainWindowViewModel()
        {
            Log.LogHandler += (object obj, Log.LogType type) =>
            {
                Console.WriteLine("________________________");
                Console.WriteLine(type);
                Console.WriteLine(obj);
            };

            this.CreateTableCommand = new RelayCommand(this.CreateTable, this.TableNotExist);
            this.DeleteTableCommand = new RelayCommand(this.DeleteTable, this.TableExist);
            this.AddMemberCommand = new RelayCommand(this.AddMember, this.TableExist);
            this.EditMemberCommand = new RelayCommand(this.EditMember, this.OneMemberSelected);
            this.DeleteMembersSelectedCommand = new RelayCommand(this.DeleteMembersSelected, this.AtLeastOneMemberSelected);

            this.SqliteConnection = new SQLiteConnection(@"Data Source=members.db;datetimeformat=CurrentCulture");
            this.SqliteConnection.Open();

            if (this.TableExist(null))
            { //carica tabella
                List<object[]> membersList = this.Run(@"
                SELECT * FROM Members
                ");
                Member[] members = new Member[membersList.Count];
                int i = 0;
                foreach (object[] record in membersList)
                {
                    members[i] = new Member(Convert.ToUInt32(record[0]), (string)record[1], (string)record[2], DateTime.Parse(record[3].ToString()), Convert.ToUInt16(record[4]));
                    i++;
                }
                this.Members = new ObservableCollection<Member>(members);
            }
        }
        private bool TableNotExist(object obj)
        {
            return !this.TableExist(obj);
        }
        private bool TableExist(object obj)
        {
            return this.Run("SELECT * FROM sqlite_master WHERE type='table' and name='Members'").Count != 0;
        }
        private void CreateTable(object obj)
        {
            this.Run(
                @"
                CREATE TABLE Members(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                LastName TEXT NOT NULL,
                FirstName TEXT NOT NULL,
                BirthDate DATETIME NOT NULL,
                Height INTEGER NOT NULL)"
            );
            this.RevalueButtons();
        }
        private void DeleteTable(object obj)
        {
            this.Run(
                @"DROP TABLE Members"
            );
            RevalueButtons();
            Members.Clear();
        }
        private Thread AlertingThread;
        private bool EditsValid()
        {
            if (this.AlertingThread != null && this.AlertingThread.IsAlive)
                this.AlertingThread.Abort();

            Match reg;

            // Validity checkers
            if (this.EditingFirstName.Length == 0)
                this.AlertLabel = "Invalid first name";
            else if (this.EditingLastName.Length == 0)
                this.AlertLabel = "Invalid last name";
            else if (this.EditingBirthDate >= DateTime.Now)
                this.AlertLabel = "Invalid birth date";
            else if (!(reg = new Regex("[0-9]").Match(this.EditingHeight)).Success)
                this.AlertLabel = "Invalid height";
            else
                return true;

            // User error handling
            this.AlertingThread = new Thread(() => { Thread.Sleep(3000); this.AlertLabel = ""; });
            this.AlertingThread.Start();
            return false;
        }
        private void AddMember(object obj)
        {
            if (this.EditsValid())
            {
                // Find id
                List<object[]> list = this.Run(@"
                                SELECT Id FROM Members ORDER BY Id DESC LIMIT 1
                            ");
                uint id;
                if (list.Count == 0)
                    id = 1;
                else id = Convert.ToUInt32(list[0][0]) + 1;

                ushort height = Convert.ToUInt16(new Regex("^[0-9]*$").Match(this.EditingHeight).Value);
                // Add in the view
                this.Members.Add(new Member(
                    id: id,
                    lastName: this.EditingLastName,
                    firstName: this.EditingFirstName,
                    birthDate: this.EditingBirthDate,
                    height: height
                ));
                // Add in the db
                this.Run(
                    @"
                    INSERT INTO Members (LastName, FirstName, BirthDate, Height)" +
                    $"VALUES ('{this.EditingLastName}', '{this.EditingFirstName}', '{this.EditingBirthDate.ToString("d")}', {height})"
                );
            }
        }
        private bool OneMemberSelected(object obj) => ((IList)obj).Count == 1;
        private void EditMember(object obj)
        {
            //contraint: this.Members.Count = 1

            if (this.EditsValid())
            {
                // Add in UI
                this.SelectedMember.FirstName = this.EditingFirstName;
                this.SelectedMember.LastName = this.EditingLastName;
                this.SelectedMember.BirthDate = this.EditingBirthDate;
                this.SelectedMember.Height = Convert.ToUInt16(new Regex("^[0-9]*$").Match(this.EditingHeight).Value);

                // Add in db
                this.Run(@"
                    UPDATE Members " +
                    $"SET LastName = '{this.SelectedMember.LastName}'," +

                        $"FirstName = '{this.SelectedMember.FirstName}'," +
                        $"BirthDate = '{this.SelectedMember.BirthDate.ToString("d")}'," +
                        $"Height = {this.SelectedMember.Height} " +
                    $"WHERE Id = {this.SelectedMember.Id}"
                );

                int index = this.Members.IndexOf(this.SelectedMember);
                this.Members[index] = new Member(fromMember: this.SelectedMember);
            }
        }
        private bool AtLeastOneMemberSelected(object obj) => this.SelectedMember != null;
        private void DeleteMembersSelected(object obj)
        {
            foreach (Member member in ((IList)obj).Cast<Member>().ToList())
            {
                this.Members.Remove(member);
                this.Run(
                    $"DELETE FROM Members WHERE Id={member.Id}"
                );
            }
        }
        public void SelectionChanged(IList selectedItems)
        {
            this.EditMemberCommand.RaiseCanExecuteChanged();
            this.DeleteMembersSelectedCommand.RaiseCanExecuteChanged();
            if (selectedItems.Count == 1)
            {
                this.EditingFirstName = this.SelectedMember.FirstName;
                this.EditingLastName = this.SelectedMember.LastName;
                this.EditingBirthDate = this.SelectedMember.BirthDate;
                this.EditingHeight = this.SelectedMember.Height.ToString();
            }
        }
        private void RevalueButtons() //when
        {
            this.CreateTableCommand.RaiseCanExecuteChanged();
            this.DeleteTableCommand.RaiseCanExecuteChanged();
            this.AddMemberCommand.RaiseCanExecuteChanged();
            this.EditMemberCommand.RaiseCanExecuteChanged();
        }
        private List<object[]> Run(string query) //run a query to the db
        {
            Log.Standard("Running query:" + query);
            LinkedList<object[]> res = new LinkedList<object[]>();
            using (SQLiteCommand command = new SQLiteCommand(query, this.SqliteConnection))
            {
                using (SQLiteDataReader table = command.ExecuteReader())
                {
                    while (table.Read())
                    {
                        object[] tmp = new object[table.FieldCount];
                        for (int i = 0; i < table.FieldCount; i++)
                            tmp[i] = table[i];
                        res.AddLast(tmp);
                    }
                }
            }
            return res.ToList();
        }
    }
}
