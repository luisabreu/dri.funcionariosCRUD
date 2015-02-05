using System.Collections.Generic;

namespace site.Models.ViewModel {
    public class DadosPesquisa {
        public string NifOuNome { get; set; }
        public IEnumerable<Funcionario> Funcionarios { get; set; }
        public bool PesquisaEfetuada { get; set; }
    }
}