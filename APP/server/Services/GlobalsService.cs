using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pes.Models;
using Pes.Models.DMdel;
using Radzen;

namespace Pes
{
    public partial class GlobalsService
    {
        public event Action<PropertyChangedEventArgs> PropertyChanged;


        Pes.Models.DMdel.Session _ActiveSession;
        public Pes.Models.DMdel.Session ActiveSession
        {
            get
            {
                return _ActiveSession;
            }
            set
            {
                if(!object.Equals(_ActiveSession, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "ActiveSession", NewValue = value, OldValue = _ActiveSession, IsGlobal = true };
                    _ActiveSession = value;
                    PropertyChanged?.Invoke(args);
                }
            }
        }

        Pes.Models.DMdel.Etablissement _Etab;
        public Pes.Models.DMdel.Etablissement Etab
        {
            get
            {
                return _Etab;
            }
            set
            {
                if(!object.Equals(_Etab, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "Etab", NewValue = value, OldValue = _Etab, IsGlobal = true };
                    _Etab = value;
                    PropertyChanged?.Invoke(args);
                }
            }
        }
    }

    public class PropertyChangedEventArgs
    {
        public string Name { get; set; }
        public object NewValue { get; set; }
        public object OldValue { get; set; }
        public bool IsGlobal { get; set; }
    }
}
