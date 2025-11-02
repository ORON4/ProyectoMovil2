using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoMovil2.ViewModels
{
    public class GrupoViewModel:BaseViewModel
    {
        public GrupoViewModel()
        {
            Title = "Grupo";
            IrATareasCommand = new Command(async () => await IrATareas());
        }

        public ICommand IrATareasCommand { get; }

        private async Task IrATareas()
        {
            await Shell.Current.GoToAsync("TareasPage");
        }

    }
}
