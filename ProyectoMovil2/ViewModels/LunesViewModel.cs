using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoMovil2.ViewModels
{
    public class LunesViewModel:BaseViewModel
    {
        public LunesViewModel()
        {
            Title = "Lunes";
            IrAGrupoCommand = new Command(async () => await IrAGrupo());
        }

        public ICommand IrAGrupoCommand { get; }

        private async Task IrAGrupo()
        {
            await Shell.Current.GoToAsync("GrupoPage");
        }
    }
}
