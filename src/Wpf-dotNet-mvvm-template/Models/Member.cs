using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWpfMvvmExercise.Models
{
    public class Member
    {
        private uint _Id;
        public uint Id
        {
            get => this._Id;
            set => this._Id = value;
        }
        private string _LastName;
        public string LastName
        {
            get => this._LastName;
            set
            {
                if (value.Trim() == "")
                    throw new ArgumentException();
                else
                    this._LastName = value;
            }
        }

        private string _FirstName;
        public string FirstName
        {
            get => this._FirstName;
            set
            {
                if (value.Trim() == "")
                    throw new ArgumentException();
                else
                    this._FirstName = value;
            }
        }
        
        private DateTime _BirthDate;
        public DateTime BirthDate
        {
            get => this._BirthDate;
            set
            {
                if (value >= DateTime.Now)
                    throw new ArgumentException();
                else
                    this._BirthDate = value;
            }
        }
        public ushort Height { set; get; }

        public Member(uint id, string lastName, string firstName, DateTime birthDate, ushort height)
        {
            this.Id = id;
            this.LastName = lastName;
            this.FirstName = firstName;
            this.BirthDate = birthDate;
            this.Height = height;
        }
        public Member(Member fromMember) : this(fromMember.Id, fromMember.LastName, fromMember.FirstName, fromMember.BirthDate, fromMember.Height) { }
        public override string ToString()
        {
            return this.FirstName + '\n' + this.LastName + '\n' + this.BirthDate + '\n' + this.Height;
        }
    }
}
