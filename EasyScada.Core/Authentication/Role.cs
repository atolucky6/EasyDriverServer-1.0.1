using System.ComponentModel;

namespace EasyScada.Core
{
    public class Role
    {
        public int Id { get; set; }

        string _name;
        public string Name
        {
            get => _name?.Trim();
            set => _name = value;
        }

        public int Level { get; set; }

        public string Description { get; set; }

        [Browsable(false)]
        public RoleCollection Source { get; set; }
    }
}
