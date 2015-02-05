using System.Collections.Generic;

namespace site.Models.ViewModel {
    public class DadosFichaFormulario {
        public bool ENovo { get; set; }
        public Funcionario Funcionario { get; set; }
        public IEnumerable<TipoFuncionario> TiposFuncionario { get; set; }
    }
}